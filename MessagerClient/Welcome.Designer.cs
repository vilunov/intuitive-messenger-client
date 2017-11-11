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
            this.UsernameBox = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.EnterName = new System.Windows.Forms.Button();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UsernameBox
            // 
            this.UsernameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.UsernameBox.Location = new System.Drawing.Point(35, 48);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(261, 30);
            this.UsernameBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label.Location = new System.Drawing.Point(30, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(215, 26);
            this.label.TabIndex = 1;
            this.label.Text = "Enter your username";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EnterName
            // 
            this.EnterName.Location = new System.Drawing.Point(35, 182);
            this.EnterName.Name = "EnterName";
            this.EnterName.Size = new System.Drawing.Size(261, 38);
            this.EnterName.TabIndex = 2;
            this.EnterName.Text = "Login";
            this.EnterName.UseVisualStyleBackColor = true;
            this.EnterName.Click += new System.EventHandler(this.EnterName_Click);
            // 
            // ipBox
            // 
            this.ipBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ipBox.Location = new System.Drawing.Point(35, 133);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(261, 30);
            this.ipBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(30, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enter room ip-address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Welcome
            // 
            this.AcceptButton = this.EnterName;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 247);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.EnterName);
            this.Controls.Add(this.label);
            this.Controls.Add(this.UsernameBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(100, 1000);
            this.Name = "Welcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UsernameBox;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button EnterName;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.Label label1;
    }
}