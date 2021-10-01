using System;
using System.Collections.Generic;
using System.Text;
using COMP123_006_OKeeffeKyle_Assignment4.Services.Interfaces;
using COMP123_006_OKeeffeKyle_Assignment4.Models;
using System.Xml.Serialization;
using System.IO;


namespace COMP123_006_OKeeffeKyle_Assignment4.Services
{
    public class StudentService : IGPAService<Student>
    {
        private string path = @$"{Path.GetRelativePath($@"{Directory.GetCurrentDirectory()}", @$"../../../DataFiles/Students")}";

        public List <Student> LoadAll()
        {
            List<Student> studentCollection = new List<Student>();
            XmlSerializer serializer = new XmlSerializer(typeof(Student));
            string[] studentFiles;
 
            if (Directory.GetFiles(path) != null)
            {
                studentFiles= Directory.GetFiles(@$"{Path.GetRelativePath($@"{Directory.GetCurrentDirectory()}", @$"../../../DataFiles/Students")}");

                foreach (string studentFile in studentFiles)
                {
                    Student thisStudent = new Student();
                    using (var reader = new FileStream(studentFile, FileMode.Open))
                    {
                        thisStudent = (Student)serializer.Deserialize(reader);
                    }
                    studentCollection.Add(thisStudent);
                }
            }
            else
            { 
                studentFiles = null;
                studentCollection = new List<Student>();
            }

            return studentCollection;
        }

        public Student Load(Student student)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Student));
            string studentFile = @$"{path}/{student.StudentID}.xml"; 

                using (var reader = new FileStream(studentFile, FileMode.Open))
                {
                   student = (Student)serializer.Deserialize(reader);
                }
                
                return student;
        }

        public void Save(Student student)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Student));
            string studentFile = @$"{path}/{student.StudentID}.xml";

            using (var stream = new FileStream(studentFile, FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, student);
            }
        }

        public void Update(Student student)
        {
            string file = $@"{path}/{student.StudentID}.xml";
            if (File.Exists(file))
                File.Delete(file);
            Save(student);
        }
    }
}

