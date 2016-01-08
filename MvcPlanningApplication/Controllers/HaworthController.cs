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

        [HttpPost]
        public JsonResult GetPlanningData(JQueryDataTablesModel jQueryDataTablesModel)
        {
            int TotalRecordCount, searchRecordCount;
            var result = new JsonResult();

            Logger.Info("Use HaworthOrdersRepository to search Haworth Orders in the Database");
            var objHaworthOrdersRepository = new HaworthOrdersRepository();
            var objItems = objHaworthOrdersRepository.GetOrders(searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel);

            Logger.Info("Get total number of Haworth orders in the database");
            using (var db = new PlanningApplicationDb())
                TotalRecordCount = db.HaworthOrders.Count();

            Logger.Info("Return a JSON object containing the required data for JQuery Datatables");
            result.Data = new 
            {
                iTotalRecords = TotalRecordCount,
                jQueryDataTablesModel.sEcho,
                iTotalDisplayRecords = searchRecordCount,
                aaData = objItems
            };

            return result;

        }

        [HttpPost]
        public JsonResult GeneratePlanningData(string SelectedFile, string SelectedRange)
        {
            var result = new JsonResult();

            try
            {
                var objQueryDefs = new QueryDefinitions();

                Logger.Info("Get Haworth XML Orders from FTP Site");
                var Orders = new HaworthOrders(new Uri(Settings.HaworthFTPURI), true);

                using(var db = new PlanningApplicationDb())
                {
                    Logger.Info("Delete All Existing Haworth Orders");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthOrders"));
                    Logger.Info("Re-Seed the Haworth Order Table");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthOrders" }));
                    Logger.Info("Upload and Save the new Haworth Orders to the Database");
                    db.HaworthOrders.AddRange(Orders);
                    db.SaveChanges();
                    Logger.Info("Archive Haworth Orders");
                    Orders.Archive(Settings.HaworthArchiveLocation + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".xml");


                    Logger.Info("Retreive Supplier Demand Data From Excel Using" +
                        " \tFile: " + SelectedFile + Environment.NewLine +
                        "\tRange: " + SelectedRange);
                    HaworthSupplierDemands objSupplierDemands = new HaworthSupplierDemands(Server.MapPath(SelectedFile), SelectedRange);

                    Logger.Info("Delete All Existing Haworth Supplier Demands");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthSupplierDemands"));
                    Logger.Info("Re-Seed the Haworth Supplier Demands Table");
                    db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthSupplierDemands" }));
                    Logger.Info("Upload and Save the new Haworth Supplier Demand to the Database");
                    db.HaworthSupplierDemands.AddRange(objSupplierDemands);
                    db.SaveChanges();

                    //foreach (var objHaworthOrder in db.HaworthOrders)
                    //{
                    //    var objHaworthSupplierDemand = db.HaworthSupplierDemands
                    //        .Where(s => s.OrderNumber.Equals(objHaworthOrder.OrderNumber))
                    //}

                }
                Logger.Info("The planning data was sucessfully generated!");
                
                result.Data = new { Success = true, Message = "The planning data was sucessfully generated!" };
                return result;
            }
            catch(Exception objEx)
            {
                Logger.Info("The planning data generation had an error..." + Environment.NewLine);
                result.Data = new { Success = false, Message = objEx.Message };
                return result;
            }
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


            Logger.Info("Use HaworthDispatchJobRepository to search Haworth Jobs in the Database");
            var objHaworthDispatchJobRepository = new HaworthDispatchJobRepository();
            var objItems = objHaworthDispatchJobRepository.GetOrders(searchRecordCount: out searchRecordCount, DataTablesModel: jQueryDataTablesModel);

            Logger.Info("Get total number of Haworth orders in the database");
            using (var db = new SytelineDbEntities())
                TotalRecordCount = db.coitems
                    .Where(c => c.stat.Equals("O"))
                    .Where(c => c.co_cust_num.Equals("   3417"))
                    .Count();

            Logger.Info("Return a JSON object containing the required data for JQuery Datatables");
            result.Data = new
            {
                iTotalRecords = TotalRecordCount,
                jQueryDataTablesModel.sEcho,
                iTotalDisplayRecords = searchRecordCount,
                aaData = objItems
            };

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
        public JsonResult GetCharacteristics(string OrderNo)
        {
            var objQueryDefs = new QueryDefinitions();
            var strSQL = objQueryDefs.GetQuery("SelectHaworthSupplierDemandsByOrderNo", new string[] { OrderNo });
            Logger.Debug("Getting Characteristics...");
            JsonResult viewresult = new JsonResult();
            string ConfigurationText;

            try
            {
                using (var db = new PlanningApplicationDb())
                    ConfigurationText = db.Database.SqlQuery<HaworthSupplierDemand>(strSQL)
                        .DefaultIfEmpty(new HaworthSupplierDemand { POItemConfigurationText = "No Configuration Text:Found" })
                        .SingleOrDefault()
                        .POItemConfigurationText + " "; //I need to add a space so my regex will match the last characteristic...

                var CharacteristicMatches = Regex
                    .Matches(ConfigurationText, @".*?:\w+\s") //must match: multiple words(.*?)->colon(:)->single word(\w+)->space(\s)
                    .Cast<Match>();

                var strBldr = new StringBuilder();
                var Characteristics = new List<HaworthOrderCharacteristic>();
                foreach (var objStr in CharacteristicMatches)
                {
                    var strArray = objStr.Value.Split(':');

                    if (strArray.Count() > 1)
                        Characteristics.Add(new HaworthOrderCharacteristic { Characteristic = strArray[0], Value = strArray[1] });
                    else
                        Characteristics.Add(new HaworthOrderCharacteristic { Characteristic = strArray[0], Value = string.Empty });
                }

                viewresult.Data = new
                {
                    aaData = Characteristics
                };

            }
            catch (Exception objEx)
            {
                viewresult.Data = new List<HaworthOrderCharacteristic> { new HaworthOrderCharacteristic { Characteristic = "Error", Value = objEx.Message } };
                Logger.Debug(objEx.Message);
            }

            Logger.Debug("Returning Characteristics...");
            return viewresult;
        }
    }
}
