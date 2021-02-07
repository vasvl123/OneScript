using System;
using ScriptEngine;
using ScriptEngine.HostedScript.Library;

namespace starter
{

    class starter : functions
    {
        Перем Хост;
        Перем Контроллеры;
        Перем Локальный;
        Перем mono;
        Перем showdata;
        Перем Соединения;
        Перем мЗадачи;

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

        public bool ЗапуститьПроцесс(string Имя, Перем Порт, string Параметры = "")
        {
            try
            {
                Сообщить("Запуск " + Имя + " ...");
                // Проверка свободного порта
                TCPСервер Сервер = Новый_TCPСервер(Порт);
                Сервер.Запустить();
                Сервер.Остановить();
                ЗапуститьПриложение(mono + "uascript.exe " + Имя + " " + Порт + " " + Параметры, ТекущийКаталог());
                Приостановить(200); // ???
            }
            catch
            {
                //Сообщить(ОписаниеОшибки());
                return Ложь;
            }
            return Истина;
        } // ЗапуститьПроцесс()


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
