using System;
using System.Collections.Generic;
using System.Text;
using COMP123_006_OKeeffeKyle_Assignment4.Services.Interfaces;

namespace COMP123_006_OKeeffeKyle_Assignment4.Models
{
    public class Student
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public int StudentID { get; set; }
        public int CurrentSem { get; set; }
        public List<Semester> StudentSems { get; set; }

        public Student()
        {
            this.StudentSems = new List<Semester>();
        }

        public Student(int currentSem)
        {
            this.CurrentSem = currentSem;
            this.StudentSems = new List<Semester>();
        }
    }
}
