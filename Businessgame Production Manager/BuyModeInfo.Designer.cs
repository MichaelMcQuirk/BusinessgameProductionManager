namespace WindowsFormsApplication1
{
    partial class BuyModeInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuyModeInfo));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblBuyStrategy = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAnnouncement = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.DarkTurquoise;
            this.progressBar1.Location = new System.Drawing.Point(12, 12);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 23);
            this.progressBar1.TabIndex = 16;
            this.progressBar1.Value = 38;
            // 
            // lblBuyStrategy
            // 
            this.lblBuyStrategy.AutoSize = true;
            this.lblBuyStrategy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuyStrategy.Location = new System.Drawing.Point(9, 67);
            this.lblBuyStrategy.Name = "lblBuyStrategy";
            this.lblBuyStrategy.Size = new System.Drawing.Size(205, 210);
            this.lblBuyStrategy.TabIndex = 17;
            this.lblBuyStrategy.Text = resources.GetString("lblBuyStrategy.Text");
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(9, 38);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(461, 15);
            this.lblStatus.TabIndex = 18;
            this.lblStatus.Text = "(1/3) Buying 100 Generators, 3000 Turbines, 2000 Pipes,  60 Machines";
            // 
            // lblAnnouncement
            // 
            this.lblAnnouncement.AutoSize = true;
            this.lblAnnouncement.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAnnouncement.Location = new System.Drawing.Point(59, 291);
            this.lblAnnouncement.Name = "lblAnnouncement";
            this.lblAnnouncement.Size = new System.Drawing.Size(216, 58);
            this.lblAnnouncement.TabIndex = 19;
            this.lblAnnouncement.Text = "ERROR\r\nNot Enough Cash";
            this.lblAnnouncement.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAnnouncement.Visible = false;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 352);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(314, 27);
            this.button1.TabIndex = 20;
            this.button1.Text = "Open Log File";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // BuyModeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gold;
            this.ClientSize = new System.Drawing.Size(338, 380);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblAnnouncement);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblBuyStrategy);
            this.Controls.Add(this.progressBar1);
            this.Name = "BuyModeInfo";
            this.Text = "BuyModeInfo";
            this.Load += new System.EventHandler(this.BuyModeInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.Label lblBuyStrategy;
        public System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.Label lblAnnouncement;
    }
}