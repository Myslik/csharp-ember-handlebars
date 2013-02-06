namespace Ember.Handlebars
{
    using Myslik.Utils;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    public class Compiler
    {
        private ScriptEngine _engine = null;
        private ParsedScript _vm = null;

        public Compiler()
        {
            _engine = new ScriptEngine("jscript");

            Debug.Assert(!string.IsNullOrWhiteSpace(LoadResource("compiler.js")));

            _vm = _engine.Parse(LoadResource("compiler.js"));
        }

        
        private ParsedScript VM
        {
            get { return this._vm; }
        }

        private static string LoadResource(string name)
        {
            var asm = Assembly.GetCallingAssembly();
            var stream = asm.GetManifestResourceStream(string.Format("Ember.{0}", name));
            var reader = new System.IO.StreamReader(stream);
            return reader.ReadToEnd();
        }

        public string Precompile(string template)
        {
            return (string)VM.CallMethod("precompile", template);
        }
    }

    public class TemplateBuilder
    {
        private StringBuilder builder;
        private Compiler compiler;

        public TemplateBuilder()
        {
            this.builder = new StringBuilder();
            this.compiler = new Compiler();
        }

        public void Register(string templateName, string templateBody)
        {
            builder.AppendFormat("Ember.TEMPLATES[\"{0}\"] = Ember.Handlebars.template({1});\n", templateName, this.compiler.Precompile(templateBody));
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
