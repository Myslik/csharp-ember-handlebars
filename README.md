csharp-ember-handlebars
=======================

.NET Library for pre-compilation of Handlebars templates directly into Ember's `TEMPLATES` collection. 

## Example

In order to register a template, the builder expectes a template name and its corresponding markup.

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

The above shows the basic template registration of templates with this library but going forward you might want 
to add this functionality as part of your application's build process including this functionality directly into
the `BundleConfig` class.

## Built-in IBundleTransform Implementation
This library now a built-in implementation of `IBundleTransform` which can be used directly in ASP.NET MVC's 
`BundleConfig` class as shown below:

```csharp
bundles.Add(new Bundle("~/bundles/templates", new EmberHandlebarsBundleTransform())
       .Include("~/scripts/app/templates/*.handlebars"));
```
Note: The built-in `EmberHandlebarsBundleTransform` allows your template follows to use use whichever extension 
that better suits your development process. The example above uses *.handlebars as an extension but it could be 
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

    public void Process(BundleContext context, BundleResponse response) {
        var builder = new Ember.Handlebars.TemplateBuilder();

        foreach (var assetFile in response.Files) {
            var template = File.ReadAllText(assetFile.FullName);
            var templateName = Path.GetFileNameWithoutExtension(assetFile.FullName);
            builder.Register(templateName, template);
        }

        var content = builder.ToString();
        if (minifyTemplates) {
            var minifier = new Minifier();
            var c = minifier.MinifyJavaScript(builder.ToString());
            if (minifier.ErrorList.Count <= 0) {
                content = c;
            }
        }

        response.ContentType = "text/javascript";
        response.Cacheability = HttpCacheability.Public;
        response.Content = content;

    }
}
```

