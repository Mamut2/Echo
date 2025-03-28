namespace Echo
{
    partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.change_pfp_btn = new System.Windows.Forms.Button();
            this.pfpImage = new System.Windows.Forms.PictureBox();
            this.pickColorBtn = new System.Windows.Forms.Button();
            this.connectbtn = new System.Windows.Forms.Button();
            this.usernametxt = new System.Windows.Forms.TextBox();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.chat_box = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pfpImage)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.change_pfp_btn);
            this.splitContainer1.Panel1.Controls.Add(this.pfpImage);
            this.splitContainer1.Panel1.Controls.Add(this.pickColorBtn);
            this.splitContainer1.Panel1.Controls.Add(this.connectbtn);
            this.splitContainer1.Panel1.Controls.Add(this.usernametxt);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.lstMessages);
            this.splitContainer1.Panel2.Controls.Add(this.chat_box);
            this.splitContainer1.Size = new System.Drawing.Size(1017, 578);
            this.splitContainer1.SplitterDistance = 237;
            this.splitContainer1.TabIndex = 0;
            // 
            // change_pfp_btn
            // 
            this.change_pfp_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_pfp_btn.Location = new System.Drawing.Point(11, 435);
            this.change_pfp_btn.Name = "change_pfp_btn";
            this.change_pfp_btn.Size = new System.Drawing.Size(202, 47);
            this.change_pfp_btn.TabIndex = 4;
            this.change_pfp_btn.Text = "Change pfp";
            this.change_pfp_btn.UseVisualStyleBackColor = true;
            this.change_pfp_btn.Click += new System.EventHandler(this.change_pfp_btn_Click);
            // 
            // pfpImage
            // 
            this.pfpImage.BackColor = System.Drawing.Color.White;
            this.pfpImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pfpImage.Location = new System.Drawing.Point(13, 216);
            this.pfpImage.Name = "pfpImage";
            this.pfpImage.Size = new System.Drawing.Size(200, 200);
            this.pfpImage.TabIndex = 3;
            this.pfpImage.TabStop = false;
            // 
            // pickColorBtn
            // 
            this.pickColorBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickColorBtn.Location = new System.Drawing.Point(13, 143);
            this.pickColorBtn.Name = "pickColorBtn";
            this.pickColorBtn.Size = new System.Drawing.Size(202, 47);
            this.pickColorBtn.TabIndex = 2;
            this.pickColorBtn.Text = "Pick color";
            this.pickColorBtn.UseVisualStyleBackColor = true;
            // 
            // connectbtn
            // 
            this.connectbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectbtn.Location = new System.Drawing.Point(13, 70);
            this.connectbtn.Name = "connectbtn";
            this.connectbtn.Size = new System.Drawing.Size(202, 47);
            this.connectbtn.TabIndex = 1;
            this.connectbtn.Text = "Connect";
            this.connectbtn.UseVisualStyleBackColor = true;
            this.connectbtn.Click += new System.EventHandler(this.connectbtn_Click);
            // 
            // usernametxt
            // 
            this.usernametxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernametxt.Location = new System.Drawing.Point(13, 13);
            this.usernametxt.Name = "usernametxt";
            this.usernametxt.Size = new System.Drawing.Size(202, 38);
            this.usernametxt.TabIndex = 0;
            // 
            // lstMessages
            // 
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.Location = new System.Drawing.Point(30, 20);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(719, 459);
            this.lstMessages.TabIndex = 1;
            // 
            // chat_box
            // 
            this.chat_box.Location = new System.Drawing.Point(30, 499);
            this.chat_box.Multiline = true;
            this.chat_box.Name = "chat_box";
            this.chat_box.Size = new System.Drawing.Size(719, 54);
            this.chat_box.TabIndex = 0;
            this.chat_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chat_box_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 578);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pfpImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox chat_box;
        private System.Windows.Forms.TextBox usernametxt;
        private System.Windows.Forms.Button connectbtn;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button pickColorBtn;
        private System.Windows.Forms.Button change_pfp_btn;
        private System.Windows.Forms.PictureBox pfpImage;
        private System.Windows.Forms.ListBox lstMessages;
    }
}

