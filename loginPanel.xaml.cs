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

namespace dziennikOnline
{
    /// <summary>
    /// Logika interakcji dla klasy loginPanel.xaml
    /// </summary>
    public partial class loginPanel : Window
    {
        public loginPanel()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            string enteredUsername = _username.Text;
            string enteredPassword = _password.Text;

            //string connection = "Data Source=10.100.100.146;Initial Catalog= szkola;User ID=Admin2; Password=zaq1@WSX";
            string connection = "Data Source=DESKTOP-244GHCS\\DZIENNIKAI;Integrated Security=True;Connect Timeout=30;Initial Catalog= szkola";
            string studentPassword = $"SELECT haslo FROM dbo.uczen WHERE PESEL='{enteredUsername}';";
            string teacherPassword = $"SELECT haslo FROM dbo.nauczyciel WHERE PESEL='{enteredUsername}';";

            using (SqlConnection c = new SqlConnection(connection))
            {
                c.Open();
                SqlCommand command = new SqlCommand(studentPassword, c);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    string receivedPassword = result.ToString();

                    if (receivedPassword == enteredPassword)
                    {
                        mainPanel otherWindow = new mainPanel(false, enteredUsername);
                        otherWindow.Show();
                        Close();
                        return;
                    }
                }

                command = new SqlCommand(teacherPassword, c);
                result = command.ExecuteScalar();

                if (result != null)
                {
                    string receivedPassword = result.ToString();

                    if (receivedPassword == enteredPassword)
                    {
                        mainPanel otherWindow = new mainPanel(true, enteredUsername);
                        otherWindow.Show();
                        Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("ACHTUNG!!!\nZłe hasło lub nazwa użytkownika.");
                }
            }
        }
    }
}