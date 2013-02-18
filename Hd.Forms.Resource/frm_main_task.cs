using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Feng.Windows.Forms;

namespace Hd.Forms
{
    public partial class frm_main_task : MyChildForm
    {
        public frm_main_task()
        {
            InitializeComponent();

            m_taskPane = new TaskPane("全部");
            m_taskPane.Dock = DockStyle.Fill;
            this.Controls.Add(m_taskPane);
        }
        private TaskPane m_taskPane;
    }
}
