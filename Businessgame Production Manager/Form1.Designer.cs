namespace WindowsFormsApplication1
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
            this.components = new System.ComponentModel.Container();
            this.templateLBL = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.loadingTimer = new System.Windows.Forms.Timer(this.components);
            this.wBrowser = new System.Windows.Forms.WebBrowser();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxProductionManager = new System.Windows.Forms.CheckBox();
            this.cbxWasteManager = new System.Windows.Forms.CheckBox();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.edtUsername = new System.Windows.Forms.TextBox();
            this.edtPassword = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.button8 = new System.Windows.Forms.Button();
            this.chkBxChainAutoBuy = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // templateLBL
            // 
            this.templateLBL.AutoSize = true;
            this.templateLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.templateLBL.Location = new System.Drawing.Point(614, 74);
            this.templateLBL.Name = "templateLBL";
            this.templateLBL.Size = new System.Drawing.Size(51, 16);
            this.templateLBL.TabIndex = 2;
            this.templateLBL.Text = "label1";
            this.templateLBL.Visible = false;
            this.templateLBL.Click += new System.EventHandler(this.label1_Click);
            this.templateLBL.MouseHover += new System.EventHandler(this.templateLBL_MouseHover);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(727, 8);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 26);
            this.button3.TabIndex = 7;
            this.button3.Text = "Enter Buy Mode";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(727, 40);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 22);
            this.button4.TabIndex = 8;
            this.button4.Text = "Clear Purchases";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(779, 637);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(56, 22);
            this.button5.TabIndex = 9;
            this.button5.Text = "About";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(701, 636);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Instructions";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(584, 637);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(111, 23);
            this.button6.TabIndex = 11;
            this.button6.Text = "Load Private Data";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(466, 637);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 13;
            this.button7.Text = "Zero Units";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // loadingTimer
            // 
            this.loadingTimer.Interval = 500;
            this.loadingTimer.Tick += new System.EventHandler(this.loadingTimer_Tick);
            // 
            // wBrowser
            // 
            this.wBrowser.Location = new System.Drawing.Point(42, 159);
            this.wBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wBrowser.Name = "wBrowser";
            this.wBrowser.ScriptErrorsSuppressed = true;
            this.wBrowser.Size = new System.Drawing.Size(793, 384);
            this.wBrowser.TabIndex = 14;
            this.wBrowser.Url = new System.Uri("http://www.businessgame.be", System.UriKind.Absolute);
            this.wBrowser.Visible = false;
            this.wBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wBrowser_DocumentCompleted);
            this.wBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.wBrowser_Navigating);
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.progressBar1.Location = new System.Drawing.Point(170, 8);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(525, 23);
            this.progressBar1.TabIndex = 15;
            this.progressBar1.Value = 38;
            this.progressBar1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "(1/3) Buying 100 Generators, 3000 Turbines, 2000 Pipes,  60 Machines";
            this.label1.Visible = false;
            // 
            // cbxProductionManager
            // 
            this.cbxProductionManager.AutoSize = true;
            this.cbxProductionManager.Enabled = false;
            this.cbxProductionManager.Location = new System.Drawing.Point(713, 613);
            this.cbxProductionManager.Name = "cbxProductionManager";
            this.cbxProductionManager.Size = new System.Drawing.Size(122, 17);
            this.cbxProductionManager.TabIndex = 17;
            this.cbxProductionManager.Text = "Production Manager";
            this.cbxProductionManager.UseVisualStyleBackColor = true;
            this.cbxProductionManager.Visible = false;
            this.cbxProductionManager.CheckedChanged += new System.EventHandler(this.cbxProductionManager_CheckedChanged);
            this.cbxProductionManager.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbxProductionManager_MouseClick);
            // 
            // cbxWasteManager
            // 
            this.cbxWasteManager.AutoSize = true;
            this.cbxWasteManager.Enabled = false;
            this.cbxWasteManager.Location = new System.Drawing.Point(713, 590);
            this.cbxWasteManager.Name = "cbxWasteManager";
            this.cbxWasteManager.Size = new System.Drawing.Size(102, 17);
            this.cbxWasteManager.TabIndex = 18;
            this.cbxWasteManager.Text = "Waste Manager";
            this.cbxWasteManager.UseVisualStyleBackColor = true;
            this.cbxWasteManager.Visible = false;
            this.cbxWasteManager.CheckedChanged += new System.EventHandler(this.cbxWasteManager_CheckedChanged);
            this.cbxWasteManager.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cbxWasteManager_MouseClick);
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(555, 159);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(100, 23);
            this.btnLogIn.TabIndex = 19;
            this.btnLogIn.Text = "Log In";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Visible = false;
            // 
            // edtUsername
            // 
            this.edtUsername.Location = new System.Drawing.Point(555, 93);
            this.edtUsername.Name = "edtUsername";
            this.edtUsername.Size = new System.Drawing.Size(100, 20);
            this.edtUsername.TabIndex = 20;
            this.edtUsername.Visible = false;
            // 
            // edtPassword
            // 
            this.edtPassword.Location = new System.Drawing.Point(555, 133);
            this.edtPassword.Name = "edtPassword";
            this.edtPassword.PasswordChar = '*';
            this.edtPassword.Size = new System.Drawing.Size(100, 20);
            this.edtPassword.TabIndex = 21;
            this.edtPassword.Visible = false;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(554, 77);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(55, 13);
            this.lblUsername.TabIndex = 22;
            this.lblUsername.Text = "Username";
            this.lblUsername.Visible = false;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(554, 116);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 23;
            this.lblPassword.Text = "Password";
            this.lblPassword.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(336, 641);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(124, 17);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "Display WebBrowser";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(167, 571);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(460, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "NOTE: If you have not done so already, you will need to open the webbrowser (see " +
    "bottom right)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(215, 590);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(366, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "and log in to businessgame to experience this full functionalty of this program";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.LargeChange = 20;
            this.vScrollBar1.Location = new System.Drawing.Point(4, 19);
            this.vScrollBar1.Maximum = 300;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(14, 636);
            this.vScrollBar1.TabIndex = 27;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(547, 636);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(34, 23);
            this.button8.TabIndex = 29;
            this.button8.Text = "???";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // chkBxChainAutoBuy
            // 
            this.chkBxChainAutoBuy.AutoSize = true;
            this.chkBxChainAutoBuy.Location = new System.Drawing.Point(230, 641);
            this.chkBxChainAutoBuy.Name = "chkBxChainAutoBuy";
            this.chkBxChainAutoBuy.Size = new System.Drawing.Size(100, 17);
            this.chkBxChainAutoBuy.TabIndex = 30;
            this.chkBxChainAutoBuy.Text = "Chain Autobuys";
            this.chkBxChainAutoBuy.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(847, 664);
            this.Controls.Add(this.chkBxChainAutoBuy);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.edtPassword);
            this.Controls.Add(this.edtUsername);
            this.Controls.Add(this.btnLogIn);
            this.Controls.Add(this.cbxWasteManager);
            this.Controls.Add(this.cbxProductionManager);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.wBrowser);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.templateLBL);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Businessgame.be - Production Manager - Code Red";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label templateLBL;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer loadingTimer;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.WebBrowser wBrowser;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cbxProductionManager;
        private System.Windows.Forms.CheckBox cbxWasteManager;
        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.TextBox edtUsername;
        private System.Windows.Forms.TextBox edtPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.CheckBox chkBxChainAutoBuy;
    }
}

