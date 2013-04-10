using System;
using System.IO;
using System.Web;
using System.Globalization;
using System.Web.Optimization;

namespace Ember.Handlebars.ExampleWebAPI
{
    public class HandlebarsBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var builder = new Ember.Handlebars.TemplateBuilder();
            var usTextInfo = new CultureInfo("en-US", false).TextInfo;

            foreach (var assetFile in response.Files)
            {
                var path = context.HttpContext.Server.MapPath(assetFile.VirtualFile.VirtualPath.Replace("/", "\\"));
                var template = File.ReadAllText(path);
                var templateName = Path.GetFileNameWithoutExtension(path).Replace("-", "/");
                builder.Register(templateName, template);
            }

            response.Content = builder.ToString();
            response.ContentType = "text/javascript";
            response.Cacheability = HttpCacheability.Public;
        }
    }
}