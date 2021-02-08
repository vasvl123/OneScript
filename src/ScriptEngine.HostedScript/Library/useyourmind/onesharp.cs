using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Values;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.HostedScript.Library.Binary;
using ScriptEngine.HostedScript.Library.Net;

namespace ScriptEngine.HostedScript
{
    public class onesharp
    {

        public SystemGlobalContext _syscon;
        public GlobalBinaryData _glbin;
        public FileOperations _fileop;

        [AttributeUsage(AttributeTargets.Parameter)]
        public class ByRefAttribute : Attribute
        {
        }

        public Перем Неопределено;
        public bool Истина = true;
        public bool Ложь = false;

        public string[] АргументыКоманднойСтроки
        {
            get { return _syscon.ApplicationHost.GetCommandLineArguments(); }
        }

        public class КлючИЗначение
        {
            private readonly Перем _key;
            private readonly Перем _value;

            public КлючИЗначение(Перем key, Перем value)
            {
                _key = key;
                _value = value;
            }

            public Перем Ключ
            {
                get
                {
                    return _key;
                }
            }

            public Перем Значение
            {
                get
                {
                    return _value;
                }
            }

        }

        public static Перем Новый(string val)
        {
            return new Перем(ValueFactory.Create(val));
        }

        public static Перем Новый(IValue val = null) {

            if (val == null) return new Перем(ValueFactory.Create());

            var vartype = val.SystemType.ToString();
            switch (vartype)
            {
            case "Структура":
                return new Структура(val);
            case "Соответствие":
                return new Соответствие(val);
            case "Массив":
                return new Массив(val);
            case "ДвоичныеДанные":
                return new ДвоичныеДанные(val);
            case "БуферДвоичныхДанных":
                return new БуферДвоичныхДанных(val);
            case "TCPСоединение":
                return new TCPСоединение(val);
            case "TCPСервер":
                return new TCPСервер(val);
            default:
                return new Перем(val);
            }

        }

 
        public class Перем : IComparable<IValue>, IEquatable<IValue>
        {
            public IValue _Value;
            public string _vartype;

            public IValue Value
            {
                get
                {
                    return _Value;
                }
            }

            public string Тип
            {
                get
                {
                    return _vartype;
                }
            }

            public Перем()
            {
            }

            public Перем(IValue val)
            {
                _vartype = val.SystemType.ToString();
                _Value = val;
            }

            public override string ToString()
            {
                return _Value.AsString();
            }

            public string Строка
            {
                get
                {
                    return _Value.AsString();
                }
            }

            public decimal Число
            {
                get
                {
                    return _Value.AsNumber();
                }
            }

            public bool Булево 
            {
                get
                {
                    return _Value.AsBoolean();
                }
            }

            #region IComparable<IValue> Members

            public int CompareTo(IValue other)
            {
                return _Value.CompareTo(other);
            }

            #endregion

            #region IEquatable<IValue> Members

            public bool Equals(IValue other)
            {
                return _Value.Equals(other);
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            #endregion

            public bool Equals(Перем other)
            {
                if (other is null) return false;
                return _Value.Equals(other._Value);
            }

            public static bool operator ==(Перем lhs, Перем rhs)
            {
                if (lhs is null) return (bool)(rhs is null);
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Перем lhs, Перем rhs)
            {
                if (lhs is null) return (bool)!(rhs is null);
                return !lhs.Equals(rhs);
            }


        }

        public class Список : Перем, IEnumerable<КлючИЗначение> 
        {
            public virtual IEnumerator<КлючИЗначение> GetEnumerator() {
                return null;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        }


        public class Структура : Список
        {
            StructureImpl _val;

            public StructureImpl Impl
            {
                get
                {
                    return _val;
                }
            }

            public Структура()
            {
                _vartype = "Структура";
                _val = new StructureImpl();
                _Value = _val;
            }

            public Структура(IValue val)
            {
                _vartype = "Структура";
                _val = val as StructureImpl;
                _Value = val;
            }


            public Структура(string strProperties, params IValue[] values)
            {
                _vartype = "Структура";
                _val = new StructureImpl(strProperties, values);
                _Value = _val;
            }

            public Структура(string strProperties)
            {
                _vartype = "Структура";
                _val = new StructureImpl(strProperties);
                _Value = _val;
            }

            public int Количество()
            {
                return _val.Count();
            }

            public bool Свойство(string name, [ByRef] Перем value = null)
            {
                if (value == null)
                    return _val.HasProperty(name);
                else
                {
                    var v = value._Value as IVariable;
                    var b = _val.HasProperty(name, v);
                    value = Новый(v);
                    return b;
                }
            }

