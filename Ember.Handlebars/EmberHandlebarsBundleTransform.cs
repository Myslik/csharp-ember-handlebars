using System;
using System.IO;
using System.Web;
using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

public class EmberHandlebarsBundleTransform : IBundleTransform {
    
    private string defaultTemplateName   = "default";
    private string resourceNameSeparator = "/";
    private string fileNameSeparator     = "-";
    private bool   minifyTemplates       = true;
    
    public string DefaultTemplateName {
        get { return defaultTemplateName; }
        set { defaultTemplateName = value; }
    }

    public string ResourceNameSeparator {
        get { return this.resourceNameSeparator; }
        set { this.resourceNameSeparator = value; }
    }

    public string FileNameSeparator {
        get { return this.fileNameSeparator; }
        set { this.fileNameSeparator = value; }
    }
    
    public bool MinifyTemplates {
        get { return this.minifyTemplates; }
        set { this.minifyTemplates = value; }
    }

    public void Process( BundleContext context, BundleResponse response ) {
        var builder = new Ember.Handlebars.TemplateBuilder();

        foreach ( var assetFile in response.Files ) {

            var virtual_root = assetFile.IncludedVirtualPath
                              .Substring( 0, assetFile.IncludedVirtualPath.IndexOf( "\\" ) );

            var virtual_file_path = string.Format("~{0}", assetFile.VirtualFile.VirtualPath)
                                          .Replace( virtual_root + "/", string.Empty );
            var file_extension    = virtual_file_path.Substring(virtual_file_path.LastIndexOf("."));
            
            var template_directory_name = string.Empty;

            var template_file_name = virtual_file_path
                                    .Replace( "~", string.Empty )
                                    .Replace( file_extension, string.Empty );

            if ( template_file_name.IndexOf( this.resourceNameSeparator ) >= 0 ) {
                var segments = template_file_name.Split( this.resourceNameSeparator[0] );
                if ( 1 < segments.Length ) {
                    var template_file_name_temp = string.Empty;
                    if ( 2 == segments.Length ) {
                        
                        template_directory_name = segments[0];
                        template_file_name_temp = segments[1];
                        
                        if ( template_directory_name.Equals( segments[1] ) || 
                             segments[1].Equals( this.defaultTemplateName ) ) {
                            template_file_name = template_directory_name;
                        }

                    } else {
                        template_directory_name = template_file_name.Substring(
                            0, template_file_name
                           .LastIndexOf(this.resourceNameSeparator + segments[segments.Length-1]));
                        
                        var template_directory_name_temp = segments[segments.Length - 2];
                        template_file_name_temp = segments[segments.Length - 1];

                        if ( template_directory_name_temp.Equals( template_file_name_temp ) ||
                             template_file_name_temp.Equals( this.defaultTemplateName ) ) {
                            template_file_name = template_file_name_temp;
                        } else {
                            template_file_name = template_directory_name_temp + 
                                                 this.resourceNameSeparator + 
                                                 template_file_name_temp;
                        }
                    }
                }
            }

            var path = context.HttpContext.Server.MapPath(virtual_root + 
                                                          this.resourceNameSeparator + 
                                                          virtual_file_path);

            var template = File.ReadAllText( path );
            var templateName = template_file_name.Replace(
                                   this.fileNameSeparator,
                                   this.resourceNameSeparator
                               );

            builder.Register( templateName, template );
        }

        var content = builder.ToString();
        if ( minifyTemplates ) {
            var minifier = new Minifier();
            var c = minifier.MinifyJavaScript( content );
            if ( minifier.ErrorList.Count <= 0 ) {
                content = c;
            }
        }

        response.ContentType = "text/javascript";
        response.Cacheability = HttpCacheability.Public;
        response.Content = content;

    }
}
