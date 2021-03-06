﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPlanningApplication.Models;
using System.IO;
using MvcFileUploader.Models;
using MvcFileUploader;
using log4net;
using System.Text.RegularExpressions;
using System.Text;
using MvcPlanningApplication.Models.Haworth;
using MvcPlanningApplication.Models.DataTablesMVC;
using Newtonsoft.Json;
using Microsoft.Reporting.WebForms;
using System.Data;



namespace MvcPlanningApplication.Controllers
{
    public class HaworthController : Controller
    {
        private static readonly ILog Logger = LogHelper.GetLogger();
        public static string VirtualFilePath { get { return @"/Content/Uploads"; } }


        public ActionResult Index()
        {
            return View();
        }

        private string ExtractPostParameter(string ParameterName)
        {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonData = new StreamReader(Request.InputStream).ReadToEnd();
            var objDictionary = HttpUtility.UrlDecode(jsonData).Split('&').ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
            if (objDictionary.ContainsKey(ParameterName))
                return objDictionary[ParameterName];
            else
                return string.Empty;
        }

        [HttpPost]
        public string GetPlanningData(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int TotalRecordCount, searchRecordCount;
            bool RemainingOrdersOnly, blnTemp;
            var result = new JsonResult();


            Logger.Info("Use HaworthOrdersRepository to search Haworth Orders in the Database");
            var objHaworthOrdersRepository = new HaworthOrdersRepository();

            var HaworthOrders = objHaworthOrdersRepository.GetOrders(searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel);

            RemainingOrdersOnly = bool.TryParse(ExtractPostParameter("RemainingOrdersOnly"), out blnTemp) ? blnTemp : true;
            if (RemainingOrdersOnly)
                RemainingOrdersOnly = true; //objItems = objItems.RemainingOrders();

            Logger.Info("Get total number of Haworth orders in the database");
            using (var db = new PlanningApplicationDb())
                TotalRecordCount = db.HaworthOrders.Count();

            Logger.Info("Return a JSON object containing the required data for JQuery Datatables");

            /*The issue that I had here was that I wanted to include the characteristics list for each order in the returned Json. However, when I added my foreign Key 'HaworthOrder' to my 'HaworthOrderCharacteristics' class
             * I would recieve 'A circular reference was detected while serializing an object of type...' Error. The only way to resolve this issue was to use JsonConvert for serialization and pass it the below settings object
             * so that it could handle the cyclic reference */
            var serializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Arrays,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            return JsonConvert.SerializeObject(new
            {
                iTotalRecords = TotalRecordCount,
                jQueryDataTablesModel.sEcho,
                iTotalDisplayRecords = searchRecordCount,
                aaData = HaworthOrders
            }, serializerSettings);
        }

