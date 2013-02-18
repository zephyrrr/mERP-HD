using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Feng;
using Feng.Utils;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Kj;
using Hd.Model.Jk;

namespace Hd.Service
{
    public class process_dzd
    {
        public static Dictionary<string, object>  自动凭证应付对账单(对账单 entity)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            IList<GridColumnInfo> infos = new List<GridColumnInfo> { 
                            new GridColumnInfo { Caption = "凭证金额", DataControlDefaultValue = entity.金额.Value.ToString("N2"), DataControlVisible = "True", DataControlType = "MyCurrencyTextBox", PropertyName = "凭证金额", NotNull = "True"} ,
                            new GridColumnInfo { Caption = "凭证币制编号", DataControlDefaultValue = "CNT", DataControlVisible = "True", DataControlType = "MyComboTextBox", PropertyName = "凭证币制编号", CellEditorManagerParam = "财务_币制", NotNull= "True"}  
                        };
            ArchiveDataControlForm form = new ArchiveDataControlForm(new ControlManager((ISearchManager)null), infos);
            if (form.ShowDialog() == DialogResult.OK)
            {
                dict["凭证金额"] = form.DataControls["凭证金额"].SelectedDataValue;
                dict["凭证币制编号"] = form.DataControls["凭证币制编号"].SelectedDataValue;
            }
            else
            {
                throw new InvalidUserOperationException("必须要填写会计金额和币制！");
            }
            return dict;
        }

