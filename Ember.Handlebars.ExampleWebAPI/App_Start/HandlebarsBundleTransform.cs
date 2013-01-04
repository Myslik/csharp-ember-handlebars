using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

namespace Ember.Handlebars.ExampleWebAPI
{
    public class EmberHandlebarsBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            Ember.Handlebars.TemplateBuilder builder = new Ember.Handlebars.TemplateBuilder();
            var usTextInfo = new CultureInfo("en-US", false).TextInfo;

            foreach (var assetFile in response.Files)
            {
                var template = File.ReadAllText(assetFile.FullName)
                                   .Replace("\r", string.Empty)
                                   .Replace("\n", string.Empty)
                                   .Replace("\t", string.Empty);

                var templateName = Path.GetFileNameWithoutExtension(assetFile.FullName);
                templateName = usTextInfo.ToTitleCase(templateName).Replace("_", string.Empty);
                builder.Register(templateName, template);
            }

            response.Content = builder.ToString();
            response.ContentType = "text/javascript";
            response.Cacheability = HttpCacheability.Public;
        }
    }
}