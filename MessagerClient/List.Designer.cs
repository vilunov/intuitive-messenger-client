namespace MessagerClient
{
    partial class List
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
            this.Files = new System.Windows.Forms.ListBox();
            this.Download = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Files
            // 
            this.Files.AccessibleName = "Files";
            this.Files.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
            this.Files.FormattingEnabled = true;
            this.Files.ItemHeight = 20;
            this.Files.Location = new System.Drawing.Point(-4, -1);
            this.Files.Name = "Files";
            this.Files.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Files.Size = new System.Drawing.Size(262, 284);
            this.Files.TabIndex = 0;
            // 
            // Download
            // 
            this.Download.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Download.Location = new System.Drawing.Point(12, 293);
            this.Download.Name = "Download";
            this.Download.Size = new System.Drawing.Size(108, 58);
            this.Download.TabIndex = 1;
            this.Download.Text = "Download";
            this.Download.UseVisualStyleBackColor = false;
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // Exit
            // 
            this.Exit.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.Exit.Location = new System.Drawing.Point(126, 293);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(116, 58);
            this.Exit.TabIndex = 2;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = false;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // List
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 363);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.Download);
            this.Controls.Add(this.Files);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "List";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "List of files";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox Files;
        private System.Windows.Forms.Button Download;
        private System.Windows.Forms.Button Exit;
    }
}