            public Перем Получить(string name = null)
            {
                return Новый(_val.GetPropValue(_val.FindProperty(name)));
            }

            public void Вставить(string name, Перем val = null)
            {
                _val.Insert(name, val.Value);
            }

            public void Вставить(string name, string val)
            {
                _val.Insert(name, ValueFactory.Create(val));
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

             public static bool operator ==(Структура lhs, Структура rhs)
            {
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Структура lhs, Структура rhs)
            {
                return !lhs.Equals(rhs);
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        Новый(item.Key), Новый(item.Value));
                }

            }

        }


        public Структура Новый_Структура()
        {
            return new Структура();
        }

        public Структура Новый_Структура(string strProperties)
        {
            return new Структура(strProperties);
        }

        public Структура Новый_Структура(string strProperties, object arg)
        {
            return Новый_Структура(strProperties, new object[] {arg});
        }

        public Структура Новый_Структура(string strProperties, params object[] values)
        {
            var arr = new List<IValue>();
            foreach (object p in values) {
                if (p is int) 
                    arr.Add(ValueFactory.Create((int)p));
                else if (p is string)
                    arr.Add(ValueFactory.Create((string)p));
                else if (p is bool)
                    arr.Add(ValueFactory.Create((bool)p));
                else if (p is Перем)
                    arr.Add(((Перем)p)._Value);
                }
            return new Структура(strProperties, arr.ToArray());
        }

        public class Соответствие : Список
        {
            MapImpl _val;

            public MapImpl Impl
            {
                get
                {
                    return _val;
                }
            }

            public Соответствие()
            {
                _vartype = "Соответствие";
                _val = new MapImpl();
                _Value = _val;
            }

            public Соответствие(IValue val)
            {
                _vartype = "Соответствие";
                _val = val as MapImpl;
                _Value = val;
            }

            public int Количество()
            {
                return _val.Count();
            }

            public Перем Получить(string name = null)
            {
                return Новый(_val.GetPropValue(_val.FindProperty(name)));
            }

            public void Вставить(Перем key, Перем val = null)
            {
                _val.Insert(key.Value, val.Value);
            }

            public void Вставить(string key, string val)
            {
                _val.Insert(ValueFactory.Create(key), ValueFactory.Create(val));
            }
            public void Удалить(string key)
            {
                _val.Delete(ValueFactory.Create(key));
            }

            public void Удалить(Перем key)
            {
                _val.Delete(key._Value);
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(Соответствие lhs, Соответствие rhs)
            {
                 return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(Соответствие lhs, Соответствие rhs)
            {
                return !lhs.Equals(rhs._Value);
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        Новый(item.Key), Новый(item.Value));
                }

            }

        }

        public Соответствие Новый_Соответствие()
        {
            return new Соответствие();
        }



        public class Массив : Список
        {
            ArrayImpl _val;

            public ArrayImpl Impl
            {
                get
                {
                    return _val;
                }
            }

            public Массив()
            {
                _vartype = "Массив";
                _val = new ArrayImpl();
                _Value = _val;
            }

            public Массив(IValue val)
            {
                _vartype = "Массив";
                _val = val as ArrayImpl;
                _Value = val;
            }

            public int Количество()
            {
                return _val.Count();
            }

            public Перем Получить(int index)
            {
                return Новый(_val.Get(index));
            }

            public void Удалить(int index)
            {
                _val.Remove(index);
            }

            public void Вставить(int pos, Перем val)
            {
                _val.Insert(pos, val.Value);
            }

            public void Добавить(Перем val)
            {
                _val.Add(val.Value);
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(Массив lhs, Массив rhs)
            {
                return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(Массив lhs, Массив rhs)
            {
                return !lhs.Equals(rhs._Value);
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        null, Новый(item));
                }

            }

        }



        public Массив Новый_Массив()
        {
            return new Массив();
        }



        public class ДвоичныеДанные : Перем
        {
            BinaryDataContext _val;
 
            public BinaryDataContext Impl
            {
                get
                {
                    return _val;
                }
            }

            public ДвоичныеДанные(IValue val)
            {
                _vartype = "ДвоичныеДанные";
                _val = val as BinaryDataContext;
                _Value = val;
            }

