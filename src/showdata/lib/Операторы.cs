﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace showdata.lib
{
    class Операторы
    {

        // MIT License
        // Copyright (c) 2020 vasvl123
        // https://github.com/vasvl123/useyourmind
        ///
        // Включает программный код https://github.com/tsukanov-as/kojura


        object Оператор_ЕстьЗначение(Данные, Аргумент)
        {
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент, Неопределено, Ложь);
            return (!("" + Значение == ""));
        } // ЕстьЗначение()

        object Оператор_Пустой(Данные, Аргумент)
        {
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Аргумент = Данные.Интерпретировать(Аргумент, Неопределено, Ложь);
            return (Аргумент == Данные.Пустой);
        } // Пустой()

        object Оператор_Сумма(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            while (Аргумент != Неопределено)
            {
                Значение = Значение + Данные.Интерпретировать(Аргумент);
                Аргумент = Аргумент.Соседний;
            }
            return Значение;
        } // Сумма()

        object Оператор_Разность(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                return -(Значение);
            }
            while (Аргумент != Неопределено)
            {
                Значение = Значение - Данные.Интерпретировать(Аргумент);
                Аргумент = Аргумент.Соседний;
            }
            return Значение;
        } // Разность()

        object Оператор_Произведение(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            while (Аргумент != Неопределено)
            {
                Значение = Значение * Данные.Интерпретировать(Аргумент);
                Аргумент = Аргумент.Соседний;
            }
            return Значение;
        } // Произведение()

        object Оператор_Частное(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            while (Аргумент != Неопределено)
            {
                Значение = Значение / Данные.Интерпретировать(Аргумент);
                Аргумент = Данные.Соседний(Аргумент);
            }
            return Значение;
        } // Частное()

        object Оператор_Остаток(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            while (Аргумент != Неопределено)
            {
                Значение = Значение % Данные.Интерпретировать(Аргумент);
                Аргумент = Аргумент.Соседний;
            }
            return Значение;
        } // Остаток()

        object Оператор_Если(Данные, Узел)
        {
            Перем СписокЕсли, СписокТогда, СписокИначе;
            СписокЕсли = Узел;
            СписокТогда = СписокЕсли.Соседний;
            СписокИначе = СписокТогда.Соседний;
            зЕсли = Данные.Интерпретировать(СписокЕсли);
            if (ТипЗнч(зЕсли) == Тип("Строка"))
            {
                зЕсли = (зЕсли == "Истина" || зЕсли == "Да");
            }
            if (зЕсли == Истина)
            {
                return Данные.Интерпретировать(СписокТогда);
            }
            else if (!(СписокИначе == Неопределено))
            {
                return Данные.Интерпретировать(СписокИначе);
            }
            else
            {
                return Неопределено;
            }
        } // ЗначениеВыраженияЕсли()

        object Оператор_Выбор(Данные, Список)
        {
            Перем СписокКогда, СписокТогда;
            СписокКогда = Список;
            if (СписокКогда == Неопределено)
            {
                ВызватьИсключение "Ожидается условие";
            }
            while (СписокКогда != Неопределено)
            {
                СписокТогда = СписокКогда.Соседний;
                if (СписокТогда == Неопределено)
                {
                    ВызватьИсключение "Ожидается выражение";
                }
                if (Данные.Интерпретировать(СписокКогда) == Истина)
                {
                    return Данные.Интерпретировать(СписокТогда);
                }
                СписокКогда = СписокТогда.Соседний;
            }
            ВызватьИсключение "Ни одно из условий не сработало!";
        } // ЗначениеВыраженияВыбор()

        object Оператор_Равно(Данные, Аргумент)
        {
            Перем Значение, Результат;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Результат = Результат && Значение == Данные.Интерпретировать(Аргумент);
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // Равно()

        object Оператор_Больше(Данные, Аргумент)
        {
            Перем Значение1, Значение2;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение1 = Число(Данные.Интерпретировать(Аргумент));
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение2 = Число(Данные.Интерпретировать(Аргумент));
                Результат = Результат && Значение1 > Значение2;
                Значение1 = Значение2;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // Больше()

        object Оператор_Меньше(Данные, Аргумент)
        {
            Перем Значение1, Значение2;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение1 = Число(Данные.Интерпретировать(Аргумент));
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение2 = Число(Данные.Интерпретировать(Аргумент));
                Результат = Результат && Значение1 < Значение2;
                Значение1 = Значение2;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // Меньше()

        object Оператор_БольшеИлиРавно(Данные, Аргумент)
        {
            Перем Значение1, Значение2;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение1 = Число(Данные.Интерпретировать(Аргумент));
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение2 = Число(Данные.Интерпретировать(Аргумент));
                Результат = Результат && Значение1 >= Значение2;
                Значение1 = Значение2;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // БольшеИлиРавно()

        object Оператор_МеньшеИлиРавно(Данные, Аргумент)
        {
            Перем Значение1, Значение2;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение1 = Число(Данные.Интерпретировать(Аргумент));
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение2 = Число(Данные.Интерпретировать(Аргумент));
                Результат = Результат && Значение1 <= Значение2;
                Значение1 = Значение2;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // МеньшеИлиРавно()

        object Оператор_НеРавно(Данные, Аргумент)
        {
            Перем Значение1, Значение2;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значение1 = Данные.Интерпретировать(Аргумент);
            Аргумент = Аргумент.Соседний;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение2 = Данные.Интерпретировать(Аргумент);
                Результат = Результат && Значение1 != Значение2;
                Значение1 = Значение2;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // НеРавно()

        object Оператор_И(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение = Данные.Интерпретировать(Аргумент);
                if (ТипЗнч(Значение) == Тип("Строка"))
                {
                    Значение = (Значение == "Истина" || Значение == "Да");
                }
                Результат = Результат && Значение;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // ЛогическоеИ()

        object Оператор_Или(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Ложь;
            while (Аргумент != Неопределено && !(Результат))
            {
                Значение = Данные.Интерпретировать(Аргумент);
                if (ТипЗнч(Значение) == Тип("Строка"))
                {
                    Значение = (Значение == "Истина" || Значение == "Да");
                }
                Результат = Результат || Значение;
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // ЛогическоеИли()

        object Оператор_Не(Данные, Аргумент)
        {
            Перем Значение;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Результат = Истина;
            while (Аргумент != Неопределено && Результат)
            {
                Значение = Данные.Интерпретировать(Аргумент);
                if (ТипЗнч(Значение) == Тип("Строка"))
                {
                    Значение = (Значение == "Истина" || Значение == "Да");
                }
                Результат = Результат && !(Значение);
                Аргумент = Аргумент.Соседний;
            }
            return Результат;
        } // ЛогическоеНе()

        object Оператор_ВывестиСообщение(Данные, Аргумент)
        {
            Перем Значения;
            if (Аргумент == Неопределено)
            {
                ВызватьИсключение "Ожидается аргумент";
            }
            Значения = Новый Массив;
            while (Аргумент != Неопределено)
            {
                Значения.Добавить(Данные.Интерпретировать(Аргумент));
                Аргумент = Аргумент.Соседний;
            }
            Данные.Процесс.ЗаписатьСобытие("Интерпретатор", СтрСоединить(Значения, " "), 1);
            return Неопределено;
        } // ВывестиСообщение

        object Оператор_ВСтуктуру(Данные, Аргумент, Результат = Неопределено)
        {
            Перем Ключ, Значение;
            if (!(Аргумент == Неопределено))
            {
                if (Аргумент.Имя == "Ключ" || Аргумент.Имя == "К")
                {
                    if (Результат == Неопределено)
                    {
                        Результат = Новый Структура;
                    }
                    Аргумент.Свойство("Значение", Ключ);
                    if (!(Ключ == Неопределено))
                    {
                        Дочерний = Аргумент.Дочерний;
                        if (!(Дочерний == Неопределено))
                        {
                            Значение = Оператор_ВСтуктуру(Данные, Дочерний);
                        }
                        else
                        {
                            Значение = "";
                        }
                        Результат.Вставить(Ключ, Значение);
                    }
                    Соседний = Аргумент.Соседний;
                    if (!(Соседний == Неопределено))
                    {
                        Результат = Оператор_ВСтуктуру(Данные, Соседний, Результат);
                    }
                }
                else
                {
                    return Данные.Интерпретировать(Аргумент);
                }
            }
            return Результат;
        }

        object Оператор_Субъект(Данные, Аргумент)
        {
            return Данные.Процесс.Субъект;
        }

        object Оператор_ТипСобытия(Данные, Аргумент)
        {
            return ? (Аргумент == "0", "Общее", ? (Аргумент == "1", "Успех", ? (Аргумент == "2", "Внимание", "Ошибка")));
        } // ТипСобытия()


    }
}
