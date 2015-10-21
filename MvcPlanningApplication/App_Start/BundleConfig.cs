using System.Web;
using System.Web.Optimization;
using System.Web.Mvc.Html;

using System.Collections.Generic;
using System.IO;


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
                "~/Content/jquery.fileupload/css/jquery.fileupload.css",
                "~/Content/jquery.fileupload/css/jquery.fileupload-ui.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            //More bootstrap themes are available at http://bootswatch.com/. They are implemented differently than the nuget package. Nuget would have you put both bootstrap.css and bootstrap-theme.css and then
            //modify bootstrap-theme.css to perform theming, whereas bootswatch themes only want you to implement the single css file...
            //When the bundle was named ~/Content/bootstrap I kept getting 403 Forbidden errors. This was because a bundle cannot have the same name as a real directory
            //The default css bundle route cannot match a valid folder name because MVC cannot resolve the conflict between a route and a folder.
            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css"));

            bundles.Add(new StyleBundle("~/Content/blueimp", 
                "http://blueimp.github.io/Gallery/css/blueimp-gallery.min.css"));




            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                        "~/Scripts/layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery", 
                "https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui", 
                "http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.js")); //Note that you must put your version of JqueryUI in jquery.themeswitcher.js or jquery.themeswitcher.min.js...

            bundles.Add(new ScriptBundle("~/bundles/themeswitcher").Include(
                        "~/Scripts/jquery.themeswitcher.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));

            var BlueImpBundle = new ScriptBundle("~/bundles/blueimp", "http://blueimp.github.io/JavaScript-Templates/js/tmpl.min.js").Include(//The Templates plugin is included to render the upload/download listings
                        "~/JavaScript-Load-Image/js/load-image.all.min.js",//The Load Image plugin is included for the preview images and image resizing functionality
                        "~/JavaScript-Canvas-to-Blob/js/canvas-to-blob.min.js",//The Canvas to Blob plugin is included for image resizing functionality
                        "~/Gallery/js/jquery.blueimp-gallery.min.js");//blueimp Gallery script
            BlueImpBundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(BlueImpBundle);

            var FileUploadBundle = new ScriptBundle("~/bundles/fileupload").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",//The Iframe Transport is required for browsers without support for XHR file uploads
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",//The basic File Upload plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",//The File Upload processing plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",//The File Upload image preview & resize plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",//The File Upload audio preview plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",//The File Upload video preview plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",//The File Upload validation plugin
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js");//The File Upload user interface plugin
            FileUploadBundle.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(FileUploadBundle);

            bundles.Add(new ScriptBundle("~/bundles/fileuploadJQueryUI").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-jquery-ui.js"));//The File Upload jQuery UI plugin

        }
    }
}

class NonOrderingBundleOrderer : IBundleOrderer
{
    public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
    {
        return files;
    }
}
