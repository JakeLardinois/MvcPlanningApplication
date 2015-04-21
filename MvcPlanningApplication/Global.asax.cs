using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Data.Entity;
using MvcPlanningApplication.Models;


namespace MvcPlanningApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<PlanningApplicationDb>(new PlanningAppDbInitializer());

        }
    }

    //public class PlanningAppDbInitializer : DropCreateDatabaseIfModelChanges<PlanningApplicationDb>
    //public class PlanningAppDbInitializer : DropCreateDatabaseAlways<PlanningApplicationDb>
    public class PlanningAppDbInitializer : DropCreateDatabaseIfModelChanges<PlanningApplicationDb>
    {
        protected override void Seed(PlanningApplicationDb context)
        {
            base.Seed(context);

            context.BriggsDemandItems.Add(new BriggsDemandItem
            {
                Item = "12345-MFG"
            });

            context.SaveChanges();
        }
    }
}