using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.HostedScript.Library.Binary;
using ScriptEngine.HostedScript.Library.Net;
using ScriptEngine.HostedScript.Library.Hash;


namespace ScriptEngine.HostedScript
{
    public class onesharp
    {

        public SystemGlobalContext _syscon;
        public GlobalBinaryData _glbin;
        public FileOperations _fileop;
        public StringOperations _strop;
        public MiscGlobalFunctions _miscf;
        public static FileStreamsManager _filestrm;

        public symbols Символы;
        public urlenc СпособКодированияСтроки;

        [AttributeUsage(AttributeTargets.Parameter)]
        public class ByRefAttribute : Attribute
        {
        }

        public static object Неопределено;
        public static bool Истина = true;
        public static bool Ложь = false;

        public static object Empty;

        public class urlenc
        {
            EnumerationValue URLEncoding;
            EnumerationValue URLInURLEncoding;

            public urlenc()
            {
                var encMethod = GlobalsManager.GetEnum<StringEncodingMethodEnum>();
                URLEncoding = encMethod.URLEncoding;
                URLInURLEncoding = encMethod.URLInURLEncoding;
            }

            public EnumerationValue КодировкаURL
            {
                get
                {
                    return URLEncoding;
                }
            }

            public EnumerationValue URLВКодировкеURL
            {
                get
                {
                    return URLInURLEncoding;
                }
            }

        }


        public class symbols
        {

            public string ПС
            {
                get
                {
                    return "\n";
                }
            }

            public string ВК
            {
                get
                {
                    return "\r";
                }
            }

            public string ВТаб
            {
                get
                {
                    return "\v";
                }
            }

            public string Таб
            {
                get
                {
                    return "\t";
                }
            }

            public string ПФ
            {
                get
                {
                    return "\f";
                }
            }

            public string НПП
            {
                get
                {
                    return "\u00A0";
                }
            }

        }

        public string[] АргументыКоманднойСтроки
        {
            get { return _syscon.ApplicationHost.GetCommandLineArguments(); }
        }

        public class КлючИЗначение
        {
            private readonly object _key;
            private readonly object _value;

            public КлючИЗначение(object key, object value)
            {
                _key = key;
                _value = value;
            }

            public object Ключ
            {
                get
                {
                    return _key;
                }
            }

            public object Значение
            {
                get
                {
                    return _value;
                }
            }

        }

        public static object Вернуть(object arg)
        {
            if (arg is Перем)
            {
                var v = ((Перем)arg);
                switch (v._vartype)
                {
                    case "Число":
                        var n = v._Value.AsNumber();
                        try
                        {
                            return Decimal.ToInt32(n);
                        }
                        catch //(OverflowException e)
                        {
                            return n;
                        }
                    case "Строка":
                        return v._Value.AsString();
                    case "Булево":
                        return v._Value.AsBoolean();
                    case "Дата":
                        return v._Value.AsDate();
                    default:
                        return arg;
                }
            }

            if (arg is IValue)
            {
                var v = (IValue)arg;
                switch (v.SystemType.ToString())
                {
                    case "Число":
                        var n = v.AsNumber();
                        try
                        {
                            return Decimal.ToInt32(n);
                        }
                        catch //(OverflowException e)
                        {
                            return n;
                        }
                    case "Строка":
                        return v.AsString();
                    case "Булево":
                        return v.AsBoolean();
                    case "Дата":
                        return v.AsDate();
                    case "Неопределено":
                        return null;
                    default:
                        return Новый(v);
                }
            }

            return arg;
        }

        public static IValue Знач(object p)
        {
            if (p is null)
                return null;
            else if (p is int)
                return ValueFactory.Create((int)p);
            else if (p is decimal)
                return ValueFactory.Create((decimal)p);
            else if (p is string)
                return ValueFactory.Create((string)p);
            else if (p is bool)
                return ValueFactory.Create((bool)p);
            else if (p is long)
                return ValueFactory.Create((long)p);
            else if (p is uint)
                return ValueFactory.Create((uint)p);
            else if (p is DateTime)
                return ValueFactory.Create((DateTime)p);
            else if (p is Перем)
                return ((Перем)p)._Value;

            return p as IValue;
        }