        [HttpPost]
        public JsonResult GeneratePlanningData(string SelectedFile, string SelectedRange)
        {
            var result = new JsonResult();

            try
            {
                var objQueryDefs = new QueryDefinitions();

                Logger.Info("Retreive Haworth XML Orders from FTP Site");
                var Orders = new HaworthOrders(new Uri(Settings.HaworthFTPURI), true);

                Logger.Info("Retreive Supplier Demand Data From Excel Using" +
                        " \tFile: " + SelectedFile + Environment.NewLine +
                        "\tRange: " + SelectedRange);
                HaworthSupplierDemands objSupplierDemands = new HaworthSupplierDemands(Server.MapPath(SelectedFile), SelectedRange);

                Logger.Info("Add the Characteristics list for each Haworth Order");
                foreach (var objHaworthOrder in Orders)
                {
                    var objSupplierDemand = objSupplierDemands
                        .Where(s => !string.IsNullOrEmpty(s.OrderNumber) && s.OrderNumber.Equals(objHaworthOrder.OrderNumber))
                        .DefaultIfEmpty(new HaworthSupplierDemand { POItemConfigurationText = string.Empty, CatalogPartNo = string.Empty })
                        .FirstOrDefault();

                    var strPOItemConfig = objSupplierDemand.POItemConfigurationText;
                    Logger.Debug(string.Format("Haworth Order: {0} has characteristics string: {1}", objHaworthOrder, strPOItemConfig));
                    if (!string.IsNullOrEmpty(strPOItemConfig))
                    {
                        objHaworthOrder.Characteristics = BuildCharacteristics(objHaworthOrder.OrderNumber, strPOItemConfig);

                        var strCatalogPartNo = objSupplierDemand.CatalogPartNo;
                        if (!string.IsNullOrEmpty(strCatalogPartNo))
                        {
                            var objStrArray = strCatalogPartNo.Split(new char[] { ',' });
                            objHaworthOrder.Characteristics.Add(new HaworthOrderCharacteristic{
                                Characteristic = "Frame Color",
                                Value = objStrArray[objStrArray.Length - 1],
                                OrderNumber = objHaworthOrder.OrderNumber,
                                CharacteristicKey = "Frame Color: " + objStrArray[objStrArray.Length - 1].Replace(',', ' ')
                            });
                        }
                            
                    }
                        
                    Logger.Debug(strPOItemConfig);
                }

                using(var db = new PlanningApplicationDb())
                {
                    //Characteristics must be deleted before Orders because of the Foreign Key Constraint.
                    Logger.Info("Delete All Existing Characteristics");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthOrderCharacteristics"));
                    Logger.Info("Re-Seed the Haworth Order Characteristics Table");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthOrderCharacteristics" }));

                    Logger.Info("Delete All Existing Haworth Orders");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthOrders"));
                    Logger.Info("Re-Seed the Haworth Order Table");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthOrders" }));

                    Logger.Info("Upload and Save the Haworth Orders and thier Characteristics to the Database");
                    db.HaworthOrders.AddRange(Orders);
                    db.SaveChanges();

                    Logger.Info("Archive Haworth Orders");
                    Orders.Archive(Settings.HaworthArchiveLocation + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".xml");


                    Logger.Info("Delete All Existing Haworth Supplier Demands");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthSupplierDemands"));
                    Logger.Info("Re-Seed the Haworth Supplier Demands Table");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthSupplierDemands" }));

                    Logger.Info("Upload and Save the new Haworth Supplier Demand to the Database");
                    db.HaworthSupplierDemands.AddRange(objSupplierDemands);
                    db.SaveChanges();
                }
                Logger.Info("The planning data was sucessfully generated!");
                
                result.Data = new { Success = true, Message = "The planning data was sucessfully generated!" };
                return result;
            }
            catch(Exception objEx)
            {
                Logger.Info("The planning data generation had an error..." + 
                    Environment.NewLine +
                    "/t" + objEx.Message);
                result.Data = new { Success = false, Message = objEx.Message };
                return result;
            }
        }

        private List<HaworthOrderCharacteristic> BuildCharacteristics(string Order, string ConfigurationText)
        {
            var Characteristics = new List<HaworthOrderCharacteristic>();
            var CharacteristicMatches = Regex
                    .Matches(ConfigurationText + " ", @".*?:\w+\s") //must match: multiple words(.*?)->colon(:)->single word(\w+)->space(\s) Note that I add a space at the end so the last 
                    .Cast<Match>();                                 //characteristic is also flagged as a match.

            if (CharacteristicMatches == null)
            {
                Logger.Info(string.Format("Order {0} has invalid ConfigurationText: {1}", Order, ConfigurationText));
                return Characteristics;
            }

            foreach (var objStr in CharacteristicMatches)
            {
                var strArray = objStr.Value.Split(':');

                if (strArray.Count() > 1)
                    Characteristics.Add(new HaworthOrderCharacteristic {
                        OrderNumber = Order,
                        Characteristic = strArray[0],
                        Value = strArray[1],
                        CharacteristicKey = strArray[0].Replace(',', ' ') + ": " + strArray[1].Replace(',', ' ')
                    });
                else
                    Characteristics.Add(new HaworthOrderCharacteristic { OrderNumber = Order, Characteristic = strArray[0], Value = string.Empty });
            }

            return Characteristics;
        }

