using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Feng;
using Feng.Utils;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Nmcg;
using Hd.Model.Jk;
using Hd.Model.Jk2;

namespace Hd.Service
{
    public class process_fy_yw
    {
        public static void AutoExecute(ArchiveOperationForm masterForm)
        {
            masterForm.MasterGrid.DataRowTemplate.BeginningEdit += new System.ComponentModel.CancelEventHandler(DataRowTemplate_BeginningEdit);
        }

        static void DataRowTemplate_BeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //masterForm.ActiveGrid.DataRowTemplate.Cells["票.费用实体类型"].ValueChanged += new EventHandler(process_fy_yw_ValueChanged);

            Xceed.Grid.DataRow dataRow = sender as Xceed.Grid.DataRow;
            if (((IBoundGrid)dataRow.GridControl).DisplayManager.InBatchOperation)
            {
                return;
            }

            //if (cell.ParentRow.Cells["费用类别编号"].Value != null
            //    && cell.ParentRow.Cells["收付标志"].Value != null)
            //{
            //    StringBuilder filter = new StringBuilder();
            //    if (((收付标志)cell.ParentRow.Cells["收付标志"].Value) == 收付标志.收)
            //    {
            //        filter.Append("收 = 'true' AND 收入类别 = '" + cell.ParentRow.Cells["费用类别编号"].Value.ToString() + "'");
            //    }
            //    else
            //    {
            //        filter.Append("付 = 'true' AND 支出类别 = '" + cell.ParentRow.Cells["费用类别编号"].Value.ToString() + "'");
            //    }

            //    cell.ParentRow.Cells["费用项编号"].CellEditorManager = Feng.Windows.ControlDataLoad.GetGridComboEditor("费用项_业务_" + m_type, filter.ToString());
            //}

            业务费用 item = dataRow.Tag as 业务费用;
            bool isPiao = !item.箱Id.HasValue;
            string filter = "现有费用实体类型 LIKE '%" + item.票.费用实体类型编号;
            if (isPiao)
            {
                filter += ",%' AND 票 = " + isPiao;
            }
            else
            {
                filter += ",%' AND 箱 = " + !isPiao;
            }
            dataRow.Cells["费用项编号"].CellEditorManager = Feng.Windows.Utils.GridDataLoad.GetGridComboEditor("费用项_业务", filter);
        }

        public static void 批量添加费用(ArchiveOperationForm masterForm)
        {
            IControlManager cm = masterForm.ControlManager;

            ArchiveSelectForm selectForm = new ArchiveSelectForm("批量添加业务费用");
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                ArchiveCheckForm form = selectForm.SelectedForm as ArchiveCheckForm;

                if (form != null && form.ShowDialog() == DialogResult.OK)
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        
                        业务费用 item = new 业务费用();
                        if (i is 普通票)
                        {
                            item.票 = i as 普通票;
                            item.费用实体 = new 普通票 { ID = item.票.ID, Version = item.票.Version, 费用实体类型编号 = item.票.费用实体类型编号 };
                        }
                        else if (i is 普通箱)
                        {
                            普通箱 xiang = i as 普通箱;

                            // it must have piao
                            item.票 = xiang.GetType().InvokeMember("票",
                                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, xiang, null) as 普通票;

                            item.箱 = (i == null ? null : xiang);
                            item.箱Id = i == null ? null : (Guid?)xiang.ID;
                            item.费用实体 = new 普通票 { ID = item.票.ID, Version = item.票.Version, 费用实体类型编号 = item.票.费用实体类型编号 };
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "选中的费用实体类型不是要求类型，而是" + i.GetType().ToString());
                        }

                        object entity = cm.AddNew();
                        if (entity != null)
                        {
                            cm.DisplayManager.Items[cm.DisplayManager.Position] = item;
                            cm.EndEdit();
                        }
                        else
                        {
                            // 出现错误，不再继续。 AddNew的时候，前一个出现错误，没保存。然后提示时候保存，选不继续
                            masterForm.ControlManager.CancelEdit();
                            break;
                        }

