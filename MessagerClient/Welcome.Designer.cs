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
            this.Encoding = new System.Windows.Forms.ComboBox();
            this.Compression = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Noise = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // UsernameBox
            // 
            this.UsernameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.UsernameBox.Location = new System.Drawing.Point(35, 48);
            this.UsernameBox.Name = "UsernameBox";
            this.UsernameBox.Size = new System.Drawing.Size(302, 30);
            this.UsernameBox.TabIndex = 0;
            this.UsernameBox.Text = "username";
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
            this.EnterName.Location = new System.Drawing.Point(54, 452);
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
            this.ipBox.Size = new System.Drawing.Size(302, 30);
            this.ipBox.TabIndex = 3;
            this.ipBox.Text = "vilunov.me";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label1.Location = new System.Drawing.Point(29, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enter room ip-address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Encoding
            // 
            this.Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Encoding.FormattingEnabled = true;
            this.Encoding.Location = new System.Drawing.Point(35, 222);
            this.Encoding.Name = "Encoding";
            this.Encoding.Size = new System.Drawing.Size(302, 28);
            this.Encoding.TabIndex = 5;
            // 
            // Compression
            // 
            this.Compression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Compression.FormattingEnabled = true;
            this.Compression.Location = new System.Drawing.Point(35, 308);
            this.Compression.Name = "Compression";
            this.Compression.Size = new System.Drawing.Size(302, 28);
            this.Compression.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label2.Location = new System.Drawing.Point(29, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(269, 26);
            this.label2.TabIndex = 7;
            this.label2.Text = "Select Encoding Algorithm";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label3.Location = new System.Drawing.Point(30, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(307, 26);
            this.label3.TabIndex = 8;
            this.label3.Text = "Select Compression Algorithm";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.label4.Location = new System.Drawing.Point(29, 357);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(205, 26);
            this.label4.TabIndex = 9;
            this.label4.Text = "Precentage of noise";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Noise
            // 
            this.Noise.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Noise.Location = new System.Drawing.Point(35, 401);
            this.Noise.Name = "Noise";
            this.Noise.Size = new System.Drawing.Size(302, 30);
            this.Noise.TabIndex = 10;
            this.Noise.Text = "0.5";
            // 
            // Welcome
            // 
            this.AcceptButton = this.EnterName;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 502);
            this.Controls.Add(this.Noise);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Compression);
            this.Controls.Add(this.Encoding);
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
        public System.Windows.Forms.ComboBox Encoding;
        public System.Windows.Forms.ComboBox Compression;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Noise;
    }
}