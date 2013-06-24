using System.Web.Hosting;

namespace Ember
{
    public static class EmberJs
    {
        static EmberJs()
        {
            TemplatesPath = "~/scripts/app/templates";
        }

        public static string TemplatesPath { get; set; }

        public static string ServerMappedTemplatesPath 
        {
            get { return HostingEnvironment.MapPath(TemplatesPath); }
        }
    }
}
