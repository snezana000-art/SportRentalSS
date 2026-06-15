using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

public partial class MainForm : Form
{
    private DataManager dataManager = new DataManager();
    private List<Person> currentList = new List<Person>();

    public MainForm()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        currentList = dataManager.GetAllPeople();
        dataGridView.DataSource = currentList;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var newPerson = new Person
        {
            Name = txtName.Text,
            Surname = txtSurname.Text,
            Age = int.Parse(txtAge.Text)
        };

        dataManager.AddPerson(newPerson);
        LoadData();
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        var selectedRow = dataGridView.CurrentRow;
        if (selectedRow != null)
        {
            var person = new Person
            {
                Id = (int)selectedRow.Cells["Id"].Value,
                Name = txtName.Text,
                Surname = txtSurname.Text,
                Age = int.Parse(txtAge.Text)
            };

            dataManager.UpdatePerson(person);
            LoadData();
        }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
        var selectedRow = dataGridView.CurrentRow;
        if (selectedRow != null)
        {
            int id = (int)selectedRow.Cells["Id"].Value;
            dataManager.DeletePerson(id);
            LoadData();
        }
    }

    private void btnFilter_Click(object sender, EventArgs e)
    {
        currentList = dataManager.FilterPeople(txtFilter.Text);
        dataGridView.DataSource = currentList;
    }

    private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var row = dataGridView.Rows[e.RowIndex];
        txtName.Text = row.Cells["Name"].Value.ToString();
        txtSurname.Text = row.Cells["Surname"].Value.ToString();
        txtAge.Text = row.Cells["Age"].Value.ToString();
    }
}
