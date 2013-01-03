using System;
using System.Reflection;

namespace Ember.Handlebars.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new Ember.Handlebars.TemplateBuilder();
            builder.Register("application", "<h1>{{App.name}}</h1>");

            Console.WriteLine(builder.ToString());
            // returns =>
            //
            // Ember.TEMPLATES["application"] = Ember.Handlebars.template(function anonymous(Handlebars, depth0, helpers, partials, data) {
            //     helpers = helpers || Ember.Handlebars.helpers; data = data || {};
            //     var buffer = '', stack1, escapeExpression=this.escapeExpression;
            //     data.buffer.push("<h1>");
            //     stack1 = helpers._triageMustache.call(depth0, "App.name", {hash:{},contexts:[depth0],data:data});
            //     data.buffer.push(escapeExpression(stack1) + "</h1>");
            //     return buffer;
            // });

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
