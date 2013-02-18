using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Hd.Service;
using Feng;

namespace AppHelper
{
    /// <summary>
    /// 辅助程序。为了解决用户版本低或Feng版本不同，无法统一更新。辅助程序将独立运行，不升级原程序。
    /// 目前只为出口添加了一个“出口票_进港地”，鼠标停留在选项上会出现功能解释。
    /// </summary>
    public partial class AppHelperForm : Form
    {
        public AppHelperForm()
        {
            InitializeComponent();
        }

        private void AppHelperForm_Load(object sender, EventArgs e)
        {
            //ServiceProvider.SetDefaultService<IExceptionProcess>(new WinFormExceptionProcess());

            //IPersistentCache c = new PersistentHashtableCache();
            //ServiceProvider.SetDefaultService<ICache>(c);
            //ServiceProvider.SetDefaultService<IPersistentCache>(c);

            Feng.DBDef def = new Feng.DBDef();
            ServiceProvider.SetDefaultService<IDefinition>(def);

            IDataBuffer buf = new Feng.DBDataBuffer();
            ServiceProvider.SetDefaultService<IDataBuffer>(buf);

            IDataBuffers bufs = new DataBuffers();
            bufs.AddDataBuffer(new Cache());
            bufs.AddDataBuffer(buf);
            bufs.AddDataBuffer(def);
            ServiceProvider.SetDefaultService<IDataBuffers>(bufs);

            ServiceProvider.SetDefaultService<IRepositoryFactory>(new Feng.NH.RepositoryFactory());
            ServiceProvider.SetDefaultService<Feng.NH.ISessionFactoryManager>(new Feng.NH.NHibernateSessionFactoryManager());

            //ServiceProvider.SetDefaultService<IEntityMetadataGenerator>(new NHDataEntityMetadataGenerator());

            //ServiceProvider.SetDefaultService<IMessageBox>(new Feng.Windows.Forms.MyMessageBox());

            IEntityScript script = new PythonScript();
            ServiceProvider.SetDefaultService<IScript>(script);
            ServiceProvider.SetDefaultService<IEntityScript>(script);

            IDataBuffers db = ServiceProvider.GetService<IDataBuffers>();
            if (db != null)
            {
                db.LoadData();
            }
        }

        Thread thread_出口票_进港地 = null;
        /// <summary>
        /// 选中创建线程并开始执行，不选中终止线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckb备案_进港地_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb出口票_进港地.Checked)
            {
                if (thread_出口票_进港地 == null)
                {
                    thread_出口票_进港地 = new Thread(出口票_进港地);
                    thread_出口票_进港地.IsBackground = true;
                    thread_出口票_进港地.Start();
                    lblCk.Text = (Convert.ToInt32(lblCk.Text) + 1).ToString();
                }
            }
            else
            {
                try
                {
                    thread_出口票_进港地.Abort();
                }
                catch
                {

                }
                thread_出口票_进港地 = null;
                lblCk.Text = (Convert.ToInt32(lblCk.Text) - 1).ToString();
            }
        }

        void 出口票_进港地()
        {
            while (true)
            {
                process_ck.Helper_出口票_进港地();
                Thread.Sleep(10000);
            }
        }

        private void AppHelperForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskExit()) e.Cancel = true;
        }

        /// <summary>
        /// 最小化到托盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppHelperForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Text = "AppHelper" + Environment.NewLine + "出口：" + lblCk.Text;
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            显示主窗体ToolStripMenuItem_Click(sender, e);
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
            return MessageBox.Show("确定要退出吗？", "AppHelper", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                == System.Windows.Forms.DialogResult.Yes;
        }
    }
}
