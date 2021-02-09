﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library;

namespace ScriptEngine.HostedScript.Library
{

    public class functions : onesharp
    {
        public functions() {
            МоментЗапуска = ТекущаяУниверсальнаяДатаВМиллисекундах();
        }

        public decimal МоментЗапуска;

        public decimal ПолучитьИД()
        {
            МоментЗапуска -= 1;
            return Цел(ТекущаяУниверсальнаяДатаВМиллисекундах() - МоментЗапуска);
        }

        public Структура ИмяЗначение(string Имя = "", string Значение = "")
        {
            var str = Новый_Структура();
            str.Вставить("Имя", Имя);
            str.Вставить("Значение", Значение);
            return str;
        }

        public Перем НПерем(IValue з)
        {
            return Новый(з);
        }

        public IValue НЗнач(Перем з)
        {
            return з._Value;
        }

        public ДвоичныеДанные СтруктуруВДвоичныеДанные(Перем знСтруктура)
        {
            var Результат = Новый_Массив();

            if (знСтруктура != Неопределено)
            {
                foreach (КлючИЗначение Элемент in знСтруктура as Список)
                {
                    object Ключ = "";
                    object Значение;
                    ДвоичныеДанные дЗначение;

                    if (ТипЗнч(знСтруктура) == Тип("Массив"))
                    {
                        Ключ = "";
                        Значение = Элемент.Значение;
                    }
                    else
                    {
                        Ключ = Элемент.Ключ;
                        Значение = Элемент.Значение;
                    }

                    if (ТипЗнч(Значение) == Тип("Структура"))
                    {
                        Ключ = "*" + Ключ;
                        дЗначение = СтруктуруВДвоичныеДанные((Структура)Значение);
                    }
                    else if (ТипЗнч(Значение) == Тип("Соответствие"))
                    {
                        Ключ = "&" + Ключ;
                        дЗначение = СтруктуруВДвоичныеДанные((Соответствие)Значение);
                    }
                    else if (ТипЗнч(Значение) == Тип("Массив"))
                    {
                        Ключ = "$" + Ключ;
                        дЗначение = СтруктуруВДвоичныеДанные((Массив)Значение);
                    }
                    else if (ТипЗнч(Значение) == Тип("ДвоичныеДанные"))
                    {
                        Ключ = "#" + Ключ;
                        дЗначение = Значение as ДвоичныеДанные;
                    }
                    else
                    {
                        if (ТипЗнч(Значение) == Тип("Число"))
                            Ключ = "!" + Ключ;
                        дЗначение = ПолучитьДвоичныеДанныеИзСтроки(Строка(Значение));
                    }

                    var дКлюч = ПолучитьДвоичныеДанныеИзСтроки(Ключ);
                    var рдКлюч = дКлюч.Размер();
                    var рдЗначение = дЗначение.Размер();
                    var бРезультат = Новый_БуферДвоичныхДанных(6);
                    бРезультат.ЗаписатьЦелое16(0, рдКлюч);
                    бРезультат.ЗаписатьЦелое32(2, рдЗначение);
                    Результат.Добавить(ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(бРезультат));
                    Результат.Добавить(дКлюч);
                    Результат.Добавить(дЗначение);

                }
            }
            return СоединитьДвоичныеДанные(Результат);
        }

        public Перем ДвоичныеДанныеВСтруктуру(Перем Данные, Перем парСтруктура = null)
        {
            Перем знСтруктура = парСтруктура;
            БуферДвоичныхДанных бдДанные;
            double рдДанные;

            if (ТипЗнч(Данные) == Тип("ДвоичныеДанные")) {
                ДвоичныеДанные дд = Данные as ДвоичныеДанные;
                рдДанные = дд.Размер();
                if (рдДанные == 0) return Неопределено;
                бдДанные = ПолучитьБуферДвоичныхДанныхИзДвоичныхДанных(дд);
            }
            else if (ТипЗнч(Данные) == Тип("БуферДвоичныхДанных"))
            {
                бдДанные = Данные as БуферДвоичныхДанных;
                рдДанные = бдДанные.Размер;
            }
            else 
                return Неопределено;

	        var Позиция = 0;

	        if (знСтруктура == Неопределено)
                знСтруктура = Новый_Структура();

            while (Позиция < рдДанные - 1)
            {

                var рдКлюч = бдДанные.ПрочитатьЦелое16(Позиция);
                var рдЗначение = бдДанные.ПрочитатьЦелое32(Позиция + 2);

                if (рдКлюч + рдЗначение > рдДанные)  // Это не структура
                    return Неопределено;

                var Ключ = ПолучитьСтрокуИзДвоичныхДанных(ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(бдДанные.Прочитать(Позиция + 6, рдКлюч)));
                var бЗначение = бдДанные.Прочитать(Позиция + 6 + рдКлюч, рдЗначение);
                Позиция = Позиция + 6 + рдКлюч + рдЗначение;

                object Значение;

                var Л = Лев(Ключ, 1);
                if (Л == "*")
                {
                    Ключ = Сред(Ключ, 2);
                    Значение = ДвоичныеДанныеВСтруктуру(бЗначение, Новый_Структура());
                }
                else if (Л == "&")
                {
                    Ключ = Сред(Ключ, 2);
                    Значение = ДвоичныеДанныеВСтруктуру(бЗначение, Новый_Соответствие());
                }
                else if (Л == "$")
                {
                    Ключ = Сред(Ключ, 2);
                    Значение = ДвоичныеДанныеВСтруктуру(бЗначение, Новый_Массив());
                }
                else if (Л == "#")
                {
                    Ключ = Сред(Ключ, 2);
                    Значение = ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(бЗначение);
                }
                else
                {
                    Значение = Новый(ПолучитьСтрокуИзДвоичныхДанных(ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(бЗначение)));
                    if (Л == "!")
                    {
                        Ключ = Сред(Ключ, 2);
                        Значение = Число(Значение);
                    }
                }
                if (Ключ == "")
                    (знСтруктура as Массив).Добавить(Значение);
                else
                    (знСтруктура as Структура).Вставить(Ключ, Значение);
            }
	        return знСтруктура;
        }