                        //bool isPiao = (i is 普通票);
                        //string filter = "现有费用实体类型 LIKE '%" + (int)item.票.费用实体类型;
                        //if (isPiao)
                        //{
                        //    filter += ",%' AND 票 = " + isPiao;
                        //}
                        //else
                        //{
                        //    filter += ",%' AND 箱 = " + !isPiao;
                        //}
                        //masterForm.ActiveGrid.CurrentDataRow.Cells["费用项编号"].CellEditorManager = ControlDataLoad.GetGridComboEditor("费用项_业务", filter);
                    }
                }
            }
        }

        public static void 生成收款相应费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.MasterGrid.GridControl.SelectedRows.Count == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请选择付款费用！");
                return;
            }
            if (!MessageForm.ShowYesNo("是否要生成选中项相应的收款费用?", "确认"))
            {
                return;
            }

            int cnt = 0;
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
            {
                foreach (Xceed.Grid.Row row in masterForm.MasterGrid.GridControl.SelectedRows)
                {
                    Xceed.Grid.DataRow dataRow = row as Xceed.Grid.DataRow;
                    if (dataRow == null)
                        continue;

                    业务费用 entity = dataRow.Tag as 业务费用;
                    if (entity.收付标志 == 收付标志.收)
                        continue;

                    rep.Initialize(entity.票, entity);

                    业务费用 item = new 业务费用();
                    item.收付标志 = 收付标志.收;
                    item.费用实体 = entity.费用实体;
                    item.费用项编号 = entity.费用项编号;
                    //item.费用信息 = entity.费用信息;
                    item.费用类别编号 = entity.费用类别编号;
                    item.金额 = entity.金额;
                    item.箱Id = entity.箱Id;
                    item.相关人编号 = entity.票.委托人编号;
                    item.票 = entity.票;
                    item.箱 = entity.箱;

                    object newEntity = masterForm.ControlManager.AddNew();
                    if (newEntity != null)
                    {
                        masterForm.ControlManager.DisplayManager.Items[masterForm.ControlManager.DisplayManager.Position] = item;
                        masterForm.ControlManager.EndEdit();
                        //masterForm.ActiveGrid.CurrentDataRow.Cells["费用项编号"].CellEditorManager = dataRow.Cells["费用项编号"].CellEditorManager;
                        cnt++;
                    }
                    else
                    {
                        masterForm.ControlManager.CancelEdit();
                        break;
                    }
                }
            }
            MessageForm.ShowInfo("已生成" + cnt + "条收款费用!");
        }

        public static void 生成凭证(ArchiveOperationForm masterForm)
        {
            IList<费用> list = GetSelectedFee4Pz(masterForm);
            if (list.Count == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请选择未出对帐单和凭证的费用！");
                return;
            }

            if (!MessageForm.ShowYesNo("是否要生成选中项相应的凭证?", "确认"))
            {
                return;
            }

            ArchiveOperationForm masterFormKj = ServiceProvider.GetService<IApplication>().ExecuteAction("资金票据_凭证") as ArchiveOperationForm;
            if (masterFormKj != null)
            {
                if (masterFormKj.DoAdd())
                {
                    ArchiveDetailForm detailFormKj = masterFormKj.ArchiveDetailForm as ArchiveDetailForm;

                    IControlManager<凭证费用明细> detailCmKj = (((IArchiveDetailFormWithDetailGrids)detailFormKj).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
                    IRelationalDao masterDao = masterFormKj.ControlManager.Dao as IRelationalDao;
                    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetRelationalDao(0) as IMemoriedRelationalDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
                    //MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMemoriedMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

                    (masterFormKj.DisplayManager.CurrentItem as 凭证).凭证类别 = 凭证类别.付款凭证;
                    detailFormKj.UpdateContent();

                    process_pz.AddFees(masterFormKj.DisplayManager.CurrentItem as 凭证, list, detailCmKj);
                }
            }
        }

        private static IList<费用> GetSelectedFee4Pz(ArchiveOperationForm masterForm)
        {
            IList<费用> list = new List<费用>();
            foreach (Xceed.Grid.Row row in masterForm.MasterGrid.GridControl.SelectedRows)
            {
                Xceed.Grid.DataRow dataRow = row as Xceed.Grid.DataRow;
                if (dataRow == null)
                    continue;
                业务费用 item = dataRow.Tag as 业务费用;
                if (item.凭证费用明细 == null && item.对账单 == null)
                {
                    // 界面之间跳转的，不共用数据，用复制数据，这样数据不会搞混
                    // 例如这里，生成凭证，费用的凭证费用明细字段设置了，但如果凭证地方取消，凭证费用明细字段却没清空
                    // 如果是用公用数据的话，会对这里产生影响。
                    list.Add(item.Clone() as 业务费用);
                }
            }
            return list;
        }

        public static void 批量添加内贸出港费用(ArchiveOperationForm masterForm)
        {
            int cnt = 0;
            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_批量添加费用_内贸出港票")) as ArchiveCheckForm;
            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach (object i in form.SelectedEntites)
                {
                    //普通票 entity = i as 普通票;
                    内贸出港票 entity2 = i as 内贸出港票;

                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<内贸出港票>())
                    {
                        rep.Initialize(entity2.箱, entity2);

                        cnt += 批量生成费用(rep, 15/*费用实体类型.内贸出港*/, entity2, entity2.箱, null, null);
                    }
                }

                MessageForm.ShowInfo("已生成" + cnt + "条收款费用!");
            }
        }

        public static void 批量添加进口费用(ArchiveOperationForm masterForm)
        {
            IControlManager cm = masterForm.ControlManager;
            
            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_批量添加费用_进口票")) as ArchiveCheckForm;

            if (form != null && form.ShowDialog() == DialogResult.OK)
            {
                foreach (object i in form.SelectedEntites)
                {
                    业务费用 item = new 业务费用();
                    if (i is 普通票)
                    {
                        item.票 = i as 普通票;
                        item.费用实体 = new 普通票 { ID = item.票.ID, Version = item.票.Version, 费用实体类型编号 = item.票.费用实体类型编号 };
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "选中的费用实体类型不是要求类型，而是" + i.GetType().ToString());
                    }

                    object entity = cm.AddNew();
                    if (entity != null)
                    {
                        cm.DisplayManager.Items[cm.DisplayManager.Position] = item;
                        cm.EndEdit();
                    }
                    else
                    {
                        // 出现错误，不再继续。 AddNew的时候，前一个出现错误，没保存。然后提示时候保存，选不继续
                        masterForm.ControlManager.CancelEdit();
                        break;
                    }
                }
            }
        }

        public static void 批量添加进口箱费用(ArchiveOperationForm masterForm)
        {
            IControlManager cm = masterForm.ControlManager;

            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_批量添加费用_进口箱")) as ArchiveCheckForm;

            if (form != null && form.ShowDialog() == DialogResult.OK)
            {
                foreach (object i in form.SelectedEntites)
                {
                    业务费用 item = new 业务费用();
                    if (i is 普通箱)
                    {
                        普通箱 xiang = i as 普通箱;

                        // it must have piao
                        item.票 = xiang.GetType().InvokeMember("票",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, xiang, null) as 普通票;

                        item.箱 = (i == null ? null : xiang);
                        item.箱Id = i == null ? null : (Guid?)xiang.ID;
                        item.费用实体 = new 普通票 { ID = item.票.ID, Version = item.票.Version, 费用实体类型编号 = item.票.费用实体类型编号 };
                    }
                    else
                    {
                        System.Diagnostics.Debug.Assert(false, "选中的费用实体类型不是要求类型，而是" + i.GetType().ToString());
                    }

                    object entity = cm.AddNew();
                    if (entity != null)
                    {
                        cm.DisplayManager.Items[cm.DisplayManager.Position] = item;
                        cm.EndEdit();
                    }
                    else
                    {
                        // 出现错误，不再继续。 AddNew的时候，前一个出现错误，没保存。然后提示时候保存，选不继续
                        masterForm.ControlManager.CancelEdit();
                        break;
                    }
                }
            }
        }

        public static void 批量添加进口其他费用(ArchiveOperationForm masterForm)
        {
            int cnt = 0;
            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_批量添加费用_进口其他业务票")) as ArchiveCheckForm;
            if (form.ShowDialog() == DialogResult.OK)
            {
                foreach (object i in form.SelectedEntites)
                {
                    //普通票 entity = i as 普通票;
                    进口其他业务票 entity2 = i as 进口其他业务票;

                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口其他业务票>())
                    {
                        rep.Initialize(entity2.箱, entity2);

                        cnt += 批量生成费用(rep, 45, entity2, entity2.箱, null, null);
                    }
                }

                MessageForm.ShowInfo("已生成" + cnt + "条收款费用!");
            }
        }

        public static int 批量生成费用(IRepository rep, int 费用实体类型, 普通票 票, IEnumerable 箱, string 费用项编号, 收付标志? 收付标志)
        {
            return 批量生成费用(rep, 费用实体类型, 票, 箱, 费用项编号, 收付标志, false);
        }

        public static int 批量生成费用(IRepository rep, int 费用实体类型, 普通票 票, IEnumerable 箱, string 费用项编号, 收付标志? 收付标志, bool service)
        {
            int cnt = 0;

            // 需按照委托人合同和付款合同生成相应费用和费用理论值
            // 如果总体来生成，则按照：
            // 如果费用已经打了完全标志，则不生成。如果相应理论值已经生成过，也不生成。
            // 如果单个费用项来生成，则不管理论值是否已经生成过
            // Todo: 理论值可能显示生成票的，后来信息完全了再生成箱的，此时要删除票的 
            //using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository(票.GetType()))
            {
                try
                {
                    rep.BeginTransaction();

                    IList<业务费用理论值> llzs = (rep as Feng.NH.INHibernateRepository).List<业务费用理论值>(NHibernate.Criterion.DetachedCriteria.For<业务费用理论值>()
                                            .Add(NHibernate.Criterion.Expression.Eq("费用实体.ID", 票.ID)));

                    rep.Initialize(票.费用, 票);

                    process_fy_generate.批量生成费用付款(rep, 费用实体类型, 票, 箱, 费用项编号, 收付标志, llzs, service);

                    process_fy_generate.批量生成费用收款(rep, 费用实体类型, 票, 箱, 费用项编号, 收付标志, llzs);

                    ////  有几项（发票税，贴息费）要看收款费用
                    // 不行，会多生成
                    //批量生成费用付款(rep, 费用实体类型, 票, 箱, 费用项编号, 收付标志, llzs);

                    // 不知道哪里更改了，会update
                    (rep as Feng.NH.INHibernateRepository).Session.Evict(票);

                    rep.CommitTransaction();
                }
                catch (Exception ex)
                {
                    rep.RollbackTransaction();
                    ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                }
            }
            return cnt;
        }
    }
}
