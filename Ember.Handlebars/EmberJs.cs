using System.Web.Hosting;

namespace Ember
{
    public static class EmberJs
    {
        static EmberJs()
        {
            TemplatesPath = "~/scripts/app/templates";
        }

        public static class BundleNames
        {
            static BundleNames()
            {
                Templates = "~/bundles/ember/templates";
                App = "~/bundles/ember/app";
            }

            public static string Templates { get; set; }
            public static string App { get; set; }
        }

        public static string TemplatesPath { get; set; }

        public static string ServerMappedTemplatesPath
        {
            get { return HostingEnvironment.MapPath(TemplatesPath); }
        }
    }
}
