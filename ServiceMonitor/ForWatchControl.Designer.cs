namespace ServiceMonitor
{
    partial class ForWatchControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMonitorPath = new System.Windows.Forms.Button();
            this.txtMonitorPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnReplayDelete = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMonitorPath
            // 
            this.btnMonitorPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMonitorPath.Location = new System.Drawing.Point(303, 18);
            this.btnMonitorPath.Name = "btnMonitorPath";
            this.btnMonitorPath.Size = new System.Drawing.Size(30, 23);
            this.btnMonitorPath.TabIndex = 5;
            this.btnMonitorPath.Text = "…";
            this.btnMonitorPath.UseVisualStyleBackColor = true;
            this.btnMonitorPath.Click += new System.EventHandler(this.btnMonitorPath_Click);
            // 
            // txtMonitorPath
            // 
            this.txtMonitorPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMonitorPath.Location = new System.Drawing.Point(65, 20);
            this.txtMonitorPath.Name = "txtMonitorPath";
            this.txtMonitorPath.Size = new System.Drawing.Size(232, 21);
            this.txtMonitorPath.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "监控路径";
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlayPause.Location = new System.Drawing.Point(352, 18);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(25, 23);
            this.btnPlayPause.TabIndex = 6;
            this.btnPlayPause.Text = "〉";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnReplayDelete
            // 
            this.btnReplayDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReplayDelete.Location = new System.Drawing.Point(378, 18);
            this.btnReplayDelete.Name = "btnReplayDelete";
            this.btnReplayDelete.Size = new System.Drawing.Size(25, 23);
            this.btnReplayDelete.TabIndex = 7;
            this.btnReplayDelete.Text = "×";
            this.btnReplayDelete.UseVisualStyleBackColor = true;
            this.btnReplayDelete.Click += new System.EventHandler(this.btnReplayDelete_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnReplayDelete);
            this.groupBox1.Controls.Add(this.txtMonitorPath);
            this.groupBox1.Controls.Add(this.btnPlayPause);
            this.groupBox1.Controls.Add(this.btnMonitorPath);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(409, 52);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // ForWatchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ForWatchControl";
            this.Size = new System.Drawing.Size(415, 59);
            this.Load += new System.EventHandler(this.ForWatchControl_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.Button btnMonitorPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnReplayDelete;
        private System.Windows.Forms.TextBox txtMonitorPath;
        private System.Windows.Forms.GroupBox groupBox1;        
    }
}
