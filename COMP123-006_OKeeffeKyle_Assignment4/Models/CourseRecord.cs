using System;
using System.Collections.Generic;
using System.Text;

namespace COMP123_006_OKeeffeKyle_Assignment4.Models
{
    public class CourseRecord
    {
        public int StudentID {get;set;}
        public int SemNumber { get; set; }
        public string CourseName { get; set; }
        public double CreditHours { get; set; }
        public string Grade { get; set; }
        public double CourseGPA { get; set; }
       
        public CourseRecord()
        {

        }

        public CourseRecord(int studentID,int semNumber)
        {
            this.StudentID = studentID;
            this.SemNumber = semNumber;
        }
    }
}