        public ActionResult Dispatch()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDispatchData(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int TotalRecordCount, searchRecordCount;
            var result = new JsonResult();
            var objQueryDefinitions = new QueryDefinitions();
            bool RemainingOrdersOnly, UseLiveData, blnTemp;


            //Extrapolate a couple extra parameters from the post data...
            RemainingOrdersOnly = bool.TryParse(ExtractPostParameter("RemainingOrdersOnly"), out blnTemp) ? blnTemp : true;
            UseLiveData = bool.TryParse(ExtractPostParameter("UseLiveData"), out blnTemp) ? blnTemp : true;

            Logger.Info("Use HaworthDispatchJobRepository to search Haworth Jobs in the Database");
            var objHaworthDispatchJobRepository = new HaworthDispatchJobRepository();
            var DispatchList = objHaworthDispatchJobRepository.GetOrders(RemainingOrdersOnly, UseLiveData, searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel);

            Logger.Info("Get total number of Haworth orders in the database");
            if (UseLiveData)
            {
                using (var db = new SytelineDbEntities())
                    TotalRecordCount = db.coitems
                        .Where(c => c.stat.Equals("O"))
                        .Where(c => c.co_cust_num.Equals("   3417"))
                        .Count();
            }
            else
                using (var db = new PlanningApplicationDb())
                    TotalRecordCount = db.HaworthDispatchJobRecords
                        .Count();
            

            Logger.Info("Return a JSON object containing the required data for JQuery Datatables");
            result.Data = new
            {
                iTotalRecords = TotalRecordCount,
                jQueryDataTablesModel.sEcho,
                iTotalDisplayRecords = searchRecordCount,
                aaData = DispatchList
            };

            return result;

        }

        [HttpGet]
        public void GetDispatchData(JQueryDataTablesModel jQueryDataTablesModel, bool RemainingOrdersOnly, bool UseLiveData, string Format = "Excel")
        {
            int searchRecordCount;


            var objHaworthDispatchJobRepository = new HaworthDispatchJobRepository();
            var DispatchList = objHaworthDispatchJobRepository.GetOrders(RemainingOrdersOnly, UseLiveData, searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel, isDownloadReport: true);
            RenderDispatchExcelReport(DispatchList);
        }

        private void RenderDispatchExcelReport(IList<IHaworthDispatchJob> DispatchJobList)
        {
            string strReportType = "Excel";
            LocalReport objLocalReport;
            ReportDataSource DispatchListSource;
            string mimeType;
            string encoding;
            string fileNameExtension;
            string deviceInfo = "";
            Warning[] warnings;
            string[] streams;
            

            objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "HaworthDispatch.rdlc") };

            //Give the reportdatasource a name so that we can reference it in our report designer
            DispatchListSource = new ReportDataSource("IHaworthDispatchJob", DispatchJobList);

            objLocalReport.DataSources.Add(DispatchListSource);
            objLocalReport.Refresh();

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
            deviceInfo = string.Format(
                        "<DeviceInfo>" +
                        "<OmitDocumentMap>True</OmitDocumentMap>" +
                        "<OmitFormulas>True</OmitFormulas>" +
                        "<SimplePageHeaders>True</SimplePageHeaders>" +
                        "</DeviceInfo>", strReportType);

