﻿# practice-4-ISP9-4

Технологический стек:

* Приложение: WPF + бд локальная MSSQL(TSQL)
* Найти библиотеку для работы с Excel файлами
* ORM: ADO.NET/Dapper

User story:
Я как клиент приложения хочу: 
* Импортировать отчет из excel файла в локальную базу данных
* Указывать исходный excel файл отчета
* Экспортировать отчет из локальной базы данных в excel файл
* Указывать выходной файл, в который необходимо сохранить экспортируемый отчет
* Иметь возможность конвертировать отчет из формата word в excel и обратно

Задача:

Разработать приложение с графическим интерфейсом для управления отчетом, отчет представляет из себя excel файл с двумя книгами, которые содержат в себе некоторых набор данных (между наборами данных установлена связь). Приложение должно уметь «склеивать» представленные данные и выполнять экспорт из базы данных и импорт из excel файла. Кроме этого требуется реализовать возможность конвертации отчета из формата word документа в excel (сконвертировать таблицы с данными).

Database
Excel
Client