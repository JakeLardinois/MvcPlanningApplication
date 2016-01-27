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
            /*bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css"));*/
            //Reference at https://vsn4ik.github.io/bootstrap-submenu/
            bundles.Add(new StyleBundle("~/Content/bootstrapcss",
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrapcsstheme",
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap-theme.min.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrapcsssubmenu").Include(
                "~/Content/bootstrap-submenu.min.css"));

            bundles.Add(new StyleBundle("~/Content/multiselect").Include(
                "~/Content/jquery.multiselect.css"));

            bundles.Add(new StyleBundle("~/Content/datatables",
                "https://cdn.datatables.net/s/ju/dt-1.10.10/datatables.min.css"));
            bundles.Add(new StyleBundle("~/Content/datatablesButtons",
                "https://cdn.datatables.net/buttons/1.1.1/css/buttons.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/Content/impromptu").Include(
                "~/Content/jquery-impromptu.css"));



            bundles.Add(new ScriptBundle("~/bundles/layout").Include(
                        "~/Scripts/layout.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery", 
                "https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui", 
                "http://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.js")); //Note that you must put your version of JqueryUI in jquery.themeswitcher.js or jquery.themeswitcher.min.js...

            bundles.Add(new ScriptBundle("~/bundles/themeswitcher").Include(
                        "~/Scripts/jquery.themeswitcher.js"));

            /*bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));*/
            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrapsubmenu").Include(
                        "~/Scripts/bootstrap-submenu.min.js"));

            //The Templates plugin is included to render the upload/download listings
            bundles.Add(new ScriptBundle("~/bundles/blueimpTemplates",
                "http://blueimp.github.io/JavaScript-Templates/js/tmpl.min.js"));
            //The Load Image plugin is included for the preview images and image resizing functionality 
            bundles.Add(new ScriptBundle("~/bundles/blueimpLoadImage",
                "http://blueimp.github.io/JavaScript-Load-Image/js/load-image.all.min.js"));
            //The Canvas to Blob plugin is included for image resizing functionality
            bundles.Add(new ScriptBundle("~/bundles/blueimpImageResizing",
                "http://blueimp.github.io/JavaScript-Canvas-to-Blob/js/canvas-to-blob.min.js"));
            //blueimp Gallery script
            bundles.Add(new ScriptBundle("~/bundles/blueimpGallery",
                "http://blueimp.github.io/Gallery/js/jquery.blueimp-gallery.min.js"));

            //The basic File Upload plugin
            bundles.Add(new ScriptBundle("~/bundles/fileupload").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload.js"));
            //The Iframe Transport is required for browsers without support for XHR file uploads
            bundles.Add(new ScriptBundle("~/bundles/fileuploadIFrameTransport").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js"));
            //The File Upload processing plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadProcessing").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js"));
            //The File Upload image preview & resize plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadImage").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js"));
            //The File Upload audio preview plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadAudio").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js"));
            //The File Upload video preview plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadVideo").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js"));
            //The File Upload validation plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadValidate").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js"));
            //The File Upload user interface plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadUI").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js"));
            //The File Upload jQuery UI plugin
            bundles.Add(new ScriptBundle("~/bundles/fileuploadJQueryUI").Include(
                        "~/Scripts/jQuery.FileUpload/jquery.fileupload-jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/multiselect").Include(
                        "~/Scripts/jquery.multiselect.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatables",
                "https://cdn.datatables.net/s/ju/dt-1.10.10/datatables.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/DataTablesButtons",
                "https://cdn.datatables.net/buttons/1.1.1/js/dataTables.buttons.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/DataTablesJSZip",
                "https://cdnjs.cloudflare.com/ajax/libs/jszip/2.5.0/jszip.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/DataTablesPDFMake",
                "https://cdn.rawgit.com/bpampuch/pdfmake/0.1.18/build/pdfmake.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/DataTablesFonts",
                "https://cdn.rawgit.com/bpampuch/pdfmake/0.1.18/build/vfs_fonts.js"));
            bundles.Add(new ScriptBundle("~/bundles/DataTablesHTML5Buttons",
                "https://cdn.datatables.net/buttons/1.1.1/js/buttons.html5.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/impromptu").Include(
                        "~/Scripts/jquery-impromptu.js"));

            //http://jqueryvalidation.org/
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalidate", 
                "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.14.0/jquery.validate.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryvalidateAdditionalMethods", 
                "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.13.1/additional-methods.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datetimeformatter").Include(
                        "~/Scripts/DateTimeFormatter.js"));

            bundles.Add(new Bundle("~/bundles/haworthindex").Include(
                        "~/Scripts/haworthindex.js"));
            bundles.Add(new Bundle("~/bundles/haworthdispatch").Include(
                        "~/Scripts/haworthdispatch.js"));
        }
    }
}

