using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MvcPlanningApplication.Models;


namespace MvcPlanningApplication.Controllers
{
    public class BriggsController : Controller
    {
        private PlanningApplicationDb db = new PlanningApplicationDb();

        //
        // GET: /Briggs/
        public ActionResult Index()
        {
            //var result = db.BriggsDemandItems
            //    .ToList();

            return View();
        }
    }
}
