using System;
using ScriptEngine;
using ScriptEngine.HostedScript.Library;

namespace starter
{

    class starter : functions
    {

        public new void Сообщить(Перем message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            ConsoleHostImpl.Echo(message.Value.AsString(), status);
        }

        public new void Сообщить(string message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            ConsoleHostImpl.Echo(message, status);
        }

        public new void Сообщить(bool message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            ConsoleHostImpl.Echo(message.ToString(), status);
        }


        public void Main() {
            Сообщить(ПолучитьИД().ToString());
            Сообщить(ПолучитьИД().ToString());
            Сообщить("Hello World!");
            Сообщить(ПолучитьИД().ToString());
            Сообщить(ПолучитьИД().ToString());
            Сообщить("Hello World!");
            Сообщить(ПолучитьИД().ToString());
            Сообщить(ПолучитьИД().ToString());
            Сообщить(ПолучитьИД().ToString());
        }


    }

    class MainClass
    {

        public static void Main(string[] args)
        {

            var e = new ScriptingEngine();
            var starter = new starter();
            starter.Main();

        }
    }
}
