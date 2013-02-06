using System;
using System.IO;
using System.Web;
using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

public class EmberHandlebarsBundleTransform : IBundleTransform {
    
    private bool minifyTemplates = true;
    
    public bool MinifyTemplates {
        get { return this.minifyTemplates; }
        set { this.minifyTemplates = value; }
    }

    public void Process( BundleContext context, BundleResponse response ) {
        var builder = new Ember.Handlebars.TemplateBuilder();

        

        foreach ( var assetFile in response.Files ) {
            var path = context.HttpContext.Server.MapPath(assetFile.VirtualPath);
            var template = File.ReadAllText( path );
            var templateName = Path.GetFileNameWithoutExtension( path );
            builder.Register( templateName, template );
        }

        var content = builder.ToString();
        if ( minifyTemplates ) {
            var minifier = new Minifier();
            var c = minifier.MinifyJavaScript( builder.ToString() );
            if ( minifier.ErrorList.Count <= 0 ) {
                content = c;
            }
        }

        response.ContentType = "text/javascript";
        response.Cacheability = HttpCacheability.Public;
        response.Content = content;

    }
}
