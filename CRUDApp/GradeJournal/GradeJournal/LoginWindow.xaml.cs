using System.Linq;
using System.Windows;
using GradeJournal.Models;

namespace GradeJournal
{
    public partial class LoginWindow : Window
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public LoginWindow()
        {
            InitializeComponent();

            if (!db.Users.Any())
            {
                db.Users.Add(new User
                {
                    Login = "admin",
                    Password = "123",
                    FullName = "Главный администратор",
                    Role = "Admin"
                });

                db.SaveChanges();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = db.Users.FirstOrDefault(u =>
                u.Login == txtLogin.Text &&
                u.Password == txtPassword.Password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                Close();
            }
            else
            {
                txtError.Text = "Неверный логин или пароль!";
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(db);
            registerWindow.ShowDialog();
        }
    }
}