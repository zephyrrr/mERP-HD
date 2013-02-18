using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Utils;
using Feng.Windows.Forms;
using Feng.Windows.Collections;
using Hd.Model;
using Hd.Model.Cn;

namespace Hd.Service
{
    public partial class frm_cw_cn_zp_addall : PositionPersistForm
    {
        private bool m_isAdd;
        private bool m_isXianjing;
        private 现金支票Bll m_bll1;
        private 转账支票Bll m_bll2;
        public frm_cw_cn_zp_addall(bool isAdd, bool isXianjing)
        {
            InitializeComponent();

            m_isAdd = isAdd;
            m_isXianjing = isXianjing;
            if (isXianjing)
            {
                m_bll1 = new 现金支票Bll();
            }
            else
            {
                m_bll2 = new 转账支票Bll();
            }

            Feng.Windows.Utils.ControlDataLoad.InitDataControl(dcCpzh.Control, "财务_银行账户");
            dcMrsj.SelectedDataValue = System.DateTime.Today;

            dcCpzh.NotNull = true;
            dcMrsj.NotNull = true;
            dcZphmS.NotNull = true;
            dcZphmZ.NotNull = true;
            m_dcc.Add(dcCpzh);
            m_dcc.Add(dcMrsj);
            m_dcc.Add(dcZphmS);
            m_dcc.Add(dcZphmZ);

            this.Text = m_isAdd ? "批量添加" : "批量删除";
        }

        private DataControlCollection m_dcc = new DataControlCollection();
        private void btnOk_Click(object sender, EventArgs e)
        {
            m_bError = false;

            try
            {
                foreach (IDataControl dc in m_dcc)
                {
                    if (!dc.ReadOnly)
                    {
                        if (dc.NotNull && dc.SelectedDataValue == null)
                        {
                            string errMsg = "请输入" + dc.Caption + "！";
                            throw new ControlCheckException(errMsg, dc);
                        }
                    }
                }

                string codeStart = dcZphmS.SelectedDataValue.ToString();
                string codeEnd = dcZphmZ.SelectedDataValue.ToString();

                StringSequence sq;
                try
                {
                    sq = StringHelper.GetSequnceString(codeStart, codeEnd);
                }
                catch (ArgumentException ex)
                {
                    throw new ControlCheckException(ex.Message, ex);
                }

                if (sq.End - sq.Begin > 999)
                {
                    throw new ControlCheckException("一次支票数量大于1000，不能提交。");
                }

                if (MessageBox.Show("本次" + (m_isAdd ? "买入" : "删除") + (sq.End - sq.Begin + 1).ToString() + "张支票，是否确认?",
                    this.Text, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        if (m_isAdd)
                        {
                            if (m_isXianjing)
                            {
                                m_bll1.批量添加((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
                            }
                            else
                            {
                                m_bll2.批量添加((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
                            }
                        }
                        else
                        {
                            if (m_isXianjing)
                            {
                                m_bll1.批量删除((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
                            }
                            else
                            {
                                m_bll2.批量删除((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                    }
                }
                else
                {
                    m_bError = true;
                }
            }
            catch (ControlCheckException ex)
            {
                m_bError = true;
                (new ErrorProviderControlCheckExceptionProcess()).ShowError(ex.InvalidDataControl, ex.Message);
            }
        }
        

        private bool m_bError = false;
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK && m_bError)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

    }
}