            //Render the report
            var renderedBytes = objLocalReport.Render(
                strReportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            //Clear the response stream and write the bytes to the outputstream
            //Set content-disposition to "attachment" so that user is prompted to take an action
            //on the file (open or save)
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=DispatchList" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        [HttpPost]
        public JsonResult GenerateDispatchData()
        {
            var result = new JsonResult();
            var objQueryDefs = new QueryDefinitions();

            try
            {
                using (var db = new SytelineDbEntities())
                {
                    using (var db2 = new PlanningApplicationDb())
                    {
                        //Do your filtering here using the above objHaworthDispatchJobSearch object...
                        var objHaworthDispatchJobRecords = db.Database.SqlQuery<COItem>(objQueryDefs.GetQuery("SelectCOItemByCustNumListAndStatus", new string[] { "3417".AddSingleQuotesAndPadLeft(7), "O" }))
                            .Select(g => new HaworthDispatchJob
                            {
                                Job = g.ref_num,
                                JobSuffix = g.ref_line_suf.HasValue ? (short)g.ref_line_suf : (short)0,
                                co_num = g.co_num,
                                co_line = g.co_line,
                                cust_po = g.cust_po,
                                QuantityOrdered = g.qty_ordered,
                                QuantityRemaining = g.qty_ordered - g.qty_complete,
                                PurchaseOrder = g.PurchaseOrder,
                                SalesOrder = db2.HaworthSupplierDemands
                                    .Where(s => s.PO == g.cust_po && s.POLine == g.co_line.ToString())
                                    .ToList()
                                    .DefaultIfEmpty(new HaworthSupplierDemand { SO = "0", SOLine = "0" })
                                    .FirstOrDefault()
                                    .SOrderNumber,
                                ItemNumber = g.item,
                                DockDate = g.due_date.HasValue ? (DateTime)g.due_date : SharedVariables.MINDATE,
                                ShipByDate = g.promise_date.HasValue ? (DateTime)g.promise_date : SharedVariables.MINDATE,
                                DispatchJobMaterials = db.jobmatls
                                    .Where(m => m.job.Equals(g.ref_num))
                                    .Select(jm => new HaworthDispatchJobMaterial
                                    {
                                        JobMaterial = jm.item,
                                        JobMaterialDescription = jm.description,
                                        UnitOfMeasure = jm.u_m
                                        //QtyRequired = g.matl_qty * objDispatchJob.QuantityOrdered,
                                        //QtyIssued = g.qty_issued * objDispatchJob.QuantityOrdered,
                                        //QtyAvailable = g.det_QtyAvailable ?? 0
                                    })
                                    .ToList()
                            })
                            .Select(d => new HaworthDispatchJobRecord
                            {
                                JobOrder = d.JobOrder,
                                CustomerOrder = d.CustomerOrder,
                                PurchaseOrder = d.PurchaseOrder,
                                SalesOrder = d.SalesOrder,
                                QuantityOrdered = d.QuantityOrdered,
                                QuantityRemaining = d.QuantityRemaining,
                                ItemNumber = d.ItemNumber,
                                Shell = d.Shell,
                                Frame = d.Frame,
                                Fabric = d.Fabric,
                                ArmCaps = d.ArmCaps,
                                ShipByDate = d.ShipByDate,
                                DockDate = d.DockDate
                            })
                            .ToList();

                        Logger.Info("Delete All Existing HaworthDispatchJobRecordc");
                        db2.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthDispatchJobRecords"));
                        Logger.Info("Re-Seed the Haworth Dispatch Job Records Table");
                        db2.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthDispatchJobRecords" }));

                        Logger.Info("Upload and Save the new Haworth Supplier Demand to the Database");
                        db2.HaworthDispatchJobRecords.AddRange(objHaworthDispatchJobRecords);
                        db2.SaveChanges();
                    }
                }
            }
            catch(Exception objEx)
            {
                Logger.Info("The planning data generation had an error..." + 
                    Environment.NewLine +
                    "/t" + objEx.Message);
                result.Data = new { Success = false, Message = objEx.Message };
                return result;
            }

            Logger.Info("The Dispatch list data was sucessfully generated!");

            result.Data = new { Success = true, Message = "The dispatch list data was sucessfully generated!" };
            return result;
        }

        public ActionResult UploadFile(int? entityId) // optionally receive values specified with Html helper
        {
            // here we can send in some extra info to be included with the delete url 
            var statuses = new List<ViewDataUploadFileResult>();
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var st = FileSaver.StoreFile(x =>
                {
                    x.File = Request.Files[i];
                    //note how we are adding an additional value to be posted with delete request
                    //and giving it the same value posted with upload
                    x.DeleteUrl = Url.Action("DeleteFile", new { entityId = entityId });
                    x.StorageDirectory = Server.MapPath("~" + VirtualFilePath + "/");
                    x.UrlPrefix = VirtualFilePath;// this is used to generate the relative url of the file


                    //overriding defaults
                    x.FileName = Request.Files[i].FileName;// default is filename suffixed with filetimestamp
                    x.ThrowExceptions = true;//default is false, if false exception message is set in error property
                });

                statuses.Add(st);
            }

            //statuses contains all the uploaded files details (if error occurs then check error property is not null or empty)
            //todo: add additional code to generate thumbnail for videos, associate files with entities etc

            //adding thumbnail url for jquery file upload javascript plugin
            statuses.ForEach(x => x.thumbnailUrl = x.url + "?width=80&height=80"); // uses ImageResizer httpmodule to resize images from this url

            //setting custom download url instead of direct url to file which is default
            statuses.ForEach(x => x.url = Url.Action("DownloadFile", new { fileUrl = x.url, mimetype = x.type }));


            //server side error generation, generate some random error if entity id is 13
            if (entityId == 13)
            {
                var rnd = new Random();
                statuses.ForEach(x =>
                {
                    //setting the error property removes the deleteUrl, thumbnailUrl and url property values
                    x.error = rnd.Next(0, 2) > 0 ? "We do not have any entity with unlucky Id : '13'" : String.Format("Your file size is {0} bytes which is un-acceptable", x.size);
                    //delete file by using FullPath property
                    if (System.IO.File.Exists(x.FullPath)) System.IO.File.Delete(x.FullPath);
                });
            }

            var viewresult = Json(new { files = statuses });
            //for IE8 which does not accept application/json
            if (Request.Headers["Accept"] != null && !Request.Headers["Accept"].Contains("application/json"))
                viewresult.ContentType = "text/plain";

            return viewresult;
        }

