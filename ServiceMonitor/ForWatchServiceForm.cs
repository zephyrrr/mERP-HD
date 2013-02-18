using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ServiceMonitor
{
    /// <summary>
    /// 附件监视程序。为了解决Hd服务过多，原服务控制台输出凌乱，无法查看附件上传具体情况。
    /// </summary>
    public partial class ForWatchServiceForm : Form
    {
        public ForWatchServiceForm()
        {
            InitializeComponent();
        }

        private void ForWatchServiceForm_Load(object sender, EventArgs e)
        {
            // 创建默认监视
            foreach (KeyValuePair<string, string> item in Global.DefaultMonitors)
            {
                AddForWatchControl(item.Key, item.Value);
            }

            Feng.ProgramHelper.InitProgram();
            SetThreadCount();
        }

        /// <summary>
        /// 增加一个新监视
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            string monitorName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(monitorName))
            {
                MessageBox.Show("请先输入监控名！");
                return;
            }
            if (Global.CurrentMonitors.ContainsKey(monitorName))
            {
                MessageBox.Show("\"" + monitorName + "\"已存在！");
                return;
            }
            AddForWatchControl(monitorName);
        }

        /// <summary>
        /// 增加ForWatchControl
        /// </summary>
        /// <param name="monitorName">监视名称</param>
        private void AddForWatchControl(string monitorName)
        {
            AddForWatchControl(monitorName, "");
        }

        /// <summary>
        /// 增加ForWatchControl
        /// </summary>
        /// <param name="monitorName">监视名称</param>
        /// <param name="path">监视路径</param>
        private void AddForWatchControl(string monitorName, string path)
        {
            ForWatchControl fwc = new ForWatchControl(monitorName, path);
            fwc.SetThreadCount += new ForWatchControl.SetThreadCountEventHandler(SetThreadCount);
            flowLayoutPanel1.Controls.Add(fwc);
            Global.CurrentMonitors.Add(monitorName, path);
        }

        /// <summary>
        /// 界面刷新当前线程数
        /// </summary>
        private void SetThreadCount()
        {
            lblThreadCount.Text = "线程数：" + System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
        }

        /// <summary>
        /// 最小化到托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForWatchServiceForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Text = "Service Monitor" + Environment.NewLine + "线程数：" + System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            显示主窗体ToolStripMenuItem_Click(sender, e);
        }

        private void lblThreadCount_Click(object sender, EventArgs e)
        {
            SetThreadCount();
        }

        private void ForWatchServiceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskExit()) e.Cancel = true;
        }

        private void 显示主窗体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
            this.Activate();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AskExit()) Application.Exit();
        }

        private bool AskExit()
        {
            return MessageBox.Show("确定要退出吗？", "Service Monitor", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes;
        }
    }
}
