using System.Collections.Generic;

namespace RaspRea.Dto
{
    public class DailyTimetable
    {
        public bool IsFree { get; set; }
        public string Name { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
    
    public class Lesson
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Building { get; set; }
        public string Room { get; set; }
        public string Info { get; set; }

    }

}