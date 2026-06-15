using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GradeJournal.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeJournal
{
    public partial class MainWindow : Window
    {
        private ApplicationDbContext _context;
        private User _currentUser;

        // НОВЫЙ КОНСТРУКТОР (с параметром User)
        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _currentUser = currentUser;
            LoadAllData();
            ApplyRolePermissions();
        }

        // Метод для применения прав в зависимости от роли
        private void ApplyRolePermissions()
        {
            if (_currentUser.Role == "Admin")
            {
                btnAddStudent.IsEnabled = true;
                btnEditStudent.IsEnabled = true;
                btnDeleteStudent.IsEnabled = true;
                btnAddSubject.IsEnabled = true;
                btnDeleteSubject.IsEnabled = true;
                btnAddGrade.IsEnabled = true;
                tbStatsStatus.Text = $"Добро пожаловать, {_currentUser.FullName} (Администратор)";
            }
            else
            {
                btnAddStudent.IsEnabled = false;
                btnEditStudent.IsEnabled = false;
                btnDeleteStudent.IsEnabled = false;
                btnAddSubject.IsEnabled = false;
                btnDeleteSubject.IsEnabled = false;
                btnAddGrade.IsEnabled = false;
                tbStatsStatus.Text = $"Добро пожаловать, {_currentUser.FullName} (Пользователь) - режим просмотра";
            }
        }

        // ==================== ЗАГРУЗКА ДАННЫХ ====================

        private void LoadAllData()
        {
            try
            {
                _context.Database.Migrate();
                LoadStudents();
                LoadSubjects();
                LoadGrades();
                LoadComboBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStudents()
        {
            var students = _context.Students.ToList();
            dgStudents.ItemsSource = students;
            tbStudentCount.Text = $"Всего студентов: {students.Count}";
        }

        private void LoadSubjects()
        {
            var subjects = _context.Subjects.ToList();
            dgSubjects.ItemsSource = subjects;
            tbSubjectCount.Text = $"Всего предметов: {subjects.Count}";
        }

        private void LoadGrades()
        {
            var grades = _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Select(g => new
                {
                    g.Id,
                    Студент = g.Student.LastName + " " + g.Student.FirstName,
                    Предмет = g.Subject.Name,
                    Оценка = g.Value,
                    Вес = g.Weight,
                    Дата = g.Date.ToString("dd.MM.yyyy")
                })
                .ToList();
            dgGrades.ItemsSource = grades;
        }

        private void LoadComboBoxes()
        {
            var students = _context.Students.ToList();

            cbStudent.ItemsSource = students;
            cbStudent.DisplayMemberPath = "LastName";
            cbStudent.SelectedValuePath = "Id";

            var subjects = _context.Subjects.ToList();
            cbSubject.ItemsSource = subjects;
            cbSubject.DisplayMemberPath = "Name";
            cbSubject.SelectedValuePath = "Id";

            cbStudentForAvg.ItemsSource = students;
            cbStudentForAvg.DisplayMemberPath = "LastName";
            cbStudentForAvg.SelectedValuePath = "Id";
        }

        // ==================== ДОБАВЛЕНИЕ СТУДЕНТА ====================

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            var tbLastName = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };
            var tbFirstName = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };
            var tbGroup = new TextBox { Margin = new Thickness(0, 0, 0, 10), Height = 30 };

            var btnSave = new Button
            {
                Content = "Сохранить",
                Height = 35,
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var btnCancel = new Button
            {
                Content = "Отмена",
                Height = 35,
                Background = System.Windows.Media.Brushes.Gray,
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 10, 0, 0)
            };

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
            buttonPanel.Children.Add(btnSave);
            buttonPanel.Children.Add(btnCancel);

            var dialog = new Window
            {
                Title = "Добавление студента",
                Width = 350,
                Height = 320,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.CanResize,
                Content = new StackPanel
                {
                    Margin = new Thickness(15),
                    Children =
                    {
                        new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) },
                        tbLastName,
                        new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) },
                        tbFirstName,
                        new TextBlock { Text = "Группа:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) },
                        tbGroup,
                        buttonPanel
                    }
                }
            };

            btnSave.Click += (s, args) =>
            {
                string lastName = tbLastName.Text.Trim();
                string firstName = tbFirstName.Text.Trim();
                string group = tbGroup.Text.Trim();

                if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
                {
                    MessageBox.Show("Заполните фамилию и имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _context.Students.Add(new Student
                {
                    LastName = lastName,
                    FirstName = firstName,
                    GroupName = group
                });
                _context.SaveChanges();
                LoadStudents();
                LoadComboBoxes();
                dialog.Close();

                MessageBox.Show("Студент успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            btnCancel.Click += (s, args) => dialog.Close();

            dialog.Owner = this;
            dialog.ShowDialog();
        }

        // ==================== ДОБАВЛЕНИЕ ПРЕДМЕТА ====================

        private void btnAddSubject_Click(object sender, RoutedEventArgs e)
        {
            var tbName = new TextBox { Margin = new Thickness(0, 0, 0, 15), Height = 30 };
            var btnSave = new Button
            {
                Content = "Сохранить",
                Height = 35,
                Background = System.Windows.Media.Brushes.Green,
                Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold
            };

            var dialog = new Window
            {
                Title = "Добавление предмета",
                Width = 350,
                Height = 160,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Content = new StackPanel
                {
                    Margin = new Thickness(15),
                    Children =
                    {
                        new TextBlock { Text = "Название предмета:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) },
                        tbName,
                        btnSave
                    }
                }
            };

            btnSave.Click += (s, args) =>
            {
                string name = tbName.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Введите название предмета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _context.Subjects.Add(new Subject { Name = name });
                _context.SaveChanges();
                LoadSubjects();
                LoadComboBoxes();
                dialog.Close();

                MessageBox.Show("Предмет успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            dialog.Owner = this;
            dialog.ShowDialog();
        }

        // ==================== ДОБАВЛЕНИЕ ОЦЕНКИ ====================

        private void btnAddGrade_Click(object sender, RoutedEventArgs e)
        {
            if (cbStudent.SelectedValue == null || cbSubject.SelectedValue == null || cbGradeValue.SelectedItem == null)
            {
                MessageBox.Show("Выберите студента, предмет и оценку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int studentId = (int)cbStudent.SelectedValue;
            int subjectId = (int)cbSubject.SelectedValue;
            int gradeValue = int.Parse(((ComboBoxItem)cbGradeValue.SelectedItem).Content.ToString().Split(' ')[0]);

            double weight = 1.0;
            if (cbWeight.SelectedItem != null)
            {
                string weightText = ((ComboBoxItem)cbWeight.SelectedItem).Content.ToString();
                string weightStr = weightText.Split(' ')[1];
                weightStr = weightStr.Replace('.', ',');
                weight = double.Parse(weightStr);
            }

            _context.Grades.Add(new Grade
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Value = gradeValue,
                Weight = weight,
                Date = DateTime.Now
            });
            _context.SaveChanges();
            LoadGrades();
            MessageBox.Show("Оценка успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== РЕДАКТИРОВАНИЕ СТУДЕНТА ====================

        private void btnEditStudent_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student selectedStudent)
            {
                var tbLastName = new TextBox { Text = selectedStudent.LastName, Margin = new Thickness(0, 0, 0, 10), Height = 30 };
                var tbFirstName = new TextBox { Text = selectedStudent.FirstName, Margin = new Thickness(0, 0, 0, 10), Height = 30 };
                var tbGroup = new TextBox { Text = selectedStudent.GroupName, Margin = new Thickness(0, 0, 0, 10), Height = 30 };

                var btnSave = new Button
                {
                    Content = "Сохранить",
                    Height = 35,
                    Background = System.Windows.Media.Brushes.Green,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                var btnCancel = new Button
                {
                    Content = "Отмена",
                    Height = 35,
                    Background = System.Windows.Media.Brushes.Gray,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(5, 10, 0, 0)
                };

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
                buttonPanel.Children.Add(btnSave);
                buttonPanel.Children.Add(btnCancel);

                var dialog = new Window
                {
                    Title = "Редактирование студента",
                    Width = 350,
                    Height = 320,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    ResizeMode = ResizeMode.CanResize,
                    Content = new StackPanel
                    {
                        Margin = new Thickness(15),
                        Children =
                        {
                            new TextBlock { Text = "Фамилия:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) },
                            tbLastName,
                            new TextBlock { Text = "Имя:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) },
                            tbFirstName,
                            new TextBlock { Text = "Группа:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) },
                            tbGroup,
                            buttonPanel
                        }
                    }
                };

                btnSave.Click += (s, args) =>
                {
                    if (string.IsNullOrWhiteSpace(tbLastName.Text) || string.IsNullOrWhiteSpace(tbFirstName.Text))
                    {
                        MessageBox.Show("Фамилия и имя не могут быть пустыми!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    selectedStudent.LastName = tbLastName.Text;
                    selectedStudent.FirstName = tbFirstName.Text;
                    selectedStudent.GroupName = tbGroup.Text;

                    _context.SaveChanges();
                    LoadStudents();
                    LoadComboBoxes();
                    dialog.Close();

                    MessageBox.Show("Данные студента обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                };

                btnCancel.Click += (s, args) => dialog.Close();

                dialog.Owner = this;
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите студента для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ==================== УДАЛЕНИЕ ====================

        private void btnDeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student student)
            {
                if (MessageBox.Show($"Удалить студента {student.LastName} {student.FirstName}? Все его оценки также будут удалены.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Students.Remove(student);
                    _context.SaveChanges();
                    LoadStudents();
                    LoadComboBoxes();
                    LoadGrades();
                }
            }
            else
            {
                MessageBox.Show("Выберите студента для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDeleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (dgSubjects.SelectedItem is Subject subject)
            {
                if (MessageBox.Show($"Удалить предмет {subject.Name}? Все оценки по нему будут удалены.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Subjects.Remove(subject);
                    _context.SaveChanges();
                    LoadSubjects();
                    LoadComboBoxes();
                    LoadGrades();
                }
            }
            else
            {
                MessageBox.Show("Выберите предмет для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ==================== ОБНОВЛЕНИЕ ====================

        private void btnRefreshStudents_Click(object sender, RoutedEventArgs e) => LoadStudents();
        private void btnRefreshSubjects_Click(object sender, RoutedEventArgs e) => LoadSubjects();

        // ==================== ГЛАВНАЯ ФУНКЦИЯ: РАСЧЁТ СРЕДНЕГО БАЛЛА ====================

        private void btnCalculateAvg_Click(object sender, RoutedEventArgs e)
        {
            if (cbStudentForAvg.SelectedValue == null)
            {
                MessageBox.Show("Выберите студента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int studentId = (int)cbStudentForAvg.SelectedValue;
            var grades = _context.Grades.Where(g => g.StudentId == studentId).ToList();

            if (grades.Count == 0)
            {
                tbAverage.Text = "Нет оценок";
                tbWeightedAverage.Text = "Нет оценок";
                tbGradeCount.Text = "0";
                return;
            }

            double simpleAvg = grades.Average(g => g.Value);
            double weightedAvg = grades.Sum(g => g.Value * g.Weight) / grades.Sum(g => g.Weight);

            tbAverage.Text = simpleAvg.ToString("F2");
            tbWeightedAverage.Text = weightedAvg.ToString("F2");
            tbGradeCount.Text = grades.Count.ToString();
        }

        // ==================== СТАТИСТИКА ====================

        private void btnRefreshStats_Click(object sender, RoutedEventArgs e)
        {
            var allGrades = _context.Grades.Include(g => g.Student).Include(g => g.Subject).ToList();

            if (allGrades.Count == 0)
            {
                tbSchoolAvg.Text = "Нет данных";
                tbBestSubject.Text = "-";
                tbBestSubjectAvg.Text = "";
                tbHonorsStudents.Text = "Нет";
                tbLowStudents.Text = "Нет";
                tbStatsStatus.Text = "Нет данных для статистики";
                return;
            }

            double schoolAvg = allGrades.Average(g => g.Value);
            tbSchoolAvg.Text = schoolAvg.ToString("F2");

            var bestSubject = allGrades.GroupBy(g => g.Subject.Name)
                .Select(g => new { Name = g.Key, Avg = g.Average(x => x.Value) })
                .OrderByDescending(x => x.Avg)
                .FirstOrDefault();

            if (bestSubject != null)
            {
                tbBestSubject.Text = bestSubject.Name;
                tbBestSubjectAvg.Text = $"средний балл: {bestSubject.Avg:F2}";
            }

            var honors = allGrades.GroupBy(g => g.Student.Id)
                .Select(g => new { Student = g.First().Student, Avg = g.Average(x => x.Value) })
                .Where(x => x.Avg >= 4.5)
                .Select(x => $"{x.Student.LastName} {x.Student.FirstName} ({x.Avg:F2})")
                .ToList();

            tbHonorsStudents.Text = honors.Count > 0 ? string.Join(", ", honors) : "Нет";

            var low = allGrades.GroupBy(g => g.Student.Id)
                .Select(g => new { Student = g.First().Student, Avg = g.Average(x => x.Value) })
                .Where(x => x.Avg < 3)
                .Select(x => $"{x.Student.LastName} {x.Student.FirstName} ({x.Avg:F2})")
                .ToList();

            tbLowStudents.Text = low.Count > 0 ? string.Join(", ", low) : "Нет";

            tbStatsStatus.Text = $"Статистика обновлена: {DateTime.Now:HH:mm:ss}";
        }

        // ==================== LINQ ЗАПРОСЫ ====================

        private void btnTopStudents_Click(object sender, RoutedEventArgs e)
        {
            var topStudents = _context.Grades
                .GroupBy(g => g.StudentId)
                .Select(g => new
                {
                    Студент = _context.Students.FirstOrDefault(s => s.Id == g.Key).LastName + " " +
                             _context.Students.FirstOrDefault(s => s.Id == g.Key).FirstName,
                    СреднийБалл = g.Average(x => x.Value),
                    КоличествоОценок = g.Count()
                })
                .OrderByDescending(x => x.СреднийБалл)
                .Take(5)
                .ToList();

            if (topStudents.Count == 0)
            {
                MessageBox.Show("Нет данных для формирования рейтинга!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = "ТОП-5 СТУДЕНТОВ:\n\n";
            for (int i = 0; i < topStudents.Count; i++)
            {
                result += $"{i + 1}. {topStudents[i].Студент}\n";
                result += $"   Средний балл: {topStudents[i].СреднийБалл:F2}\n";
                result += $"   Количество оценок: {topStudents[i].КоличествоОценок}\n\n";
            }

            MessageBox.Show(result, "Рейтинг студентов", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSubjectStats_Click(object sender, RoutedEventArgs e)
        {
            var subjectStats = _context.Grades
                .Include(g => g.Subject)
                .GroupBy(g => g.Subject.Name)
                .Select(g => new
                {
                    Предмет = g.Key,
                    СреднийБалл = g.Average(x => x.Value),
                    Средневзвешенный = g.Sum(x => x.Value * x.Weight) / g.Sum(x => x.Weight),
                    ВсегоОценок = g.Count(),
                    Отлично = g.Count(x => x.Value == 5),
                    Хорошо = g.Count(x => x.Value == 4),
                    Удовлетворительно = g.Count(x => x.Value == 3),
                    Неудовлетворительно = g.Count(x => x.Value == 2)
                })
                .OrderByDescending(x => x.СреднийБалл)
                .ToList();

            if (subjectStats.Count == 0)
            {
                MessageBox.Show("Нет данных по предметам!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = "СТАТИСТИКА ПО ПРЕДМЕТАМ:\n\n";
            foreach (var item in subjectStats)
            {
                result += $"Предмет: {item.Предмет}\n";
                result += $"   Средний балл: {item.СреднийБалл:F2}\n";
                result += $"   Средневзвешенный: {item.Средневзвешенный:F2}\n";
                result += $"   Оценки: 5-{item.Отлично}, 4-{item.Хорошо}, 3-{item.Удовлетворительно}, 2-{item.Неудовлетворительно}\n";
                result += $"   Всего: {item.ВсегоОценок}\n\n";
            }

            MessageBox.Show(result, "Статистика по предметам", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnLowStudents_Click(object sender, RoutedEventArgs e)
        {
            var lowStudents = _context.Grades
                .GroupBy(g => new { g.StudentId, g.Student.LastName, g.Student.FirstName })
                .Where(g => g.Average(x => x.Value) < 3)
                .Select(g => new
                {
                    g.Key.LastName,
                    g.Key.FirstName,
                    СреднийБалл = g.Average(x => x.Value),
                    КоличествоОценок = g.Count()
                })
                .OrderBy(x => x.СреднийБалл)
                .ToList();

            if (lowStudents.Count == 0)
            {
                MessageBox.Show("Двоечников нет! Все студенты успевают!", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = "СПИСОК ДВОЕЧНИКОВ (средний балл < 3):\n\n";
            foreach (var student in lowStudents)
            {
                result += $"{student.LastName} {student.FirstName}\n";
                result += $"   Средний балл: {student.СреднийБалл:F2}\n";
                result += $"   Количество оценок: {student.КоличествоОценок}\n\n";
            }

            MessageBox.Show(result, "Неуспевающие студенты", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void btnBestPerSubject_Click(object sender, RoutedEventArgs e)
        {
            var bestStudents = _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .GroupBy(g => g.SubjectId)
                .Select(g => new
                {
                    Предмет = g.First().Subject.Name,
                    ЛучшийСтудент = g.OrderByDescending(x => x.Value).First().Student.LastName + " " +
                                   g.OrderByDescending(x => x.Value).First().Student.FirstName,
                    МаксимальнаяОценка = g.Max(x => x.Value)
                })
                .ToList();

            if (bestStudents.Count == 0)
            {
                MessageBox.Show("Нет данных по предметам!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = "ЛУЧШИЙ СТУДЕНТ ПО КАЖДОМУ ПРЕДМЕТУ:\n\n";
            foreach (var item in bestStudents)
            {
                result += $"Предмет: {item.Предмет}: {item.ЛучшийСтудент} - {item.МаксимальнаяОценка}\n";
            }

            MessageBox.Show(result, "Лучшие студенты", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnMonthlyProgress_Click(object sender, RoutedEventArgs e)
        {
            var monthlyStats = _context.Grades
                .GroupBy(g => new { g.Date.Year, g.Date.Month })
                .Select(g => new
                {
                    Год = g.Key.Year,
                    Месяц = g.Key.Month,
                    СреднийБалл = g.Average(x => x.Value),
                    КоличествоОценок = g.Count()
                })
                .OrderBy(x => x.Год)
                .ThenBy(x => x.Месяц)
                .ToList();

            if (monthlyStats.Count == 0)
            {
                MessageBox.Show("Нет данных по месяцам!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string result = "ДИНАМИКА УСПЕВАЕМОСТИ ПО МЕСЯЦАМ:\n\n";
            foreach (var month in monthlyStats)
            {
                string monthName = GetMonthName(month.Месяц);
                result += $"{monthName} {month.Год}\n";
                result += $"   Средний балл: {month.СреднийБалл:F2}\n";
                result += $"   Количество оценок: {month.КоличествоОценок}\n\n";
            }

            MessageBox.Show(result, "Успеваемость по месяцам", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string GetMonthName(int month)
        {
            string[] months = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
                                "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };
            return months[month - 1];
        }

        private void btnExportStats_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = $"Статистика_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string content = "========================================\n";
                content += "ЭЛЕКТРОННЫЙ ЖУРНАЛ УСПЕВАЕМОСТИ - ОТЧЁТ\n";
                content += "========================================\n\n";
                content += $"Дата формирования: {DateTime.Now}\n";
                content += "========================================\n\n";

                content += $"Средний балл по школе: {tbSchoolAvg.Text}\n";
                content += $"Лучший предмет: {tbBestSubject.Text} {tbBestSubjectAvg.Text}\n\n";

                content += $"ОТЛИЧНИКИ:\n{tbHonorsStudents.Text}\n\n";
                content += $"ДВОЕЧНИКИ:\n{tbLowStudents.Text}\n";

                System.IO.File.WriteAllText(filePath, content);
                MessageBox.Show($"Отчёт сохранён в файл:\n{filePath}", "Экспорт",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}