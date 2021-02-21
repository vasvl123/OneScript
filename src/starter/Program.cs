﻿// /*----------------------------------------------------------
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v.2.0. If a copy of the MPL
// was not distributed with this file, You can obtain one
// at http://mozilla.org/MPL/2.0/.
// ----------------------------------------------------------*/

using System;
using System.Linq;
using ScriptEngine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;

namespace starter
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


    class MainClass
    {

        public static void Main(string[] args)
        {
            var hostedScript = new HostedScriptEngine();
            var app = new starter();
            app._syscon = new SystemGlobalContext();
            var host = new ApplicationHost();
            host.CommandLineArguments = args.ToArray();
            app._syscon.ApplicationHost = host;
            app.Main();
        }
    }
}
