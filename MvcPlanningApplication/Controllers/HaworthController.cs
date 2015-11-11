using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPlanningApplication.Models;
using System.IO;
using MvcFileUploader.Models;
using MvcFileUploader;
using log4net;



namespace MvcPlanningApplication.Controllers
{
    public class HaworthController : Controller
    {
        private static readonly ILog Logger = LogHelper.GetLogger();
        public static string VirtualFilePath { get { return @"/Content/Uploads"; } }
        private PlanningApplicationDb db = new PlanningApplicationDb();


        public ActionResult Index()
        {
            var result = db.HaworthOrders
                .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult GenerateData(string SelectedFile, string SelectedRange)
        {
            var objQueryDefs = new QueryDefinitions();


            //////var objList = new HaworthDispatchList();
            //Logger.Info("Get Haworth XML Orders from FTP Site");
            ////var Orders = new HaworthOrders(new Uri("ftp://FTP.HAWORTH.COM/Company113/Company113Ext/XML/Prod/Out"), true);
            //var Orders = new HaworthOrders(new Uri("ftp://FTP.HAWORTH.COM/Company113/Company113Ext/XML/Test/Out"), true);
            //var RemainingOrders = Orders.RemainingOrders;
            //Logger.Info("Successfully retrieved and processed Haworth Orders from FTP Site");

            //Logger.Info("Delete All Existing Haworth Orders and reseed the Haworth Order Table");
            //db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthOrders"));
            //db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new [] {"HaworthOrders"}));
            //db.HaworthOrders.AddRange(Orders);
            //Logger.Info("Upload and Save the new Haworth Orders to the Database");
            //db.SaveChanges();
            //Logger.Info("Archive Haworth Orders");
            //Orders.Archive(Settings.HaworthArchiveLocation + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".xml");


            Logger.Info("Retreive Supplier Demand Data From Excel");
            Logger.Debug("Using File: " + SelectedFile + Environment.NewLine + "\tUsing Range: " + SelectedRange);
            HaworthSupplierDemands objSupplierDemands = new HaworthSupplierDemands(Server.MapPath(SelectedFile), SelectedRange);
            Logger.Info("Successfully retrieved and processed Supplier Demand from Selected excel sheet.");

            Logger.Info("Delete All Existing Haworth Supplier Demands and reseed the Haworth Supplier Demands Table");
            db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("DeleteAllHaworthSupplierDemands"));
            db.Database.ExecuteSqlCommand(objQueryDefs.GetQuery("ReSeedTable", new[] { "HaworthSupplierDemands" }));
            Logger.Info("Upload and Save the new Haworth Supplier Demand to the Database");
            db.HaworthSupplierDemands.AddRange(objSupplierDemands);
            db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult Dispatch()
        {
            return View();
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
            //var UploadFilePath = Server.MapPath("~/Content/Uploads/");
            var UploadFilePath = Server.MapPath("~" + VirtualFilePath + "/"); // ("~/Content/Uploads/");
            Logger.Debug("Upload Directory: " + UploadFilePath);
            
            var CurrentFiles = new List<ViewDataUploadFileResult>();
            DirectoryInfo directory = new DirectoryInfo(UploadFilePath);
            if (!directory.Exists)
                Logger.Error("The following directory does not exist: " + directory.FullName);
            UrlHelper objUrlHelper = new UrlHelper(this.ControllerContext.RequestContext);
            var Files = directory.GetFiles();

            
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

    }
}
