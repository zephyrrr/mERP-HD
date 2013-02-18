namespace Hd.Utils
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExportYwsj = new System.Windows.Forms.Button();
            this.btnImportYwsj = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnDoWork = new System.Windows.Forms.Button();
            this.btnUploadCx = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExportYwsj
            // 
            this.btnExportYwsj.Location = new System.Drawing.Point(29, 22);
            this.btnExportYwsj.Name = "btnExportYwsj";
            this.btnExportYwsj.Size = new System.Drawing.Size(108, 23);
            this.btnExportYwsj.TabIndex = 0;
            this.btnExportYwsj.Text = "导出网站数据";
            this.btnExportYwsj.UseVisualStyleBackColor = true;
            this.btnExportYwsj.Click += new System.EventHandler(this.btnExportYwsj_Click);
            // 
            // btnImportYwsj
            // 
            this.btnImportYwsj.Location = new System.Drawing.Point(29, 62);
            this.btnImportYwsj.Name = "btnImportYwsj";
            this.btnImportYwsj.Size = new System.Drawing.Size(108, 23);
            this.btnImportYwsj.TabIndex = 1;
            this.btnImportYwsj.Text = "导入网站数据";
            this.btnImportYwsj.UseVisualStyleBackColor = true;
            this.btnImportYwsj.Click += new System.EventHandler(this.btnImportYwsj_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(169, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(242, 107);
            this.textBox1.TabIndex = 2;
            this.textBox1.WordWrap = false;
            // 
            // btnDoWork
            // 
            this.btnDoWork.Location = new System.Drawing.Point(40, 95);
            this.btnDoWork.Name = "btnDoWork";
            this.btnDoWork.Size = new System.Drawing.Size(75, 23);
            this.btnDoWork.TabIndex = 3;
            this.btnDoWork.Text = "DoWork";
            this.btnDoWork.UseVisualStyleBackColor = true;
            this.btnDoWork.Click += new System.EventHandler(this.btnDoWork_Click);
            // 
            // btnUploadCx
            // 
            this.btnUploadCx.Location = new System.Drawing.Point(29, 135);
            this.btnUploadCx.Name = "btnUploadCx";
            this.btnUploadCx.Size = new System.Drawing.Size(108, 23);
            this.btnUploadCx.TabIndex = 4;
            this.btnUploadCx.Text = "上传网站数据";
            this.btnUploadCx.UseVisualStyleBackColor = true;
            this.btnUploadCx.Click += new System.EventHandler(this.btnUploadCx_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 188);
            this.Controls.Add(this.btnUploadCx);
            this.Controls.Add(this.btnDoWork);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnImportYwsj);
            this.Controls.Add(this.btnExportYwsj);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExportYwsj;
        private System.Windows.Forms.Button btnImportYwsj;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnDoWork;
        private System.Windows.Forms.Button btnUploadCx;
    }
}