        public TCPСоединение ПередатьДанные(string Хост, int Порт, Перем стрДанные) 
        {
            var Соединение = Новый_TCPСоединение(Хост, Порт);

            try
            {
                Соединение.ТаймаутОтправки = 5000;
                Соединение.ОтправитьДвоичныеДанныеАсинхронно(СтруктуруВДвоичныеДанные(стрДанные));
                return Соединение;
            } 
            catch
            {
                //Сообщить(ОписаниеОшибки());
                if (Соединение == Неопределено)
                {
                    Сообщить("starter: Хост недоступен: " + Хост + ":" + Порт);
                } 
                else
                {
                    Соединение.Закрыть();
                    Соединение = null;
                }
            }
            return Соединение; // ПередатьДанные()
        }

        public IValue Тест(IValue arg)
        {
            var струк = Новый(arg) as Структура;

            var k = Новый_Структура();

            var ууу = ИмяЗначение("ааа", "ппп");

            var k1 = Строка(k.Количество());

            Сообщить(k1);
            //Сообщить(ууу.Получить("Имя"));

            //                var ттип1 = Тип("Структура");
            //                var ттип2 = ТипЗнч(k._str);
            //
            //                Сообщить(ТипЗнч(ттип1));

            //Console.WriteLine(123);

            var b = СтруктуруВДвоичныеДанные(Новый(arg));

            //Сообщить(b);

            //foreach (var Элемент in ууу)
            //{
            //    Сообщить(Элемент.Ключ);
            //}

            var c = ДвоичныеДанныеВСтруктуру(b);

            //var струк1 = Новый(c) as Структура;

            if (струк == c)
            {
                Сообщить("ага!");
            }

            return c.Value;

        }


    }


    [ContextClass("Фреймворк", "useyourmind")]
    public class useyourmind : AutoContext<useyourmind>
    {

        private functions func = new functions();
   
        public void initobj(SystemGlobalContext syscon)
        {
            func._syscon = syscon;
        }

        [ContextMethod("СтруктуруВДвоичныеДанные", "СтруктуруВДвоичныеДанные")]
        public IValue СтруктуруВДвоичныеДанные(IValue arg = null)
        {
            return func.НЗнач(func.СтруктуруВДвоичныеДанные(func.НПерем(arg)));
        }

        [ContextMethod("ДвоичныеДанныеВСтруктуру", "ДвоичныеДанныеВСтруктуру")]
        public IValue ДвоичныеДанныеВСтруктуру(IValue arg = null)
        {
            return func.НЗнач(func.ДвоичныеДанныеВСтруктуру(func.НПерем(arg)));
        }

        [ContextMethod("ПередатьДанные", "ПередатьДанные")]
        public IValue ПередатьДанные(IValue arg1, IValue arg2, IValue arg3)
        {
            return func.НЗнач(func.ПередатьДанные(arg1.AsString(), (int)arg2.AsNumber(), func.НПерем(arg3)));
        }

        [ContextMethod("Тест", "Тест")]
        public IValue Тест(IValue arg = null)
        {
            return func.Тест(arg);
        }

        [ContextMethod("ИмяЗначение", "ИмяЗначение")]
        public IValue ИмяЗначение(string Имя = "", string Значение = "")
        {
            return func.ИмяЗначение(Имя, Значение).Value;
        }

        [ScriptConstructor]
        public static useyourmind Construct()
        {
            return new useyourmind();
        }
    }
}