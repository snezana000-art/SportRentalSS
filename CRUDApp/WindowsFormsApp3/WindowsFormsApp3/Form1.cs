using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "basketbolDataSet.partner". При необходимости она может быть перемещена или удалена.
            this.partnerTableAdapter.Fill(this.basketbolDataSet.partner);
        }

        // Кнопка ДОБАВИТЬ
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Замените Test (Test) на название вашей таблицы и атрибута в ней
            string query = "INSERT INTO Test (Test) VALUES (@name)";
            ExecuteSql(query, textBox1.Text);
            MessageBox.Show("Запись добавлена!");
        }

        // Кнопка УДАЛИТЬ
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Замените Test (Test) на название вашей таблицы и атрибута в ней
            string query = "DELETE FROM Test WHERE Test = @name";
            ExecuteSql(query, textBox1.Text);
            MessageBox.Show("Запись удалена!");
        }

        private void ExecuteSql(string query, string userName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", userName);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
