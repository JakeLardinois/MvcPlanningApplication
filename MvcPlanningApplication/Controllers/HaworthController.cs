using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPlanningApplication.Models;


namespace MvcPlanningApplication.Controllers
{
    public class HaworthController : Controller
    {

        public ActionResult Index()
        {
            var strArchiveFile = System.Configuration.ConfigurationManager.AppSettings["HaworthArchiveLocation"] + string.Format("{0:yyyyMMdd}", DateTime.Now) + ".xml";
            //var objList = new HaworthDispatchList();
            var Orders = new HaworthOrders(new Uri("ftp://FTP.HAWORTH.COM/Company113/Company113Ext/XML/Prod/Out"), true);
            var RemainingOrders = Orders.RemainingOrders;

            Orders.Archive(strArchiveFile);


            return View();
        }

        public ActionResult Dispatch()
        {
            return View();
        }
    }
}