        public static Перем Новый(object _val = null)
        {
            IValue val = null;

            if (val is IValue)
            {
                val = (IValue)_val;
            }
            else
            {
                val = Знач(_val);

            }

            if (val == null) return new Перем();

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
                case "Файл":
                    return new Файл(val);
                case "Объект":
                    return new Объект(_val);
                default:
                    return new Перем(val);
            }

        }


        public class Перем : DynamicObject, IComparable<IValue>, IEquatable<IValue>
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
                if (_Value == null) return "";
                return _Value.AsString();
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

            public override int GetHashCode()
            {
                return 0;
            }

            #endregion

            public override bool Equals(object other)
            {
                if (other is null) return (_Value is null);
                var v = Вернуть(_Value);
                if (v is Перем) return (v as Перем)._Value.Equals(other);
                return v.Equals(other);
            }

            public static bool operator ==(Перем lhs, object rhs)
            {
                if (lhs is null || lhs._vartype == "Неопределено") return (bool)(rhs is null);
                return lhs.Equals(rhs);
            }

            public static bool operator !=(Перем lhs, object rhs)
            {
                if (lhs is null) return (bool)!(rhs is null);
                return !lhs.Equals(rhs);
            }


        }

        public class Список : Перем, IEnumerable<КлючИЗначение>
        {
            class t_val: Dictionary<object, object> {}
            t_val _val;

            public Список()
            {
                _val = new t_val();    
            }

            public virtual IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        item.Key, item.Value);
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Свойство(object key, out object value)
            {
                return _val.TryGetValue(key, out value);
            }

            public object Получить(object key)
            {
                object value = null;
                _val.TryGetValue(key, out value);
                return value;
            }

            public void Вставить(object key, object value)
            {
                _val.Add(key, value);
            }

            public void Удалить(object key)
            {
                _val.Remove(key);
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

            // получить свойство
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;
                var v = Variable.Create(null, "");
                if (_val.HasProperty(binder.Name, v))
                {
                    result = Вернуть(v.Value);
                    return true;
                }
                return false;
            }

            // установить свойство
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                _val.Insert(binder.Name, Знач(value));
                return true;
            }

            public int Количество()
            {
                return _val.Count();
            }

            public bool Свойство(string name)
            {
                return _val.HasProperty(name);
            }

            public bool Свойство(string name, ref Перем value)
            {
                var v = Variable.Create(null, "");
                var b = _val.HasProperty(name, v);
                if (b) value = Новый(v.Value);
                return b;
            }

            public bool Свойство(string name, ref object value)
            {
                var v = Variable.Create(null, "");
                var b = _val.HasProperty(name, v);
                if (b) value = Вернуть(v.Value);
                return b;
            }

            public object Получить(string name)
            {
                var v = Variable.Create(null, "");
                if (_val.HasProperty(name, v)) return Новый(v.Value);
                return Неопределено;
            }

            public void Вставить(string name, object val = null)
            {

                _val.Insert(name, Знач(val));
            }

            public void Удалить(string name)
            {

                _val.Remove(name);
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        Вернуть(item.Key), Вернуть(item.Value));
                }

            }

        }


        public static Структура Новый_Структура()
        {
            return new Структура();
        }

        public static Структура Новый_Структура(string strProperties)
        {
            return new Структура(strProperties);
        }

        public static Структура Новый_Структура(string strProperties, params object[] values)
        {
            var arr = new List<IValue>();
            foreach (object p in values)
            {
                arr.Add(Знач(p));
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

            public object Получить(object key)
            {
                try
                {
                    return Вернуть(_val.Retrieve(Знач(key)));
                }
                catch
                {
                    return null;
                }

            }

            public void Вставить(object key, object val = null)
            {
                _val.Insert(Знач(key), Знач(val));
            }

            public void Вставить(string key, string val)
            {
                _val.Insert(Знач(key), Знач(val));
            }
            public void Удалить(string key)
            {
                _val.Delete(Знач(key));
            }

            public void Удалить(object key)
            {
                _val.Delete(Знач(key));
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        Вернуть(item.Key), Вернуть(item.Value));
                }

            }

        }

        public static Соответствие Новый_Соответствие()
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

            public object Получить(int index)
            {
                return Вернуть(_val.Get(index));
            }

            public void Удалить(int index)
            {
                _val.Remove(index);
            }

            public void Вставить(int pos, object val)
            {
                _val.Insert(pos, Знач(val));
            }

            public void Добавить(object val)
            {
                _val.Add(Знач(val));
            }

            public override IEnumerator<КлючИЗначение> GetEnumerator()
            {
                foreach (var item in _val)
                {
                    yield return new КлючИЗначение(
                        null, Вернуть(item));
                }

            }

        }



        public static Массив Новый_Массив()
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

        }

        public static ДвоичныеДанные Новый_ДвоичныеДанные(string arg1)
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

            public long Размер
            {
                get { return _val.Size; }
            }

            public int Получить(int arg)
            {
                return _val.Get(arg);
            }

            public void Установить(int arg1, int arg2)
            {
                _val.Set(arg1, Знач(arg2));
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

            public ulong ПрочитатьЦелое64(int arg1)
            {
                return _val.ReadInt64(arg1);
            }

            public void ЗаписатьЦелое16(int arg1, int arg2)
            {
                _val.WriteInt16(arg1, ValueFactory.Create(arg2));
            }

            public void ЗаписатьЦелое32(int arg1, int arg2)
            {
                _val.WriteInt32(arg1, ValueFactory.Create(arg2));
            }

            public void ЗаписатьЦелое64(int arg1, ulong arg2)
            {
                _val.WriteInt64(arg1, ValueFactory.Create(arg2));
            }

            public void ЗаписатьЦелое16(int arg1, Перем arg2)
            {
                _val.WriteInt16(arg1, arg2._Value);
            }

            public void ЗаписатьЦелое32(int arg1, Перем arg2)
            {
                _val.WriteInt32(arg1, arg2._Value);
            }

            public void ЗаписатьЦелое64(int arg1, Перем arg2)
            {
                _val.WriteInt64(arg1, arg2._Value);
            }

        }

        public static БуферДвоичныхДанных Новый_БуферДвоичныхДанных(int arg1)
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

            public string УдаленныйУзел => _val.RemoteEndPoint;

            public bool Активно
            {
                get { return _val.IsActive; }
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

            public string ПолучитьСтроку(string enc = null)
            {
                return _val.GetString(enc);
            }

            public string ПолучитьЗаголовки()
            {
                return _val.GetHeaders();
            }

            public void Закрыть()
            {
                _val.Close();
            }

        }

        public static TCPСоединение Новый_TCPСоединение(string Хост, int Порт)
        {
            return new TCPСоединение(TCPClient.Constructor(Знач(Хост), Знач(Порт)));
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

        }

        public static TCPСервер Новый_TCPСервер(int Порт)
        {
            return new TCPСервер(TCPServer.ConstructByPort(Знач(Порт)));
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


        public class Файл : Перем
        {
            FileContext _val;

            public FileContext Impl
            {
                get
                {
                    return _val;
                }
            }

            public Файл(IValue val)
            {
                _vartype = "Файл";
                _val = val as FileContext;
                _Value = val;
            }

            public string Имя => _val.Name;
            public string Расширение => _val.Extension;
            public string ИмяБезРасширения => _val.BaseName;
            public bool Существует() => _val.Exist();
            public long Размер() => _val.Size();
            public DateTime ПолучитьВремяИзменения() => _val.GetModificationTime();

        }

        public static Файл Новый_Файл(string filename)
        {
            return new Файл(new FileContext(filename));
        }

        public class Объект : Перем
        {
            object _val;

            public object Impl
            {
                get
                {
                    return _val;
                }
            }

            public Объект(object val)
            {
                _vartype = "Объект";
                _val = val as object;
            }

        }

        public static Объект Новый_Объект(object obj)
        {
            return new Объект(obj);
        }


        public class ТекстовыйДокумент : Перем
        {
            TextDocumentContext _val;

            public TextDocumentContext Impl
            {
                get
                {
                    return _val;
                }
            }

            public ТекстовыйДокумент(IValue val)
            {
                _vartype = "ТекстовыйДокумент";
                _val = val as TextDocumentContext;
                _Value = val;
            }

            public void Записать(string path, IValue encoding = null, string lineSeparator = null) => _val.Write(path, encoding, lineSeparator);

        }

        public static ТекстовыйДокумент Новый_ТекстовыйДокумент()
        {
            return new ТекстовыйДокумент(new TextDocumentContext());
        }


        public enum ХешФункция
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512,
            CRC32
        }

        static HashAlgorithm convfunc(ХешФункция func)
        {
            var algName = func.ToString();
            if (algName == "CRC32")
                return new ScriptEngine.HostedScript.Library.Hash.Crc32();

            var ret = HashAlgorithm.Create(algName);
            if (ret == null)
                throw RuntimeException.InvalidArgumentType();
            return ret;
        }

        public class ХешированиеДанных : Перем
        {
            HashImpl _val;
            HashFunctionEnum _hfunc;

            public HashImpl Impl
            {
                get
                {
                    return _val;
                }
            }

            public ХешированиеДанных(IValue val)
            {
                _vartype = "ТекстовыйДокумент";
                _val = val as HashImpl;
                _Value = val;
            }

            public void Добавить(object toAdd, uint count = 0) => _val.Append(Знач(toAdd),count);
            public ДвоичныеДанные ХешСумма => new ДвоичныеДанные(_val.Hash);

        }

        public static ХешированиеДанных Новый_ХешированиеДанных(ХешФункция provider, IValue enumValue = null)
        {
            return new ХешированиеДанных(new HashImpl(convfunc(provider), enumValue));
        }


        public enum ПозицияВПотоке
        {
            Начало,
            Конец,
            Текущая
        }

        public class ФайловыйПоток : Перем
        {
            FileStreamContext _val;

            public FileStreamContext Impl
            {
                get
                {
                    return _val;
                }
            }

            public ФайловыйПоток(IValue val)
            {
                _vartype = "ФайловыйПоток";
                _val = val as FileStreamContext;
                _Value = val;
            }

            StreamPositionEnum convpos(ПозицияВПотоке initpos)
            {
                switch (initpos)
                {
                    case ПозицияВПотоке.Начало: return StreamPositionEnum.Begin;
                    case ПозицияВПотоке.Конец: return StreamPositionEnum.End;
                }
                return StreamPositionEnum.Current;
            }

            public long Перейти(long offset, ПозицияВПотоке initialPosition = ПозицияВПотоке.Начало)
            {
                return _val.Seek((int)offset, convpos(initialPosition));
            }

            public long Прочитать(БуферДвоичныхДанных buffer, int positionInBuffer, int number)
            {
                return _val.Read(buffer.Impl, positionInBuffer, number);
            }

            public void Записать(БуферДвоичныхДанных buffer, int positionInBuffer, int number)
            {
                _val.Write(buffer.Impl, positionInBuffer, number);
            }

            public void КопироватьВ(ФайловыйПоток targetStream, int bufferSize = 0)
            {
                _val.CopyTo(targetStream._Value, bufferSize);
            }


            public void СброситьБуферы() => _val.Flush();
            public void Закрыть() => _val.Close();
            public long Размер() => _val.Size();
            public long ТекущаяПозиция() => _val.CurrentPosition();

            public bool ДоступнаЗапись => _val.CanWrite;
            public bool ДоступноЧтение => _val.CanRead;

        }


        public static class ФайловыеПотоки {
        
            public static ФайловыйПоток ОткрытьДляЗаписи(string fileName)
            {
                if (_filestrm == null)
                {
                    _filestrm = new FileStreamsManager();
                }
                return new ФайловыйПоток(_filestrm.OpenForWrite(fileName));
            }

            public static ФайловыйПоток ОткрытьДляЧтения(string fileName)
            {
                if (_filestrm == null)
                {
                    _filestrm = new FileStreamsManager();
                }
                return new ФайловыйПоток(_filestrm.OpenForRead(fileName));
            }
        
        }


        public static СистемнаяИнформация Новый_СистемнаяИнформация()
        {
            return new СистемнаяИнформация();
        }

        public string ОписаниеОшибки(Exception e) { return "Ошибка!\n" + e.Message + "\n" + e.StackTrace; }

        public string ВызватьИсключение(string exept) { return exept; }


        public string ТекущийКаталог()
        {
            return _fileop.CurrentDirectory();
        }

        public string ОбъединитьПути(string path1, string path2, string path3 = null, string path4 = null)
        {
            return _fileop.CombinePath(path1, path2, path3, path4);
        }

        public void СоздатьКаталог(string path)
        {
            _fileop.CreateDirectory(path);
        }

        public Массив НайтиФайлы(string dir, string mask = null, bool recursive = false)
        {
            return new Массив(_fileop.FindFiles(dir, mask, recursive));
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

        public static string Строка(object arg)
        {
            if (arg == null) return "";
            return Знач(arg).AsString();
        }

        public static object Число(object arg)
        {
            decimal n = Знач(arg).AsNumber();
            try
            {
                return Decimal.ToInt32(n);
            }
            catch //(OverflowException e)
            {
                return n;
            }
        }

        public static bool Булево(object arg)
        {
            return Знач(arg).AsBoolean();
        }

        public static string ТипЗнч(object p)
        {
            if (p is int)
                return "Число";
            else if (p is decimal)
                return "Число";
            else if (p is string)
                return "Строка";
            else if (p is bool)
                return "Булево";
            else if (p is Перем)
                return ((Перем)p)._vartype;

            return "Неопределено";
        }

        public static string Тип(string arg)
        {
            return arg;
            //return new TypeTypeValue(arg);
        }

        public static DateTime ТекущаяДата()
        {
            return DateTime.Now;
        }

        public static string Лев(string str, int len)
        {
            if (len > str.Length)
                len = str.Length;
            else if (len < 0)
            {
                return "";
            }

            return str.Substring(0, len);
        }

        public static string Прав(string str, int len)
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

        public static string Сред(string str, int start)
        {
            return Сред(str, start, str.Length - start + 1);
        }

        public static string Сред(string str, int start, int len)
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

        public static int Найти(string haystack, string needle)
        {
            return haystack.IndexOf(needle, StringComparison.Ordinal) + 1;
        }

        public static string СтрЗаменить(string sourceString, string searchVal, string newVal)
        {
            return sourceString.Replace(searchVal, newVal);
        }

        public static int СтрДлина(string str)
        {
            return str.Length;
        }

        public static string СокрЛП(string str)
        {
            return str.Trim();
        }

        public static string СокрЛП(object str)
        {
            return Знач(str).AsString().Trim();
        }



        public static void Приостановить(int delay)
        {
            System.Threading.Thread.Sleep(delay);
        }

        public static decimal ТекущаяУниверсальнаяДатаВМиллисекундах()
        {
            return (decimal)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static decimal Цел(decimal val)
        {
            return Math.Truncate(val);
        }

        public ДвоичныеДанные СоединитьДвоичныеДанные(Массив arg)
        {
            return new ДвоичныеДанные(_glbin.ConcatenateBinaryData(arg.Impl));
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(string arg, string enc)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromString(arg, Знач(enc)));
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(object arg, string enc)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromString((string)Вернуть(arg), Знач(enc)));
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
        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзBase64Строки(string arg)
        {
            return new ДвоичныеДанные(_glbin.GetBinaryDataFromBase64String(arg));
        }

        public string ПолучитьBase64СтрокуИзДвоичныхДанных(ДвоичныеДанные data)
        {
            return _glbin.GetBase64StringFromBinaryData(data.Impl);
        }

        public string РаскодироватьСтроку(string encodedString, EnumerationValue codeType, IValue encoding = null)
        {
            return _miscf.DecodeString(encodedString, codeType as SelfAwareEnumValue<StringEncodingMethodEnum>, encoding);
        }


        public onesharp ()
        {
            _glbin = new GlobalBinaryData();
            _fileop = new FileOperations();
            _strop = new StringOperations();
            _miscf = new MiscGlobalFunctions();

            Символы = new symbols();
            СпособКодированияСтроки  = new urlenc();
        }
    }
}

