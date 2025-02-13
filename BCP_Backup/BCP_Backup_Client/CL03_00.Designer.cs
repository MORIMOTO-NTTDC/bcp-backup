namespace BCP_Backup_Client
{
    partial class CL03_00
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CL03_00));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.状況確認ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.アカウント設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.即時アップロードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.即時ダウンロードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "BCP管理ソフト";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.状況確認ToolStripMenuItem,
            this.アカウント設定ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.即時アップロードToolStripMenuItem,
            this.即時ダウンロードToolStripMenuItem,
            this.toolStripMenuItem2,
            this.終了ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 126);
            // 
            // 状況確認ToolStripMenuItem
            // 
            this.状況確認ToolStripMenuItem.Name = "状況確認ToolStripMenuItem";
            this.状況確認ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.状況確認ToolStripMenuItem.Text = "状況確認";
            this.状況確認ToolStripMenuItem.Click += new System.EventHandler(this.状況確認ToolStripMenuItem_Click);
            // 
            // アカウント設定ToolStripMenuItem
            // 
            this.アカウント設定ToolStripMenuItem.Name = "アカウント設定ToolStripMenuItem";
            this.アカウント設定ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.アカウント設定ToolStripMenuItem.Text = "アカウント設定";
            this.アカウント設定ToolStripMenuItem.Click += new System.EventHandler(this.アカウント設定ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
            // 
            // 即時アップロードToolStripMenuItem
            // 
            this.即時アップロードToolStripMenuItem.Name = "即時アップロードToolStripMenuItem";
            this.即時アップロードToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.即時アップロードToolStripMenuItem.Text = "即時アップロード";
            this.即時アップロードToolStripMenuItem.Click += new System.EventHandler(this.即時アップロードToolStripMenuItem_Click);
            // 
            // 即時ダウンロードToolStripMenuItem
            // 
            this.即時ダウンロードToolStripMenuItem.Name = "即時ダウンロードToolStripMenuItem";
            this.即時ダウンロードToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.即時ダウンロードToolStripMenuItem.Text = "即時ダウンロード";
            this.即時ダウンロードToolStripMenuItem.Click += new System.EventHandler(this.即時ダウンロードToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 6);
            // 
            // 終了ToolStripMenuItem
            // 
            this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
            this.終了ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.終了ToolStripMenuItem.Text = "終了";
            this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem_Click);
            // 
            // CL03_00
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(134, 40);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CL03_00";
            this.Text = "CL03_00";
            this.Load += new System.EventHandler(this.CL03_00_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 状況確認ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem アカウント設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 即時アップロードToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 即時ダウンロードToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
    }
}