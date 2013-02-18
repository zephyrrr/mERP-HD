using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Hd.Service;

namespace ServiceMonitor
{
    /// <summary>
    /// 监视控件。每个监视都会创建一个ForWatchControl，ForWatchControl在新线程中执行Hd.Service.ForWatcher进行监视。
    /// </summary>
    public partial class ForWatchControl : UserControl
    {
        public ForWatchControl()
        {
            InitializeComponent();
        }

        public ForWatchControl(string name)
        {
            InitializeComponent();
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">监视名称</param>
        /// <param name="path">监视路径</param>
        public ForWatchControl(string name, string path)
        {
            InitializeComponent();
            this.Name = name;
            txtMonitorPath.Text = path;
        }

        private void ForWatchControl_Load(object sender, EventArgs e)
        {
            //btnPlayPause.Text = m_play;
            //btnReplayDelete.Text = m_delete;
            groupBox1.Text = this.Name;
        }

        private Thread m_thread;// 监视线程
        private string m_play = "〉";
        private string m_pause = "‖";
        private string m_replay = "■";
        private string m_delete = "×";

        /// <summary>
        /// 选择监视路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMonitorPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtMonitorPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        
        /// <summary>
        /// 开始、暂停监视
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            string path = txtMonitorPath.Text.Trim();
            if (!System.IO.Directory.Exists(path))
            {
                MessageBox.Show("找不到路径!");
                return;
            }

            if (btnPlayPause.Text == m_play)
            {
                // 没有监视线程就创建新监视，有就恢复监视
                if (m_thread == null)
                    Play(path, "Hd.Model.Jk.进口票", "货代自编号");
                else
                    m_thread.Resume();
            }
            else
            {
                m_thread.Suspend();
            }
            btnPlayPause.Text = btnPlayPause.Text == m_play ? m_pause : m_play;
            btnReplayDelete.Text = m_replay;
            txtMonitorPath.ReadOnly = true;
            SetThreadCount();
        }

        /// <summary>
        /// 重启、删除监视
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReplayDelete_Click(object sender, EventArgs e)
        {
            if (btnReplayDelete.Text == m_delete)
            {
                Global.CurrentMonitors.Remove(this.Name);
                this.Dispose();
            }
            else
            {
                try
                {
                    m_thread.Abort();
                }
                catch
                {

                }
                m_thread = null;
            }
            btnReplayDelete.Text = btnReplayDelete.Text == m_replay ? m_delete : m_replay;
            btnPlayPause.Text = m_play;
            txtMonitorPath.ReadOnly = false;
            SetThreadCount();
        }

        /// <summary>
        /// 启动控制台，创建一个新线程和ForWatcher，ForWatcher将会循环检查指定路径并对附件进行操作
        /// </summary>
        /// <param name="path"></param>
        /// <param name="entityName"></param>
        /// <param name="propertyName"></param>
        void Play(string path, string entityName, string propertyName)
        {
            ConsoleHelper.AllocConsole();
            m_thread = new Thread(new ParameterizedThreadStart(ThredStart));
            m_thread.IsBackground = true;
            ForWatcher forWatch = new ForWatcher(path, entityName, propertyName);                        
            m_thread.Start(forWatch);
        }

        void ThredStart(object forWatch)
        {
            ForWatcher fw = forWatch as ForWatcher;
            if (fw != null)
            {
                ConsoleHelper.WriteLineGreen("开始监控: " + fw.MonitorPath);
                while (true)
                {
                    try
                    {
                        DateTime beginTime = DateTime.Now;
                        ConsoleHelper.WriteLineYellow(string.Format("【{0}】Start...", this.Name));
                        fw.Run();// 运行ForWatcher
                        DateTime endTime = DateTime.Now;
                        ConsoleHelper.WriteLineYellow(string.Format("【{0}】End... {1} 至 {2} 用时：{3}", this.Name, beginTime, endTime, endTime - beginTime));
                    }
                    catch (Exception ex)// 输出错误，继续执行
                    {
                        ConsoleHelper.WriteLineRed(string.Format("ForWatcher exception for \"{0}\"：{1}" + Environment.NewLine + "{2}", this.Name, fw.MonitorPath, ex.Message));
                    }
                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// 设置当前线程数委托
        /// </summary>
        public delegate void SetThreadCountEventHandler();
        /// <summary>
        /// 设置当前线程数事件。因为ForWatchControl重启、删除按钮的操作会影响线程数量。
        /// </summary>
        public event SetThreadCountEventHandler SetThreadCount;
    }
}
