/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace onesharp
{
    public class Узел : Структура
    {
 
        public Узел() : base() {}

        public dynamic с { get { return this; } }
        public dynamic д { get { return this["д"]; } }


        public string Код { get { return (string)this["Код"]; } }
        public string Имя { get { return (string)this["Имя"]; } }
        public object Значение { get { return this["Значение"]; } set { this["Значение"] = value; } }

        public Узел Дочерний { get { return (Узел)this["Дочерний"]; } set { this["Дочерний"] = value; } }
        public Узел Соседний { get { return (Узел)this["Соседний"]; } set { this["Соседний"] = value; } }
        public Узел Атрибут { get { return (Узел)this["Атрибут"]; } set { this["Атрибут"] = value; } }
        public Узел Старший { get { return (Узел)this["Старший"]; } set { this["Старший"] = value; } }
        public Узел Родитель { get { return (Узел)this["Родитель"]; } set { this["Родитель"] = value; } }

        public Узел(Структура structure) : base(structure) { }

        public Узел(string strProperties, params object[] values) : base(strProperties, values) { }


        /// <summary>
        /// Создает структуру по фиксированной структуре
        /// </summary>
        /// <param name="fixedStruct">Исходная структура</param>
        //[ScriptConstructor(Name = "Из фиксированной структуры")]
        private new static Узел Новый(Структура fixedStruct)
        {
            return new Узел(fixedStruct);
        }

        /// <summary>
        /// Создает структуру по заданному перечню свойств и значений
        /// </summary>
        /// <param name="param1">Фиксированная структура либо строка с именами свойств, указанными через запятую.</param>
        /// <param name="args">Только для перечня свойств:
        /// Значения свойств. Каждое значение передается, как отдельный параметр.</param>
        public new static Узел Новый(string param1, params object[] args)
        {
            return new Узел(param1, args);
        }

        public new static Узел Новый()
        {
            return new Узел();
        }

        private static RuntimeException InvalidPropertyNameException( string name )
        {
            return new RuntimeException($"Задано неправильное имя атрибута структуры '{name}'");
        }

    }
}
