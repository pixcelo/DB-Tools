namespace EntityBuilder.Views
{
    partial class DefinitionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.definitionTextBox = new TextBox();
            this.SuspendLayout();
            // 
            // definitionTextBox
            // 
            this.definitionTextBox.Dock = DockStyle.Fill;
            this.definitionTextBox.Location = new Point(0, 0);
            this.definitionTextBox.Multiline = true;
            this.definitionTextBox.Name = "definitionTextBox";
            this.definitionTextBox.ScrollBars = ScrollBars.Both;
            this.definitionTextBox.Size = new Size(800, 618);
            this.definitionTextBox.TabIndex = 0;
            // 
            // DefinitionForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 618);
            this.Controls.Add(this.definitionTextBox);
            this.Name = "DefinitionForm";
            this.Text = "DefinitionForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private TextBox definitionTextBox;
    }
}