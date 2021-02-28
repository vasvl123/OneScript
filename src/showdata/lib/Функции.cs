// MIT License
// Copyright (c) 2020 vasvl123
// https://github.com/vasvl123/useyourmind

using System;
using ScriptEngine.HostedScript.Library;

namespace showdata.lib
{
    class Функции : functions
    {
        public Функции() : base("Функции") { }

        public object УзелСвойство(Структура Узел, string Свойство)
        {
            Перем УзелСвойство = null;
            if (!(Узел == Неопределено))
            {
                Узел.Свойство(Свойство, ref УзелСвойство);
            }
            return Вернуть(УзелСвойство);
        } // УзелСвойство(Узел)


        public object НоваяВкладка(pagedata Данные, dynamic Параметры)
        {
            Параметры.Вставить("cmd", "newtab");
            Данные.Процесс.НоваяЗадача(Параметры, "Служебный");
            return Неопределено;
        } // НоваяВкладка()


        public object НоваяБаза(pagedata Данные, dynamic Параметры)
        {
            var БазаДанных = Параметры.БазаДанных;
            var Запрос = Новый_Структура("Данные, БазаДанных, cmd", Данные, БазаДанных, "НоваяБаза");
            Данные.Процесс.НоваяЗадача(Запрос, "Служебный");
            return Неопределено;
        }


    }
}
