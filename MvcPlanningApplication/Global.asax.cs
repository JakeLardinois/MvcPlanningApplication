using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using log4net;
using log4net.Config;
using System.Data.Entity;
using MvcPlanningApplication.Models;

using System.IO;


namespace MvcPlanningApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly string _servicePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
        //private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog Logger = LogHelper.GetLogger(); //can only be used with .NET 4.5 and above... Else use the above reflection method...


        protected void Application_Start()
        {
            //Start log4net for the entire application...
            XmlConfigurator.ConfigureAndWatch(new FileInfo(_servicePath + "log4net.config"));

            Logger.Info("Application has started...");
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Logger.Info("Database is being Initialized...");
            Database.SetInitializer<PlanningApplicationDb>(new PlanningAppDbInitializer());

            // Lets MVC know that anytime there is a JQueryDataTablesModel as a parameter in an action to use the
            // JQueryDataTablesModelBinder when binding the model.
            Logger.Info("JQueryDataTablesModelBinder is being implemented...");
            ModelBinders.Binders.Add(typeof(JQueryDataTablesModel), new JQueryDataTablesModelBinder());
            
        }
    }

    //public class PlanningAppDbInitializer : DropCreateDatabaseIfModelChanges<PlanningApplicationDb>
    //public class PlanningAppDbInitializer : DropCreateDatabaseAlways<PlanningApplicationDb>
    public class PlanningAppDbInitializer : DropCreateDatabaseIfModelChanges<PlanningApplicationDb>
    {
        private static readonly ILog Logger = LogHelper.GetLogger();


        protected override void Seed(PlanningApplicationDb context)
        {
            Logger.Info("Database is being Seeded...");
            base.Seed(context);

            context.BriggsDemandItems.Add(new BriggsDemandItem
            {
                Item = "12345-MFG"
            });

            context.HaworthOrders.Add(new HaworthOrder 
            { 
                WTFItemNumber = "10",
                PartInformation = new HaworthPartInformation(),
                DeliveryInformation = new HaworthDeliveryInformation()
            });

            context.SaveChanges();
            Logger.Info("Database Seeding is Complete...");
        }
    }
}