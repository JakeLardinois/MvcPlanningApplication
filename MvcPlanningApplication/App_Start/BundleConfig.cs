using System.Web;
using System.Web.Optimization;
using System.Web.Mvc.Html;
namespace MvcPlanningApplication
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true; // when configuring the bundles, if there are minified or CDN versions found then the files don't show up on the page; This forces them to display...
            bundles.UseCdn = true; //Enables CDN Support.

            bundles.Add(new StyleBundle("~/Content/reset").Include(
                "~/Content/reset.css",
                "~/Content/html5-reset.css"));

            bundles.Add(new StyleBundle("~/Content/fileupload").Include(
                "~/Content/jquery.fileupload/jquery.fileupload.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            //More bootstrap themes are available at http://bootswatch.com/. They are implemented differently than the nuget package. Nuget would have you put both bootstrap.css and bootstrap-theme.css and then
            //modify bootstrap-theme.css to perform theming, whereas bootswatch themes only want you to implement the single css file...
            //When the bundle was named ~/Content/bootstrap I kept getting 403 Forbidden errors. This was because a bundle cannot have the same name as a real directory
            //The default css bundle route cannot match a valid folder name because MVC cannot resolve the conflict between a route and a folder.
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                "~/Content/bootstrap-theme-default.css"));




            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                        "~/Scripts/layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery", "https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.js")); //Note that you must put your version of JqueryUI in jquery.themeswitcher.js or jquery.themeswitcher.min.js...

            bundles.Add(new ScriptBundle("~/bundles/themeswitcher").Include(
                        "~/Scripts/jquery.themeswitcher.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/fileupload").Include(
                        "~/Scripts/jquery.fileupload/jquery.iframe-transport",
                        "~/Scripts/jquery.fileupload/jquery.fileupload.js"));
        }
    }
}