using System;
using System.Collections.Generic;

namespace SchoolJournalEF.Models;

public partial class VSubjectStatsByClass
{
    public string Класс { get; set; } = null!;

    public string НазваниеПредмета { get; set; } = null!;

    public int? ВсегоПроведенныхУроков { get; set; }

    public string УчебныйГод { get; set; } = null!;
}
