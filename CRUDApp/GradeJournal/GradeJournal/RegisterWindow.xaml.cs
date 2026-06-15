using System;
using System.Linq;
using System.Windows;
using GradeJournal.Models;

namespace GradeJournal
{
    public partial class RegisterWindow : Window
    {
        private ApplicationDbContext _context;

        public RegisterWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = tbFullName.Text.Trim();
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                tbError.Text = "Введите ФИО!";
                return;
            }

            if (string.IsNullOrWhiteSpace(login))
            {
                tbError.Text = "Введите логин!";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                tbError.Text = "Введите пароль!";
                return;
            }

            if (password.Length < 4)
            {
                tbError.Text = "Пароль должен быть не менее 4 символов!";
                return;
            }

            if (_context.Users.Any(u => u.Login == login))
            {
                tbError.Text = "Пользователь уже существует!";
                return;
            }

            User newUser = new User
            {
                FullName = fullName,
                Login = login,
                Password = password,
                Role = "User",
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            MessageBox.Show(
                "Регистрация успешна!",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            MainWindow mainWindow = new MainWindow(newUser);
            mainWindow.Show();

            Close();
        }
    }
}