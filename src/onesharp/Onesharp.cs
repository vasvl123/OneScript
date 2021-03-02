using System;
using System.Dynamic;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

using onesharp.Binary;

namespace onesharp
{

    public class DynObj : DynamicObject
    {
        public DynObj() {}

        public Структура _val;

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            if (_val.Свойство(binder.Name, out result))
            {
                return true;
            }
            return false;
        }

        // установить свойство
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _val.Вставить(binder.Name, value);
            return true;
        }

    }


    public class Onesharp
    {

        public Onesharp(string _ИмяМодуля)
        {
            МоментЗапуска = ТекущаяУниверсальнаяДатаВМиллисекундах();
            ИмяМодуля = _ИмяМодуля;
        }

        public static object Неопределено = null;
        public static bool Истина = true;
        public static bool Ложь = false;

        public decimal МоментЗапуска;
        public string[] АргументыКоманднойСтроки;
        public string ИмяМодуля;

        public decimal ПолучитьИД()
        {
            МоментЗапуска -= 1;
            return Цел(ТекущаяУниверсальнаяДатаВМиллисекундах() - МоментЗапуска);
        }

        public static void ВызватьИсключение(string msg)
        {
            throw new SystemException(msg);
        }

        public static void Приостановить(int delay)
        {
            System.Threading.Thread.Sleep(delay);
        }

        public static decimal Цел(decimal val)
        {
            return Math.Truncate(val);
        }

        public static decimal ТекущаяУниверсальнаяДатаВМиллисекундах()
        {
            return (decimal)DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
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

        public static string ВРег(string arg)
        {
            return arg.ToUpper();
        }

        public static string НРег(string arg)
        {
            return arg.ToLower();
        }


        public static int КодСимвола(string strChar, int position = 0)
        {
            int result;
            if (strChar.Length == 0)
                result = 0;
            else if (position >= 0 && position < strChar.Length)
                result = (int)strChar[position];
            else
                throw RuntimeException.InvalidArgumentValue();

            return result;
        }

        public static string Символ(int code)
        {
            return new string(new char[1] { (char)code });
        }

        public static int Найти(string haystack, string needle)
        {
            return haystack.IndexOf(needle, StringComparison.Ordinal) + 1;
        }

        public static string СокрЛП(string str)
        {
            return str.Trim();
        }

        public static class Символы
        {

            public static string ПС
            {
                get
                {
                    return "\n";
                }
            }

            public static string ВК
            {
                get
                {
                    return "\r";
                }
            }

            public static string ВТаб
            {
                get
                {
                    return "\v";
                }
            }

            public static string Таб
            {
                get
                {
                    return "\t";
                }
            }

            public static string ПФ
            {
                get
                {
                    return "\f";
                }
            }

            public static string НПП
            {
                get
                {
                    return "\u00A0";
                }
            }

        }

        public void Сообщить(string message, СтатусСообщения status = СтатусСообщения.Обычное)
        {
            ConsoleHostImpl.Echo(message, status);
        }

        public void Сообщить(object message, СтатусСообщения status = СтатусСообщения.Обычное)
        {
            ConsoleHostImpl.Echo(message.ToString(), status);
        }

        public static object Parse(object obj, Type type)
        {

            string presentation = obj.ToString();
            object result;
            if (type == typeof(bool))
            {
                if (String.Compare(presentation, "истина", StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(presentation, "true", StringComparison.OrdinalIgnoreCase) == 0
                    || String.Compare(presentation, "да", StringComparison.OrdinalIgnoreCase) == 0)
                    result = true;
                else if (String.Compare(presentation, "ложь", StringComparison.OrdinalIgnoreCase) == 0
                         || String.Compare(presentation, "false", StringComparison.OrdinalIgnoreCase) == 0
                         || String.Compare(presentation, "нет", StringComparison.OrdinalIgnoreCase) == 0)
                    result = false;
                else
                    throw RuntimeException.ConvertToBooleanException();
            }
            else if (type == typeof(DateTime))
            {
                string format;
                if (presentation.Length == 14)
                    format = "yyyyMMddHHmmss";
                else if (presentation.Length == 8)
                    format = "yyyyMMdd";
                else if (presentation.Length == 12)
                    format = "yyyyMMddHHmm";
                else
                    throw RuntimeException.ConvertToDateException();

                if (presentation == "00000000"
                 || presentation == "000000000000"
                 || presentation == "00000000000000")
                {
                    result = null;
                }
                else
                    try
                    {
                        result = DateTime.ParseExact(presentation, format, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        throw RuntimeException.ConvertToDateException();
                    }
            }
            else if (type == typeof(decimal))
            {
                var numInfo = NumberFormatInfo.InvariantInfo;
                var numStyle = NumberStyles.AllowDecimalPoint
                            | NumberStyles.AllowLeadingSign
                            | NumberStyles.AllowLeadingWhite
                            | NumberStyles.AllowTrailingWhite;

                try
                {
                    result = Decimal.Parse(presentation, numStyle, numInfo);
                }
                catch (FormatException)
                {
                    throw RuntimeException.ConvertToNumberException();
                }
            }
            else if (type == typeof(string))
            {
                result = presentation;
            }
            else throw new NotSupportedException("constant type is not supported");

            return result;
        }


        public static object Число(object arg)
        {
            var n = (decimal)Parse(arg, typeof(decimal));
            if (Math.Truncate(n) == n)
            {
                try
                {
                    return Decimal.ToInt32(n);
                }
                catch { }
            }
            return n;
        }

        public static string Строка(object arg)
        {
            return (string)Parse(arg, typeof(string));
        }

        public string ОписаниеОшибки(Exception e) { return ИмяМодуля + " ошибка!\n" + e.Message + "\n" + e.StackTrace; }


        public string ТекущийКаталог()
        {
            return ФайловыеОперации.ТекущийКаталог();
        }

        public string ОбъединитьПути(string path1, string path2, string path3 = null, string path4 = null)
        {
            return ФайловыеОперации.ОбъединитьПути(path1, path2, path3, path4);
        }

        public void СоздатьКаталог(string path)
        {
            ФайловыеОперации.СоздатьКаталог(path);
        }

        public Массив НайтиФайлы(string dir, string mask = null, bool recursive = false)
        {
            return ФайловыеОперации.НайтиФайлы(dir, mask, recursive);
        }

        public ДвоичныеДанные СоединитьДвоичныеДанные(Массив arg)
        {
            return GlobalBinaryData.СоединитьДвоичныеДанные(arg);
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(string arg, string enc)
        {
            return GlobalBinaryData.ПолучитьДвоичныеДанныеИзСтроки(arg, enc);
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзСтроки(string arg)
        {
            return GlobalBinaryData.ПолучитьДвоичныеДанныеИзСтроки(arg);
        }

        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(БуферДвоичныхДанных arg)
        {
            return GlobalBinaryData.ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных(arg);
        }

        public БуферДвоичныхДанных ПолучитьБуферДвоичныхДанныхИзДвоичныхДанных(ДвоичныеДанные arg)
        {
            return GlobalBinaryData.ПолучитьБуферДвоичныхДанныхИзДвоичныхДанных(arg);
        }

        public string ПолучитьСтрокуИзДвоичныхДанных(ДвоичныеДанные arg)
        {
            return GlobalBinaryData.ПолучитьСтрокуИзДвоичныхДанных(arg);
        }
        public ДвоичныеДанные ПолучитьДвоичныеДанныеИзBase64Строки(string arg)
        {
            return GlobalBinaryData.ПолучитьДвоичныеДанныеИзBase64Строки(arg);
        }

        public string ПолучитьBase64СтрокуИзДвоичныхДанных(ДвоичныеДанные data)
        {
            return GlobalBinaryData.ПолучитьBase64СтрокуИзДвоичныхДанных(data);
        }

        public string РаскодироватьСтроку(string encodedString, СпособКодированияСтроки codeType, string encoding = null)
        {
            return ПрочиеФункции.РаскодироватьСтроку(encodedString, codeType, encoding);
        }

        public string КодироватьСтроку(string sourceString, СпособКодированияСтроки codeType, string encoding = null)
        {
            return ПрочиеФункции.КодироватьСтроку(sourceString, codeType, encoding);
        }

        public void ОсвободитьОбъект(object obj)
        {
            var disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }


    }
}
