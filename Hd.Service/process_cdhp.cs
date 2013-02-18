using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Cn;

namespace Hd.Service
{
    /* 就按钮来说有：托收、贴现、返回
【托收贴现】字段  只读
点托收  或  贴现，自动保存【托收贴现】字段

点托收  或  贴现 ：可编辑【出去时间】
点 返回：可编辑【返回时间】*/

    public class process_cdhp
    {
        public static void 托收(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已操作完全！");
                return;
            }
            if (entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已" + entity.托收贴现.Value + "！");
                return;
            }
            masterForm.DoEdit();

            entity.托收贴现 = 托收贴现.托收;

            masterForm.ControlManager.OnCurrentItemChanged();
        }

        public static void 贴现(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已操作完全！");
                return;
            }
            if (entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已" + entity.托收贴现.Value + "！");
                return;
            }
            
            masterForm.DoEdit();

            entity.托收贴现 = 托收贴现.贴现;

            masterForm.ControlManager.OnCurrentItemChanged();
        }


        public static void 现金返回(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已操作完全！");
                return;
            }
            if (!entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票还未托收或贴现");
                return;
            }
            if (entity.返回方式.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已" + entity.返回方式.Value + "返回！");
                return;
            }

            masterForm.DoEdit();

            entity.返回方式 = 承兑汇票返回方式.现金;
            if (entity.托收贴现 == 托收贴现.托收)
            {
                entity.返回金额 = entity.金额;
            }
            entity.Submitted = true;

            masterForm.ControlManager.OnCurrentItemChanged();
        }

        public static void 银行返回(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已操作完全！");
                return;
            }
            if (!entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票还未托收或贴现");
                return;
            }
            if (entity.返回方式.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已" + entity.返回方式.Value + "返回！");
                return;
            }
            masterForm.DoEdit();

            entity.返回方式 = 承兑汇票返回方式.银行;
            if (entity.托收贴现 == 托收贴现.托收)
            {
                entity.返回金额 = entity.金额;
            }
            entity.Submitted = true;

            masterForm.ControlManager.OnCurrentItemChanged();
        }

        public static void 撤销返回(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (!entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票未进行返回操作！");
                return;
            }

            if (entity.Submitted && !entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已支付！");
                return;
            }
            if (!MessageForm.ShowYesNo("是否确认撤销？", "确认"))
            {
                return;
            }

            //Dictionary<string, object> saved = new Dictionary<string, object>();
            //saved["返回方式"] = entity.返回方式;
            //saved["返回时间"] = entity.返回时间;
            //saved["返回经手人编号"] = entity.返回经手人编号;
            //saved["入款账户编号"] = entity.入款账户编号;
            //saved["返回金额"] = entity.返回金额;


            //entity.Submitted = false;
            //entity.返回方式 = null;
            //entity.返回时间 = null;
            //entity.返回经手人编号 = null;
            //entity.入款账户编号 = null;
            //entity.返回金额 = null;

            //// 不加DisplayCurrent，会导致数据控件中的值未改变，再次保存进entity中
            //masterForm.ControlManager.DisplayManager.DisplayCurrent();
            //masterForm.ControlManager.EditCurrent();
            //masterForm.ControlManager.EndEdit();

            // 直接保存。如果通过界面操作，会和界面的Validator搞混，导致不能保存，出现数据错误
            entity.Submitted = false;
            entity.返回方式 = null;
            entity.返回时间 = null;
            entity.返回经手人编号 = null;
            entity.入款账户编号 = null;
            entity.返回金额 = null;
            (new 承兑汇票Bll()).Update(entity);
            masterForm.ControlManager.OnCurrentItemChanged();
        }

        public static void 撤销托收贴现(ArchiveOperationForm masterForm)
        {
            承兑汇票 entity = masterForm.DisplayManager.CurrentItem as 承兑汇票;
            if (entity == null)
                return;
            if (entity.Submitted)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票已操作完全！");
                return;
            }

            if (!entity.托收贴现.HasValue)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("此承兑汇票未进行托收贴现操作！");
                return;
            }

            if (!MessageForm.ShowYesNo("是否确认撤销？", "确认"))
            {
                return;
            }


            //Dictionary<string, object> saved = new Dictionary<string, object>();
            //saved["托收贴现"] = entity.托收贴现;
            //saved["出去时间"] = entity.出去时间;
            //saved["出去经手人编号"] = entity.出去经手人编号;
            //saved["经办人编号"] = entity.经办人编号;


            //entity.Submitted = false;
            //entity.托收贴现 = null;
            //entity.出去时间 = null;
            //entity.出去经手人编号 = null;
            //entity.经办人编号 = null;

            //masterForm.ControlManager.DisplayManager.DisplayCurrent();
            //masterForm.ControlManager.EditCurrent();
            //masterForm.ControlManager.EndEdit();

            entity.Submitted = false;
            entity.托收贴现 = null;
            entity.出去时间 = null;
            entity.出去经手人编号 = null;
            entity.经办人编号 = null;
            (new 承兑汇票Bll()).Update(entity);
            masterForm.ControlManager.OnCurrentItemChanged();
        }
    }
}
