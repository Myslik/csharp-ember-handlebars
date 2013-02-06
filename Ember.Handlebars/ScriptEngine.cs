namespace Myslik.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;

    /// <summary>
    /// Represents a Windows Script Engine such as JScript, VBScript, etc.
    /// </summary>
    public sealed class ScriptEngine : IDisposable
    {
        private IActiveScript _engine;
        private IActiveScriptParse32 _parse32;
        private IActiveScriptParse64 _parse64;
        internal ScriptSite _site;

        [Guid("BB1A2AE1-A4F9-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IActiveScript
        {
            void SetScriptSite(IActiveScriptSite pass);
            void GetScriptSite(Guid riid, out IntPtr site);
            void SetScriptState(ScriptState state);
            void GetScriptState(out ScriptState scriptState);
            void Close();
            void AddNamedItem(string name, ScriptItem flags);
            void AddTypeLib(Guid typeLib, uint major, uint minor, uint flags);
            void GetScriptDispatch(string itemName, out IntPtr dispatch);
            void GetCurrentScriptThreadID(out uint thread);
            void GetScriptThreadID(uint win32ThreadId, out uint thread);
            void GetScriptThreadState(uint thread, out ScriptThreadState state);
            void InterruptScriptThread(uint thread, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo, uint flags);
            void Clone(out IActiveScript script);
        }

        [Guid("DB01A1E3-A42B-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IActiveScriptSite
        {
            void GetLCID(out int lcid);
            void GetItemInfo(string name, ScriptInfo returnMask, out IntPtr item, IntPtr typeInfo);
            void GetDocVersionString(out string version);
            void OnScriptTerminate(object result, System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
            void OnStateChange(ScriptState scriptState);
            void OnScriptError(IActiveScriptError scriptError);
            void OnEnterScript();
            void OnLeaveScript();
        }

        [Guid("EAE1BA61-A4ED-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IActiveScriptError
        {
            void GetExceptionInfo(out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
            void GetSourcePosition(out uint sourceContext, out int lineNumber, out int characterPosition);
            void GetSourceLineText(out string sourceLine);
        }

        [Guid("BB1A2AE2-A4F9-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IActiveScriptParse32
        {
            void InitNew();
            void AddScriptlet(string defaultName, string code, string itemName, string subItemName, string eventName, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out string name, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
            void ParseScriptText(string code, string itemName, object context, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out object result, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
        }

        [Guid("C7EF7658-E1EE-480E-97EA-D52CB4D76D17"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IActiveScriptParse64
        {
            void InitNew();
            void AddScriptlet(string defaultName, string code, string itemName, string subItemName, string eventName, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out string name, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
            void ParseScriptText(string code, string itemName, object context, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out object result, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
        }

        [Flags]
        private enum ScriptText
        {
            None = 0,
            DelayExecution = 1,
            IsVisible = 2,
            IsExpression = 32,
            IsPersistent = 64,
            HostManageSource = 128
        }

        [Flags]
        private enum ScriptInfo
        {
            None = 0,
            IUnknown = 1,
            ITypeInfo = 2
        }

        [Flags]
        private enum ScriptItem
        {
            None = 0,
            IsVisible = 2,
            IsSource = 4,
            GlobalMembers = 8,
            IsPersistent = 64,
            CodeOnly = 512,
            NoCode = 1024
        }

        private enum ScriptThreadState
        {
            NotInScript = 0,
            Running = 1
        }

        private enum ScriptState
        {
            Uninitialized = 0,
            Started = 1,
            Connected = 2,
            Disconnected = 3,
            Closed = 4,
            Initialized = 5
        }

        private const int TYPE_E_ELEMENTNOTFOUND = unchecked((int)(0x8002802B));

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptEngine"/> class.
        /// </summary>
        /// <param name="language">The scripting language. Standard Windows Script engines names are 'jscript' or 'vbscript'.</param>
        public ScriptEngine(string language)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            Type engine = Type.GetTypeFromProgID(language, true);
            _engine = Activator.CreateInstance(engine) as IActiveScript;
            if (_engine == null)
                throw new ArgumentException(language + " is not an Windows Script Engine", "language");

            _site = new ScriptSite();
            _engine.SetScriptSite(_site);

            // support 32-bit & 64-bit process
            if (IntPtr.Size == 4)
            {
                _parse32 = _engine as IActiveScriptParse32;
                _parse32.InitNew();
            }
            else
            {
                _parse64 = _engine as IActiveScriptParse64;
                _parse64.InitNew();
            }
        }

        /// <summary>
        /// Adds the name of a root-level item to the scripting engine's name space.
        /// </summary>
        /// <param name="name">The name. May not be null.</param>
        /// <param name="value">The value. It must be a ComVisible object.</param>
        public void SetNamedItem(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            _engine.AddNamedItem(name, ScriptItem.IsVisible | ScriptItem.IsSource);
            _site._namedItems[name] = value;
        }

        internal class ScriptSite : IActiveScriptSite
        {
            internal ScriptException _lastException;
            internal Dictionary<string, object> _namedItems = new Dictionary<string, object>();

            void IActiveScriptSite.GetLCID(out int lcid)
            {
                lcid = Thread.CurrentThread.CurrentCulture.LCID;
            }

            void IActiveScriptSite.GetItemInfo(string name, ScriptInfo returnMask, out IntPtr item, IntPtr typeInfo)
            {
                if ((returnMask & ScriptInfo.ITypeInfo) == ScriptInfo.ITypeInfo)
                    throw new NotImplementedException();

                object value;
                if (!_namedItems.TryGetValue(name, out value))
                    throw new COMException(null, TYPE_E_ELEMENTNOTFOUND);

                item = Marshal.GetIUnknownForObject(value);
            }

            void IActiveScriptSite.GetDocVersionString(out string version)
            {
                version = null;
            }

            void IActiveScriptSite.OnScriptTerminate(object result, System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo)
            {
                //Trace.WriteLine("OnScriptTerminate result=" + result);
            }

            void IActiveScriptSite.OnStateChange(ScriptState scriptState)
            {
                //Trace.WriteLine("OnStateChange scriptState=" + scriptState);
            }

            void IActiveScriptSite.OnScriptError(IActiveScriptError scriptError)
            {
                //Trace.WriteLine("OnScriptError scriptError=" + scriptError);
                string sourceLine = null;
                try
                {
                    scriptError.GetSourceLineText(out sourceLine);
                }
                catch
                {
                    // happens sometimes...
                }
                uint sourceContext;
                int lineNumber;
                int characterPosition;
                scriptError.GetSourcePosition(out sourceContext, out lineNumber, out characterPosition);
                lineNumber++;
                characterPosition++;
                System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo;
                scriptError.GetExceptionInfo(out exceptionInfo);

                string message;
                if (!string.IsNullOrEmpty(sourceLine))
                {
                    message = "Script exception: {1}. Error number {0} (0x{0:X8}): {2} at line {3}, column {4}. Source line: '{5}'.";
                }
                else
                {
                    message = "Script exception: {1}. Error number {0} (0x{0:X8}): {2} at line {3}, column {4}.";
                }
                _lastException = new ScriptException(string.Format(message, exceptionInfo.scode, exceptionInfo.bstrSource, exceptionInfo.bstrDescription, lineNumber, characterPosition, sourceLine));
                _lastException.Column = characterPosition;
                _lastException.Description = exceptionInfo.bstrDescription;
                _lastException.Line = lineNumber;
                _lastException.Number = exceptionInfo.scode;
                _lastException.Text = sourceLine;
            }

            void IActiveScriptSite.OnEnterScript()
            {
                //Trace.WriteLine("OnEnterScript");
                _lastException = null;
            }

            void IActiveScriptSite.OnLeaveScript()
            {
                //Trace.WriteLine("OnLeaveScript");
            }
        }

        /// <summary>
        /// Evaluates an expression using the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression. May not be null.</param>
        /// <returns>The result of the evaluation.</returns>
        public static object Eval(string language, string expression)
        {
            return Eval(language, expression, null);
        }

        /// <summary>
        /// Evaluates an expression using the specified language, with an optional array of named items.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression. May not be null.</param>
        /// <param name="namedItems">The named items array.</param>
        /// <returns>The result of the evaluation.</returns>
        public static object Eval(string language, string expression, params KeyValuePair<string, object>[] namedItems)
        {
            if (language == null)
                throw new ArgumentNullException("language");

            if (expression == null)
                throw new ArgumentNullException("expression");

            using (ScriptEngine engine = new ScriptEngine(language))
            {
                if (namedItems != null)
                {
                    foreach (KeyValuePair<string, object> kvp in namedItems)
                    {
                        engine.SetNamedItem(kvp.Key, kvp.Value);
                    }
                }
                return engine.Eval(expression);
            }
        }

        /// <summary>
        /// Evaluates an expression.
        /// </summary>
        /// <param name="expression">The expression. May not be null.</param>
        /// <returns>The result of the evaluation.</returns>
        public object Eval(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            return Parse(expression, true);
        }

        /// <summary>
        /// Parses the specified text and returns an object that can be used for evaluation.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>An instance of the ParsedScript class.</returns>
        public ParsedScript Parse(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            return (ParsedScript)Parse(text, false);
        }

        private object Parse(string text, bool expression)
        {
            const string varName = "x___";
            System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo;
            object result;

            _engine.SetScriptState(ScriptState.Connected);

            ScriptText flags = ScriptText.None;
            if (expression)
            {
                flags |= ScriptText.IsExpression;
            }

            try
            {
                // immediate expression computation seems to work only for 64-bit
                // so hack something for 32-bit...
                if (_parse32 != null)
                {
                    if (expression)
                    {
                        // should work for jscript & vbscript at least...
                        text = varName + "=" + text;
                    }
                    _parse32.ParseScriptText(text, null, null, null, IntPtr.Zero, 0, flags, out result, out exceptionInfo);
                }
                else
                {
                    _parse64.ParseScriptText(text, null, null, null, IntPtr.Zero, 0, flags, out result, out exceptionInfo);
                }
            }
            catch
            {
                if (_site._lastException != null)
                    throw _site._lastException;

                throw;
            }

            IntPtr dispatch;
            if (expression)
            {
                // continue  out 32-bit hack...
                if (_parse32 != null)
                {
                    _engine.GetScriptDispatch(null, out dispatch);
                    object dp = Marshal.GetObjectForIUnknown(dispatch);
                    try
                    {
                        return dp.GetType().InvokeMember(varName, BindingFlags.GetProperty, null, dp, null);
                    }
                    catch
                    {
                        if (_site._lastException != null)
                            throw _site._lastException;

                        throw;
                    }
                }
                return result;
            }

            _engine.GetScriptDispatch(null, out dispatch);
            ParsedScript parsed = new ParsedScript(this, dispatch);
            return parsed;
        }

        void IDisposable.Dispose()
        {
            if (_parse32 != null)
            {
                Marshal.ReleaseComObject(_parse32);
                _parse32 = null;
            }

            if (_parse64 != null)
            {
                Marshal.ReleaseComObject(_parse64);
                _parse64 = null;
            }

            if (_engine != null)
            {
                Marshal.ReleaseComObject(_engine);
                _engine = null;
            }
        }
    }

    /// <summary>
    /// Defines a Windows Script Engine exception.
    /// </summary>
    [Serializable]
    public class ScriptException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        public ScriptException()
            : base("Script Exception")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ScriptException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public ScriptException(Exception innerException)
            : base(null, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ScriptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected ScriptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the error description intended for the customer.
        /// </summary>
        /// <value>The description text.</value>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the line number of error.
        /// </summary>
        /// <value>The line number.</value>
        public int Line { get; internal set; }

        /// <summary>
        /// Gets the character position of error.
        /// </summary>
        /// <value>The column number.</value>
        public int Column { get; internal set; }

        /// <summary>
        /// Gets a value describing the error.
        /// </summary>
        /// <value>The error number.</value>
        public int Number { get; internal set; }

        /// <summary>
        /// Gets the text line in the source file where an error occurred.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; internal set; }
    }

    /// <summary>
    /// Defines a pre-parsed script object that can be evaluated at runtime.
    /// </summary>
    public sealed class ParsedScript : IDisposable
    {
        private object _dispatch;
        private readonly ScriptEngine _engine;

        internal ParsedScript(ScriptEngine engine, IntPtr dispatch)
        {
            _engine = engine;
            _dispatch = Marshal.GetObjectForIUnknown(dispatch);
        }

        /// <summary>
        /// Calls a method.
        /// </summary>
        /// <param name="methodName">The method name. May not be null.</param>
        /// <param name="arguments">The optional arguments.</param>
        /// <returns>The call result.</returns>
        public object CallMethod(string methodName, params object[] arguments)
        {
            if (_dispatch == null)
                throw new InvalidOperationException();

            if (methodName == null)
                throw new ArgumentNullException("methodName");

            try
            {
                return _dispatch.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, _dispatch, arguments);
            }
            catch
            {
                if (_engine._site._lastException != null)
                    throw _engine._site._lastException;

                throw;
            }
        }

        void IDisposable.Dispose()
        {
            if (_dispatch != null)
            {
                Marshal.ReleaseComObject(_dispatch);
                _dispatch = null;
            }
        }
    }
}