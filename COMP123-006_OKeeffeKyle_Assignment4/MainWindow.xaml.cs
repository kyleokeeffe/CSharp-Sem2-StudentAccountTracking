using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using COMP123_006_OKeeffeKyle_Assignment4.Models;
using COMP123_006_OKeeffeKyle_Assignment4.Services;
using System.Xml.Serialization;
using System.IO;


namespace COMP123_006_OKeeffeKyle_Assignment4
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            StudentService studentService = new StudentService();
            dataStudents.ItemsSource = studentService.LoadAll();

            btnNewStudent.Click += OnAddStudent;
            btnSubmitNewStudent.Click += OnSubmitNewStudent;
            dataStudents.SelectionChanged += OnStudentSelected;

            dataSems.SelectionChanged += OnSemSelected;
            btnNewSem.Click += OnAddNewSem;
            btnSubmitSem.Click += OnSubmitNewSem; 

            btnAddCourse.Click += OnAddNewCourse;
            btnCancelAddCourse.Click += OnCancelAddNewCourse;
            btnSubmitCourse.Click += OnSubmitNewCourse;
            btnSubmitByCourse.Click += OnCheckCourse;

            btnAdvancedOptions.Click += OnSeeAdvancedOptions;
            btnHideAdvanced.Click += OnHideAdvanced;
            btngetAllCourseStats.Click += OnGetAllCourseStats;
        }

        public List<Student> LoadStudents()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
            var reader = new FileStream(@$"{Directory.GetCurrentDirectory()}/StudentList.xml", FileMode.Open);
            List<Student> StudentCollection = (List<Student>)serializer.Deserialize(reader);
            reader.Close();

            return StudentCollection;
        }

        public void OnAddStudent(object sender, EventArgs e)
        {
            hideNewStudent.Visibility = Visibility.Visible;
        }

        public void OnSubmitNewStudent(object sender, EventArgs e)
        {
            StudentService studentService = new StudentService();
            Student student = new Student()
            {
                FName = txtFName.Text,
                LName = txtLName.Text,
                StudentID = Convert.ToInt32(txtStudentID.Text),
                CurrentSem = Convert.ToInt32(txtCurrentSem.Text)
            };

            for (int i = 1; i <= student.CurrentSem; i++)
            {
                student.StudentSems.Add(new Semester(i));
            }

            studentService.Save(student);
            
            hideNewStudent.Visibility = Visibility.Collapsed;
            txtFName.Text = null;
            txtLName.Text = null;
            txtStudentID.Text = null;
            txtCurrentSem.Text = null;

            dataStudents.ItemsSource=studentService.LoadAll();
        }


        public void OnStudentSelected(object sender, EventArgs e)
        {
            hideSemButton.Visibility = Visibility.Visible;

            Student selectedStudent = new Student();
            StudentService studentService = new StudentService();

            if (dataStudents.SelectedItem != null && dataStudents.SelectedItem.GetType()==typeof(Student))
            {
                selectedStudent = (Student)dataStudents.SelectedItem;
                dataSems.ItemsSource = selectedStudent.StudentSems;
            }
            else if (dataStudents.SelectedItem == null)
            {
                dataSems.ItemsSource = null;
            }
        }

        public void OnAddNewSem(object sender, EventArgs e)
        {
            btnNewSem.Visibility = Visibility.Collapsed;
            hideSemAdd.Visibility = Visibility.Visible;
        }

        public void OnSubmitNewSem(object sender, EventArgs e)
        {
            StudentService studentService = new StudentService();

            var selectedStudent = (Student)dataStudents.SelectedItem;
            var thisStudent = studentService.Load(selectedStudent);

            var requestedSemNumber = Int32.Parse(txtSemToAdd.Text);
            bool uniqueSemValue = true;

            foreach (Semester semester in thisStudent.StudentSems)
            {
                if (semester.SemNumber == requestedSemNumber)
                {
                    MessageBox.Show("A record for that Semester already exists.\n Please choose a different Semester.");
                    uniqueSemValue = false;
                }   
            }

            if (uniqueSemValue == true)
            {
                Semester semester = new Semester(requestedSemNumber);
                thisStudent.StudentSems.Add(semester);
            }
                
            studentService.Save(thisStudent);

            dataSems.ItemsSource = studentService.Load(thisStudent).StudentSems;
            txtSemToAdd.Text = null;
        }

        public void OnAddNewCourse(object sender, EventArgs e)
        {    
            dataCourses.Height = 150;
            pnlNewCourse.Visibility = Visibility.Visible;
            btnAddCourse.Visibility = Visibility.Collapsed;
            btnSubmitCourse.Visibility = Visibility.Visible;
        }

        public void OnSubmitNewCourse(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCourseName.Text) )
            {
                MessageBox.Show("Please enter a course name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCreditHours.Text) || !double.TryParse(txtCreditHours.Text, out _))
            {
                MessageBox.Show("Please enter a valid number for Credit Hours");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtGrade.Text))
            {
                MessageBox.Show("Please enter a valid number for Grade");
                return;
            }

            //string gpaType;
            double thisGrade=0;
            bool validGradeInput = true;
            double normalizedGrade = 0;

            if (!double.TryParse(txtGrade.Text, out _))
            {
                switch (txtGrade.Text.Substring(0, 1).ToUpper())
                {
                    case "A":
                        thisGrade = 4;
                        break;
                    case "B":
                        thisGrade = 3;
                        break;
                    case "C":
                        thisGrade = 2;
                        break;
                    case "D":
                        thisGrade = 1;
                        break;
                    case "F":
                        thisGrade = 1;
                        break;
                    default:
                        validGradeInput = false;
                        break;
                }

                if (txtGrade.Text.Length > 1)
                {
                    switch (txtGrade.Text.Substring(1, 1))
                    {
                        case "+":
                            if (thisGrade != 4)
                                thisGrade += 0.3;
                            break;
                        case "-":
                            if (thisGrade != 0)
                                thisGrade -= 0.3;
                            break;
                        default:
                            validGradeInput = false;
                            break;
                    }
                }

                if (validGradeInput == true)
                    normalizedGrade = thisGrade;
            }
            else
            {
                if (double.Parse(txtGrade.Text) <= 4 && double.Parse(txtGrade.Text) > 0)
                    normalizedGrade = double.Parse(txtGrade.Text);
                else if (double.Parse(txtGrade.Text) >= 4 && double.Parse(txtGrade.Text) <= 100)
                {
                    switch (double.Parse(txtGrade.Text))
                    {
                        case double n when (n <= 100 && n >= 94):
                            thisGrade = 4;
                            break;
                        case double n when (n <= 93 && n >= 90):
                            thisGrade = 3.7;
                            break;
                        case double n when (n <= 89 && n >= 87):
                            thisGrade = 3.3;
                            break;
                        case double n when (n <= 86 && n >= 84):
                            thisGrade = 3;
                            break;
                        case double n when (n <= 83 && n >= 80):
                            thisGrade = 2.7;
                            break;
                        case double n when (n <= 79 && n >= 77):
                            thisGrade = 2.3;
                            break;
                        case double n when (n <= 76 && n >= 74):
                            thisGrade = 2;
                            break;
                        case double n when (n <= 73 && n >= 70):
                            thisGrade = 1.7;
                            break;
                        case double n when (n <= 69 && n >= 67):
                            thisGrade = 1.3;
                            break;
                        case double n when (n <= 66 && n >= 64):
                            thisGrade = 1;
                            break;
                        case double n when (n <= 63 && n >= 60):
                            thisGrade = 0.7;
                            break;
                        case double n when (n <= 59 && n >= 4.1):
                            thisGrade = 0;
                            break;
                        default:
                            validGradeInput = false;
                            break;
                    }
                    if (validGradeInput == true)
                        normalizedGrade = thisGrade;
                }
            }
               
            StudentService studentService = new StudentService();

            Student student = new Student();
            student =  (Student)dataStudents.SelectedItem;
            Semester semester = new Semester();
            semester = (Semester)dataSems.SelectedItem;

            var thisHours = Convert.ToDouble(txtCreditHours.Text);
            CourseRecord courseRecord = new CourseRecord(student.StudentID, semester.SemNumber)
            {
                CourseName = txtCourseName.Text,
                CreditHours = thisHours,
                Grade = txtGrade.Text,
                CourseGPA = normalizedGrade
                };

            int semStudentIndex = student.StudentSems.IndexOf(student.StudentSems.Find(x => x.SemNumber == semester.SemNumber));

            Semester thisSemester = student.StudentSems[semStudentIndex];

            student.StudentSems[semStudentIndex].CourseRecords.Add(courseRecord);
            thisSemester.GPA = thisSemester.CalculateGPA();

            studentService.Update(student);

            Student studentUpdate = new Student();
            studentUpdate = studentService.Load(student);

            dataSems.ItemsSource = studentUpdate.StudentSems;
            dataSems.SelectedIndex = semStudentIndex;
            dataCourses.ItemsSource = studentUpdate.StudentSems[semStudentIndex].CourseRecords;
            lblGPACourses.Content = studentUpdate.StudentSems[semStudentIndex].GPA;
            lblStudentNameCourses.Content = $"{studentUpdate.FName} {studentUpdate.LName}";
            lblSemNumberCourses.Content = studentUpdate.StudentSems[semStudentIndex].SemNumber;

            txtCourseName.Text = null;
            txtCreditHours.Text = null;
            txtGrade.Text = null;
        }

        public void OnCancelAddNewCourse(object sender, EventArgs e)
        {
            pnlNewCourse.Visibility = Visibility.Collapsed;
            btnAddCourse.Visibility = Visibility.Visible;
        }

        public void OnSemSelected(object sender, EventArgs e)
        {
            if (dataSems.SelectedItem != null && dataSems.SelectedItem.GetType() == typeof(Semester))
            {
                pnlAllAddCourse.Visibility = Visibility.Visible;

                Semester semester = new Semester();
                semester = (Semester)dataSems.SelectedItem;

                if (dataStudents.SelectedItem != null && dataStudents.SelectedItem.GetType() == typeof(Student))
                {
                    Student student = new Student();
                    student = (Student)dataStudents.SelectedItem;

                    lblStudentNameCourses.Content = $"{student.FName} {student.LName}";
                }

                dataCourses.ItemsSource = semester.CourseRecords;
                lblGPACourses.Content = semester. GPA;
                lblSemNumberCourses.Content = semester.SemNumber;
            }
            else
            { 
                pnlNewCourse.Visibility = Visibility.Collapsed;
                btnAddCourse.Visibility = Visibility.Visible;

                dataCourses.ItemsSource = null;
            }
        }

        public void OnSeeAdvancedOptions(object sender, EventArgs e)
        {
            pnlAdvancedOptions.Visibility = Visibility.Visible;
            btnAdvancedOptions.Visibility = Visibility.Collapsed;
            btnHideAdvanced.Visibility = Visibility.Visible;

            StudentService studentService = new StudentService();

            List<Student> students = new List<Student>();
            students = studentService.LoadAll();
            List<string> allCourses = new List<string>();

            foreach (Student student in students)
            {
                List<Semester> semesters = new List<Semester>();
                semesters = student.StudentSems;

                foreach (Semester semester in semesters)
                {
                    List<CourseRecord> courseRecords = new List<CourseRecord>();
                    courseRecords = semester.CourseRecords;

                    foreach (CourseRecord courseRecord in courseRecords)
                    {
                        if (!allCourses.Contains(courseRecord.CourseName))
                            allCourses.Add(courseRecord.CourseName);
                    }
                }
            }
            cmbxCourseNames.ItemsSource = allCourses;
        }

        public void OnHideAdvanced(object sender, EventArgs e)
        {
            pnlAdvancedOptions.Visibility = Visibility.Collapsed;
            btnAdvancedOptions.Visibility = Visibility.Visible;
            btnHideAdvanced.Visibility = Visibility.Collapsed;
        }

        public void OnGetAllCourseStats(object sender, EventArgs e)
        {
            StudentService studentService = new StudentService();

            List<Student> students = new List<Student>();
            students = studentService.LoadAll();
            List<string> allCourses = new List<string>();
            
            foreach (Student student in students)
            {
                List<Semester> semesters = new List<Semester>();
                semesters = student.StudentSems;

                foreach (Semester semester in semesters)
                {
                    List<CourseRecord> courseRecords = new List<CourseRecord>();
                    courseRecords = semester.CourseRecords;

                    foreach (CourseRecord courseRecord in courseRecords)
                    {
                        if (!allCourses.Contains(courseRecord.CourseName))
                            allCourses.Add(courseRecord.CourseName);
                    }
                }
            }

            Dictionary<string,double> allCourseGPAs= new Dictionary<string, double>();

            foreach (string courseName in allCourses)
            {
                allCourseGPAs.Add(courseName, CheckCourseGPA(courseName));
            }

            pnlSingleCourse.Visibility = Visibility.Collapsed;
            dataByCourse.Visibility = Visibility.Visible;
            dataByCourse.ItemsSource = allCourseGPAs;
        }

        public double CheckCourseGPA(string courseToFind)
        {
            StudentService studentService = new StudentService();
            List<Student> students = new List<Student>();

            students = studentService.LoadAll();

            List<string> allCourses = new List<string>();
            List<string> courseRecordsFoundName = new List<string>();
            List<double> courseRecordsFoundGPA = new List<double>();

            foreach (Student student in students)
            {
                List<Semester> semesters = new List<Semester>();
                semesters = student.StudentSems;

                foreach (Semester semester in semesters)
                {
                    List<CourseRecord> courseRecords = new List<CourseRecord>();
                    courseRecords = semester.CourseRecords;

                    foreach (CourseRecord courseRecord in courseRecords)
                    {
                        if (!allCourses.Contains(courseRecord.CourseName))
                            allCourses.Add(courseRecord.CourseName);
                            
                        if (courseRecord.CourseName == courseToFind)
                            courseRecordsFoundGPA.Add(courseRecord.CourseGPA);
                    }
                }
            }

            dataByCourse.ItemsSource = courseRecordsFoundGPA;
            lblAdvCourseName.Content = courseToFind;
            lblAdvCourseAvg.Content = courseRecordsFoundGPA.Sum() / courseRecordsFoundGPA.Count;

            return courseRecordsFoundGPA.Sum() / courseRecordsFoundGPA.Count;
        } 

        public void OnCheckCourse(object sender, EventArgs e)
        {
            if (cmbxCourseNames.SelectedItem != null)
            {
                string courseToFind =cmbxCourseNames.SelectedItem.ToString();
                
                StudentService studentService = new StudentService();

                List<Student> students = new List<Student>();
                students = studentService.LoadAll();

                var querySample = from student in students from semester in student.StudentSems from courseRecord in semester.CourseRecords where courseRecord.CourseName == courseToFind select new { thisStudent = student, thisCourseRecord = courseRecord };

                List<string> allCourses = new List<string>();
                List<string> courseRecordsFoundName = new List<string>();
                List<double> courseRecordsFoundGPA = new List<double>();
                Dictionary<string, double> allCourseStudentGPAs = new Dictionary<string, double>();

                foreach (var item in querySample)
                {
                    allCourseStudentGPAs.Add($"{item.thisStudent.LName},{item.thisStudent.FName}:{item.thisCourseRecord.SemNumber}", item.thisCourseRecord.CourseGPA);
                }

            pnlSingleCourse.Visibility = Visibility.Visible;
            dataByCourse.Visibility = Visibility.Visible;

            dataByCourse.ItemsSource = allCourseStudentGPAs;
            lblAdvCourseName.Content = courseToFind;
            lblAdvCourseAvg.Content = courseRecordsFoundGPA.Sum() / courseRecordsFoundGPA.Count;
            }
        }

        
    }
}
