namespace EntityBuilder
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.generateButton = new Button();
            this.tableNameComboBox = new ComboBox();
            this.attributeCheckBox = new CheckBox();
            this.commentCheckBox = new CheckBox();
            this.tableDataGridView = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)this.tableDataGridView).BeginInit();
            this.SuspendLayout();
            // 
            // generateButton
            // 
            this.generateButton.Location = new Point(260, 95);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new Size(75, 23);
            this.generateButton.TabIndex = 0;
            this.generateButton.Text = "generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += this.generateButton_Click;
            // 
            // tableNameComboBox
            // 
            this.tableNameComboBox.FormattingEnabled = true;
            this.tableNameComboBox.Location = new Point(40, 95);
            this.tableNameComboBox.Name = "tableNameComboBox";
            this.tableNameComboBox.Size = new Size(198, 23);
            this.tableNameComboBox.TabIndex = 3;
            this.tableNameComboBox.SelectedIndexChanged += this.tableNameComboBox_SelectedIndexChanged;
            // 
            // attributeCheckBox
            // 
            this.attributeCheckBox.AutoSize = true;
            this.attributeCheckBox.Checked = true;
            this.attributeCheckBox.CheckState = CheckState.Checked;
            this.attributeCheckBox.Location = new Point(40, 33);
            this.attributeCheckBox.Name = "attributeCheckBox";
            this.attributeCheckBox.Size = new Size(50, 19);
            this.attributeCheckBox.TabIndex = 4;
            this.attributeCheckBox.Text = "属性";
            this.attributeCheckBox.UseVisualStyleBackColor = true;
            // 
            // commentCheckBox
            // 
            this.commentCheckBox.AutoSize = true;
            this.commentCheckBox.Checked = true;
            this.commentCheckBox.CheckState = CheckState.Checked;
            this.commentCheckBox.Location = new Point(40, 58);
            this.commentCheckBox.Name = "commentCheckBox";
            this.commentCheckBox.Size = new Size(59, 19);
            this.commentCheckBox.TabIndex = 5;
            this.commentCheckBox.Text = "コメント";
            this.commentCheckBox.UseVisualStyleBackColor = true;
            // 
            // tableDataGridView
            // 
            this.tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableDataGridView.Dock = DockStyle.Bottom;
            this.tableDataGridView.Location = new Point(0, 154);
            this.tableDataGridView.Name = "tableDataGridView";
            this.tableDataGridView.Size = new Size(479, 606);
            this.tableDataGridView.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(479, 760);
            this.Controls.Add(this.tableDataGridView);
            this.Controls.Add(this.commentCheckBox);
            this.Controls.Add(this.attributeCheckBox);
            this.Controls.Add(this.tableNameComboBox);
            this.Controls.Add(this.generateButton);
            this.Name = "MainForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)this.tableDataGridView).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Button generateButton;
        private ComboBox tableNameComboBox;
        private CheckBox attributeCheckBox;
        private CheckBox commentCheckBox;
        private DataGridView tableDataGridView;
    }
}
