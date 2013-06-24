using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using Ember;

public static class HtmlHelperExtensions
{
    public static IHtmlString RenderEmberTemplates(this HtmlHelper helper, string path = "", bool noTemplateName = false)
    {
        var templateFolder = EmberJs.ServerMappedTemplatesPath;

        if (BundleTable.EnableOptimizations)
            return Scripts.Render(EmberJs.BundleNames.Templates);

        if (HttpRuntime.Cache[path] == null)
        {
            string absolutePath;
            string templateName = "";
            if (string.IsNullOrEmpty(path))
            {
                absolutePath = templateFolder;
            }
            else
            {
                templateName = path.Replace("\\", "-");
                absolutePath = Path.Combine(templateFolder, path);
            }

            if (File.Exists(absolutePath))
            {
                int fileExtensionPosition = templateName.LastIndexOf('.');
                if (fileExtensionPosition > 0)
                {
                    templateName = templateName.Substring(0, fileExtensionPosition);
                }

                string templateContent = ReadTemplate(templateName, new FileInfo(absolutePath), noTemplateName);

                HttpRuntime.Cache.Insert(path, new MvcHtmlString(templateContent), new CacheDependency(absolutePath));
            }
            else
            {
                if (Directory.Exists(absolutePath))
                {
                    if (templateName.Length > 0 && templateName[templateName.Length - 1] != '-')
                    {
                        templateName += "-";
                    }
                    List<string> dependencyList = new List<string>();

                    MvcHtmlString result = new MvcHtmlString(GetDirectoryTemplates(templateName, "", new DirectoryInfo(absolutePath), dependencyList));
                    HttpRuntime.Cache.Insert(path, result, new CacheDependency(dependencyList.ToArray()));
                }
                else
                {
                    return new MvcHtmlString(""); //nothing is found, return empty string and do not cache
                }
            }
        }

        return HttpRuntime.Cache[path] as MvcHtmlString;
    }

    private static string GetDirectoryTemplates(string templateName, string relativeDirName, DirectoryInfo rootDirectory, List<string> dependencyList)
    {
        dependencyList.Add(rootDirectory.FullName);

        var newSubRelativeDirName = relativeDirName;
        if (!string.IsNullOrEmpty(newSubRelativeDirName))
        {
            newSubRelativeDirName = newSubRelativeDirName + "-";
        }
        var content = "";

        foreach (DirectoryInfo subDir in rootDirectory.GetDirectories())
        {
            content += GetDirectoryTemplates(templateName, newSubRelativeDirName + subDir.Name, subDir, dependencyList);
        }

        foreach (FileInfo templateFile in rootDirectory.GetFiles())
        {
            string subtemplateName = templateFile.Name;
            int fileExtensionPosition = subtemplateName.LastIndexOf('.');
            if (fileExtensionPosition > 0)
            {
                subtemplateName = subtemplateName.Substring(0, fileExtensionPosition);
            }
            if (relativeDirName.Length > 0 && relativeDirName[relativeDirName.Length - 1] != '-')
            {
                relativeDirName += "-";
            }
            content += ReadTemplate(templateName + relativeDirName + subtemplateName, templateFile);
        }
        return content;
    }

    private static string ReadTemplate(string templateName, FileInfo templateFile, bool noTemplateName = false)
    {
        string content = File.ReadAllText(templateFile.FullName);
        if (templateFile.Extension.ToLowerInvariant().StartsWith(".htm"))
        {
            //for .html/.htm* files, we'll simply return the content
            return content;
        }

        //for other template files, we treate them as x-handlebars script type
        if (noTemplateName)
        {
            return "<script type=\"text/x-handlebars\">\n" + content + "\n</script>\n";
        }

        templateName = templateName.Replace("-", "/");
        var segments = templateName.Split('/');
        if (segments.Length > 2) {
            templateName = string.Format("{0}/{1}", segments[segments.Length - 2], segments[segments.Length - 1]);
        }

        return "<script type=\"text/x-handlebars\" data-template-name=\"" + templateName + "\">\n" + content + "\n</script>\n";
    }

}
