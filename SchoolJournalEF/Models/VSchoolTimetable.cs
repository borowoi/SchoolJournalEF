using System;
using System.Collections.Generic;

namespace SchoolJournalEF.Models;

public partial class VSchoolTimetable
{
    public int IdУрока { get; set; }

    public DateOnly Дата { get; set; }

    public int Урока { get; set; }

    public string Класс { get; set; } = null!;

    public string Предмет { get; set; } = null!;

    public string ТемаЗанятия { get; set; } = null!;

    public string ТипАттестации { get; set; } = null!;
}