        //here i am receving the extra info injected
        [HttpPost] // should accept only post
        public ActionResult DeleteFile(int? entityId, string fileUrl)
        {
            var filePath = Server.MapPath("~" + fileUrl);

            Logger.Debug("Deleting File: " + filePath);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            
            var viewresult = Json(new { error = String.Empty });
            //for IE8 which does not accept application/json
            if (Request.Headers["Accept"] != null && !Request.Headers["Accept"].Contains("application/json"))
                viewresult.ContentType = "text/plain";

            return viewresult; // trigger success
        }

        public ActionResult DownloadFile(string fileUrl, string mimetype)
        {
            var filePath = Server.MapPath("~" + fileUrl);

            if (System.IO.File.Exists(filePath))
                return File(filePath, mimetype);
            else
            {
                return new HttpNotFoundResult("File not found");
            }
        }

        [HttpPost]
        public JsonResult GetCurrentFiles()
        {
            var UploadFilePath = Server.MapPath("~" + VirtualFilePath + "/"); // ("~/Content/Uploads/");
            Logger.Debug("Upload Directory is: " + UploadFilePath);
            
            var CurrentFiles = new List<ViewDataUploadFileResult>();
            DirectoryInfo directory = new DirectoryInfo(UploadFilePath);
            if (!directory.Exists)
                Logger.Error("The following directory does not exist: " + directory.FullName);
            UrlHelper objUrlHelper = new UrlHelper(this.ControllerContext.RequestContext);
            var Files = directory.GetFiles();

            Logger.Info("Getting the list of files from the Upload Directory");
            foreach (var objFile in Files)
            {
                var strMimeType = MimeMapping.GetMimeMapping(objFile.Name);

                CurrentFiles.Add(new ViewDataUploadFileResult { 
                    name = objFile.Name, 
                    size = (int)objFile.Length,
                    type = strMimeType,
                    url = objUrlHelper.Action("DownloadFile", "Haworth", null) + "?" +
                        HttpUtility.UrlPathEncode("fileUrl=" + VirtualFilePath + "/" + objFile.Name + 
                        "&mimetype=" + strMimeType),
                    deleteUrl = objUrlHelper.Action("DeleteFile", "Haworth", null) + "?" +
                        "entityId=0" + 
                        "&fileUrl=" + VirtualFilePath + "/" + objFile.Name,
                    thumbnailUrl = HttpUtility.UrlPathEncode(VirtualFilePath + "/" + objFile.Name + "?" + 
                        "width=80&height=80"),
                    deleteType = "POST",
                    FullPath = "~" + VirtualFilePath + "/" + objFile.Name, // objFile.FullName, //not required an so just reveals info about the filesystem...
                    SavedFileName = objFile.Name,
                    Title = Path.GetFileNameWithoutExtension(objFile.Name)
                });
            }

            var viewresult = Json(new { files = CurrentFiles });

            return viewresult;
        }

