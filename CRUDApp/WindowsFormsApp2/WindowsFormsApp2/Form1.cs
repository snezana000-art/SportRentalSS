using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Formula1Manager
{
    public class MainForm : Form
    {
        // ---------------------- Элементы управления ----------------------
        private ComboBox cmbTables;
        private DataGridView dataGridView;
        private TextBox txtValue1;
        private TextBox txtValue2;
        private TextBox txtValue3;
        private Label lblValue1;
        private Label lblValue2;
        private Label lblValue3;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnSearch;
        private Label lblStatus;

        // ---------------------- Строка подключения ----------------------
        // ПРИМЕЧАНИЕ: измените Server и Database под вашу конфигурацию
        private string connectionString = @"Server=(localdb)\MSSQLLocalDB; Database=Formula1DB; Integrated Security=True;";

        // ---------------------- Конструктор ----------------------
        public MainForm()
        {
            // Настройки формы
            this.Text = "Управление базой данных Формулы-1";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Создаём и размещаем все элементы
            InitializeControls();

            // Загружаем первую таблицу
            cmbTables.SelectedIndex = 0;
        }

        // ---------------------- Создание всех элементов в коде ----------------------
        private void InitializeControls()
        {
            // 1. ComboBox для выбора таблицы
            cmbTables = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 20),
                Width = 200
            };
            cmbTables.Items.AddRange(new object[] { "Driver", "Team", "Race", "Result" });
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;

            // 2. DataGridView для отображения данных
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Width = 840,
                Height = 350,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dataGridView.CellClick += DataGridView_CellClick;

            // 3. Поля ввода и подписи (три строки)
            int labelX = 20;
            int textX = 150;
            int rowY = 440;
            int step = 35;

            lblValue1 = new Label { Text = "Значение 1:", Location = new Point(labelX, rowY), AutoSize = true };
            txtValue1 = new TextBox { Location = new Point(textX, rowY - 3), Width = 200 };

            lblValue2 = new Label { Text = "Значение 2:", Location = new Point(labelX, rowY + step), AutoSize = true };
            txtValue2 = new TextBox { Location = new Point(textX, rowY + step - 3), Width = 200 };

            lblValue3 = new Label { Text = "Значение 3:", Location = new Point(labelX, rowY + step * 2), AutoSize = true };
            txtValue3 = new TextBox { Location = new Point(textX, rowY + step * 2 - 3), Width = 200 };

            // 4. Кнопки
            btnAdd = new Button { Text = "Добавить", Location = new Point(400, 440), Width = 100 };
            btnDelete = new Button { Text = "Удалить", Location = new Point(510, 440), Width = 100 };
            btnSearch = new Button { Text = "Поиск", Location = new Point(620, 440), Width = 100 };

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSearch.Click += BtnSearch_Click;

            // 5. Строка статуса
            lblStatus = new Label
            {
                Location = new Point(20, 530),
                AutoSize = true,
                ForeColor = Color.DarkGreen
            };

            // Добавляем все элементы на форму
            Controls.AddRange(new Control[] {
                cmbTables, dataGridView,
                lblValue1, txtValue1,
                lblValue2, txtValue2,
                lblValue3, txtValue3,
                btnAdd, btnDelete, btnSearch,
                lblStatus
            });
        }

        // ---------------------- Свойство: имя текущей таблицы ----------------------
        private string CurrentTable => cmbTables.SelectedItem?.ToString();

        // ---------------------- Загрузка данных из выбранной таблицы ----------------------
        private void LoadData()
        {
            if (string.IsNullOrEmpty(CurrentTable)) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM {CurrentTable}";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------- Выполнение SQL-запроса с параметрами ----------------------
        private void ExecuteSql(string query, params string[] parameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue($"@p{i + 1}", parameters[i] ?? (object)DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowStatus("Операция выполнена успешно", Color.Green);
            }
            catch (Exception ex)
            {
                ShowStatus("Ошибка: " + ex.Message, Color.Red);
                MessageBox.Show("SQL ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------- Добавление записи ----------------------
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string query = null;
            switch (CurrentTable)
            {
                case "Driver":
                    if (CheckFields(3))
                        query = "INSERT INTO Driver (FirstName, LastName, Nationality) VALUES (@p1, @p2, @p3)";
                    break;
                case "Team":
                    if (CheckFields(3))
                        query = "INSERT INTO Team (TeamName, BaseCountry, Principal) VALUES (@p1, @p2, @p3)";
                    break;
                case "Race":
                    // Для Race нужно 4 поля: RaceName, RaceDate, CircuitID, SeasonID
                    // Используем txtValue1 - название, txtValue2 - дата, txtValue3 - CircuitID, а SeasonID зададим по умолчанию = 1
                    if (CheckFields(3))
                    {
                        string raceName = txtValue1.Text.Trim();
                        string raceDate = txtValue2.Text.Trim();
                        if (!int.TryParse(txtValue3.Text.Trim(), out int circuitId))
                        {
                            ShowStatus("CircuitID должен быть числом!", Color.Red);
                            return;
                        }
                        int seasonId = 1; // Можно изменить или добавить отдельное полеquery = "INSERT INTO Race (RaceName, RaceDate, CircuitID, SeasonID) VALUES (@p1, @p2, @p3, @p4)";
                        ExecuteSql(query, raceName, raceDate, circuitId.ToString(), seasonId.ToString());
                        LoadData();
                        ClearFields();
                        return;
                    }
                    break;
                case "Result":
                    // Для Result нужно 5 полей: Position, Points, GridPosition, DriverID, RaceID
                    if (CheckFields(3))
                    {
                        if (!int.TryParse(txtValue1.Text, out int pos) ||
                            !int.TryParse(txtValue2.Text, out int pts) ||
                            !int.TryParse(txtValue3.Text, out int grid))
                        {
                            ShowStatus("Позиция, очки и стартовая позиция должны быть числами", Color.Red);
                            return;
                        }
                        // Запросим DriverID и RaceID в отдельных диалогах (упрощённо)
                        string driverIdStr = Microsoft.VisualBasic.Interaction.InputBox("Введите DriverID:", "Driver ID", "1");
                        string raceIdStr = Microsoft.VisualBasic.Interaction.InputBox("Введите RaceID:", "Race ID", "1");
                        if (!int.TryParse(driverIdStr, out int driverId) || !int.TryParse(raceIdStr, out int raceId))
                        {
                            ShowStatus("Некорректные ID пилота или гонки", Color.Red);
                            return;
                        }
                        query = "INSERT INTO Result (Position, Points, GridPosition, DriverID, RaceID) VALUES (@p1, @p2, @p3, @p4, @p5)";
                        ExecuteSql(query, pos.ToString(), pts.ToString(), grid.ToString(), driverId.ToString(), raceId.ToString());
                        LoadData();
                        ClearFields();
                        return;
                    }
                    break;
            }

            if (query != null)
            {
                ExecuteSql(query, txtValue1.Text.Trim(), txtValue2.Text.Trim(), txtValue3.Text.Trim());
                LoadData();
                ClearFields();
            }
        }

        // ---------------------- Удаление выбранной записи ----------------------
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                ShowStatus("Не выбрана запись для удаления", Color.Red);
                return;
            }

            string primaryKeyColumn = dataGridView.Columns[0].Name;
            object idValue = dataGridView.CurrentRow.Cells[0].Value;
            if (idValue == null) return;

            DialogResult result = MessageBox.Show($"Удалить запись с {primaryKeyColumn} = {idValue}?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string query = $"DELETE FROM {CurrentTable} WHERE {primaryKeyColumn} = @p1";
                ExecuteSql(query, idValue.ToString());
                LoadData();
                ClearFields();
            }
        }

        // ---------------------- Поиск по первой колонке ----------------------
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchValue = txtValue1.Text.Trim();
            if (string.IsNullOrEmpty(searchValue))
            {
                LoadData();
                return;
            }

            string firstColumn = dataGridView.Columns[0].Name;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM {CurrentTable} WHERE {firstColumn} LIKE @p1 + '%'";
                    SqlCommand cmd = new SqlCommand(query, conn); cmd.Parameters.AddWithValue("@p1", searchValue);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView.DataSource = dt;
                    ShowStatus($"Найдено {dt.Rows.Count} записей", Color.Blue);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Ошибка поиска: " + ex.Message, Color.Red);
            }
        }

        // ---------------------- Изменение подписей полей при смене таблицы ----------------------
        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string table = CurrentTable;
            switch (table)
            {
                case "Driver":
                    lblValue1.Text = "Имя:";
                    lblValue2.Text = "Фамилия:";
                    lblValue3.Text = "Национальность:";
                    break;
                case "Team":
                    lblValue1.Text = "Название команды:";
                    lblValue2.Text = "Страна:";
                    lblValue3.Text = "Руководитель:";
                    break;
                case "Race":
                    lblValue1.Text = "Название гонки:";
                    lblValue2.Text = "Дата (ГГГГ-ММ-ДД):";
                    lblValue3.Text = "CircuitID (число):";
                    break;
                case "Result":
                    lblValue1.Text = "Позиция (число):";
                    lblValue2.Text = "Очки (число):";
                    lblValue3.Text = "Старт. позиция (число):";
                    break;
                default:
                    lblValue1.Text = "Значение 1:";
                    lblValue2.Text = "Значение 2:";
                    lblValue3.Text = "Значение 3:";
                    break;
            }
            ClearFields();
            LoadData();
        }

        // ---------------------- Заполнение полей ввода при клике на строку таблицы ----------------------
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                txtValue1.Text = row.Cells[0].Value?.ToString();
                if (row.Cells.Count > 1) txtValue2.Text = row.Cells[1].Value?.ToString();
                if (row.Cells.Count > 2) txtValue3.Text = row.Cells[2].Value?.ToString();
            }
        }

        // ---------------------- Вспомогательные методы ----------------------
        private bool CheckFields(int requiredCount)
        {
            if (string.IsNullOrWhiteSpace(txtValue1.Text))
            {
                ShowStatus("Заполните первое поле!", Color.Red);
                return false;
            }
            if (requiredCount >= 2 && string.IsNullOrWhiteSpace(txtValue2.Text))
            {
                ShowStatus("Заполните второе поле!", Color.Red);
                return false;
            }
            if (requiredCount >= 3 && string.IsNullOrWhiteSpace(txtValue3.Text))
            {
                ShowStatus("Заполните третье поле!", Color.Red);
                return false;
            }
            return true;
        }

        private void ShowStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void ClearFields()
        {
            txtValue1.Clear();
            txtValue2.Clear();
            txtValue3.Clear();
        }
    }
}