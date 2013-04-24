using System.Web;
using System.Web.Optimization;

namespace Ember.Handlebars.ExampleWebAPI {
    public class BundleConfig {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new Bundle("~/bundles/base", new JsMinify()).Include(
                        "~/scripts/jquery-{version}.js",
                        "~/scripts/bootstrap.js",
                        "~/scripts/handlebars.runtime.js",
                        "~/scripts/ember-1.0.0-rc.2.js",
                        "~/scripts/ember-data.js",
                        "~/scripts/app/extensions/webapi_serializer.js",
                        "~/scripts/app/extensions/webapi_adapter.js"));

            bundles.Add(new Bundle("~/bundles/modernizr", new JsMinify()).Include(
                        "~/scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/templates", new EmberHandlebarsBundleTransform())
                   .IncludeDirectory("~/scripts/app/templates", "*.hbs", true));

            bundles.Add(new Bundle("~/bundles/app", new JsMinify()).Include(
                "~/scripts/app/App.js",
                "~/scripts/app/setup/*.js",
                "~/scripts/app/models/*.js",
                "~/scripts/app/views/*.js",
                "~/scripts/app/controllers/*.js",
                "~/scripts/app/routes/*.js",
                "~/scripts/app/models/fixtures/*.js"
            ));

            bundles.Add(new Bundle("~/Content/css", new CssMinify()).Include(
                "~/content/bootstrap.css",
                "~/content/bootstrap-responsive.css",
                "~/content/site.css"));

        }
    }
}