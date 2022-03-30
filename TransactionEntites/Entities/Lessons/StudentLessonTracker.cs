using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Lessons
{
    public class StudentLessonTracker : TrackableEntity
    {
        public Guid StudentId { get; set; }
        public double StudentPoints { get; set; }
        public int LessonId { get; set; }
        public DateTime LastDateTime { get; set; }

    }
}