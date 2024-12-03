using dziennikOnlineAI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace dziennikOnline
{
    /// <summary>
    /// Logika interakcji dla klasy mainPanel.xaml
    /// </summary>
    public partial class mainPanel : Window
    {
        string connection = "Data Source=DESKTOP-244GHCS\\DZIENNIKAI;Integrated Security=True;Connect Timeout=30;Initial Catalog= szkola";
        bool isTeacher;
        string username;

        public mainPanel(bool isTeacher, string pesel)
        {
            InitializeComponent();
            this.isTeacher = isTeacher;
            this.username = pesel;

            if (this.isTeacher)
            {
                setTeacherHeader(username);
                _classId.Content = getTeachersClass(username);
                getStudents(getTeachersClass(username));
            }
            else
            {
                _addGrade.Visibility = Visibility.Hidden;
                setStudentHeader(username);
                getStudentData(username);
                displayGrades(username);
            }
        }

        private void setTeacherHeader(string pesel)
        {
            string query = $"SELECT PESEL, imie, nazwisko FROM dbo.nauczyciel WHERE PESEL='{pesel}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _userHeader.Content = String.Format(
                            "Witaj {0} {1}", reader[1], reader[2]
                        );
                    }
                }
            }
        }

        private void setStudentHeader(string pesel)
        {
            string query = $"SELECT PESEL, imie, nazwisko FROM dbo.uczen WHERE PESEL='{pesel}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _userHeader.Content = String.Format(
                            "Witaj {0} {1}", reader[1], reader[2]
                        );
                    }
                }
            }
        }

        private void getStudentData(string pesel)
        {
            string query = $"SELECT PESEL, imie, nazwisko, klasa_id, punkty FROM dbo.uczen WHERE PESEL='{pesel}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _studentData.Content = String.Format(
                            "Pesel: {0}\n" +
                            "Imię: {1}\n" +
                            "Nazwisko: {2}\n" +
                            "Klasa: {3}\n" +
                            "Punkty: {4}",
                            reader[0], reader[1], reader[2], reader[3], reader[4]
                        );
                    }
                }
            }
        }

        private string getTeachersClass(string pesel)
        {
            string query = $"SELECT Id FROM dbo.klasa WHERE wychowawca_id='{pesel}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return String.Format("{0}", reader[0]);
                    }
                }
            }

            return "";
        }

        private void getStudents(string klasa)
        {
            string query = $"SELECT imie, nazwisko, klasa_id, PESEL FROM dbo.uczen WHERE klasa_id='{klasa}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    _studentList.Items.Clear();
                    while (reader.Read())
                    {
                        string pesel = reader[3].ToString();

                        TreeViewItem treeItem = new TreeViewItem
                        {
                            Header = String.Format(
                                "{0} {1}", reader[0], reader[1]
                            )
                        };

                        treeItem.Selected += (s, e) =>
                        {
                            getStudentData(pesel);

                            if (isTeacher)
                            {
                                displayGrades(pesel);
                            }
                        };

                        _studentList.Items.Add(treeItem);
                    }
                }
            }
        }

        private string getSubjectName(string subject)
        {
            string query = $"SELECT nazwa FROM dbo.przedmiot WHERE Id='{subject}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        return reader[0].ToString();
                    }
                }
            }

            return "";
        }

        private void getGradesFromSubject(string pesel, string subject)
        {
            string query = $"SELECT ocena FROM dbo.ocena WHERE id_ucznia='{pesel}' AND id_przedmiotu='{subject}'";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<string> grades = new List<string> { getSubjectName(subject) };

                    while (reader.Read())
                    {
                        string grade = reader[0].ToString();
                        grades.Add(grade);
                    }
                    
                    var row = grades;

                    _gradesDataGrid.Items.Add(row);
                }
            }
        }

        private void displayGrades(string pesel)
        {
            _gradesDataGrid.Columns.Clear();
            _gradesDataGrid.Items.Clear();

            for (int i = 0; i < 25; i++)
            {
                _gradesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = i == 0 ? "Przedmiot" : "",
                    Binding = new Binding($"[{i}]")
                });
            }

            for (int i = 1; i <= 10; i++)
            {
                getGradesFromSubject(pesel, i.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InsertGrade otherWindow = new InsertGrade(username);
            otherWindow.Show();
        }
    }
}