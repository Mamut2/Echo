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
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.messagingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.youtubePlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ytTabPage = new System.Windows.Forms.TabPage();
            this.lstYtSearchResults = new System.Windows.Forms.ListBox();
            this.btnSearchYt = new System.Windows.Forms.Button();
            this.searchYtTextBox = new System.Windows.Forms.TextBox();
            this.messagingTabPage = new System.Windows.Forms.TabPage();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.chat_box = new System.Windows.Forms.TextBox();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.loginTabPage = new System.Windows.Forms.TabPage();
            this.ipInput = new System.Windows.Forms.TextBox();
            this.change_pfp_btn = new System.Windows.Forms.Button();
            this.pfpImage = new System.Windows.Forms.PictureBox();
            this.pickColorBtn = new System.Windows.Forms.Button();
            this.connectbtn = new System.Windows.Forms.Button();
            this.usernametxt = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.ytTabPage.SuspendLayout();
            this.messagingTabPage.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.loginTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pfpImage)).BeginInit();
            this.SuspendLayout();
            // 
            // messagingToolStripMenuItem
            // 
            this.messagingToolStripMenuItem.ForeColor = System.Drawing.Color.LawnGreen;
            this.messagingToolStripMenuItem.Name = "messagingToolStripMenuItem";
            this.messagingToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.messagingToolStripMenuItem.Text = "Messaging";
            this.messagingToolStripMenuItem.Click += new System.EventHandler(this.messagingToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.messagingToolStripMenuItem,
            this.youtubePlayerToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(1229, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.ForeColor = System.Drawing.Color.LawnGreen;
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // youtubePlayerToolStripMenuItem
            // 
            this.youtubePlayerToolStripMenuItem.ForeColor = System.Drawing.Color.LawnGreen;
            this.youtubePlayerToolStripMenuItem.Name = "youtubePlayerToolStripMenuItem";
            this.youtubePlayerToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.youtubePlayerToolStripMenuItem.Text = "Youtube Player";
            this.youtubePlayerToolStripMenuItem.Click += new System.EventHandler(this.youtubePlayerToolStripMenuItem_Click);
            // 
            // ytTabPage
            // 
            this.ytTabPage.Controls.Add(this.lstYtSearchResults);
            this.ytTabPage.Controls.Add(this.btnSearchYt);
            this.ytTabPage.Controls.Add(this.searchYtTextBox);
            this.ytTabPage.Location = new System.Drawing.Point(4, 22);
            this.ytTabPage.Name = "ytTabPage";
            this.ytTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.ytTabPage.Size = new System.Drawing.Size(1218, 650);
            this.ytTabPage.TabIndex = 1;
            this.ytTabPage.Text = "ytTabPage";
            this.ytTabPage.UseVisualStyleBackColor = true;
            // 
            // lstYtSearchResults
            // 
            this.lstYtSearchResults.FormattingEnabled = true;
            this.lstYtSearchResults.Location = new System.Drawing.Point(16, 63);
            this.lstYtSearchResults.Name = "lstYtSearchResults";
            this.lstYtSearchResults.Size = new System.Drawing.Size(1180, 563);
            this.lstYtSearchResults.TabIndex = 2;
            this.lstYtSearchResults.SelectedIndexChanged += new System.EventHandler(this.lstYtSearchResults_SelectedIndexChanged);
            // 
            // btnSearchYt
            // 
            this.btnSearchYt.Location = new System.Drawing.Point(1117, 14);
            this.btnSearchYt.Name = "btnSearchYt";
            this.btnSearchYt.Size = new System.Drawing.Size(79, 30);
            this.btnSearchYt.TabIndex = 1;
            this.btnSearchYt.Text = "Search";
            this.btnSearchYt.UseVisualStyleBackColor = true;
            this.btnSearchYt.Click += new System.EventHandler(this.btnSearchYt_Click);
            // 
            // searchYtTextBox
            // 
            this.searchYtTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchYtTextBox.Location = new System.Drawing.Point(16, 11);
            this.searchYtTextBox.Name = "searchYtTextBox";
            this.searchYtTextBox.Size = new System.Drawing.Size(1080, 30);
            this.searchYtTextBox.TabIndex = 0;
            // 
            // messagingTabPage
            // 
            this.messagingTabPage.Controls.Add(this.lstMessages);
            this.messagingTabPage.Controls.Add(this.chat_box);
            this.messagingTabPage.Controls.Add(this.lstUsers);
            this.messagingTabPage.Location = new System.Drawing.Point(4, 22);
            this.messagingTabPage.Name = "messagingTabPage";
            this.messagingTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.messagingTabPage.Size = new System.Drawing.Size(1218, 650);
            this.messagingTabPage.TabIndex = 0;
            this.messagingTabPage.Text = "messagingTabPage";
            this.messagingTabPage.UseVisualStyleBackColor = true;
            // 
            // lstMessages
            // 
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.Location = new System.Drawing.Point(6, 6);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(981, 576);
            this.lstMessages.TabIndex = 1;
            // 
            // chat_box
            // 
            this.chat_box.Location = new System.Drawing.Point(6, 602);
            this.chat_box.Multiline = true;
            this.chat_box.Name = "chat_box";
            this.chat_box.Size = new System.Drawing.Size(981, 42);
            this.chat_box.TabIndex = 0;
            this.chat_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chat_box_KeyDown);
            // 
            // lstUsers
            // 
            this.lstUsers.FormattingEnabled = true;
            this.lstUsers.Location = new System.Drawing.Point(1010, 3);
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.Size = new System.Drawing.Size(202, 641);
            this.lstUsers.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.messagingTabPage);
            this.tabControl.Controls.Add(this.ytTabPage);
            this.tabControl.Controls.Add(this.loginTabPage);
            this.tabControl.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.tabControl.Location = new System.Drawing.Point(3, 30);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1226, 676);
            this.tabControl.TabIndex = 12;
            this.tabControl.TabStop = false;
            // 
            // loginTabPage
            // 
            this.loginTabPage.Controls.Add(this.ipInput);
            this.loginTabPage.Controls.Add(this.change_pfp_btn);
            this.loginTabPage.Controls.Add(this.pfpImage);
            this.loginTabPage.Controls.Add(this.pickColorBtn);
            this.loginTabPage.Controls.Add(this.connectbtn);
            this.loginTabPage.Controls.Add(this.usernametxt);
            this.loginTabPage.Location = new System.Drawing.Point(4, 22);
            this.loginTabPage.Name = "loginTabPage";
            this.loginTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.loginTabPage.Size = new System.Drawing.Size(1218, 650);
            this.loginTabPage.TabIndex = 2;
            this.loginTabPage.Text = "loginTabPage";
            this.loginTabPage.UseVisualStyleBackColor = true;
            // 
            // ipInput
            // 
            this.ipInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ipInput.Location = new System.Drawing.Point(381, 192);
            this.ipInput.Name = "ipInput";
            this.ipInput.Size = new System.Drawing.Size(200, 38);
            this.ipInput.TabIndex = 31;
            this.ipInput.Text = "Server IP";
            // 
            // change_pfp_btn
            // 
            this.change_pfp_btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.change_pfp_btn.Location = new System.Drawing.Point(646, 373);
            this.change_pfp_btn.Name = "change_pfp_btn";
            this.change_pfp_btn.Size = new System.Drawing.Size(200, 47);
            this.change_pfp_btn.TabIndex = 30;
            this.change_pfp_btn.Text = "Change pfp";
            this.change_pfp_btn.UseVisualStyleBackColor = true;
            this.change_pfp_btn.Click += new System.EventHandler(this.change_pfp_btn_Click);
            // 
            // pfpImage
            // 
            this.pfpImage.BackColor = System.Drawing.Color.White;
            this.pfpImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pfpImage.Location = new System.Drawing.Point(646, 116);
            this.pfpImage.Name = "pfpImage";
            this.pfpImage.Size = new System.Drawing.Size(200, 200);
            this.pfpImage.TabIndex = 29;
            this.pfpImage.TabStop = false;
            // 
            // pickColorBtn
            // 
            this.pickColorBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickColorBtn.Location = new System.Drawing.Point(381, 373);
            this.pickColorBtn.Name = "pickColorBtn";
            this.pickColorBtn.Size = new System.Drawing.Size(200, 47);
            this.pickColorBtn.TabIndex = 28;
            this.pickColorBtn.Text = "Pick color";
            this.pickColorBtn.UseVisualStyleBackColor = true;
            // 
            // connectbtn
            // 
            this.connectbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectbtn.Location = new System.Drawing.Point(381, 289);
            this.connectbtn.Name = "connectbtn";
            this.connectbtn.Size = new System.Drawing.Size(200, 47);
            this.connectbtn.TabIndex = 27;
            this.connectbtn.Text = "Connect";
            this.connectbtn.UseVisualStyleBackColor = true;
            this.connectbtn.Click += new System.EventHandler(this.connectbtn_Click);
            // 
            // usernametxt
            // 
            this.usernametxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernametxt.Location = new System.Drawing.Point(381, 116);
            this.usernametxt.Name = "usernametxt";
            this.usernametxt.Size = new System.Drawing.Size(200, 38);
            this.usernametxt.TabIndex = 26;
            this.usernametxt.Text = "Username";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 708);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ytTabPage.ResumeLayout(false);
            this.ytTabPage.PerformLayout();
            this.messagingTabPage.ResumeLayout(false);
            this.messagingTabPage.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.loginTabPage.ResumeLayout(false);
            this.loginTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pfpImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem messagingToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem youtubePlayerToolStripMenuItem;
        private System.Windows.Forms.TabPage ytTabPage;
        private System.Windows.Forms.TabPage messagingTabPage;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.TextBox chat_box;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage loginTabPage;
        private System.Windows.Forms.TextBox ipInput;
        private System.Windows.Forms.Button change_pfp_btn;
        private System.Windows.Forms.PictureBox pfpImage;
        private System.Windows.Forms.Button pickColorBtn;
        private System.Windows.Forms.Button connectbtn;
        private System.Windows.Forms.TextBox usernametxt;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.TextBox searchYtTextBox;
        private System.Windows.Forms.Button btnSearchYt;
        private System.Windows.Forms.ListBox lstYtSearchResults;
    }
}