        [HttpPost]
        public JsonResult GetExcelRanges(string File)
        {
            ExcelOpenXMLInfo objExcelInfo = new ExcelOpenXMLInfo();
            var strFullFilePath = Server.MapPath(File);
            Logger.Debug("Getting Excel Ranges from file: " + strFullFilePath);
            try
            {
                objExcelInfo = new ExcelOpenXMLInfo(strFullFilePath);
                objExcelInfo.GetInformation();
            }
            catch(Exception objEx)
            {
                Logger.Error("Excel Info Exception Thrown", objEx);
            }

            var Ranges = objExcelInfo.NamedRanges
                .Select(g => new[] { g });
            var viewresult = Json(Ranges);

            return viewresult;
        }

        [HttpPost]
        public JsonResult GetStatusCodes()
        {
            Logger.Debug("Getting Status Codes");
            JsonResult viewresult = Json(new[] { "Error" });
            IEnumerable<string[]> jsonStatusCodes;


            try
            {
                using (var db = new PlanningApplicationDb())
                {
                    var StatusCodes = db.HaworthOrders
                        .GroupBy(s => s.StatusCode)
                        .Select(s => new { StatusCode = s.Key })
                        .ToList();
                    jsonStatusCodes = StatusCodes
                        .Select(c => new[] { c.StatusCode });
                }

                viewresult = Json(jsonStatusCodes);
            }
            catch (Exception objEx)
            {
                Logger.Debug(objEx.Message);
            }

            Logger.Debug("Returning Status Codes");
            return viewresult;
        }

        [HttpPost]
        public string GetCharacteristics(string OrderNo)
        {
            List<HaworthOrderCharacteristic> Characteristics;

            try
            {
                using (var db = new PlanningApplicationDb())
                {
                    Characteristics = db.HaworthOrders
                        .Where(o => o.OrderNumber.Equals(OrderNo))
                        .SingleOrDefault()
                        .Characteristics
                        .ToList();
                }
            }
            catch (Exception objEx)
            {
                Characteristics = new List<HaworthOrderCharacteristic> { new HaworthOrderCharacteristic { Characteristic = "Error", Value = objEx.Message } };
                Logger.Debug(objEx.Message);
            }

            Logger.Debug("Returning Characteristics...");
            var serializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Arrays,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            return JsonConvert.SerializeObject(new
            {
                aaData = Characteristics
            }, serializerSettings);

        }

        [HttpPost]
        public JsonResult GetCharacteristicList()
        {
            Logger.Debug("Getting Characteristics");
            JsonResult viewresult = Json(new[] { "Error" });
            IEnumerable<string[]> jsonCharacteristics;
            var objQueryDefinitions = new QueryDefinitions();


            using (var db = new PlanningApplicationDb())
            {
                var Characteristics = db.HaworthOrderCharacteristics
                    .GroupBy(s => s.CharacteristicKey)
                    .Select(s => new { CharacteristicKey = s.Key })
                    .ToList();
                jsonCharacteristics = Characteristics
                    .Select(c => new[] { c.CharacteristicKey });
            }
            viewresult = Json(jsonCharacteristics);

            Logger.Debug("Returning Characteristics...");
            return viewresult;

        }

