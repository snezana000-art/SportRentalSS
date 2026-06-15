namespace WindowsFormsApp4
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.fotbolDataSet = new WindowsFormsApp4.FotbolDataSet();
            this.poleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.poleTableAdapter = new WindowsFormsApp4.FotbolDataSetTableAdapters.poleTableAdapter();
            this.iDpolaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nomerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Удаление = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fotbolDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.poleBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDpolaDataGridViewTextBoxColumn,
            this.nomerDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.poleBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(272, 73);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(361, 87);
            this.dataGridView1.TabIndex = 0;
            // 
            // fotbolDataSet
            // 
            this.fotbolDataSet.DataSetName = "FotbolDataSet";
            this.fotbolDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // poleBindingSource
            // 
            this.poleBindingSource.DataMember = "pole";
            this.poleBindingSource.DataSource = this.fotbolDataSet;
            // 
            // poleTableAdapter
            // 
            this.poleTableAdapter.ClearBeforeFill = true;
            // 
            // iDpolaDataGridViewTextBoxColumn
            // 
            this.iDpolaDataGridViewTextBoxColumn.DataPropertyName = "IDpola";
            this.iDpolaDataGridViewTextBoxColumn.HeaderText = "IDpola";
            this.iDpolaDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.iDpolaDataGridViewTextBoxColumn.Name = "iDpolaDataGridViewTextBoxColumn";
            this.iDpolaDataGridViewTextBoxColumn.Width = 150;
            // 
            // nomerDataGridViewTextBoxColumn
            // 
            this.nomerDataGridViewTextBoxColumn.DataPropertyName = "nomer";
            this.nomerDataGridViewTextBoxColumn.HeaderText = "nomer";
            this.nomerDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.nomerDataGridViewTextBoxColumn.Name = "nomerDataGridViewTextBoxColumn";
            this.nomerDataGridViewTextBoxColumn.Width = 150;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(342, 260);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(182, 26);
            this.textBox1.TabIndex = 1;
            // 
            // Удаление
            // 
            this.Удаление.Location = new System.Drawing.Point(160, 322);
            this.Удаление.Name = "Удаление";
            this.Удаление.Size = new System.Drawing.Size(172, 41);
            this.Удаление.TabIndex = 2;
            this.Удаление.Text = "Удаление";
            this.Удаление.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(522, 322);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(172, 41);
            this.button2.TabIndex = 3;
            this.button2.Text = "Сохранение";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Удаление);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fotbolDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.poleBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private FotbolDataSet fotbolDataSet;
        private System.Windows.Forms.BindingSource poleBindingSource;
        private FotbolDataSetTableAdapters.poleTableAdapter poleTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDpolaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nomerDataGridViewTextBoxColumn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Удаление;
        private System.Windows.Forms.Button button2;
    }
}

