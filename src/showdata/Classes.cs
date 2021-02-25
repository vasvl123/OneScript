using System;
using System.Collections.Generic;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;

namespace showdata
{

    public class ТипУзел : onesharp.Структура
    {

        public string Код { get { return (string)Получить("Код"); } set { Вставить("Код", value); } }
        public string Имя { get { return (string)Получить("Имя"); } set { Вставить("Имя", value); } }
        public object Значение { get { return Получить("Значение"); } set { Вставить("Значение", value); } }
        public ТипУзел Атрибут { get { return _ТипУзел(Получить("Атрибут")); } set { Вставить("Атрибут", value); } }
        public ТипУзел Дочерний { get { return _ТипУзел(Получить("Дочерний")); } set { Вставить("Дочерний", value); } }
        public ТипУзел Соседний { get { return _ТипУзел(Получить("Соседний")); } set { Вставить("Соседний", value); } }
        public ТипУзел Старший { get { return _ТипУзел(Получить("Старший")); } set { Вставить("Старший", value); } }
        public ТипУзел Родитель { get { return _ТипУзел(Получить("Родитель")); } set { Вставить("Родитель", value); } }
        public dynamic п { get { return Получить("п") as onesharp.Структура; } set { Вставить("п", value); } }
        public dynamic д { get { return Получить("д") as onesharp.Структура; } set { Вставить("д", value); } }

        public ТипУзел(string strProperties, params object[] values)
        {
            var arr = new List<IValue>();
            foreach (object p in values)
            {
                arr.Add(onesharp.Знач(p));
            }
            _vartype = "Структура";
            Impl = new StructureImpl(strProperties, arr.ToArray());
        }

        public ТипУзел(onesharp.Структура str)
        {
            _vartype = "Структура";
            Impl = str.Impl;
        }

        ТипУзел _ТипУзел(object str) {
            if (str is null) return null;
            return new ТипУзел(str as onesharp.Структура);
        }
    }
}
