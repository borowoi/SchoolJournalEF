# SchoolJournalEF

[![Build](https://github.com/borowoi/SchoolJournalEF/actions/workflows/build.yml/badge.svg)](https://github.com/borowoi/SchoolJournalEF/actions/workflows/build.yml)

Лабораторная работа №2: Entity Framework Core + LINQ.
Предметная область: электронный журнал успеваемости школы (MS SQL Server, база данных SchoolJournalDB).

## Что реализовано
- Платформа: .NET 10, Entity Framework Core 10.
- Подход Database First (Scaffold-DbContext): модели в папке `Models`, контекст `SchoolJournalDbContext` в папке `Data`.
- Строка подключения хранится в `appsettings.json`.
- LINQ-запросы: выборка, фильтрация, группировка, соединение таблиц.
- Вставка, обновление и удаление данных (таблицы на сторонах «один» и «многие»).
- GitHub Actions: сборка под Ubuntu и Windows при любом push и pull request.

## Запуск
1. Создать базу данных скриптом из лабораторной работы №1 и заполнить её данными.
2. При необходимости изменить `DefaultConnection` в `appsettings.json`.
3. Выполнить `dotnet run`.