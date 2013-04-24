csharp-ember-handlebars
=======================

.NET Library for pre-compilation of Handlebars templates directly into Ember's `TEMPLATES` collection. 

![NuGet-Install](https://raw.github.com/MilkyWayJoe/csharp-ember-handlebars/master/nuget.png)
## Example

In order to register a template, the builder expects a string with the template name and and another 
with its corresponding markup:

```csharp
var builder = new Ember.Handlebars.TemplateBuilder();
builder.Register("application", "<h1>{{App.name}}</h1>");
```

To retrieve the pre-compiled templates, simply call `builder.ToString()`. This will return the following:     

```javascript
Ember.TEMPLATES["application"] = Ember.Handlebars.template(
    function anonymous(Handlebars, depth0, helpers, partials, data) {
        helpers = helpers || Ember.Handlebars.helpers; data = data || {};
        var buffer = '', stack1, escapeExpression=this.escapeExpression;
        data.buffer.push("<h1>");
        stack1 = helpers._triageMustache.call(depth0, "App.name", {hash:{},contexts:[depth0],data:data});
        data.buffer.push(escapeExpression(stack1) + "</h1>");
        return buffer;
    }
);
```

The above shows the basic registration of templates, but going forward you might want to add this functionality 
as part of your application's build process including this functionality directly into the `BundleConfig` class.

## Built-in IBundleTransform Implementation
This library now has a built-in implementation of `IBundleTransform` which can be used directly in ASP.NET MVC's 
`BundleConfig` class as shown below:

```csharp
     bundles.Add(new Bundle("~/bundles/templates",
             new EmberHandlebarsBundleTransform())
            .IncludeDirectory("~/scripts/app/templates", "*.hbs", true)
     );
```

In developmet, the template will be rendered directly in the cshtml view. See `~/Views/Home/Index.cshtml`

    @if (HttpContext.Current.IsDebuggingEnabled) {
        @Html.RenderEmberTemplates()
    } else {
        @Scripts.Render("~/bundles/templates")
    }

    @Scripts.Render("~/bundles/app")
              
To enable the bundle with pre-compilation of Handlebars templates, edit 
the web.config to disable debug, setting it to false:
         
     <configuration>
         ...
         <system.web>
             <compilation debug="false" targetFramework="4.5" />
             ...
                              
Optimizations must be enabled in Global.asax.cs Application_Start method:
             
     BundleTables.EnableOptimizations = true;
              
The pre-compiled templates are minified by default, but if for some reason 
one needs the template not to be minified, do:
      
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
            | |_default.hbs
            | |_edit.hbs
            | |_remove.hbs
            |
            |_index.hbs
            |_add.hbs 
            
The templates will be compiled as:

    ~/scripts/app/templates/shows/show/default.hbs  => Em.TEMPLATES["show"]
    ~/scripts/app/templates/shows/show/edit.hbs     => Em.TEMPLATES["show/edit"]
    ~/scripts/app/templates/shows/show/remove.hbs   => Em.TEMPLATES["show/remove"]
    ~/scripts/app/templates/shows/index.hbs         => Em.TEMPLATES["shows/index"]
    ~/scripts/app/templates/shows/add.hbs           => Em.TEMPLATES["shows/add"]
                 
Note that the templates named `default.hbs` under a directory `show` will be compiled as 
`Em.TEMPLATES["show"]`. The same happens if the template has the same name of the parent
directory: If that template has been named `show.hbs`, it would also be compiled as `Em.TEMPLATES["show"]`.
                 
If one does not need to separate templates in sub directories, but needs to name templates according to
conventions, a character replacement will compile `show-edit` to `Em.TEMPLATES["show/edit"]` replacing
the dash (`-`) with a slash (`/`) since Windows does not allow special characters in file or folder names.

Note: The built-in `EmberHandlebarsBundleTransform` allows your templates to have whichever extension 
that better suits your development process. The examples above use *.hbs as an extension but it could be 
something else, like *.html for example.

## Creating a custom implementation of IBundleTransform
The code snippet below shows the implementation of the built-in `EmberHandlebarsBundleTransform` class which you can 
rename and adapt to your needs:

```csharp
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
            var path = context.HttpContext.Server.MapPath(assetFile.VirtualFile.VirtualPath.Replace("/", "\\"));
            var template = File.ReadAllText( path );
            var templateName = Path.GetFileNameWithoutExtension( path ).Replace("-", "/");
            builder.Register( templateName, template );
        }

        var content = builder.ToString();
        if ( minifyTemplates ) {
            var minifier = new Minifier();
            var c = minifier.MinifyJavaScript(content);
            if ( minifier.ErrorList.Count <= 0 ) {
                content = c;
            }
        }

        response.ContentType = "text/javascript";
        response.Cacheability = HttpCacheability.Public;
        response.Content = content;

    }
}
```

