using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Utils;
using Feng.Windows.Utils;
using Feng.Windows.Forms;
using Feng.Windows.Collections;
using Hd.Model;
using Hd.Model.Fp;

namespace Hd.Service
{
    public partial class frm_cw_kj_fp_addall : PositionPersistForm
    {
        private bool m_isAdd;
        public frm_cw_kj_fp_addall(bool isAdd)
        {
            InitializeComponent();

            m_isAdd = isAdd;

            ControlDataLoad.InitDataControl(dcCpzh.Control, "财务_发票账户");
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


                if (MessageBox.Show("本次" + (m_isAdd ? "买入" : "删除") + (sq.End - sq.Begin + 1).ToString() + "张发票，是否确认?",
                    this.Text, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        if (m_isAdd)
                        {
                            m_bll.批量添加((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
                        }
                        else
                        {
                            m_bll.批量删除((DateTime)dcMrsj.SelectedDataValue, (Guid)dcCpzh.SelectedDataValue, sq.Precode, sq.Begin, sq.End);
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
        private 发票Bll m_bll = new 发票Bll();

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

    public class 发票Bll : HdBaseDao<发票>
    {
        public void 批量添加(DateTime 买入时间, Guid 出票账户, string preCode, int start, int end)
        {
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<发票>())
            {
                try
                {
                    rep.BeginTransaction();
                    for (int i = start; i <= end; ++i)
                    {
                        发票 fp = new 发票();
                        fp.发票账户编号 = 出票账户;
                        fp.买入时间 = 买入时间;
                        fp.票据号码 = preCode + i.ToString().PadLeft(end.ToString().Length, '0');
                        fp.Submitted = false;
                        fp.是否作废 = false;
                        fp.开票类别收 = 开票类别.业务款;
                        fp.开票类别付 = 开票类别.业务款;
                        this.Save(rep, fp);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                    throw;
                }
            }
        }

        public void 批量删除(DateTime 买入时间, Guid 出票账户, string preCode, int start, int end)
        {
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<发票>())
            {
                try
                {
                    rep.BeginTransaction();
                    for (int i = start; i <= end; ++i)
                    {
                        string code = preCode + i.ToString().PadLeft(end.ToString().Length, '0');

                        IList<发票> fps = (rep as Feng.NH.INHibernateRepository).List<发票>(NHibernate.Criterion.DetachedCriteria.For<发票>()
                            .Add(NHibernate.Criterion.Expression.Eq("票据号码", code)));

                        if (fps.Count != 1 || fps[0].发票账户编号 != 出票账户 || fps[0].买入时间 != 买入时间
                            || fps[0].Submitted || fps[0].是否作废)
                        {
                            throw new InvalidOperationException("发票" + code + "数据有误，请查证是否可以删除！");
                        }

                        rep.Delete(fps[0]);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                    throw;
                }
            }
        }
    }
}
