using System;
using System.Collections.Generic;
using System.Text;
using COMP123_006_OKeeffeKyle_Assignment4.Services;
using System.Windows;
using System.Windows.Controls;

namespace COMP123_006_OKeeffeKyle_Assignment4.Models
{
    public class Semester
    {
        public int SemNumber { get; set; }
        public List <CourseRecord> CourseRecords { get; set; }
        public double GPA { get; set; }

        public Semester()
        {
         
        }

        public Semester(int semNumber)
        {
            this.SemNumber = semNumber;
            this.CourseRecords = new List<CourseRecord>();
        }

        public double CalculateGPA()
        {
            double creditHoursSum = 0;
            double totalGPA = 0;

            List<double> creditHoursWeight = new List<double>();
   
            foreach (CourseRecord courseRecord in this.CourseRecords)
                creditHoursSum += courseRecord.CreditHours;
                
            foreach (CourseRecord courseRecord in this.CourseRecords)
                creditHoursWeight.Add(courseRecord.CreditHours / creditHoursSum);

            for (int i = 0; i < this.CourseRecords.Count; i++)
                totalGPA += this.CourseRecords[i].CourseGPA * creditHoursWeight[i];
 
            return totalGPA;
        }
    }
}
