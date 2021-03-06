![alt text](https://github.com/AntonNov/Sharp_3sem/blob/main/lab3/mems/EI6sv3seZJA.jpg)

Лабораторная работа № 3
=======================

Реализовано:
------------

* Класс `Json5Parser` строит список из пар "ключ-значение", где ключ - строка, а значение - строка или такой же список. (`List<KeyValuePair<string, object>>`) любой сложности для указанного файла.

* Класс `XmlParser` строит `List<KeyValuePair<string, object>>` любой сложности для указанного файла.

* Класс `ClassConstructor` создаёт экземпляр класса при помощи самого подходящего конструктора класса, который определяется наибольшим соответствием **имён** параметров конструктора с **ключами** `List<KeyValuePair<string, object>>`.

* Класс `ConfigReader` - местный менеджер конфигурации. Он ищет файлы в каталоге приложения, начинающиеся на "config" и парсит первый по алфавиту подходящий файл (с расширением ".json" или ".xml") `Json5Parser`'ом или `XmlParser`'ом соответственно. Метод `GetOptions<T>()` создаёт класс `T` при помощи `ClassConstructor` на основе конфигурационного файла.

* Классы, которые строятся при помощи `GetOptions<T>()` имеют вид `XXXSettings` и находятся в файлах `XXX.cs` в Lab2.

> В папке "exsamples" находятся примеры конфигурационных файлов для `DirectoryFileExtractor` из Lab2.

![alt text](https://github.com/AntonNov/Sharp_3sem/blob/main/lab3/mems/xpTdzTKQoUs.jpg)

Особенности:
------------

* `Json5Parser` поддержиает файлы в формате Json5.

* `ClassConstructor` способен создавать иерархии классов.

![alt text](https://github.com/AntonNov/Sharp_3sem/blob/main/lab3/mems/0DHikolJYck.jpg)

Краткое описание классов:
-------------------------

* `ConfigReader` создаёт экземпляр класса, используя первый подходящий конфигурационный файл из найденых по условиям поиска.

* `ClassConstructor` cоздаёт экземпляр класса из `List<KeyValuePair<string, object>>`. Для создания выбирается самый подходящий конструктор класса, который определяется наибольшим соответствием **имён** переменных конструктора с **ключами** `List<KeyValuePair<string, object>>`. Проверка типов происходит на этапе создания экземпляра класса, несоответствие типов приводит к генерации исключения.

* `Json5Parser` и `XmlParser` строят `List<KeyValuePair<string, object>>` из указанных файлов.
> Чтобы построить класс `item` с конструктором без параметров, в Json: `"item" : { }`, а в XML: `<item/>`/
> Чтобы построить пустую строку `str` в Json: `"str" : ""`, а в XML: `<srt></str>`.

![alt text](https://github.com/AntonNov/Sharp_3sem/blob/main/lab3/mems/-WQ2OrJrEP4.jpg)
![alt text](https://github.com/AntonNov/Sharp_3sem/blob/main/lab3/mems/W6pvvLhZhZ4.jpg)
