using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hd.Utils
{
    public partial class FormUpload : Form
    {
        public FormUpload()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerAddress.Text))
            {
                MessageBox.Show("请先写服务器地址！");
                return;
            }
            m_up.ServerAddress = (txtServerAddress.Text); //192.168.0.10 localhost:20557

            if (checkBox1.Checked)
            {
                m_up.UpdateData(dateTimePicker1.Value, dateTimePicker2.Value, 1);
            }
        }
        UploadCx m_up = new UploadCx();

        private void FormUpload_Load(object sender, EventArgs e)
        {
            m_up.ServerAddress = (txtServerAddress.Text);
            DateTime? d = m_up.GetLastUpdateDate();
            if (d.HasValue)
            {
                dateTimePicker1.Value = d.Value;
            }
            dateTimePicker2.Value = System.DateTime.Today;
        }
    }
}
