namespace BCP_Backup_Client
{
    partial class CL03_01
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CL03_01));
            this.lblTitle = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAccountId = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboxUploadEnable = new System.Windows.Forms.CheckBox();
            this.pnlUploadTiming = new System.Windows.Forms.Panel();
            this.rbUploadTiming3 = new System.Windows.Forms.RadioButton();
            this.rbUploadTiming2 = new System.Windows.Forms.RadioButton();
            this.rbUploadTiming1 = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.textLocalDir = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRegist = new System.Windows.Forms.Button();
            this.btnLocalDir = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlUploadTiming.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Font = new System.Drawing.Font("メイリオ", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(10, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(614, 43);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "アカウント設定変更";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("メイリオ", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(10, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(310, 23);
            this.label4.TabIndex = 13;
            this.label4.Text = "下記の情報を変更し更新を行ってください。";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(13, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 23);
            this.label1.TabIndex = 16;
            this.label1.Text = "申込番号";
            // 
            // lblAccountId
            // 
            this.lblAccountId.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblAccountId.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblAccountId.Location = new System.Drawing.Point(181, 90);
            this.lblAccountId.Name = "lblAccountId";
            this.lblAccountId.Size = new System.Drawing.Size(129, 34);
            this.lblAccountId.TabIndex = 17;
            this.lblAccountId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(13, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 23);
            this.label2.TabIndex = 18;
            this.label2.Text = "アップロード実行";
            // 
            // cboxUploadEnable
            // 
            this.cboxUploadEnable.Appearance = System.Windows.Forms.Appearance.Button;
            this.cboxUploadEnable.BackColor = System.Drawing.Color.Blue;
            this.cboxUploadEnable.Checked = true;
            this.cboxUploadEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxUploadEnable.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cboxUploadEnable.Location = new System.Drawing.Point(181, 135);
            this.cboxUploadEnable.Name = "cboxUploadEnable";
            this.cboxUploadEnable.Size = new System.Drawing.Size(79, 33);
            this.cboxUploadEnable.TabIndex = 1;
            this.cboxUploadEnable.Text = "ON";
            this.cboxUploadEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cboxUploadEnable.UseVisualStyleBackColor = false;
            this.cboxUploadEnable.CheckedChanged += new System.EventHandler(this.CboxUploadEnable_CheckedChanged);
            // 
            // pnlUploadTiming
            // 
            this.pnlUploadTiming.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUploadTiming.Controls.Add(this.rbUploadTiming3);
            this.pnlUploadTiming.Controls.Add(this.rbUploadTiming2);
            this.pnlUploadTiming.Controls.Add(this.rbUploadTiming1);
            this.pnlUploadTiming.Controls.Add(this.label5);
            this.pnlUploadTiming.Location = new System.Drawing.Point(16, 182);
            this.pnlUploadTiming.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlUploadTiming.Name = "pnlUploadTiming";
            this.pnlUploadTiming.Size = new System.Drawing.Size(395, 56);
            this.pnlUploadTiming.TabIndex = 24;
            // 
            // rbUploadTiming3
            // 
            this.rbUploadTiming3.AutoSize = true;
            this.rbUploadTiming3.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rbUploadTiming3.Location = new System.Drawing.Point(325, 14);
            this.rbUploadTiming3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbUploadTiming3.Name = "rbUploadTiming3";
            this.rbUploadTiming3.Size = new System.Drawing.Size(48, 28);
            this.rbUploadTiming3.TabIndex = 4;
            this.rbUploadTiming3.Text = "60";
            this.rbUploadTiming3.UseVisualStyleBackColor = true;
            // 
            // rbUploadTiming2
            // 
            this.rbUploadTiming2.AutoSize = true;
            this.rbUploadTiming2.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rbUploadTiming2.Location = new System.Drawing.Point(262, 14);
            this.rbUploadTiming2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbUploadTiming2.Name = "rbUploadTiming2";
            this.rbUploadTiming2.Size = new System.Drawing.Size(48, 28);
            this.rbUploadTiming2.TabIndex = 3;
            this.rbUploadTiming2.Text = "30";
            this.rbUploadTiming2.UseVisualStyleBackColor = true;
            // 
            // rbUploadTiming1
            // 
            this.rbUploadTiming1.AutoSize = true;
            this.rbUploadTiming1.Checked = true;
            this.rbUploadTiming1.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rbUploadTiming1.Location = new System.Drawing.Point(200, 14);
            this.rbUploadTiming1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbUploadTiming1.Name = "rbUploadTiming1";
            this.rbUploadTiming1.Size = new System.Drawing.Size(48, 28);
            this.rbUploadTiming1.TabIndex = 2;
            this.rbUploadTiming1.TabStop = true;
            this.rbUploadTiming1.Text = "10";
            this.rbUploadTiming1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("メイリオ", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(22, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 23);
            this.label5.TabIndex = 24;
            this.label5.Text = "アップロード頻度（分）";
            // 
            // textLocalDir
            // 
            this.textLocalDir.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textLocalDir.Location = new System.Drawing.Point(16, 270);
            this.textLocalDir.MaxLength = 100;
            this.textLocalDir.Name = "textLocalDir";
            this.textLocalDir.Size = new System.Drawing.Size(554, 31);
            this.textLocalDir.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("メイリオ", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(10, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 23);
            this.label6.TabIndex = 25;
            this.label6.Text = "同期元ディレクトリ";
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(487, 316);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(123, 43);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnRegist
            // 
            this.btnRegist.BackColor = System.Drawing.Color.Green;
            this.btnRegist.Font = new System.Drawing.Font("メイリオ", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnRegist.ForeColor = System.Drawing.Color.White;
            this.btnRegist.Location = new System.Drawing.Point(342, 316);
            this.btnRegist.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRegist.Name = "btnRegist";
            this.btnRegist.Size = new System.Drawing.Size(128, 43);
            this.btnRegist.TabIndex = 7;
            this.btnRegist.Text = "更新";
            this.btnRegist.UseVisualStyleBackColor = false;
            this.btnRegist.Click += new System.EventHandler(this.BtnRegist_Click);
            // 
            // btnLocalDir
            // 
            this.btnLocalDir.Image = global::BCP_Backup_Client.Properties.Resources.folder2;
            this.btnLocalDir.Location = new System.Drawing.Point(572, 270);
            this.btnLocalDir.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnLocalDir.Name = "btnLocalDir";
            this.btnLocalDir.Size = new System.Drawing.Size(51, 31);
            this.btnLocalDir.TabIndex = 6;
            this.btnLocalDir.Text = "…";
            this.btnLocalDir.UseVisualStyleBackColor = true;
            this.btnLocalDir.Click += new System.EventHandler(this.BtnLocalDir_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 372);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(632, 21);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 30;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 16);
            // 
            // CL03_01
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 393);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRegist);
            this.Controls.Add(this.btnLocalDir);
            this.Controls.Add(this.textLocalDir);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pnlUploadTiming);
            this.Controls.Add(this.cboxUploadEnable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblAccountId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CL03_01";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BCP Backup Service";
            this.Load += new System.EventHandler(this.CL03_01_Load);
            this.pnlUploadTiming.ResumeLayout(false);
            this.pnlUploadTiming.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAccountId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cboxUploadEnable;
        private System.Windows.Forms.Panel pnlUploadTiming;
        private System.Windows.Forms.RadioButton rbUploadTiming3;
        private System.Windows.Forms.RadioButton rbUploadTiming2;
        private System.Windows.Forms.RadioButton rbUploadTiming1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnLocalDir;
        private System.Windows.Forms.TextBox textLocalDir;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRegist;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}