        public static void 选择进口应收对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写费用项编号！");
                return;
            }
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }

            masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].ReadOnly = true;
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "费用项编号", masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue);
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);

            string type = (string)masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue;
            ArchiveCheckForm form = null;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            if (type == "000")  // 常规
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_进口_应收对账单_常规费用", dict);
            }
            else if (type == "001") // 额外
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_进口_应收对账单_额外费用", dict);
            }

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                foreach (object i in form.SelectedEntites)
                {
                    业务费用 item = i as 业务费用;
                    item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                    detailCm.EndEdit();
                }
            }
        }

        public static void 选择出口应收对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写费用项编号！");
                return;
            }
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }

            masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].ReadOnly = true;
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "费用项编号", masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue);
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);

            string type = (string)masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue;
            ArchiveCheckForm form = null;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            if (type == "000")  // 常规
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_出口_应收对账单_常规费用", dict);
            }
            else if (type == "001") // 额外
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_出口_应收对账单_额外费用", dict);
            }

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                foreach (object i in form.SelectedEntites)
                {
                    业务费用 item = i as 业务费用;
                    item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                    detailCm.EndEdit();
                }
            }
        }

        ///// <summary>
        ///// 应收调节款
        ///// </summary>
        ///// <param name="masterForm"></param>
        ///// <param name="detailForm"></param>
        //public static void 选择应收调节款箱(ArchiveOperationForm masterForm)
        //{
        //    ArchiveSelectForm selectForm = new ArchiveSelectForm("调节款");
        //    if (selectForm.ShowDialog() == DialogResult.OK)
        //    {
        //        ArchiveCheckForm checkForm = selectForm.SelectedForm as ArchiveCheckForm;

        //        Dictionary<string, object> dict = new Dictionary<string, object>();
        //        dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
        //        ArchiveCheckForm form = ProcessSelect.Execute(masterForm.ArchiveDetailForm.ControlManager.DisplayManager, checkForm, dict);

        //        if (form != null)
        //        {
        //            IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

        //            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
        //            {
        //                foreach (object i in form.SelectedEntites)
        //                {
        //                    业务费用 item = new 业务费用();
        //                    DataRow row = i as DataRowView;
        //                    item.箱Id = (Guid)row["箱"];
        //                    item.箱 = rep.Session.Get<普通箱>((Guid)row["箱"]);
        //                    item.票 = rep.Session.Get<普通票>((Guid)row["票"]);
        //                    item.费用实体 = item.票;
        //                    item.相关人编号 = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue.ToString();
        //                    item.费用项编号 = "146";    // 调节业务款
        //                    item.收付标志 = 收付标志.收;

        //                    detailCm.AddNew();
        //                    detailCm.SetItem(detailCm.DisplayManager.Position, item);
        //                    detailCm.EndEdit();
        //                }
        //            }
        //        }
        //    }

            
        //}

        public static void 选择内贸出港应收对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写费用项编号！");
                return;
            }
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }
            masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].ReadOnly = true;
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "费用项编号", masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue);
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);


            string type = (string)masterForm.ControlManager.DisplayManager.DataControls["费用项编号"].SelectedDataValue;
            ArchiveCheckForm form = null;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            if (type ==  "000")
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_内贸出港_应收对账单_常规费用", dict);
            }
            else if (type == "001")
            {
                form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_内贸出港_应收对账单_额外费用", dict);
            }

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                foreach (object i in form.SelectedEntites)
                {
                    业务费用 item = i as 业务费用;
                    item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                    detailCm.EndEdit();
                }
            }
        }

        public static void 选择进口其他业务应收对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);


            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            ArchiveCheckForm form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_进口其他业务_应收对账单_费用", dict);

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        业务费用 item = i as 业务费用;
                        item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                        detailCm.AddNew();
                        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                        detailCm.EndEdit();
                    }
                }
            }
        }

        public static void 选择自动凭证应付对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);


            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            ArchiveCheckForm form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_自动凭证应付对账单_费用", dict);

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        业务费用 item = i as 业务费用;
                        item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                        detailCm.AddNew();
                        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                        detailCm.EndEdit();
                    }
                }
            }
        }

        //public static void 自动凭证应付对账单生成费用(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<业务费用> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager
        //        as IControlManager<业务费用>;

        //    IList<费用> list = new List<费用>();
        //    foreach (业务费用 item in detailCm.DisplayManagerT.Entities)
        //    {
        //        list.Add(item.Clone() as 业务费用);
        //    }

        //    ArchiveOperationForm masterFormKj = TabbedMdiForm.ShowMenuFormInMdi("凭证_会计付款") as ArchiveOperationForm;
        //    if (masterFormKj != null)
        //    {
        //        if (masterFormKj.DoAdd())
        //        {
        //            IControlManager<凭证费用明细> detailCmKj = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //            IBaseDao masterDao = ((IArchiveGrid)masterFormKj.ActiveGrid).Bll as IBaseDao;
        //            MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //            MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //            (masterFormKj.DisplayManager.CurrentItem as 凭证).自动手工标志 = 自动手工标志.对账单;

        //            process_pz.AddFees(masterFormKj.DisplayManager.CurrentItem as 凭证, list, detailCmKj, memoryBll, memoryBll2);
        //            //frm_cw_fkpz_kj_detail detailFormTo = form.ArchiveDetailForm as frm_cw_fkpz_kj_detail;

        //            detailCmKj.DisplayManager.DataControls["金额.数额"].SelectedDataValue = detailForm.ControlManager.DisplayManager.DataControls["凭证金额"].SelectedDataValue;
        //            detailCmKj.DisplayManager.DataControls["金额.币制编号"].SelectedDataValue = detailForm.ControlManager.DisplayManager.DataControls["凭证币制编号"].SelectedDataValue;
        //        }
        //    }
        //}

        public static void 选择应付对账单费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue == null)
            {
                MessageForm.ShowError("请先填写相关人编号！");
                return;
            }
            masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].ReadOnly = true;
            EntityScript.SetPropertyValue(masterForm.DisplayManager.CurrentItem, "相关人编号", masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["相关人编号"] = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            ArchiveCheckForm form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_应付对账单_费用", dict);

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        业务费用 item = i as 业务费用;
                        item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                        detailCm.AddNew();
                        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                        detailCm.EndEdit();
                    }
                }
            }
        }

        public static void 选择固定资产(ArchiveOperationForm masterForm)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            ArchiveCheckForm form = ProcessSelect.Execute((masterForm.ArchiveDetailForm as IDisplayManagerContainer).DisplayManager, "选择_非业务财务_固定资产折旧", dict);

            if (form != null)
            {
                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<非业务费用>())
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        非业务费用 item = i as 非业务费用;
                        item.对账单 = masterForm.DisplayManager.CurrentItem as 对账单;
                        detailCm.AddNew();
                        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                        detailCm.EndEdit();
                    }

                    //foreach (object i in form.SelectedEntites)
                    //{
                    //    非业务费用 item = new 非业务费用();
                    //    DataRowView row = i as DataRowView;
                    //    item.费用实体 = rep.Get<费用实体>((Guid)row["Id"]);
                    //    item.相关人编号 = masterForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue.ToString();
                    //    item.费用项编号 = "387";        // 固定资产折旧
                    //    item.收付标志 = 收付标志.付;

                    //    detailCm.AddNew();
                    //    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                    //    detailCm.EndEdit();
                    //}
                }
            }
        }

        public static void 打印对账单(ArchiveOperationForm masterForm)
        {
            对账单 entity = masterForm.DisplayManager.CurrentItem as 对账单;
            if (entity == null)
            {
                MessageForm.ShowError("请选择要打印的对账单！");
                return;
            }

            ReflectionHelper.RunStaticMethod("Hd.Report", "Hd.Report.ReportPrint", "打印对账单", new object[] { entity.编号 });
        }
    }
}