            public int Размер()
            {
                return _val.Size();
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(ДвоичныеДанные lhs, ДвоичныеДанные rhs)
            {
                return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(ДвоичныеДанные lhs, ДвоичныеДанные rhs)
            {
                return !lhs.Equals(rhs._Value);
            }


        }

        public ДвоичныеДанные Новый_ДвоичныеДанные(string arg1)
        {
            return new ДвоичныеДанные(new BinaryDataContext(arg1));
        }



        public class БуферДвоичныхДанных : Перем
        {
            BinaryDataBuffer _val;
 
            public BinaryDataBuffer Impl
            {
                get
                {
                    return _val;
                }
            }

            public БуферДвоичныхДанных(IValue val)
            {
                _vartype = "БуферДвоичныхДанных";
                _val = val as BinaryDataBuffer;
                _Value = val;
            }

            public double Размер
            {
                get { return _val.Size; }
            }

            public БуферДвоичныхДанных Прочитать(int arg1, int arg2)
            {
                return new БуферДвоичныхДанных(_val.Read(arg1, arg2));
            }

            public int ПрочитатьЦелое16(int arg1)
            {
                return _val.ReadInt16(arg1);
            }

            public int ПрочитатьЦелое32(int arg1)
            {
                return (int)_val.ReadInt32(arg1);
            }

            public void ЗаписатьЦелое16(int arg1, int arg2)
            {
                _val.WriteInt16(arg1, ValueFactory.Create(arg2));
            }

            public void ЗаписатьЦелое32(int arg1, int arg2)
            {
                _val.WriteInt32(arg1, ValueFactory.Create(arg2));
            }

            public void ЗаписатьЦелое16(int arg1, Перем arg2)
            {
                _val.WriteInt16(arg1, arg2._Value);
            }

            public void ЗаписатьЦелое32(int arg1, Перем arg2)
            {
                _val.WriteInt32(arg1, arg2._Value);
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(БуферДвоичныхДанных lhs, БуферДвоичныхДанных rhs)
            {
                return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(БуферДвоичныхДанных lhs, БуферДвоичныхДанных rhs)
            {
                return !lhs.Equals(rhs._Value);
            }

        }

        public БуферДвоичныхДанных Новый_БуферДвоичныхДанных(int arg1)
        {
            return new БуферДвоичныхДанных(BinaryDataBuffer.Constructor(ValueFactory.Create(arg1)));
        }




        public class TCPСоединение : Перем
        {
            TCPClient _val;

            public TCPClient Impl
            {
                get
                {
                    return _val;
                }
            }

            public TCPСоединение(IValue val)
            {
                _vartype = "TCPСоединение";
                _val = val as TCPClient;
                _Value = val;
            }

            public string Статус
            {
                get { return _val.Status; }
            }

            public int ТаймаутОтправки
            {
                get { return _val.WriteTimeout; }
                set { _val.WriteTimeout = value; }
            }

            public void ОтправитьДвоичныеДанныеАсинхронно(ДвоичныеДанные data)
            {
                _val.SendBinaryDataAsync(data.Impl);
            }

            public ДвоичныеДанные ПолучитьДвоичныеДанные()
            {
                return new ДвоичныеДанные(_val.GetBinaryData());
            }

            public void Закрыть()
            {
                _val.Close();
            }

            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(TCPСоединение lhs, Перем rhs)
            {
                return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(TCPСоединение lhs, Перем rhs)
            {
                return !lhs.Equals(rhs._Value);
            }

        }

        public TCPСоединение Новый_TCPСоединение(Перем Хост, Перем Порт)
        {
            return new TCPСоединение(TCPClient.Constructor(Хост.Value, Порт.Value));
        }



        public class TCPСервер : Перем
        {
            TCPServer _val;

            public TCPServer Impl
            {
                get
                {
                    return _val;
                }
            }

            public TCPСервер(IValue val)
            {
                _vartype = "TCPСервер";
                _val = val as TCPServer;
                _Value = val;
            }

            public void Запустить()
            {
                _val.Start();
            }
            public void ЗапуститьАсинхронно()
            {
                _val.StartAsync();
            }
            public TCPСоединение ОжидатьСоединения(int timeout = 0)
            {
                return new TCPСоединение(_val.WaitForConnection(timeout));
            }

            public TCPСоединение ПолучитьСоединение(int timeout = 0)
            {
                return new TCPСоединение(_val.GetConnection(timeout));
            }

            public bool ПриниматьЗаголовки
            {
                get { return _val.ReadHeaders; }
                set { _val.ReadHeaders = value; }
            }

            public void Остановить()
            {
                _val.Stop();
            }


            public override bool Equals(object other)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            new public bool Equals(IValue other)
            {
                return _val.Equals(other);
            }

            public static bool operator ==(TCPСервер lhs, Перем rhs)
            {
                return lhs.Equals(rhs._Value);
            }

            public static bool operator !=(TCPСервер lhs, Перем rhs)
            {
                return !lhs.Equals(rhs._Value);
            }

        }

        public TCPСервер Новый_TCPСервер(Перем Порт)
        {
            return new TCPСервер(TCPServer.ConstructByPort(Порт._Value));
        }

        public class СистемнаяИнформация
        {
            SystemEnvironmentContext _sysenv;

            public СистемнаяИнформация()
            {
                _sysenv = new SystemEnvironmentContext();
            }

             public string ВерсияОС
            {
                get
                {
                    return _sysenv.OSVersion;
                }
            }

        }

        public СистемнаяИнформация Новый_СистемнаяИнформация()
        {
            return new СистемнаяИнформация();
        }



        public string ТекущийКаталог()
        {
            return _fileop.CurrentDirectory();
        }

        public void ЗапуститьПриложение(string cmdLine, string currentDir = null, bool wait = false, [ByRef] Перем retCode = null)
        {
            if (retCode == null)
                _syscon.RunApp(cmdLine, currentDir, wait); 
            else
                _syscon.RunApp(cmdLine, currentDir, wait, retCode._Value as IVariable);
        }

        public void Сообщить(Перем message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            _syscon.ApplicationHost.Echo(message.Value.AsString(), status);
        }

        public void Сообщить(string message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            _syscon.ApplicationHost.Echo(message, status);
        }

        public void Сообщить(bool message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            _syscon.ApplicationHost.Echo(message.ToString(), status);
        }

        public Перем Строка(string arg)
        {
            return new Перем(ValueFactory.Create(arg));
        }

        public Перем Строка(int arg)
        {
            return new Перем(ValueFactory.Create(ValueFactory.Create(arg).AsString()));
        }

        public Перем Строка(Перем arg)
        {
            return new Перем(ValueFactory.Create(arg.Value.AsString()));
        }

        public Перем Число(int arg)
        {
            return new Перем(ValueFactory.Create(arg));
        }

        public Перем Число(decimal arg)
        {
            return new Перем(ValueFactory.Create(arg));
        }

        public Перем Число(Перем arg)
        {
            return new Перем(ValueFactory.Create(arg._Value.AsNumber()));
        }

        public Перем Булево(bool arg)
        {
            return new Перем(ValueFactory.Create(arg));
        }

        public string ТипЗнч(Перем arg)
        {
            return arg._vartype;
            //return new TypeTypeValue(arg._Value.SystemType);
        }

        public string Тип(string arg)
        {
            return arg;
            //return new TypeTypeValue(arg);
        }

        public DateTime ТекущаяДата()
        {
            return DateTime.Now;
        }

        public string Лев(string str, int len)
        {
            if (len > str.Length)
                len = str.Length;
            else if (len < 0)
            {
                return "";
            }

            return str.Substring(0, len);
        }

        public string Прав(string str, int len)
        {
            if (len > str.Length)
                len = str.Length;
            else if (len < 0)
            {
                return "";
            }

            int startIdx = str.Length - len;
            return str.Substring(startIdx, len);
        }

        public string Сред(string str, int start)
        {
            return Сред(str, start, str.Length - start + 1);
        }

        public string Сред(string str, int start, int len)
        {
            if (start < 1)
                start = 1;

            if (start + len > str.Length || len < 0)
                len = str.Length - start + 1;

            string result;

            if (start > str.Length || len == 0)
            {
                result = "";
            }
            else
            {
                result = str.Substring(start - 1, len);
            }

            return result;
        }

        public void Приостановить(int delay)
        {
            System.Threading.Thread.Sleep(delay);
        }



        public decimal ТекущаяУниверсальнаяДатаВМиллисекундах()
        {
            return (decimal)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public decimal Цел(decimal val)
        {
            return Math.Truncate(val);
        }

        public ДвоичныеДанные СоединитьДвоичныеДанные(Массив arg)
        {
            return new ДвоичныеДанные(_glbin.ConcatenateBinaryData(arg.Impl));
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(Перем arg)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromString(arg.Value.AsString()));
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(string arg)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromString(arg));
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(БуферДвоичныхДанных arg)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromBinaryDataBuffer(arg.Impl));
        }

        public БуферДвоичныхДанных ПолучитьБуферДвоичныхДанныхИзДвоичныхДанных(ДвоичныеДанные arg)
        {
            return new БуферДвоичныхДанных(_glbin.GetBinaryDataBufferFromBinaryData(arg.Impl));
        }

        public string ПолучитьСтрокуИзДвоичныхДанных(ДвоичныеДанные arg)
        {
            return _glbin.GetStringFromBinaryData(arg.Impl);
        }


        public onesharp ()
        {
            _glbin = new GlobalBinaryData();
            _fileop = new FileOperations();
        }
    }
}

