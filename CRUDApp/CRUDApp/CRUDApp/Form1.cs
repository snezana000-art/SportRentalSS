using System;
using System.Drawing;
using System.Windows.Forms;

namespace CRUDApp
{
    public partial class Form1 : Form
    {
        private TextBox txtId;
        private TextBox txtName;
        private TextBox txtSurname;
        private TextBox txtAge;
        private DataGridView dataGrid;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnFilter;
        private Label lblStatus;

        public Form1()
        {
            InitializeComponent();
            SetupControls();
            Load += Form1_Load;
        }

        private void SetupControls()
        {
            // Настройка формы
            this.Text = "CRUD Приложение";
            this.Size = new Size(600, 400);

            // Создание текстовых полей
            txtId = new TextBox { Location = new Point(20, 20), Width = 50 };
            txtName = new TextBox { Location = new Point(80, 20), Width = 150 };
            txtSurname = new TextBox { Location = new Point(240, 20), Width = 150 };
            txtAge = new TextBox { Location = new Point(400, 20), Width = 50 };

            // Настройка DataGridView
            dataGrid = new DataGridView
            {
                Location = new Point(20, 50),
                Width = 560,
                Height = 250,
                ColumnCount = 4
            };
            dataGrid.Columns[0].Name = "ID";
            dataGrid.Columns[1].Name = "Имя";
            dataGrid.Columns[2].Name = "Фамилия";
            dataGrid.Columns[3].Name = "Возраст";
            dataGrid.CellClick += dataGrid_CellClick;

            // Создание кнопок
            btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(20, 310),
                Width = 70
            };
            btnAdd.Click += btnAdd_Click;

            btnEdit = new Button
            {
                Text = "Редактировать",
                Location = new Point(100, 310),
                Width = 70
            };
            btnEdit.Click += btnEdit_Click;

            btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(180, 310),
                Width = 70
            };
            btnDelete.Click += btnDelete_Click;

            btnFilter = new Button
            {
                Text = "Фильтр",
                Location = new Point(260, 310),
                Width = 70
            };
            btnFilter.Click += btnFilter_Click;

            // Метка статуса
            lblStatus = new Label
            {
                Location = new Point(20, 350),
                Width = 560
            };

            // Добавляем элементы на форму
            Controls.Add(txtId);
            Controls.Add(txtName);
            Controls.Add(txtSurname);
            Controls.Add(txtAge);
            Controls.Add(dataGrid);
            Controls.Add(btnAdd);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnFilter);
            Controls.Add(lblStatus);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                // Здесь код добавления в БД
                ShowMessage("Запись добавлена", Color.Green);
                LoadData();
                ClearFields();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (ValidateInput() && !string.IsNullOrEmpty(txtId.Text))
            {
                // Здесь код обновления записи в БД
                ShowMessage("Запись обновлена", Color.Green);
                LoadData();
                ClearFields();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtId.Text))
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Здесь код удаления записи из БД
                    ShowMessage("Запись удалена", Color.Green);
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string filter = txtName.Text;
            if (!string.IsNullOrEmpty(filter))
            {
                // Здесь код фильтрации данных
                ShowMessage("Фильтрация выполнена", Color.Green);
                LoadData();
            }
        }

        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGrid.Rows[e.RowIndex];
                txtId.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                txtSurname.Text = row.Cells[2].Value.ToString();
                txtAge.Text = row.Cells[3].Value.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            // Здесь должен быть код загрузки данных из БД
            // Пример тестовых данных:
            dataGrid.Rows.Clear();
            dataGrid.Rows.Add(
                new object[] { 1, "Иван", "Иванов", 25 },
                new object[] { 2, "Петр", "Петров", 30 }
            );
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(txtSurname.Text) ||
                string.IsNullOrEmpty(txtAge.Text))
            {
                ShowMessage("Заполните все поля!", Color.Red);
                return false;
            }

            if (!int.TryParse(txtAge.Text, out _))
            {
                ShowMessage("Возраст должен быть числом", Color.Red);
                return false;
            }

            return true;
        }

        private void ShowMessage(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void ClearFields()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtSurname.Text = "";
            txtAge.Text = "";
        }

       
            }
}
        