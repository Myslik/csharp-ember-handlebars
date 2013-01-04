csharp-ember-handlebars
=======================

CSharp Library for precompilation of Ember Handlebars

## Example

```csharp
var builder = new Ember.Handlebars.TemplateBuilder();
builder.Register("application", "<h1>{{App.name}}</h1>");
builder.ToString();
```
    
will return
    
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

or you can implement it as `IBundleTransform` as follows

```csharp
public class EmberTemplateTransform : IBundleTransform
{
    private string virtualRootPath;

    public EmberTemplateTransform(string virtualRootPath)
    {
        this.virtualRootPath = virtualRootPath;
    }

    public void Process(BundleContext context, BundleResponse response)
    {
        var builder = new Ember.Handlebars.TemplateBuilder();
        var rootPath = new Uri(context.HttpContext.Server.MapPath(virtualRootPath));
        foreach (var info in response.Files)
        {
            using (var reader = info.OpenText())
            {
                var name = rootPath.MakeRelativeUri(new Uri(info.FullName)).ToString();
                builder.Register(Path.ChangeExtension(name, null).Replace(Path.DirectorySeparatorChar, '/'),
                    reader.ReadToEnd());
            }
        }
        var minifier = new Minifier();
        response.ContentType = "text/javascript";
        var content = minifier.MinifyJavaScript(builder.ToString());
        if (minifier.ErrorList.Any())
        {
            response.Content = builder.ToString();
        }
        else
        {
            response.Content = content;
        }
    }
}
```

and then use it in `BundleConfig.cs` this way

```csharp
bundles.Add(new Bundle("~/bundles/templates", new EmberTemplateTransform("~/Client/Templates/"))
    .IncludeDirectory("~/Client/Templates/", "*.html", true));
```
