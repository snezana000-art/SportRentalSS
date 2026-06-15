using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Formula1Manager
{
    public class Form1 : Form
    {
        // ---------------------- Элементы управления ----------------------
        private ComboBox cmbTables;
        private DataGridView dataGridView;
        private TextBox txtValue1;
        private TextBox txtValue2;
        private TextBox txtValue3;
        private TextBox txtDriverID;    // для Result
        private TextBox txtRaceID;      // для Result
        private Label lblValue1;
        private Label lblValue2;
        private Label lblValue3;
        private Label lblDriverID;
        private Label lblRaceID;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnSearch;
        private Label lblStatus;

        // ---------------------- Строка подключения (измените под себя!) ----------------------
        private string connectionString = @"Server=(localdb)\MSSQLLocalDB; Database=Formula1DB; Integrated Security=True;";

        // ---------------------- Конструктор ----------------------
        public Form1()
        {
            this.Text = "Управление базой данных Формулы-1";
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeControls();
            cmbTables.SelectedIndex = 0;
        }

        private void InitializeControls()
        {
            // ComboBox
            cmbTables = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 20),
                Width = 200
            };
            cmbTables.Items.AddRange(new object[] { "Driver", "Team", "Race", "Result" });
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new Point(20, 60),
                Width = 890,
                Height = 350,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };
            dataGridView.CellClick += DataGridView_CellClick;

            // Поля ввода (первые три - общие)
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

            // Дополнительные поля для Result (DriverID, RaceID) – по умолчанию скрыты
            lblDriverID = new Label { Text = "DriverID:", Location = new Point(labelX, rowY + step * 3), AutoSize = true, Visible = false };
            txtDriverID = new TextBox { Location = new Point(textX, rowY + step * 3 - 3), Width = 200, Visible = false };

            lblRaceID = new Label { Text = "RaceID:", Location = new Point(labelX, rowY + step * 4), AutoSize = true, Visible = false };
            txtRaceID = new TextBox { Location = new Point(textX, rowY + step * 4 - 3), Width = 200, Visible = false };

            // Кнопки
            btnAdd = new Button { Text = "Добавить", Location = new Point(400, 440), Width = 100 };
            btnDelete = new Button { Text = "Удалить", Location = new Point(510, 440), Width = 100 };
            btnSearch = new Button { Text = "Поиск", Location = new Point(620, 440), Width = 100 };
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSearch.Click += BtnSearch_Click;

            // Строка статуса
            lblStatus = new Label
            {
                Location = new Point(20, 570),
                AutoSize = true,
                ForeColor = Color.DarkGreen
            };

            // Добавляем все на форму
            Controls.AddRange(new Control[] {
                cmbTables, dataGridView,
                lblValue1, txtValue1,
                lblValue2, txtValue2,
                lblValue3, txtValue3,
                lblDriverID, txtDriverID,
                lblRaceID, txtRaceID,
                btnAdd, btnDelete, btnSearch,
                lblStatus
            });
        }

        private string CurrentTable => cmbTables.SelectedItem?.ToString();

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
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

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
                            cmd.Parameters.AddWithValue($"@p{i + 1}", parameters[i] ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowStatus("Успешно!", Color.Green);
            }
            catch (Exception ex)
            {
                ShowStatus("Ошибка: " + ex.Message, Color.Red);
                MessageBox.Show("SQL ошибка: " + ex.Message);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            switch (CurrentTable)
            {
                case "Driver":
                    if (CheckFields(3))
                        ExecuteSql("INSERT INTO Driver (FirstName, LastName, Nationality) VALUES (@p1, @p2, @p3)",
                                   txtValue1.Text, txtValue2.Text, txtValue3.Text);
                    break;
                case "Team":
                    if (CheckFields(3))
                        ExecuteSql("INSERT INTO Team (TeamName, BaseCountry, Principal) VALUES (@p1, @p2, @p3)",
                                   txtValue1.Text, txtValue2.Text, txtValue3.Text);
                    break;
                case "Race":
                    if (CheckFields(3) && int.TryParse(txtValue3.Text, out int cid))
                    {
                        int seasonId = 1; // или можно добавить поле
                        ExecuteSql("INSERT INTO Race (RaceName, RaceDate, CircuitID, SeasonID) VALUES (@p1, @p2, @p3, @p4)",
                                   txtValue1.Text, txtValue2.Text, cid.ToString(), seasonId.ToString());
                    }
                    else ShowStatus("CircuitID должен быть числом", Color.Red);
                    break;
                case "Result":
                    if (CheckFields(3) &&
                        int.TryParse(txtValue1.Text, out int pos) &&
                        int.TryParse(txtValue2.Text, out int pts) &&
                        int.TryParse(txtValue3.Text, out int grid) &&
                        int.TryParse(txtDriverID.Text, out int did) &&
                        int.TryParse(txtRaceID.Text, out int rid))
                    {
                        ExecuteSql("INSERT INTO Result (Position, Points, GridPosition, DriverID, RaceID) VALUES (@p1, @p2, @p3, @p4, @p5)",
                                   pos.ToString(), pts.ToString(), grid.ToString(), did.ToString(), rid.ToString());
                    }
                    else ShowStatus("Все поля Result должны быть числами (позиция, очки, старт, DriverID, RaceID)", Color.Red);
                    break;
                default: return;
            }
            LoadData();
            ClearFields();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                ShowStatus("Выберите запись для удаления", Color.Red);
                return;
            }
            string pk = dataGridView.Columns[0].Name;
            object id = dataGridView.CurrentRow.Cells[0].Value;
            if (id == null) return;
            if (MessageBox.Show($"Удалить запись с {pk} = {id}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ExecuteSql($"DELETE FROM {CurrentTable} WHERE {pk} = @p1", id.ToString());
                LoadData();
                ClearFields();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string search = txtValue1.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                LoadData();
                return;
            }
            string firstCol = dataGridView.Columns[0].Name;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM {CurrentTable} WHERE {firstCol} LIKE @p + '%'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@p", search);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView.DataSource = dt;
                    ShowStatus($"Найдено {dt.Rows.Count} записей", Color.Blue);
                }
            }
            catch (Exception ex) { ShowStatus("Ошибка поиска: " + ex.Message, Color.Red); }
        }

        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string table = CurrentTable;
            // Показываем/скрываем дополнительные поля для Result
            bool isResult = table == "Result";
            lblDriverID.Visible = isResult;
            txtDriverID.Visible = isResult;
            lblRaceID.Visible = isResult;
            txtRaceID.Visible = isResult;

            // Меняем подписи
            switch (table)
            {
                case "Driver":
                    lblValue1.Text = "Имя:"; lblValue2.Text = "Фамилия:"; lblValue3.Text = "Национальность:";
                    break;
                case "Team":
                    lblValue1.Text = "Команда:"; lblValue2.Text = "Страна:"; lblValue3.Text = "Руководитель:";
                    break;
                case "Race":
                    lblValue1.Text = "Название гонки:"; lblValue2.Text = "Дата (ГГГГ-ММ-ДД):"; lblValue3.Text = "CircuitID:";
                    break;
                case "Result":
                    lblValue1.Text = "Позиция (число):"; lblValue2.Text = "Очки (число):"; lblValue3.Text = "Старт.поз (число):";
                    break;
            }
            ClearFields();
            LoadData();
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];
                txtValue1.Text = row.Cells[0].Value?.ToString();
                if (row.Cells.Count > 1) txtValue2.Text = row.Cells[1].Value?.ToString();
                if (row.Cells.Count > 2) txtValue3.Text = row.Cells[2].Value?.ToString();
                // Для Result стараемся подставить DriverID и RaceID, если они есть
                if (CurrentTable == "Result" && row.Cells.Count > 3)
                {
                    if (row.Cells["DriverID"] != null) txtDriverID.Text = row.Cells["DriverID"].Value?.ToString();
                    if (row.Cells["RaceID"] != null) txtRaceID.Text = row.Cells["RaceID"].Value?.ToString();
                }
            }
        }

        private bool CheckFields(int count)
        {
            if (string.IsNullOrWhiteSpace(txtValue1.Text)) { ShowStatus("Заполните первое поле", Color.Red); return false; }
            if (count >= 2 && string.IsNullOrWhiteSpace(txtValue2.Text)) { ShowStatus("Заполните второе поле", Color.Red); return false; }
            if (count >= 3 && string.IsNullOrWhiteSpace(txtValue3.Text)) { ShowStatus("Заполните третье поле", Color.Red); return false; }
            return true;
        }

        private void ShowStatus(string msg, Color color)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = color;
        }

        private void ClearFields()
        {
            txtValue1.Clear();
            txtValue2.Clear();
            txtValue3.Clear();
            if (txtDriverID != null) txtDriverID.Clear();
            if (txtRaceID != null) txtRaceID.Clear();
        }
    }
}