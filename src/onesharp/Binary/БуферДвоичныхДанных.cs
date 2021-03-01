﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace onesharp.Binary
{
    /// <summary>
    /// 
    /// Коллекция байтов фиксированного размера с возможностью произвольного доступа и изменения по месту.
    /// Размер буфера формально не ограничен, но поскольку все данные буфера полностью находятся в оперативной памяти, при попытке создать буфер слишком большого размера доступной памяти может оказаться недостаточно, в результате чего будет вызвано исключение. Поэтому при работе с буферами двоичных данных необходимо соотносить их размер с доступным объемом оперативной памяти.
    /// При создании буфера можно указать порядок байтов, который будет использован для операций с целыми числами. При этом если буфер не создан явно, а получен с помощью вызова метода другого объекта, то порядок байтов в полученном буфере будет унаследован от порядка байтов, заданного для того объекта, метод которого вызывается.
    /// Например, если буфер получен с помощью вызова метода ПрочитатьВБуферДвоичныхДанных, то порядок байтов в полученном буфере будет равен значению свойства ПорядокБайтов.
    /// Возможен также более сложный случай наследования порядка байтов. Если буфер получен с помощью вызова метода ПолучитьБуферДвоичныхДанных, то порядок байтов у полученного буфера будет выбираться из объекта ЧтениеДанных, из которого был получен объект РезультатЧтенияДанных. 
    /// Порядок байтов, заданный для объекта ЧтениеДанных, будет использован во всех объектах, полученных на его основании.
    /// </summary>
    public class БуферДвоичныхДанных
    {
        private bool _readOnly;
        private readonly byte[] _buffer;

        public БуферДвоичныхДанных(byte[] buffer, ByteOrderEnum byteOrder = ByteOrderEnum.LittleEndian)
        {
            _buffer = buffer;
            ByteOrder = byteOrder;
        }
    
        // для операций с содержимым буфера внутри 1Script
        //
        public byte[] Bytes
        {
            get { return _buffer; }
        }

        /// <param name="size">
        /// Размер буфера в байтах. </param>
        /// <param name="byteOrder">
        /// Порядок байтов.
        /// Значение по умолчанию: LittleEndian. </param>
        ///
        public static БуферДвоичныхДанных Новый(int size, object byteOrder = null)
        {
            var orderValue = byteOrder == null ? ByteOrderEnum.LittleEndian : (ByteOrderEnum)byteOrder;

            return new БуферДвоичныхДанных(
                new byte[size],
                orderValue);
        }

        public int GetIndexedValue(int index)
        {
            return Получить(index);
        }

        public void SetIndexedValue(int index, byte val)
        {
            ThrowIfReadonly();

            int value = val;
            if (value < byte.MinValue || value > byte.MaxValue)
                throw RuntimeException.InvalidArgumentValue();

            var idx = index;
            _buffer[idx] = (byte) value;
        }

        /// <summary>
        /// 
        /// Текущий порядок байтов. Влияет на операции чтения и записи целых чисел в буфер.
        /// </summary>
        /// <value>ПорядокБайтов (ByteOrder)</value>
        //[ContextProperty("ПорядокБайтов", "ByteOrder")]
        public ByteOrderEnum ByteOrder { get; set; }

        /// <summary>
        /// 
        /// Размер буфера в байтах.
        /// </summary>
        /// <value>Число (Number)</value>
        //[ContextProperty("Размер", "Size")]
        public long Размер => _buffer.LongLength;

        /// <summary>
        /// 
        /// Значение Истина указывает, что данный буфер предназначен только для чтения.
        /// </summary>
        /// <value>Булево (Boolean)</value>
        //[ContextProperty("ТолькоЧтение", "ReadOnly")]
        public bool ТолькоЧтение
        {
            get { return _readOnly; }

        }

        /// <summary>
        /// 
        /// Заменить значения, начиная с заданной позиции, значениями из заданного буфера.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, начиная с которой требуется записать содержимое буфера. </param>
        /// <param name="bytes">
        /// Байты, которыми нужно заполнить часть буфера. </param>
        /// <param name="number">
        /// Количество байт, которые требуется заменить. </param>
        ///
        //[ContextMethod("Записать", "Write")]
        public void Записать(int position, БуферДвоичныхДанных bytes, int number = 0)
        {
            ThrowIfReadonly();

            if (number == 0)
                Array.Copy(bytes._buffer, 0, _buffer, position, bytes._buffer.Length);
            else
                Array.Copy(bytes._buffer, 0, _buffer, position, number);
        }

        private byte[] GetBytes<T>(T value, Converter<T, byte[]> leConverter, Converter<T, byte[]> beConverter, object byteOrder = null)
        {
            ByteOrderEnum workByteOrder;
            if (byteOrder == null)
                workByteOrder = ByteOrder;
            else
            {
                var enumVal = byteOrder;
                if (enumVal == null)
                    throw RuntimeException.InvalidArgumentType(nameof(byteOrder));

                try
                {
                    workByteOrder = (ByteOrderEnum)enumVal;
                }
                catch (InvalidCastException)
                {
                    throw RuntimeException.InvalidArgumentType(nameof(byteOrder));
                }
            }

            var converter = workByteOrder == ByteOrderEnum.BigEndian ? beConverter : leConverter;
            return converter(value);
        }

        private void CopyBytes(int position, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                _buffer[position + i] = bytes[i];
            }
        }

        private static ulong AsUnsignedLong( object value, ulong maxValue=ulong.MaxValue )
        {
            if (!(value is ulong))
                throw RuntimeException.InvalidArgumentType(2,nameof(value));

            var number = (ulong)value;
            if ( number < 0 || number > maxValue || number != (ulong)number )
                throw RuntimeException.InvalidArgumentValue(number);

            return (ulong)number;
        }


        /// <summary>
        /// 
        /// Записать целое 16-битное положительное число в заданную позицию.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется записать число. </param>
        /// <param name="value">
        /// Число, которое требуется записать.
        /// Если значение не помещается в 16 бит, будет вызвано исключение. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, который будет использован для кодировки числа при записи в буфер. Если не установлен, то будет использован порядок байтов, заданный для текущего экземпляра БуферДвоичныхДанных.
        /// Значение по умолчанию: Неопределено. </param>
        ///
        ///
        public void ЗаписатьЦелое16(int position, int value, object byteOrder = null)
        {
            ThrowIfReadonly();

            var number = AsUnsignedLong(value,ushort.MaxValue);
            var bytes = GetBytes( (ushort)number, BitConversionFacility.LittleEndian.GetBytes, BitConversionFacility.BigEndian.GetBytes, byteOrder);
            CopyBytes(position, bytes);
        }

        /// <summary>
        /// 
        /// Записать целое 32-битное положительное число в заданную позицию.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется записать число. </param>
        /// <param name="value">
        /// Число, которое требуется записать.
        /// Если значение не помещается в 32 бита, будет вызван исключение. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, который будет использован для кодировки числа при записи в буфер. Если не установлен, то будет использован порядок байтов, заданный для текущего экземпляра БуферДвоичныхДанных.
        /// Значение по умолчанию: Неопределено. </param>
        public void ЗаписатьЦелое32(int position, int value, object byteOrder = null)
        {
            ThrowIfReadonly();

            var number = AsUnsignedLong(value, uint.MaxValue);
            var bytes = GetBytes((uint)number, BitConversionFacility.LittleEndian.GetBytes, BitConversionFacility.BigEndian.GetBytes, byteOrder);
            CopyBytes(position, bytes);
        }


        /// <summary>
        /// 
        /// Записать целое 64-битное положительное число в заданную позицию.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется записать число. </param>
        /// <param name="value">
        /// Число, которое требуется записать.
        /// Если значение не помещается в 64 бита, будет вызвано исключение. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, который будет использован для кодировки числа при записи в буфер. Если не установлен, то используется порядок байтов, заданный для текущего экземпляра БуферДвоичныхДанных.
        /// Значение по умолчанию: Неопределено. </param>

        ///

        ///
        public void ЗаписатьЦелое64(int position, ulong value, object byteOrder = null)
        {
            ThrowIfReadonly();

            var number = AsUnsignedLong(value);
            var bytes = GetBytes( number, BitConversionFacility.LittleEndian.GetBytes, BitConversionFacility.BigEndian.GetBytes, byteOrder);
            CopyBytes(position, bytes);
        }
        
        private void WriteBitwiseOp(int position, БуферДвоичныхДанных buffer, int number, Func<byte, byte, byte> op)
        {
            if(position < 0)
                throw new IndexOutOfRangeException("Значение индекса выходит за границы диапазона");

            try
            {
                var bytesToCopy = (number == 0 ? buffer._buffer.Length : number);
                for (int i = 0; i < bytesToCopy; i++)
                    _buffer[i + position] = op(_buffer[i + position], buffer._buffer[i]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Переполнение при работе с буфером");
            }
        }
        
        /// <summary>
        /// 
        /// Объединить заданное количество байтов, начиная с указанной позиции с байтами из заданного буфера
        /// с использованием побитового И.
        /// Если количество байтов не указано, то объединяются все байты до конца буфера.
        /// </summary>
        ///
        /// <param name="position">
        /// Начальная позиция в буфере. </param>
        /// <param name="bytes">
        /// Буфер, с которым выполняется объединение. </param>
        /// <param name="number">
        /// Количество байт, которые требуется объединить. </param>
        ///
        public void ЗаписатьПобитовоеИ(int position, БуферДвоичныхДанных bytes, int number = 0)
        {
            ThrowIfReadonly();
            WriteBitwiseOp(position, bytes, number, ((i, j) => (byte)(i & j)));
        }
        
        /// <summary>
        /// 
        /// Объединить заданное количество байтов, начиная с указанной позиции с байтами из заданного буфера
        /// с использованием побитового И НЕ.
        /// Если количество байтов не указано, то объединяются все байты до конца буфера.
        /// </summary>
        ///
        /// <param name="position">
        /// Начальная позиция в буфере. </param>
        /// <param name="bytes">
        /// Буфер, с которым выполняется объединение. </param>
        /// <param name="number">
        /// Количество байт, которые требуется объединить. </param>
        ///
        public void ЗаписатьПобитовоеИНе(int position, БуферДвоичныхДанных bytes, int number = 0)
        {
            ThrowIfReadonly();
            WriteBitwiseOp(position, bytes, number, ((i, j) => (byte)(i & ~j)));
        }
        
        /// <summary>
        /// 
        /// Объединить заданное количество байтов, начиная с указанной позиции с байтами из заданного буфера
        /// с использованием побитового ИЛИ.
        /// Если количество байтов не указано, то объединяются все байты до конца буфера.
        /// </summary>
        ///
        /// <param name="position">
        /// Начальная позиция в буфере. </param>
        /// <param name="bytes">
        /// Буфер, с которым выполняется объединение. </param>
        /// <param name="number">
        /// Количество байт, которые требуется объединить. </param>
        ///
        public void ЗаписатьПобитовоеИли(int position, БуферДвоичныхДанных bytes, int number = 0)
        {
            ThrowIfReadonly();
            WriteBitwiseOp(position, bytes, number, ((i, j) => (byte)(i | j)));
        }
        
        /// <summary>
        /// 
        /// Объединить заданное количество байтов, начиная с указанной позиции с байтами из заданного буфера
        /// с использованием побитового ИСКЛЮЧИТЕЛЬНОГО ИЛИ (XOR).
        /// Если количество байтов не указано, то объединяются все байты до конца буфера.
        /// </summary>
        ///
        /// <param name="position">
        /// Начальная позиция в буфере. </param>
        /// <param name="bytes">
        /// Буфер, с которым выполняется объединение. </param>
        /// <param name="number">
        /// Количество байт, которые требуется объединить. </param>
        ///
        public void ЗаписатьПобитовоеИсключительноеИли(int position, БуферДвоичныхДанных bytes, int number = 0)
        {
            ThrowIfReadonly();
            WriteBitwiseOp(position, bytes, number, ((i, j) => (byte)(i ^ j)));
        }

        /// <summary>
        /// 
        /// Создает новый буфер, содержащий элементы текущего буфера в противоположном порядке.
        /// </summary>
        ///
        ///
        /// <returns name="BinaryDataBuffer"/>
        ///
        public БуферДвоичныхДанных Перевернуть()
        {
            var bytes = _buffer.Reverse().ToArray();
            return new БуферДвоичныхДанных(bytes, ByteOrder);
        }


        /// <summary>
        /// 
        /// Получает значение элемента на указанной позиции.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция элемента в буфере. Нумерация начинается с 0. </param>

        ///
        /// <returns name="Number">
        /// Числовым типом может быть представлено любое десятичное число. Над данными числового типа определены основные арифметические операции: сложение, вычитание, умножение и деление. Максимально допустимая разрядность числа 38 знаков.</returns>

        ///
        public int Получить(int position)
        {
            return _buffer[position];
        }


        /// <summary>
        /// Создает новый буфер, использующий заданное количество байтов из исходного буфера, начиная с заданной позиции (нумерация с 0). Если количество не задано, то новый буфер является представлением элементов текущего буфера, начиная с заданного индекса и до конца.
        /// 
        /// НЕ РЕАЛИЗОВАН
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, начиная с которой будет создан новый буфер. </param>
        /// <param name="number">
        /// Количество байтов, которые требуется отобразить в срезе. Если на задано, то отображаются все байты от начала среза до конца исходного буфера.
        /// Значение по умолчанию: Неопределено. </param>
        ///
        /// <returns name="BinaryDataBuffer">
        /// </returns>

        ///
        public БуферДвоичныхДанных ПолучитьСрез(int position, object number = null)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// Выполняет чтение байтов из буфера и помещает их в новый буфер.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, начиная с которой требуется прочитать байты. </param>
        /// <param name="number">
        /// Количество байтов, которое требуется прочитать. </param>
        ///
        /// <returns name="BinaryDataBuffer"/>
        ///
        public БуферДвоичныхДанных Прочитать(int position, int number)
        {
            var data = new byte[number];
            Array.Copy(_buffer, position, data, 0, number);
            return new БуферДвоичныхДанных(data, ByteOrder);
        }
    
        private T FromBytes<T>(int position, Func<byte[], int, T> leConverter, Func<byte[], int, T> beConverter, object byteOrder = null)
        {
            ByteOrderEnum workByteOrder;
            if (byteOrder == null)
                workByteOrder = ByteOrder;
            else
            {
                var enumVal = (ByteOrderEnum)byteOrder;
                if (enumVal == null)
                    throw RuntimeException.InvalidArgumentType(nameof(byteOrder));

                try
                {
                    workByteOrder = (ByteOrderEnum)enumVal;
                }
                catch (InvalidCastException)
                {
                    throw RuntimeException.InvalidArgumentType(nameof(byteOrder));
                }
            }

            var converter = workByteOrder == ByteOrderEnum.BigEndian ? beConverter : leConverter;
            return converter(_buffer, position);
        }

        /// <summary>
        /// 
        /// Выполняет чтение целого 16-битного положительного числа на заданной позиции.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется прочитать число. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, используемый при чтении числа.
        /// Если не задан, используется порядок, определенный для текущего экземпляра ЧтениеДанных.
        /// Значение по умолчанию: Неопределено. </param>
        ///
        /// <returns name="Number"/>
        ///
        public int ПрочитатьЦелое16(int position, object byteOrder = null)
        {
            return FromBytes(position, BitConversionFacility.LittleEndian.ToInt16, BitConversionFacility.BigEndian.ToInt16, byteOrder);
        }


        /// <summary>
        /// 
        /// Прочитать целое 32-битное положительное число на заданной позиции.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется прочитать число. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, используемый при чтении числа.
        /// Если не задан, используется порядок, определенный для текущего экземпляра ЧтениеДанных.
        /// Значение по умолчанию: Неопределено. </param>
        ///
        /// <returns name="Number">
        /// Числовым типом может быть представлено любое десятичное число. Над данными числового типа определены основные арифметические операции: сложение, вычитание, умножение и деление. Максимально допустимая разрядность числа 38 знаков.</returns>

        ///
        public int ПрочитатьЦелое32(int position, object byteOrder = null)
        {
            return (int)FromBytes(position, BitConversionFacility.LittleEndian.ToUInt32, BitConversionFacility.BigEndian.ToUInt32, byteOrder);
        }


        /// <summary>
        /// 
        /// Выполняет чтение целого 64-битного положительного числа на заданной позиции.
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которой требуется прочитать число. </param>
        /// <param name="byteOrder">
        /// Порядок байтов, используемый при чтении числа.
        /// Если не задан, используется порядок, определенный для текущего экземпляра ЧтениеДанных.
        /// Значение по умолчанию: Неопределено. </param>
        ///
        /// <returns name="Number">
        /// Числовым типом может быть представлено любое десятичное число. Над данными числового типа определены основные арифметические операции: сложение, вычитание, умножение и деление. Максимально допустимая разрядность числа 38 знаков.</returns>

        ///
        public ulong ПрочитатьЦелое64(int position, object byteOrder = null)
        {
            return FromBytes(position, BitConversionFacility.LittleEndian.ToUInt64, BitConversionFacility.BigEndian.ToUInt64, byteOrder);
        }


        /// <summary>
        /// Разделить буфер на части по заданному разделителю.
        /// 
        /// НЕ РЕАЛИЗОВАН
        /// </summary>
        ///
        /// <remarks>
        /// 
        /// По двоичному буферу
        /// </remarks>
        ///
        /// <param name="separator">
        /// Разделитель. </param>
        ///
        /// <returns name="Array"/>
        ///
        public object Разделить(object separator)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// Создает копию массива.
        /// </summary>
        ///
        ///
        /// <returns name="BinaryDataBuffer"/>
        ///
        public БуферДвоичныхДанных Скопировать()
        {
            byte[] copy = new byte[_buffer.Length];
            Array.Copy(_buffer, copy, _buffer.Length);
            return new БуферДвоичныхДанных(copy, ByteOrder);
        }


        /// <summary>
        /// 
        /// Создает новый буфер, содержащий элементы текущего буфера и, за ними, элементы заданного буфера.
        /// </summary>
        ///
        /// <param name="buffer">
        /// Буфер, который будет соединен с исходным. </param>
        ///
        /// <returns name="BinaryDataBuffer"/>
        ///
        public БуферДвоичныхДанных Соединить(БуферДвоичныхДанных buffer)
        {
            var source = buffer._buffer;
            var totalLength = _buffer.Length + source.Length;
            var joinedArray = new byte[totalLength];
            Array.Copy(_buffer, joinedArray, _buffer.Length);
            Array.Copy(source, 0, joinedArray, _buffer.Length, source.Length);

            return new БуферДвоичныхДанных(joinedArray, ByteOrder);
        }


        /// <summary>
        /// 
        /// Устанавливает значение элемента на заданной позиции (нумерация начинается с 0).
        /// </summary>
        ///
        /// <param name="position">
        /// Позиция, на которую требуется поместить новое значение. </param>
        /// <param name="value">
        /// Значение, которое требуется установить в заданную позицию буфера.
        /// Если значение больше 255 или меньше 0, будет выдана ошибка о неверном значении параметра. </param>
        public void Установить(int position, byte value)
        {
            ThrowIfReadonly();

            _buffer[position] = (byte)AsUnsignedLong(value, byte.MaxValue);
        }


        /// <summary>
        /// 
        /// Переводит текущий буфер в режим "только для чтения". 
        /// Попытка изменить состояние буфера приведет к вызову исключения.
        /// </summary>
        ///

        ///

        ///
        public void УстановитьТолькоЧтение()
        {
            _readOnly = true;
        }

        public int Count()
        {
            return _buffer.Length;
        }

        private IEnumerator<byte> GetEnumerator()
        {
            for (long i = 0; i < _buffer.LongLength; i++)
            {
                yield return _buffer[i];
            }
        }

        public void ThrowIfReadonly()
        {
            if (_readOnly)
                throw new RuntimeException("Буфер находится в режиме \"Только чтение\"");
        }
    }
}
