using System.Web;
using System.Web.Optimization;

namespace AndroidToolkitWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-unobtrusive-ajax.js",
                        "~/Scripts/jquery.mobile-custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            #region Global / Blog
            bundles.Add(new ScriptBundle("~/bundles/global").Include(
                      "~/Styles/js/bootstrap.js",
                      "~/Styles/js/bootmetro.js",
                      "~/Styles/js/bootmetro-pivot.js",
                      "~/Styles/js/bootmetro-charms.js"));

            bundles.Add(new StyleBundle("~/Styles/global").Include(
                      "~/Styles/css/bootstrap.css",
                      "~/Styles/css/bootmetro.css",
                      "~/Styles/css/bootmetro-responsive.css",
                      "~/Styles/css/bootmetro-ui-light.css",
                      "~/Styles/css/bootmetro-icons.css",
                      "~/Styles/css/datepicker.css",
                      "~/Styles/css/TextHelpers.css",
                      "~/Styles/css/MetroWells.css",
                      "~/Styles/css/icon-sizer.css",
                      "~/Styles/css/icon-colors.css",
                      "~/Styles/css/validation-styles.css",
                      "~/Styles/css/img-rotate.css"));

            bundles.Add(new StyleBundle("~/Styles/blog").Include(
                 "~/Styles/css/img-rotate.css",
                  "~/Styles/css/blog-post.css"
                ));
            #endregion

            #region Admin

            bundles.Add(new ScriptBundle("~/bundles/admin/js").Include(
                       "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-unobtrusive-ajax.js",
                        "~/Scripts/jquery-ui-1.9.2.js",
                        "~/Scripts/jquery.fileupload.js",
                        "~/Scripts/jquery.iframe-transport.js",
                        "~/Scripts/bootstrap.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/admin/css").Include("~/Styles/admin/css/bootstrap.min.css",
               "~/Styles/admin/jquery.fileupload-ui-noscript.css",
               "~/Styles/admin/jquery.fileupload-ui.css",
               "~/Styles/admin/themes/base/jquery-ui*",
               "~/Styles/admin/themes/base/jquery.ui*",
                 "~/Styles/css/TextHelpers.css",
                      "~/Styles/css/MetroWells.css",
                      "~/Styles/css/icon-sizer.css",
                      "~/Styles/css/icon-colors.css",
                      "~/Styles/css/validation-styles.css",
                      "~/Styles/css/img-rotate.css"));
            #endregion


            bundles.Add(new StyleBundle("~/Styles/animations").Include(
                "~/Styles/css/animations.css"
                ));


        }
    }
}