        [HttpGet]
        public void PrintIDLabel(string CustomerOrder, string PurchaseOrder, string SalesOrder, string JobOrder)
        {
            string strReportType = "PDF";
            LocalReport objLocalReport;
            ReportParameter[] objParameterArray;
            string deviceInfo = "";
            string mimeType;
            string encoding;
            string fileNameExtension;
            Warning[] warnings;
            string[] streams;
            HaworthSupplierDemand objHaworthSupplierDemand;
            HaworthOrder objHaworthOrder;
            StringBuilder objStrBldr = new StringBuilder();

            try
            {
                using (var db = new PlanningApplicationDb())
                {
                    objHaworthSupplierDemand = db.HaworthSupplierDemands
                        .Where(h => h.OrderNumber.Equals(PurchaseOrder))
                        .SingleOrDefault();
                    objHaworthOrder = db.HaworthOrders
                        .Where(h => h.OrderNumber.Equals(PurchaseOrder))
                        .SingleOrDefault();
                }

                if (objHaworthSupplierDemand == null || objHaworthOrder == null)
                {
                    objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "HaworthIDLabelError.rdlc") };

                    objStrBldr.Clear();
                    objStrBldr.Append(string.Format("Missing Info!!\r\n\tCustomerOrder:{0}\r\n\tPurchaseOrder:{1}\r\n\tSalesOrder:{2}\r\n\r\n", CustomerOrder, PurchaseOrder, SalesOrder));
                    if (objHaworthSupplierDemand == null)
                        objStrBldr.Append("Missing Haworth Supplier Demand Record...\r\n");
                    if (objHaworthOrder == null)
                        objStrBldr.Append("Missing Haworth Order Record...\r\n");

                    objParameterArray = new ReportParameter[] {
                        new ReportParameter("ErrorText", objStrBldr.ToString()),
                    };
                }
                else
                {
                    objLocalReport = new LocalReport { ReportPath = Server.MapPath(Settings.ReportDirectory + "HaworthIDLabel.rdlc") };

                    objParameterArray = new ReportParameter[] {
                        new ReportParameter("ItemID", objHaworthOrder.ItemNumber),
                        new ReportParameter("ItemDescription", objHaworthOrder.PartInformation.Description),
                        new ReportParameter("CatalogNo", objHaworthSupplierDemand.CatalogPartNo),
                        new ReportParameter("MfgDate", DateTime.Now.ToString("yyyy - MMM - dd")),
                        new ReportParameter("SO", objHaworthSupplierDemand.SO),
                        new ReportParameter("SOLine", objHaworthSupplierDemand.SOLine.PadLeft(6, '0')),
                        new ReportParameter("OrderNumber", objHaworthSupplierDemand.OrderNumber),
                        new ReportParameter("JobOrder", JobOrder),
                        new ReportParameter("AddressLine1", "Made for Haworth Inc"),
                        new ReportParameter("AddressLine2", "1 Haworth Ctr, Holland, MI USA"),
                        new ReportParameter("AddressLine3", "Assembled in US with US and foreign Components")
                    };
                }
                
                objLocalReport.SetParameters(objParameterArray);
                objLocalReport.Refresh();

                //The DeviceInfo settings should be changed based on the reportType
                //http://msdn2.microsoft.com/en-us/library/ms155397.aspx
                deviceInfo = string.Format(
                            "<DeviceInfo>" +
                            "<PageHeight>2in</PageHeight>" +
                            "<PageWidth>4in</PageWidth>" +
                            "<MarginBottom>0in</MarginBottom>" +
                            "<MarginTop>0in</MarginTop>" +
                            "<MarginLeft>0in</MarginLeft>" +
                            "<MarginRight>0in</MarginRight>" +
                            "</DeviceInfo>", strReportType);

                //Render the report
                var renderedBytes = objLocalReport.Render(
                    strReportType,
                    deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                //Clear the response stream and write the bytes to the outputstream
                //Set content-disposition to "attachment" so that user is prompted to take an action
                //on the file (open or save)
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=WorkOrderEquip" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "." + fileNameExtension);
                Response.BinaryWrite(renderedBytes);
                Response.Flush(); 
            }
            catch(Exception objEx)
            {
                objStrBldr.Clear();

                objStrBldr.Append("Exception Thrown!\r\n\t" + objEx.Message);
                if (objEx.InnerException != null)
                    objStrBldr.Append("Inner Exception...\r\n\t" + objEx.InnerException.Message);

                Logger.Debug(objStrBldr.ToString());
            }
            
        }

    }
}
