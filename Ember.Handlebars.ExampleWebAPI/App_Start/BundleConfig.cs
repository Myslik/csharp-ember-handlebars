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
                "~/scripts/jquery-{version}.js",
                "~/scripts/bootstrap.js",
                "~/scripts/handlebars.runtime.js"
            ));

            bundles.Add(new Bundle("~/bundles/modernizr", new JsMinify()).Include(
                "~/scripts/modernizr-*"
            ));

            bundles.Add(new Bundle(EmberJs.BundleNames.App, new JsMinify()).Include(
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
                "~/content/site.css"
            ));


            /* 
                 
             NOTE: Ember 1.0 RC3 is not compatible with bundling and minifying. Removing all-ember-things
                   from the bundles in favor of script tags in _Layout.cshtml
                 
             In developmet, the template will be rendered directly in the cshtml view
             See ~/Views/Home/Index.cshtml 
              
             To enable the bundle with pre-compilation of Handlebars templates, edit 
             the web.config to disable debug, setting it to false:
             
             <configuration>
                 ...
                 <system.web>
                     <compilation debug="false" targetFramework="4.5" />
                     ...
                              
             Optimizations must be enabled in Global.asax.cs Application_Start method:
             
             BundleTables.EnableOptimizations = true;
              
             The pre-compiled templates are minified by default
             If for some reason one needs the template not to be minified, do:
              
             bundles.Add(new Bundle("~/bundles/templates",
                new EmberHandlebarsBundleTransform() { minifyTemplates = false })
                .IncludeDirectory("~/scripts/app/templates", "*.hbs", true)
             ); 
                 
             Template names must follow Ember.js Naming Conventions found in:
             - http://emberjs.com/guides/concepts/naming-conventions/
             - http://emberjs.com/guides/routing/defining-your-routes/
                 
             Templates in sub directories will have their name appended to the parent directory, even if the 
             parent directory is also a sub-directory. Consider the following router map:
                 
             App.Router.map ->
                @resource 'shows', ->
                    @route 'add'
                    @resource 'show', {path: ':show_id'}, ->
                        @route 'edit'
                        @route 'remove' 
                  
             The directory structure for these routes could be:
                  
                 ~/
                  |_scripts
                    |_app
                      |_templates
                      |_shows
                        |_show
                        |   |_default.hbs
                        |   |_edit.hbs
                        |   |_remove.hbs
                        |
                        |_index.hbs
                        |_add.hbs 
                  
             The templates will be compiled as
                 
             ~/scripts/app/templates/shows/show/default.hbs  => Em.TEMPLATES["show"]
             ~/scripts/app/templates/shows/show/edit.hbs     => Em.TEMPLATES["show/edit"]
             ~/scripts/app/templates/shows/show/remove.hbs   => Em.TEMPLATES["show/remove"]
             ~/scripts/app/templates/shows/index.hbs          => Em.TEMPLATES["shows/index"]
             ~/scripts/app/templates/shows/add.hbs           => Em.TEMPLATES["shows/add"]
                 
             Note that the templates named `default.hbs` under a directory `show` will be compiled as 
             `Em.TEMPLATES["show"]`. The same happens if the template has the same name of the parent
             directory: If that template has been named `show.hbs`, it would also be compiled as `Em.TEMPLATES["show"]`.
                 
             If one does not need to separate templates in sub directories, but needs to name templates according to
             conventions, a character replacement will compile `show-edit` to `Em.TEMPLATES["show/edit"]` replacing
             the dash (`-`) with a slash (`/`) since Windows does not allow special characters in file or folder names.
                  
            */
            bundles.Add(new Bundle(EmberJs.BundleNames.Templates,
                new EmberHandlebarsBundleTransform())
                .IncludeDirectory(EmberJs.TemplatesPath, "*.hbs", true)
            );

        }
    }
}