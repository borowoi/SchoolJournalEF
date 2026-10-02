using System.Collections;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchoolJournalEF.Data;
using SchoolJournalEF.Models;

namespace SchoolJournalEF;

public class Program
{
    // Тестовый предмет: вставляется, обновляется и удаляется
    private const string TestSubject = "Робототехника (тест)";

    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Строка подключения из appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        string connectionString = config.GetConnectionString("DefaultConnection")!;

        var options = new DbContextOptionsBuilder<SchoolJournalDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using (var db = new SchoolJournalDbContext(options))
        {
            Pause("====== Будет выполнена выборка данных (нажмите любую клавишу) ========");
            Select(db);

            Pause("====== Будет выполнена вставка данных (нажмите любую клавишу) ========");
            Insert(db);
            Console.WriteLine("====== Выборка после вставки ========");
            SelectTestData(db);

            Pause("====== Будет выполнено обновление данных (нажмите любую клавишу) ========");
            Update(db);
            Console.WriteLine("====== Выборка после обновления ========");
            SelectTestData(db);

            Pause("====== Будет выполнено удаление данных (нажмите любую клавишу) ========");
            Delete(db);
            Console.WriteLine("====== Выборка после удаления ========");
            SelectTestData(db);
        }

        Pause("====== Готово. Нажмите любую клавишу для выхода ========");
    }

    // ---------- Вспомогательные методы ----------

    static void Pause(string message)
    {
        Console.WriteLine(message);
        if (!Console.IsInputRedirected)
            Console.ReadKey(true);
    }

    static void Print(string title, IEnumerable items)
    {
        Console.WriteLine(title);
        Console.WriteLine("Записи: ");
        int count = 0;
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
            count++;
        }
        if (count == 0) Console.WriteLine("(нет записей)");
        Console.WriteLine();
    }

    // ---------- Выборка ----------

    static void Select(SchoolJournalDbContext db)
    {
        // 1. Все данные из таблицы на стороне «один» (Subjects)
        var query1 = from s in db.Subjects
                     orderby s.SubjectId
                     select new
                     {
                         Код_предмета = s.SubjectId,
                         Название = s.SubjectName,
                         Вид_контроля = s.ExamType
                     };
 
        Print("1. Все записи таблицы Subjects (сторона «один»), первые 5:\r\n",
            query1.Take(5).ToList());

        // 2. Таблица «один» с фильтром по нескольким полям
        var query2 = from s in db.Subjects
                     where s.ExamType == "Экзамен" && s.SubjectId > 100
                     orderby s.SubjectId
                     select new
                     {
                         Код_предмета = s.SubjectId,
                         Название = s.SubjectName,
                         Вид_контроля = s.ExamType
                     };
        
        Print("2. Предметы с видом контроля «Экзамен» и кодом > 100, первые 5:\r\n",
            query2.Take(5).ToList());

        // 3. Группировка в таблице на стороне «многие» (Lessons): Count / Min / Max
        var query3 = from l in db.Lessons
                     group l by l.SubjectId into g
                     select new
                     {
                         Код_предмета = g.Key,
                         Количество_уроков = g.Count(),
                         Первый_урок = g.Min(x => x.LessonDate),
                         Последний_урок = g.Max(x => x.LessonDate)
                     };
 
        Print("3. Количество уроков по предметам (группировка по SubjectID), топ-5:\r\n",
            query3.OrderByDescending(x => x.Количество_уроков).Take(5).ToList());

        // 4. Два поля из двух таблиц (Lessons «многие», Subjects «один»)
        var query4 = from l in db.Lessons
                     join s in db.Subjects on l.SubjectId equals s.SubjectId
                     orderby l.LessonId
                     select new
                     {
                         Название_предмета = s.SubjectName,
                         Тема_урока = l.Topic
                     };
  
        Print("4. Название предмета и тема урока (Lessons + Subjects), первые 5:\r\n",
            query4.Take(5).ToList());

        // 5. Две таблицы (Lessons, Classes) с фильтром по нескольким полям
        var query5 = from l in db.Lessons
                     join c in db.Classes on l.ClassId equals c.ClassId
                     where c.GradeLevel == 5 && l.LessonNumber <= 2
                     orderby l.LessonDate, l.LessonNumber
                     select new
                     {
                         Класс = c.GradeLevel,
                         Буква = c.ClassLetter,
                         Дата = l.LessonDate,
                         Номер_урока = l.LessonNumber,
                         Тема = l.Topic
                     };
        Print("5. Уроки 5-х классов (1-й и 2-й уроки дня), первые 5:\r\n",
            query5.Take(5).ToList());
    }

    // Показывает тестовый предмет и его уроки (для проверки вставки/обновления/удаления)
    static void SelectTestData(SchoolJournalDbContext db)
    {
        var subjects = (from s in db.Subjects
                        where s.SubjectName == TestSubject
                        select new
                        {
                            Код_предмета = s.SubjectId,
                            Название = s.SubjectName,
                            Вид_контроля = s.ExamType
                        }).ToList();
        Print("Тестовый предмет в таблице Subjects:\r\n", subjects);

        var lessons = (from l in db.Lessons
                       where l.Subject.SubjectName == TestSubject
                       select new
                       {
                           Код_урока = l.LessonId,
                           Дата = l.LessonDate,
                           Номер_урока = l.LessonNumber,
                           Тема = l.Topic
                       }).ToList();
        Print("Уроки тестового предмета в таблице Lessons:\r\n", lessons);
    }

    // ---------- Вставка ----------

    static void Insert(SchoolJournalDbContext db)
    {
        // Сторона «один»: новый предмет
        var subject = new Subject
        {
            SubjectName = TestSubject,
            ExamType = "Экзамен"
        };
        db.Subjects.Add(subject);
        db.SaveChanges(); // после сохранения SubjectId получает значение из IDENTITY

        // Сторона «многие»: урок для существующего класса и нового предмета
        int classId = db.Classes.OrderBy(c => c.ClassId).Select(c => c.ClassId).First();
        var lesson = new Lesson
        {
            ClassId = classId,
            SubjectId = subject.SubjectId,
            TeacherId = null,
            LessonDate = DateOnly.FromDateTime(DateTime.Today),
            LessonNumber = 1,
            Topic = "Введение в робототехнику"
        };
        db.Lessons.Add(lesson);
        db.SaveChanges();

        Console.WriteLine($"Добавлен предмет SubjectID={subject.SubjectId} и урок LessonID={lesson.LessonId}");
        Console.WriteLine();
    }

    // ---------- Обновление ----------

    static void Update(SchoolJournalDbContext db)
    {
        var subjects = db.Subjects.Where(s => s.SubjectName == TestSubject).ToList();
        foreach (var s in subjects)
            s.ExamType = "Зачет";

        var lessons = db.Lessons.Where(l => l.Subject.SubjectName == TestSubject).ToList();
        foreach (var l in lessons)
        {
            l.Topic = "Введение в робототехнику (обновлено)";
            l.LessonNumber = 2;
        }

        db.SaveChanges();
        Console.WriteLine($"Обновлено предметов: {subjects.Count}, уроков: {lessons.Count}");
        Console.WriteLine();
    }

    // ---------- Удаление ----------

    static void Delete(SchoolJournalDbContext db)
    {
        // Сторона «многие»: сначала уроки (FK Lessons -> Subjects без каскада)
        var lessons = db.Lessons.Where(l => l.Subject.SubjectName == TestSubject).ToList();
        db.Lessons.RemoveRange(lessons);
        db.SaveChanges();

        // Сторона «один»: затем предмет (SubjectHours удалятся каскадом)
        var subjects = db.Subjects.Where(s => s.SubjectName == TestSubject).ToList();
        db.Subjects.RemoveRange(subjects);
        db.SaveChanges();

        Console.WriteLine($"Удалено уроков: {lessons.Count}, предметов: {subjects.Count}");
        Console.WriteLine();
    }
}