namespace Server
{
    partial class Server1Form
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
            this.lsvMessage = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectServer2Btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lsvMessage
            // 
            this.lsvMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lsvMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsvMessage.Location = new System.Drawing.Point(33, 38);
            this.lsvMessage.Name = "lsvMessage";
            this.lsvMessage.Size = new System.Drawing.Size(831, 389);
            this.lsvMessage.TabIndex = 3;
            this.lsvMessage.UseCompatibleStateImageBehavior = false;
            this.lsvMessage.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 809;
            // 
            // connectServer2Btn
            // 
            this.connectServer2Btn.Location = new System.Drawing.Point(676, 9);
            this.connectServer2Btn.Name = "connectServer2Btn";
            this.connectServer2Btn.Size = new System.Drawing.Size(134, 23);
            this.connectServer2Btn.TabIndex = 4;
            this.connectServer2Btn.Text = "Connect Server 2";
            this.connectServer2Btn.UseVisualStyleBackColor = true;
            this.connectServer2Btn.Click += new System.EventHandler(this.connectServer2Btn_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 439);
            this.Controls.Add(this.connectServer2Btn);
            this.Controls.Add(this.lsvMessage);
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ServerForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lsvMessage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button connectServer2Btn;
    }
}

