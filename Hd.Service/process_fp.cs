using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Fp;

namespace Hd.Service
{
    public class process_fp
    {
        public static void 批量添加发票(ArchiveOperationForm masterForm)
        {
            frm_cw_kj_fp_addall form = new frm_cw_kj_fp_addall(true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                masterForm.ControlManager.DisplayManager.SearchManager.LoadData(null, null);
            }
        }

 
        //public static void 发票入账(ArchiveOperationForm masterForm)
        //{
        //    发票 entity = masterForm.DisplayManager.CurrentItem as 发票;
        //    if (entity == null)
        //        return;
        //    if (entity.Submitted)
        //    {
        //        MessageForm.ShowWarning("此发票不能操作！");
        //        return;
        //    }
        //    if (entity.是否作废)
        //    {
        //        MessageForm.ShowWarning("此发票已作废！");
        //        return;
        //    }
        //    entity.Submitted = true;

        //    masterForm.DoEdit();
        //}


        //public static void 作废发票(ArchiveOperationForm masterForm)
        //{
        //    发票 entity = masterForm.DisplayManager.CurrentItem as 发票;
        //    if (entity == null)
        //        return;
        //    if (entity.Submitted)
        //    {
        //        MessageForm.ShowWarning("此发票不能作废！");
        //        return;
        //    }
        //    if (entity.是否作废)
        //    {
        //        MessageForm.ShowWarning("此发票已作废！");
        //        return;
        //    }
        //    entity.是否作废 = true;

        //    masterForm.DoEdit();
        //}

        //public static void 撤销操作发票(ArchiveOperationForm masterForm)
        //{
        //    发票 entity = masterForm.DisplayManager.CurrentItem as 发票;
        //    if (entity == null)
        //        return;
        //    if (!entity.Submitted && !entity.是否作废)
        //    {
        //        MessageForm.ShowWarning("此发票未进行操作！");
        //        return;
        //    }

        //    if (MessageForm.ShowYesNo("是否确认撤销？", "确认") == DialogResult.No)
        //    {
        //        return;
        //    }

        //    Dictionary<string, object> saved = new Dictionary<string, object>();
        //    saved["入账日期"] = entity.入账日期;

        //    entity.Submitted = false;
        //    entity.是否作废 = false;
        //    entity.入账日期 = null;

        //    masterForm.ControlManager.DisplayManager.DisplayCurrent();
        //    masterForm.ControlManager.EditCurrent();
        //    masterForm.ControlManager.EndEdit();
        //}

        public static void AutoExecute(ArchiveSeeForm masterForm)
        {
            (masterForm.ArchiveDetailForm as IArchiveDetailFormAuto).DataControlsCreated += new EventHandler(ArchiveDetailForm_DataControlsCreated);
        }

        static void ArchiveDetailForm_DataControlsCreated(object sender, EventArgs e)
        {
            ArchiveDetailForm form = sender as ArchiveDetailForm;
            ((form.DisplayManager.DataControls["对账单"] as IWindowControl).Control as MyTextBox).DoubleClick += new EventHandler(Fp_dzd_DoubleClick);
            ((form.DisplayManager.DataControls["金额"] as IWindowControl).Control as MyCurrencyTextBox).TextChanged += new EventHandler(process_fp_TextChanged);
        }

        static void process_fp_TextChanged(object sender, EventArgs e)
        {
            MyCurrencyTextBox dc = sender as MyCurrencyTextBox;
            ArchiveDetailForm form = dc.FindForm() as ArchiveDetailForm;
            if (form != null)
            {
                decimal? d = Feng.Utils.ConvertHelper.ToDecimal(dc.Value);
                if (d.HasValue)
                {
                    form.DisplayManager.DataControls["大写金额"].SelectedDataValue = Feng.Windows.Utils.ChineseHelper.ConvertToChinese(d.Value);
                }
                else
                {
                    form.DisplayManager.DataControls["大写金额"].SelectedDataValue = null;
                }
            }
        }

        public static void Fp_dzd_DoubleClick(object sender, EventArgs e)
        {
            if (sender is MyTextBox)
            {
                MyTextBox box_dzd = sender as MyTextBox;
                if (box_dzd.SelectedDataValue == null)
                {
                    ArchiveDetailForm form = box_dzd.FindForm() as ArchiveDetailForm;
                    decimal money = Convert.ToDecimal(((form.DisplayManager.DataControls["金额"] as IWindowControl).Control as MyCurrencyTextBox).SelectedDataValue);
                    string xgr = ((form.DisplayManager.DataControls["相关人编号"] as IWindowControl).Control as MyComboBox).SelectedDataValue.ToString();
           
                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<对账单>())
                    {
                        IList<对账单> list = (rep as Feng.NH.INHibernateRepository).List<对账单>(NHibernate.Criterion.DetachedCriteria.For<对账单>()
                            .Add(NHibernate.Criterion.Expression.Eq("金额", money))
                            .Add(NHibernate.Criterion.Expression.Eq("相关人编号", xgr))
                            .AddOrder(NHibernate.Criterion.Order.Desc("关账日期")).SetMaxResults(1));

                        if (list == null || list.Count == 0)
                        {
                            MessageForm.ShowWarning("找不到有效的对账单！");
                            return;
                        }

                        box_dzd.SelectedDataValue = list[0].编号;
                    }
                }
            }
        }
    }    
}
