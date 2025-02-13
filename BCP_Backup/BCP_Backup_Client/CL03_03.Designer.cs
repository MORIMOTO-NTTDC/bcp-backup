namespace BCP_Backup_Client
{
    partial class CL03_03
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CL03_03));
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.lblErrorInfo = new System.Windows.Forms.Label();
            this.lblLastDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatusNG = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblUsage = new System.Windows.Forms.Label();
            this.prgBarRate = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.lblUploadEnable = new System.Windows.Forms.Label();
            this.lblUploadTiming = new System.Windows.Forms.Label();
            this.lblLocalDir = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSoftVersion = new System.Windows.Forms.Label();
            this.picUploadTiming = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblAccountId = new System.Windows.Forms.Label();
            this.lblStatusOK = new System.Windows.Forms.Label();
            this.pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUploadTiming)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(476, 477);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(79, 44);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "閉じる";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Font = new System.Drawing.Font("メイリオ", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(547, 41);
            this.lblTitle.TabIndex = 42;
            this.lblTitle.Text = "状況確認　　申込番号：";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.DarkGray;
            this.pnlInfo.Controls.Add(this.lblStatusOK);
            this.pnlInfo.Controls.Add(this.lblErrorInfo);
            this.pnlInfo.Controls.Add(this.lblLastDate);
            this.pnlInfo.Controls.Add(this.label1);
            this.pnlInfo.Controls.Add(this.lblStatusNG);
            this.pnlInfo.Location = new System.Drawing.Point(9, 50);
            this.pnlInfo.Margin = new System.Windows.Forms.Padding(2);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(548, 210);
            this.pnlInfo.TabIndex = 45;
            // 
            // lblErrorInfo
            // 
            this.lblErrorInfo.AutoEllipsis = true;
            this.lblErrorInfo.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblErrorInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.lblErrorInfo.Location = new System.Drawing.Point(17, 82);
            this.lblErrorInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblErrorInfo.Name = "lblErrorInfo";
            this.lblErrorInfo.Size = new System.Drawing.Size(509, 71);
            this.lblErrorInfo.TabIndex = 50;
            this.lblErrorInfo.Text = "s3error:1234567890123456789012345678901234567890123456789012345678901234567890123" +
    "45678901234567890123456789012345678901234567890123456789012345678901234567890123" +
    "4567890123456789012345678901234567890";
            // 
            // lblLastDate
            // 
            this.lblLastDate.AutoSize = true;
            this.lblLastDate.Font = new System.Drawing.Font("メイリオ", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLastDate.ForeColor = System.Drawing.SystemColors.Info;
            this.lblLastDate.Location = new System.Drawing.Point(201, 166);
            this.lblLastDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLastDate.Name = "lblLastDate";
            this.lblLastDate.Size = new System.Drawing.Size(238, 28);
            this.lblLastDate.TabIndex = 49;
            this.lblLastDate.Text = "2024/06/01 14:44:30";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(110, 166);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 28);
            this.label1.TabIndex = 48;
            this.label1.Text = "同期日時：";
            // 
            // lblStatusNG
            // 
            this.lblStatusNG.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatusNG.Font = new System.Drawing.Font("メイリオ", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatusNG.ForeColor = System.Drawing.SystemColors.Info;
            this.lblStatusNG.Location = new System.Drawing.Point(21, 10);
            this.lblStatusNG.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatusNG.Name = "lblStatusNG";
            this.lblStatusNG.Size = new System.Drawing.Size(504, 63);
            this.lblStatusNG.TabIndex = 47;
            this.lblStatusNG.Text = "アップロード成功";
            this.lblStatusNG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRate
            // 
            this.lblRate.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblRate.Location = new System.Drawing.Point(18, 273);
            this.lblRate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(146, 36);
            this.lblRate.TabIndex = 49;
            this.lblRate.Text = "120%使用";
            this.lblRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUsage
            // 
            this.lblUsage.AutoSize = true;
            this.lblUsage.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblUsage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUsage.Location = new System.Drawing.Point(168, 273);
            this.lblUsage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUsage.Name = "lblUsage";
            this.lblUsage.Size = new System.Drawing.Size(153, 36);
            this.lblUsage.TabIndex = 49;
            this.lblUsage.Text = "2GB / 10GB";
            this.lblUsage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // prgBarRate
            // 
            this.prgBarRate.Location = new System.Drawing.Point(19, 311);
            this.prgBarRate.Margin = new System.Windows.Forms.Padding(2);
            this.prgBarRate.Name = "prgBarRate";
            this.prgBarRate.Size = new System.Drawing.Size(519, 18);
            this.prgBarRate.Step = 1;
            this.prgBarRate.TabIndex = 50;
            this.prgBarRate.Value = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("メイリオ", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(19, 345);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 28);
            this.label4.TabIndex = 51;
            this.label4.Text = "アップロード：";
            // 
            // lblUploadEnable
            // 
            this.lblUploadEnable.AutoSize = true;
            this.lblUploadEnable.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblUploadEnable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUploadEnable.Location = new System.Drawing.Point(156, 344);
            this.lblUploadEnable.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUploadEnable.Name = "lblUploadEnable";
            this.lblUploadEnable.Size = new System.Drawing.Size(51, 28);
            this.lblUploadEnable.TabIndex = 52;
            this.lblUploadEnable.Text = "OFF";
            // 
            // lblUploadTiming
            // 
            this.lblUploadTiming.AutoSize = true;
            this.lblUploadTiming.Font = new System.Drawing.Font("メイリオ", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblUploadTiming.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblUploadTiming.Location = new System.Drawing.Point(269, 345);
            this.lblUploadTiming.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUploadTiming.Name = "lblUploadTiming";
            this.lblUploadTiming.Size = new System.Drawing.Size(55, 28);
            this.lblUploadTiming.TabIndex = 53;
            this.lblUploadTiming.Text = "30分";
            // 
            // lblLocalDir
            // 
            this.lblLocalDir.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLocalDir.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLocalDir.Location = new System.Drawing.Point(19, 407);
            this.lblLocalDir.Name = "lblLocalDir";
            this.lblLocalDir.Size = new System.Drawing.Size(519, 58);
            this.lblLocalDir.TabIndex = 56;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(20, 383);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 24);
            this.label2.TabIndex = 55;
            this.label2.Text = "同期元ディレクトリ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(15, 497);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 24);
            this.label3.TabIndex = 57;
            this.label3.Text = "Version：";
            // 
            // lblSoftVersion
            // 
            this.lblSoftVersion.AutoSize = true;
            this.lblSoftVersion.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblSoftVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSoftVersion.Location = new System.Drawing.Point(88, 497);
            this.lblSoftVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSoftVersion.Name = "lblSoftVersion";
            this.lblSoftVersion.Size = new System.Drawing.Size(36, 24);
            this.lblSoftVersion.TabIndex = 58;
            this.lblSoftVersion.Text = "1.0";
            // 
            // picUploadTiming
            // 
            this.picUploadTiming.Image = global::BCP_Backup_Client.Properties.Resources.repeat;
            this.picUploadTiming.InitialImage = null;
            this.picUploadTiming.Location = new System.Drawing.Point(233, 345);
            this.picUploadTiming.Margin = new System.Windows.Forms.Padding(2);
            this.picUploadTiming.Name = "picUploadTiming";
            this.picUploadTiming.Size = new System.Drawing.Size(32, 24);
            this.picUploadTiming.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUploadTiming.TabIndex = 54;
            this.picUploadTiming.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 529);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(566, 29);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 59;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("メイリオ", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 24);
            // 
            // lblAccountId
            // 
            this.lblAccountId.AutoSize = true;
            this.lblAccountId.BackColor = System.Drawing.Color.DarkBlue;
            this.lblAccountId.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblAccountId.ForeColor = System.Drawing.Color.White;
            this.lblAccountId.Location = new System.Drawing.Point(306, 7);
            this.lblAccountId.Name = "lblAccountId";
            this.lblAccountId.Size = new System.Drawing.Size(188, 41);
            this.lblAccountId.TabIndex = 60;
            this.lblAccountId.Text = "1234567890";
            this.lblAccountId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatusOK
            // 
            this.lblStatusOK.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatusOK.Font = new System.Drawing.Font("メイリオ", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblStatusOK.ForeColor = System.Drawing.SystemColors.Info;
            this.lblStatusOK.Location = new System.Drawing.Point(22, 74);
            this.lblStatusOK.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatusOK.Name = "lblStatusOK";
            this.lblStatusOK.Size = new System.Drawing.Size(504, 63);
            this.lblStatusOK.TabIndex = 51;
            this.lblStatusOK.Text = "アップロード成功";
            this.lblStatusOK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CL03_03
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 558);
            this.Controls.Add(this.lblAccountId);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lblSoftVersion);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLocalDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picUploadTiming);
            this.Controls.Add(this.lblUploadTiming);
            this.Controls.Add(this.lblUploadEnable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.prgBarRate);
            this.Controls.Add(this.lblUsage);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CL03_03";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BCP Backup Service";
            this.Load += new System.EventHandler(this.CL03_03_Load);
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUploadTiming)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Label lblLastDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatusNG;
        private System.Windows.Forms.Label lblErrorInfo;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblUsage;
        private System.Windows.Forms.ProgressBar prgBarRate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblUploadEnable;
        private System.Windows.Forms.Label lblUploadTiming;
        private System.Windows.Forms.PictureBox picUploadTiming;
        private System.Windows.Forms.Label lblLocalDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSoftVersion;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblAccountId;
        private System.Windows.Forms.Label lblStatusOK;
    }
}