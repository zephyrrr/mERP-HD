using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Utils;

namespace Hd.Utils
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnExportYwsj_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.RestoreDirectory = true;
            dlg.Filter = "Excel Xml(*.xml)|*.xml";
            //saveFileDialog1.Title = "保存";
            string today = System.DateTime.Today.ToString("yyyy-MM-dd");
            today = "2010-01-28";
            string whereSql = "(A.Created >= '" + today + "' OR A.Updated >= '" + today + "')";
            string[] sqls = new string[] {
                "SELECT A.* FROM 财务_费用实体 A WHERE (A.费用实体类型 = 11 OR A.费用实体类型 = 15 OR A.费用实体类型 = 45) AND " + whereSql,
                "SELECT B.* FROM 业务备案_普通票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_内贸出港票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口其他业务票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 业务备案_普通箱 A WHERE " + whereSql,
                "SELECT B.* FROM 业务备案_进口箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_内贸出港箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口其他业务箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 业务过程_进口票_转关标志 A WHERE " + whereSql,
                "SELECT B.* FROM 业务过程_进口票_转关 B INNER JOIN 业务过程_进口票_转关标志 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务过程_进口票_清关 B INNER JOIN 业务过程_进口票_转关标志 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 参数备案_币制 A WHERE " + whereSql,
                "SELECT A.* FROM 参数备案_人员单位 A WHERE " + whereSql,
                "SELECT A.* FROM 参数备案_箱型 A WHERE " + whereSql,
                "SELECT A.* FROM 信息_角色用途 A WHERE " + whereSql,
                "SELECT A.* FROM 信息_业务类型 A WHERE " + whereSql};

            string[] dbTables = new string[] {
                "财务_费用实体", 
                "业务备案_普通票",
                "业务备案_进口票", 
                "业务备案_内贸出港票", 
                "业务备案_进口其他业务票", 
                "业务备案_普通箱", 
                "业务备案_进口箱", 
                "业务备案_内贸出港箱",
                "业务备案_进口其他业务箱", 
                "业务过程_进口票_转关标志",
                "业务过程_进口票_转关",
                "业务过程_进口票_清关",
                "参数备案_币制",
                "参数备案_人员单位", 
                "参数备案_箱型", 
                "信息_角色用途", 
                "信息_业务类型"};
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(dlg.FileName, false, Encoding.UTF8);
                ExcelXmlHelper.WriteExcelXmlHead(sw);

                if (dbTables.Length != sqls.Length)
                {
                    throw new ArgumentException("length should be equal!");
                }
                for(int i=0; i<sqls.Length; ++i)
                {
                    ExcelXmlHelper.WriteExcelXml(Feng.Data.DbHelper.Instance.ExecuteDataTable(sqls[i]), sw, dbTables[i]);
                }
                ExcelXmlHelper.WriteExcelXmlTail(sw);
                sw.Close();
            }
        }

        private void btnImportYwsj_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.RestoreDirectory = true;
            dlg.Filter = "Excel Xml(*.xml)|*.xml";
            //saveFileDialog1.Title = "保存";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("您正在导入数据到数据库，如果有相同的主键，将会替换数据库数据，是否确认？", "确认", MessageBoxButtons.YesNo)
                    == DialogResult.Yes)
                {
                    ADUtils.DisableFKConstraint();

                    ADUtils.ImportFromXmlFile(dlg.FileName);

                    ADUtils.EnableFKConstraint();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            IList<Hd.Model.Jk.进口票> list;
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口票>())
            {
                list = (rep as Feng.NH.INHibernateRepository).List<Hd.Model.Jk.进口票>(NHibernate.Criterion.DetachedCriteria.For<Hd.Model.Jk.进口票>()
                    .AddOrder(NHibernate.Criterion.Order.Desc("委托时间")));
            }

            int cnt = 0;
            foreach (Hd.Model.Jk.进口票 i in list)
            {
                try
                {
                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口票>())
                    {
                        rep.Initialize(i.箱, i);
                        Hd.Service.process_fy_yw.批量生成费用(rep, i.费用实体类型编号, i, i.箱, null, null);
                    }
                    cnt++;
                    backgroundWorker1.ReportProgress(cnt / list.Count, "First:" + cnt.ToString() + ".");
                }
                catch (Exception)
                {
                }
            }

            cnt++;
            foreach (Hd.Model.Jk.进口票 i in list)
            {
                try
                {
                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口票>())
                    {
                        Hd.Service.process_fy_yw.批量生成费用(rep, i.费用实体类型编号, i, i.箱, null, null);
                    }
                    cnt++;
                    backgroundWorker1.ReportProgress(cnt / list.Count, "Second:" + cnt.ToString() + ".");
                }
                catch (Exception)
                {
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string cnt = (string)e.UserState;
            textBox1.AppendText(cnt.ToString() + e.ProgressPercentage);
            textBox1.AppendText(System.Environment.NewLine);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.AppendText("All Done.");
            if (e.Error != null)
            {
                textBox1.AppendText(e.Error.Message);
            }
            textBox1.AppendText(System.Environment.NewLine);
        }

        private void btnDoWork_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void btnUploadCx_Click(object sender, EventArgs e)
        {
            UploadCx up = new UploadCx("http://17haha8.oicp.net:8088"); //192.168.0.10 localhost:20557
            up.UpdateData();
        }
    }
}
