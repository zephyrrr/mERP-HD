using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Feng;
using Feng.Windows.Utils;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Kj;

namespace Hd.Service
{
    public class process_fy_fyw
    {
        public static void AutoExecute(ArchiveOperationForm masterForm)
        {
            masterForm.MasterGrid.DataRowTemplate.BeginningEdit += new System.ComponentModel.CancelEventHandler(DataRowTemplate_BeginningEdit);
            //masterForm.ActiveGrid.DataRowTemplate.Cells["非业务费用实体.费用实体类型"].ValueChanged += new EventHandler(process_fy_yw_ValueChanged);
        }

        static void DataRowTemplate_BeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Xceed.Grid.DataRow row = sender as Xceed.Grid.DataRow;

            非业务费用 item = row.Tag as 非业务费用;
            string filter = "现有费用实体类型 LIKE '%" + item.费用实体.费用实体类型编号 + ",%'";
            row.Cells["费用项编号"].CellEditorManager = GridDataLoad.GetGridComboEditor("费用项_非业务", filter);
        }

        public static void 批量添加费用(ArchiveOperationForm masterForm)
        {
            IControlManager cm = masterForm.ControlManager;

            ArchiveSelectForm selectForm = new ArchiveSelectForm("批量添加非业务费用");
            if (selectForm.ShowDialog() == DialogResult.OK)
            {
                ArchiveCheckForm form = selectForm.SelectedForm as ArchiveCheckForm;

                if (form != null && form.ShowDialog() == DialogResult.OK)
                {
                    foreach (object i in form.SelectedEntites)
                    {
                        非业务费用 newItem = new 非业务费用();
   
                        if (i is 费用实体)
                        {
                            newItem.费用实体 = i as 费用实体;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "选中的费用实体类型不是要求类型，而是" + i.GetType().ToString());
                        }

                        cm.AddNew();
                        cm.DisplayManager.Items[cm.DisplayManager.Position] = newItem;
                        cm.EndEdit();


                        //string filter = "现有费用实体类型 LIKE '%" + (int)item.费用实体.费用实体类型 + ",%'";
                        //masterForm.ActiveGrid.CurrentDataRow.Cells["费用项编号"].CellEditorManager = ControlDataLoad.GetGridComboEditor("费用项_非业务", filter);
                    }
                }
            }
        }

        public static void 生成相同费用(ArchiveOperationForm masterForm)
        {
            if (masterForm.MasterGrid.GridControl.SelectedRows.Count == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请选择费用！");
                return;
            }
            if (!MessageForm.ShowYesNo("是否要生成选中项相同的费用?", "确认"))
            {
                return;
            }

            int cnt = 0;
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<非业务费用>())
            {
                foreach (Xceed.Grid.Row row in masterForm.MasterGrid.GridControl.SelectedRows)
                {
                    Xceed.Grid.DataRow dataRow = row as Xceed.Grid.DataRow;
                    if (dataRow == null)
                        continue;

                    非业务费用 entity = dataRow.Tag as 非业务费用;

                    if (entity.非业务费用实体.费用实体类型编号 != 37/*费用实体类型.其他非业务*/)
                        continue;

                    rep.Initialize(entity.费用实体, entity);

                    非业务费用 item = new 非业务费用();
                    item.收付标志 = entity.收付标志;
                    item.费用实体 = entity.费用实体;
                    item.费用项编号 = entity.费用项编号;
                    item.费用类别编号 = entity.费用类别编号;
                    item.金额 = entity.金额;
                    item.相关人编号 = entity.相关人编号;

                    object newEntity = masterForm.ControlManager.AddNew();
                    if (newEntity != null)
                    {
                        masterForm.ControlManager.DisplayManager.Items[masterForm.ControlManager.DisplayManager.Position] = item;
                        masterForm.ControlManager.EndEdit();
                        (masterForm.MasterGrid.CurrentRow as Xceed.Grid.DataRow).Cells["费用项编号"].CellEditorManager = dataRow.Cells["费用项编号"].CellEditorManager;
                        cnt++;
                    }
                    else
                    {
                        masterForm.ControlManager.CancelEdit();
                        break;
                    }
                }
            }

            MessageForm.ShowInfo("已生成" + cnt + "条费用!");
        }

        public static void 生成凭证(ArchiveOperationForm masterForm)
        {
            IList<费用> list = GetSelectedFee4Pz(masterForm);
            if (list.Count == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请选择未出凭证的费用！");
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
                非业务费用 item = dataRow.Tag as 非业务费用;
                if (item.凭证费用明细 == null )
                {
                    list.Add(item.Clone() as 非业务费用);
                }
            }
            return list;
        }
    }
}
