using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Cn;

namespace Hd.Service
{
    /* Submitted＝true  表示支票正常使用结束（支付（支付凭证号<>null）、提现(支付凭证号＝null)
是否作废＝true 表示作废
     Submitted＝false and 是否作废＝false
说明这种支票还没有最后结束，可以 提现、支付、作废
     Submitted＝true 的 不能作废
Submitted＝true 的能撤销到 Submitted＝false

是否作废＝true的能 撤销到 是否作废＝false
     */

    public class process_zp
    {
        public static void 批量添加现金支票(ArchiveOperationForm masterForm)
        {
            frm_cw_cn_zp_addall form = new frm_cw_cn_zp_addall(true, true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                masterForm.ControlManager.DisplayManager.SearchManager.LoadData(null, null);
            }
        }

        public static void 批量删除现金支票(ArchiveOperationForm masterForm)
        {
            frm_cw_cn_zp_addall form = new frm_cw_cn_zp_addall(false, true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                masterForm.ControlManager.DisplayManager.SearchManager.LoadData(null, null);
            }
        }

        public static void 批量添加转账支票(ArchiveOperationForm masterForm)
        {
            frm_cw_cn_zp_addall form = new frm_cw_cn_zp_addall(true, false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                masterForm.ControlManager.DisplayManager.SearchManager.LoadData(null, null);
            }
        }

        public static void 批量删除转账支票(ArchiveOperationForm masterForm)
        {
            frm_cw_cn_zp_addall form = new frm_cw_cn_zp_addall(false, false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                masterForm.ControlManager.DisplayManager.SearchManager.LoadData(null, null);
            }
        }

        public static void 提现或转账支票(ArchiveOperationForm masterForm)
        {
            支票 entity = masterForm.DisplayManager.CurrentItem as 支票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票不能操作！");
                return;
            }
            if (entity.是否作废)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票已作废！");
                return;
            }

            masterForm.DoEdit();

            entity.Submitted = true;

            masterForm.ControlManager.OnCurrentItemChanged();
        }


        public static void 作废支票(ArchiveOperationForm masterForm)
        {
            支票 entity = masterForm.DisplayManager.CurrentItem as 支票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票不能作废！");
                return;
            }
            if (entity.是否作废)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票已作废！");
                return;
            }

            masterForm.DoEdit();

            entity.是否作废 = true;

            masterForm.ControlManager.OnCurrentItemChanged();
        }

        public static void 撤销操作支票(ArchiveOperationForm masterForm)
        {
            支票 entity = masterForm.DisplayManager.CurrentItem as 支票;
            if (entity == null)
                return;
            if (!entity.Submitted && !entity.是否作废)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票未进行操作！");
                return;
            }

            if (entity.Submitted && !string.IsNullOrEmpty(entity.支付凭证号))
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票已支付！");
                return;
            }

            if (!MessageForm.ShowYesNo("是否确认撤销？", "确认"))
            {
                return;
            }

            Dictionary<string, object> saved = new Dictionary<string, object>();
            saved["日期"] = entity.日期;

            if (entity is 转账支票)
            {
                saved["入款账户编号"] = (entity as 转账支票).入款账户编号;
            }

            entity.Submitted = false;
            entity.是否作废 = false;
            entity.日期 = null;

            masterForm.ControlManager.Dao.Update(entity);

            // 不能采用这种方式，EndEdit()会把界面上的值保存到Entity中
            //masterForm.ControlManager.EditCurrent();
            //masterForm.ControlManager.EndEdit();

            masterForm.ControlManager.OnCurrentItemChanged();
        }


        //public static void 撤销作废支票(ArchiveOperationForm masterForm)
        //{
        //    支票 entity = masterForm.DisplayManager.CurrentItem as 支票;
        //    if (entity == null)
        //        return;
        //    if (entity.Submitted)
        //    {
        //        ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票不能撤销作废！");
        //        return;
        //    }
        //    if (!entity.是否作废)
        //    {
        //        ServiceProvider.GetService<IMessageBox>().ShowWarning("此支票未作废！");
        //        return;
        //    }
        //    masterForm.DoEdit();
        //    entity.是否作废 = false;

        //}
    }


    
}
