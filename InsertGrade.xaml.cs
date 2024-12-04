using dziennikOnline;
using System;
using System.Collections.Generic;
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

namespace dziennikOnlineAI
{
    /// <summary>
    /// Interaction logic for InsertGrade.xaml
    /// </summary>
    public partial class InsertGrade : Window
    {
        string connection = "Data Source=DESKTOP-244GHCS\\DZIENNIKAI;Integrated Security=True;Connect Timeout=30;Initial Catalog= szkola";

        public InsertGrade(string pesel)
        {
            InitializeComponent();
            _student.ItemsSource = getStudents(getTeachersClass(pesel));
            getSubjects();
        }

        private List<string> getStudents(string klasa)
        {
            string query = $"SELECT PESEL FROM dbo.uczen WHERE klasa_id='{klasa}'";
            List<string> students = new List<string>();

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(String.Format("{0}", reader[0]));
                    }

                    return students;
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

        private string getSubjectId(string subject)
        {
            string query = $"SELECT Id FROM dbo.przedmiot WHERE nazwa='{subject}'";
            string Id = "";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Id = String.Format("{0}", reader[0]);
                    }

                    return Id;
                }
            }
        }

        private void getSubjects()
        {
            string query = $"SELECT nazwa FROM dbo.przedmiot";
            List<string> subjects = new List<string>();

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    _subjects.Items.Clear();

                    while (reader.Read())
                    {
                        ComboBoxItem comboItem = new ComboBoxItem
                        {
                            Content = String.Format("{0}", reader[0])
                        };

                        _subjects.Items.Add(comboItem);
                    }
                }
            }
        }

        private string getLastGradeId()
        {
            string query = $"SELECT Id FROM dbo.ocena";
            int gradeId = 0;

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gradeId = Convert.ToInt32(reader[0].ToString());
                    }

                    return gradeId.ToString();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string subject = getSubjectId(_subjects.SelectedItem.ToString()).ToString();
            string student = "";
            string grade = "";

            Console.WriteLine("xd", student);
            Console.WriteLine("xd2", grade);
            Console.WriteLine(_subjects.SelectedValue);

            string query = $"INSERT INTO dbo.ocena (id_ucznia, id_przedmiotu, ocena) VALUES ({student}, {subject}, {grade})";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(query, c);
                command.ExecuteNonQuery();
            }
        }
    }
}
