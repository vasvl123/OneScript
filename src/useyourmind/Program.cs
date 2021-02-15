using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ScriptEngine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;

namespace useyourmind
{
    public class ApplicationHost : IHostApplication
    {
        public string[] CommandLineArguments { get; set; } = new string[0];

        public void Echo(string text, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            ConsoleHostImpl.Echo(text, status);
        }

        public void ShowExceptionInfo(Exception exc)
        {
            ConsoleHostImpl.ShowExceptionInfo(exc);
        }

        public bool InputString(out string result, int maxLen)
        {
            return ConsoleHostImpl.InputString(out result, maxLen);
        }

        public string[] GetCommandLineArguments()
        {
            return CommandLineArguments;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var host = new ApplicationHost();
            var source = args[0];
            host.CommandLineArguments = args.Skip(1).ToArray();
            var Engine = new HostedScriptEngine();
            Engine.Initialize();
            var process = Engine.CreateProcess(host, Engine.Loader.FromFile(source));
            process.Start();
        }

    }
}
