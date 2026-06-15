using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Models; // Добавьте пространство имен для моделей

namespace WpfApp2

{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Здесь размещаем код инициализации
            using (var db = new AppDbContext())
            {
                // Пример добавления данных
                var student = new Student
                {
                    LastName = "Петров",
                    FirstName = "Иван",
                    GroupName = "ИВТ-25"
                };
                db.Students.Add(student);
                db.SaveChanges();
            }
        }
    }
}

