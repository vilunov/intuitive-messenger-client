using System.Windows.Forms;

namespace MessagerClient
{
    partial class Client
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
            this.Send = new System.Windows.Forms.Button();
            this.History = new System.Windows.Forms.RichTextBox();
            this.Message = new System.Windows.Forms.RichTextBox();
            this.FilesList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.BackColor = System.Drawing.SystemColors.Highlight;
            this.Send.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Send.Font = new System.Drawing.Font("Franklin Gothic Medium", 16F, System.Drawing.FontStyle.Bold);
            this.Send.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Send.Location = new System.Drawing.Point(689, 338);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(155, 93);
            this.Send.TabIndex = 0;
            this.Send.Text = "Send";
            this.Send.UseVisualStyleBackColor = false;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // History
            // 
            this.History.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.History.Location = new System.Drawing.Point(12, 12);
            this.History.Name = "History";
            this.History.ReadOnly = true;
            this.History.Size = new System.Drawing.Size(991, 308);
            this.History.TabIndex = 2;
            this.History.Text = "";
            // 
            // Message
            // 
            this.Message.BackColor = System.Drawing.SystemColors.Window;
            this.Message.EnableAutoDragDrop = true;
            this.Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.Message.Location = new System.Drawing.Point(13, 338);
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(669, 93);
            this.Message.TabIndex = 3;
            this.Message.Tag = "";
            this.Message.Text = "";
            this.Message.DragDrop += new System.Windows.Forms.DragEventHandler(this.FilesListBox_DragDrop);
            this.Message.DragEnter += new System.Windows.Forms.DragEventHandler(this.FilesListBox_DragEnter);
            // 
            // FilesList
            // 
            this.FilesList.BackColor = System.Drawing.SystemColors.Highlight;
            this.FilesList.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.FilesList.Font = new System.Drawing.Font("Franklin Gothic Medium", 16F, System.Drawing.FontStyle.Bold);
            this.FilesList.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FilesList.Location = new System.Drawing.Point(850, 338);
            this.FilesList.Name = "FilesList";
            this.FilesList.Size = new System.Drawing.Size(155, 93);
            this.FilesList.TabIndex = 4;
            this.FilesList.Text = "FILES";
            this.FilesList.UseVisualStyleBackColor = false;
            this.FilesList.Click += new System.EventHandler(this.FilesList_Click);
            // 
            // Client
            // 
            this.AcceptButton = this.Send;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1017, 453);
            this.Controls.Add(this.FilesList);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.History);
            this.Controls.Add(this.Send);
            this.Name = "Client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.RichTextBox History;
        private System.Windows.Forms.RichTextBox Message;
        private Button FilesList;
    }
}

