using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace GradeJournal.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int Value { get; set; }        // оценка 1-5
        public double Weight { get; set; }    // вес
        public DateTime Date { get; set; }

        // Внешние ключи
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}

