using System.Web;
using System.Web.Optimization;

namespace Ember.Handlebars.ExampleWebAPI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/base", new JsMinify()).Include(
                        "~/scripts/vendor/jquery-1.8.3.js",
                        "~/scripts/vendor/bootstrap.js",
                        "~/scripts/vendor/handlebars.js",
                        "~/scripts/vendor/ember.js"));

            bundles.Add(new Bundle("~/bundles/modernizr", new JsMinify()).Include(
                        "~/scripts/vendor/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/templates", new EmberHandlebarsBundleTransform())
                        .Include("~/scripts/app/templates/*.handlebars"));

            bundles.Add(new Bundle("~/bundles/app", new JsMinify()).Include(
                        "~/scripts/app/App.js",
                        "~/scripts/app/models/*.js",
                        "~/scripts/app/views/*.js",
                        "~/scripts/app/controllers/*.js",
                        "~/scripts/app/routes/*.js"));

            bundles.Add(new Bundle("~/Content/css", new CssMinify()).Include(
                "~/content/bootstrap.css",
                "~/content/bootstrap-stickyfooter.css",
                "~/content/bootstrap-responsive.css"));

        }
    }
}