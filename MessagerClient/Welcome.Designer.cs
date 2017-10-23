namespace MessagerClient
{
    partial class Welcome
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.EnterName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBox.Location = new System.Drawing.Point(35, 73);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(188, 30);
            this.textBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label.Location = new System.Drawing.Point(50, 30);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(215, 26);
            this.label.TabIndex = 1;
            this.label.Text = "Enter your username";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EnterName
            // 
            this.EnterName.Location = new System.Drawing.Point(243, 71);
            this.EnterName.Name = "EnterName";
            this.EnterName.Size = new System.Drawing.Size(39, 38);
            this.EnterName.TabIndex = 2;
            this.EnterName.Text = ">";
            this.EnterName.UseVisualStyleBackColor = true;
            this.EnterName.Click += new System.EventHandler(this.EnterName_Click);
            // 
            // Welcome
            // 
            this.AcceptButton = this.EnterName;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 123);
            this.Controls.Add(this.EnterName);
            this.Controls.Add(this.label);
            this.Controls.Add(this.textBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(100, 1000);
            this.Name = "Welcome";
            this.Text = "Welcome";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button EnterName;
    }
}