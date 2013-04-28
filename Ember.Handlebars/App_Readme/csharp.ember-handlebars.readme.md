NOTE: Ember 1.0 RC3 is not compatible with .NET's bundling and minifying. Ember and Ember-Data are being referenced through `<script>` tags in `_Layout.cshtml` using minified versions from NuGet packages.

In developmet, the template will be rendered directly in the cshtml view. See `~/Views/Home/Index.cshtml`. 

To enable the bundle with pre-compilation of Handlebars templates, edit the web.config to disable debug, setting it to false:

    <configuration>
        ...
            <system.web>
                <compilation debug="false" targetFramework="4.5" />
                   ...

Optimizations must be enabled in `Global.asax.cs` in `Application_Start` method:
             
     if (!HttpContext.Current.IsDebuggingEnabled) {
         // required to precompile Handlebars templates into Ember.TEMPLATES
         BundleTable.EnableOptimizations = true;
     }

 
The pre-compiled templates are minified by default. If for some reason one needs the template not to be minified, do:
              
    bundles.Add(new Bundle("~/bundles/templates",
        new EmberHandlebarsBundleTransform() { minifyTemplates = false })
       .IncludeDirectory("~/scripts/app/templates", "*.hbs", true)
    ); 
                 
Template names must follow Ember.js Naming Conventions found in:

 - [Naming Conventions](http://emberjs.com/guides/concepts/naming-conventions/)

 - [Defining Routes](http://emberjs.com/guides/routing/defining-your-routes/)
                 
Templates in sub directories will have their name appended to the parent directory, even if the parent directory is also a sub-directory. Consider the following router map:
           
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
      |   |_show
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
    ~/scripts/app/templates/shows/index.hbs         => Em.TEMPLATES["shows/index"]
    ~/scripts/app/templates/shows/add.hbs           => Em.TEMPLATES["shows/add"]
                 
Note that the templates named `default.hbs` under a directory `show` will be compiled as `Em.TEMPLATES["show"]`. The same happens if the template has the same name of the parent directory: If that template has been named `show.hbs`, it would also be compiled as `Em.TEMPLATES["show"]`.
                 
If one does not need to separate templates in sub directories, but needs to name templates according to conventions, a character replacement will compile `show-edit` to `Em.TEMPLATES["show/edit"]` replacing the dash (`-`) with a slash (`/`) since Windows does not allow special characters in file or folder names. 

Note: The built-in `EmberHandlebarsBundleTransform` allows your templates to have whichever extension that better suits your development process. The examples above use `*.hbs` as an extension but it could be something else, like `*.html` for example.

Note 2: For details see sample app in [GitHub](https://github.com/Myslik/csharp-ember-handlebars).