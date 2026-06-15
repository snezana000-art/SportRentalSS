namespace WindowsFormsApp3
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
            this.basketbolDataSet = new WindowsFormsApp3.basketbolDataSet();
            this.basketbolDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.basketbolDataSet1 = new WindowsFormsApp3.basketbolDataSet1();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.partnerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.partnerTableAdapter = new WindowsFormsApp3.basketbolDataSetTableAdapters.partnerTableAdapter();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nasvanieDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnmodify = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.partnerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // basketbolDataSet
            // 
            this.basketbolDataSet.DataSetName = "basketbolDataSet";
            this.basketbolDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // basketbolDataSetBindingSource
            // 
            this.basketbolDataSetBindingSource.DataSource = this.basketbolDataSet;
            this.basketbolDataSetBindingSource.Position = 0;
            // 
            // basketbolDataSet1
            // 
            this.basketbolDataSet1.DataSetName = "basketbolDataSet1";
            this.basketbolDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.nasvanieDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.partnerBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(30, 42);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(361, 150);
            this.dataGridView1.TabIndex = 0;
            // 
            // partnerBindingSource
            // 
            this.partnerBindingSource.DataMember = "partner";
            this.partnerBindingSource.DataSource = this.basketbolDataSetBindingSource;
            // 
            // partnerTableAdapter
            // 
            this.partnerTableAdapter.ClearBeforeFill = true;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "id";
            this.idDataGridViewTextBoxColumn.HeaderText = "id";
            this.idDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.Width = 150;
            // 
            // nasvanieDataGridViewTextBoxColumn
            // 
            this.nasvanieDataGridViewTextBoxColumn.DataPropertyName = "nasvanie";
            this.nasvanieDataGridViewTextBoxColumn.HeaderText = "nasvanie";
            this.nasvanieDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.nasvanieDataGridViewTextBoxColumn.Name = "nasvanieDataGridViewTextBoxColumn";
            this.nasvanieDataGridViewTextBoxColumn.Width = 150;
            // 
            // btnmodify
            // 
            this.btnmodify.Location = new System.Drawing.Point(275, 230);
            this.btnmodify.Name = "btnmodify";
            this.btnmodify.Size = new System.Drawing.Size(75, 34);
            this.btnmodify.TabIndex = 1;
            this.btnmodify.Text = "изменить";
            this.btnmodify.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 230);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 26);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnmodify);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basketbolDataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.partnerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private basketbolDataSet basketbolDataSet;
        private System.Windows.Forms.BindingSource basketbolDataSetBindingSource;
        private basketbolDataSet1 basketbolDataSet1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource partnerBindingSource;
        private basketbolDataSetTableAdapters.partnerTableAdapter partnerTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nasvanieDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnmodify;
        private System.Windows.Forms.TextBox textBox1;
    }
}

