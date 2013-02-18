using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Feng.Utils;
using Hd.Model;

namespace Hd.Service
{
    public class process_pz
    {
        //public static bool 凭证金额判断汇率(凭证 entity)
        //{
        //    if (Feng.Windows.Forms.MessageForm.ShowYesNo("当前汇率为" + entity.会计金额.Value / entity.金额.数额.Value, "确认")
        //        == System.Windows.Forms.DialogResult.No)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        public static void AutoExecute(ArchiveSeeForm masterForm)
        {
            ((masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[1] as IArchiveGrid).DisplayManager.SelectedDataValueChanged += new EventHandler<SelectedDataValueChangedEventArgs>(DisplayManagerPzsfmx_SelectedDataValueChanged);

            ((masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0] as IArchiveGrid).DisplayManager.SelectedDataValueChanged += new EventHandler<SelectedDataValueChangedEventArgs>(DisplayManagerPzfymx_SelectedDataValueChanged);

            ((masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0] as IArchiveGrid).InsertionRow.EditBegun += new EventHandler(InsertionRow_EditBegun);

            ((masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[1] as IArchiveGrid).InsertionRow.EditBegun += new EventHandler(InsertionRow2_EditBegun);

            (masterForm.ArchiveDetailForm as IArchiveDetailFormAuto).DataControlsCreated += new EventHandler(ArchiveDetailForm_DataControlsCreated);
        }

        static void ArchiveDetailForm_DataControlsCreated(object sender, EventArgs e)
        {
            ArchiveDetailForm form = sender as ArchiveDetailForm;
            ((form.DisplayManager.DataControls["金额.数额"] as IWindowControl).Control as MyCurrencyTextBox).TextChanged += new EventHandler(process_pz_TextChanged);
            ((form.DisplayManager.DataControls["金额.数额"] as IWindowControl).Control as MyCurrencyTextBox).DoubleClick += new EventHandler(process_pz_DoubleClick);
        }

        static void process_pz_DoubleClick(object sender, EventArgs e)
        {
            MyCurrencyTextBox dc = sender as MyCurrencyTextBox;
            ArchiveDetailForm form = dc.FindForm() as ArchiveDetailForm;
            if (form != null)
            {
                decimal sum = 0;
                IArchiveGrid grid = ((form as IArchiveDetailFormWithDetailGrids).DetailGrids[0] as IArchiveGrid);
                foreach (Xceed.Grid.DataRow row in grid.DataRows)
                {
                    if (row.Cells["收付标志"].Value != null
                        && row.Cells["金额"].Value != null)
                    {
                        decimal? d = ConvertHelper.ToDecimal(row.Cells["金额"].Value);
                        if ((收付标志)row.Cells["收付标志"].Value == 收付标志.收)
                        {
                            sum += d.Value;
                        }
                        else
                        {
                            sum -= d.Value;
                        }
                    }
                }

                if ((凭证类别)form.DisplayManager.DataControls["凭证类别"].SelectedDataValue == 凭证类别.收款凭证)
                {
                    dc.SelectedDataValue = sum;
                }
                else
                {
                    dc.SelectedDataValue = -sum;
                }
            }
        }

        static void process_pz_TextChanged(object sender, EventArgs e)
        {
            MyCurrencyTextBox dc = sender as MyCurrencyTextBox;
            ArchiveDetailForm form = dc.FindForm() as ArchiveDetailForm;
            if (form != null)
            {
                if (form.DisplayManager.DataControls["大写金额"] == null)
                    return;

                decimal? d = Feng.Utils.ConvertHelper.ToDecimal(dc.TextBoxArea.Text);  // dc.Value 不反映Text，只反映上一步的Text。 即摁入123456时，Value=12345
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

        static void InsertionRow2_EditBegun(object sender, EventArgs e)
        {
            Xceed.Grid.InsertionRow row = sender as Xceed.Grid.InsertionRow;
            row.Cells["金额"].Value = (row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["金额.数额"].SelectedDataValue;
            if ((凭证类别)(row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["凭证类别"].SelectedDataValue == 凭证类别.付款凭证)
            {
                row.Cells["收付标志"].Value = 收付标志.付;
            }
            else
            {
                row.Cells["收付标志"].Value = 收付标志.收;
            }
            (row.GridControl as IArchiveGrid).DisplayManager.OnSelectedDataValueChanged(new SelectedDataValueChangedEventArgs("收付标志", row.Cells["收付标志"]));
        }

        static void InsertionRow_EditBegun(object sender, EventArgs e)
        {
            Xceed.Grid.InsertionRow row = sender as Xceed.Grid.InsertionRow;
            row.Cells["相关人编号"].Value = (row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["相关人编号"].SelectedDataValue;
            row.Cells["金额"].Value = (row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["金额.数额"].SelectedDataValue;

            if ((凭证类别)(row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["凭证类别"].SelectedDataValue == 凭证类别.付款凭证)
            {
                row.Cells["收付标志"].Value = 收付标志.付;
            }
            else
            {
                row.Cells["收付标志"].Value = 收付标志.收;
            }

            (row.GridControl as IArchiveGrid).DisplayManager.OnSelectedDataValueChanged(new SelectedDataValueChangedEventArgs("收付标志", row.Cells["收付标志"]));
        }

        static void DisplayManagerPzfymx_SelectedDataValueChanged(object sender, SelectedDataValueChangedEventArgs e)
        {
            Xceed.Grid.Cell cell = e.Container as Xceed.Grid.Cell;
            IDisplayManager dm = sender as IDisplayManager;

            if (e.DataControlName == "费用项")
            {
                string fyx = (string)cell.ParentRow.Cells["费用项"].Value;
                if (fyx == "012")   // 还借款
                {
                    cell.ParentRow.Cells["结算期限"].ReadOnly = false;
                }
                else
                {
                    cell.ParentRow.Cells["结算期限"].ReadOnly = true;
                }
            }
            
        }

        static void DisplayManagerPzsfmx_SelectedDataValueChanged(object sender, SelectedDataValueChangedEventArgs e)
        {
            if (e.DataControlName == "收付款方式" || e.DataControlName == "收付标志")
            {
                Xceed.Grid.Cell cell = e.Container as Xceed.Grid.Cell;
                cell.ParentRow.Cells["票据号码"].ReadOnly = true;
                cell.ParentRow.Cells["银行账户编号"].ReadOnly = true;
                cell.ParentRow.Cells["出票银行"].ReadOnly = true;
                cell.ParentRow.Cells["承兑期限"].ReadOnly = true;
                cell.ParentRow.Cells["付款人编号"].ReadOnly = true;

                if (cell.ParentRow.Cells["收付款方式"].Value != null && cell.ParentRow.Cells["收付标志"].Value != null)
                {
                    收付标志 a = (收付标志)cell.ParentRow.Cells["收付标志"].Value;
                    收付款方式 b = (收付款方式)cell.ParentRow.Cells["收付款方式"].Value;
                    switch (a)
                    {
                        case 收付标志.收:
                            cell.ParentRow.Cells["票据号码"].ReadOnly = !(b == 收付款方式.现金支票 || b == 收付款方式.转账支票 || b == 收付款方式.银行承兑汇票 || b == 收付款方式.银行本票汇票);
                            cell.ParentRow.Cells["银行账户编号"].ReadOnly = !(b == 收付款方式.转账支票 || b == 收付款方式.银行本票汇票 || b == 收付款方式.银行收付);
                            cell.ParentRow.Cells["出票银行"].ReadOnly = !(b == 收付款方式.银行承兑汇票);
                            cell.ParentRow.Cells["承兑期限"].ReadOnly = !(b == 收付款方式.银行承兑汇票);
                            cell.ParentRow.Cells["付款人编号"].ReadOnly = !(b == 收付款方式.银行承兑汇票);
                            if (b == 收付款方式.银行承兑汇票)
                            {
                                cell.ParentRow.Cells["付款人编号"].Value = (cell.ParentRow.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.DataControls["相关人编号"].SelectedDataValue;
                            }
                            else
                            {
                                cell.ParentRow.Cells["付款人编号"].Value = null;
                            }
                            break;
                        case 收付标志.付:
                            cell.ParentRow.Cells["票据号码"].ReadOnly = !(b == 收付款方式.现金支票 || b == 收付款方式.转账支票 || b == 收付款方式.银行承兑汇票);
                            cell.ParentRow.Cells["银行账户编号"].ReadOnly = !(b == 收付款方式.银行收付 || b == 收付款方式.电汇);
                            cell.ParentRow.Cells["出票银行"].ReadOnly = true;
                            cell.ParentRow.Cells["承兑期限"].ReadOnly = true;
                            cell.ParentRow.Cells["付款人编号"].ReadOnly = true;
                            break;
                    }
                }
            }
        }

        public static void 出纳修改(ArchiveOperationForm masterForm)
        {
            凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
            if (pz == null)
            {
                MessageForm.ShowError("请选择要修改的凭证！");
                return;
            }
            pz.操作人 = "出纳";

            masterForm.DoEdit();
        }

        public static void 会计修改(ArchiveOperationForm masterForm)
        {
            凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
            if (pz == null)
            {
                MessageForm.ShowError("请选择要修改的凭证！");
                return;
            }
            pz.操作人 = "会计";

            masterForm.DoEdit();
        }

        public static void 审核人修改(ArchiveOperationForm masterForm)
        {
            凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
            if (pz == null)
            {
                MessageForm.ShowError("请选择要修改的凭证！");
                return;
            }
            pz.操作人 = "审核人";

            masterForm.DoEdit();
        }

        public static void 新增出纳收款(ArchiveOperationForm masterForm)
        {
            if (masterForm.DoAdd())
            {
                凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
                pz.凭证类别 = 凭证类别.收款凭证;
                pz.自动手工标志 = 自动手工标志.手工;
                pz.操作人 = "出纳";
                //pz.日期 = System.DateTime.Today;
                (masterForm.ArchiveDetailForm as ArchiveDetailForm).UpdateContent();
            }
        }

        public static void 新增会计付款(ArchiveOperationForm masterForm)
        {
            if (masterForm.DoAdd())
            {
                凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
                pz.凭证类别 = 凭证类别.付款凭证;
                pz.自动手工标志 = 自动手工标志.手工;
                pz.操作人 = "会计";
                //pz.日期 = System.DateTime.Today;
                (masterForm.ArchiveDetailForm as ArchiveDetailForm).UpdateContent();
            }
        }


        public static void 添加新建非业务费用(ArchiveOperationForm masterForm)
        {
            IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
            //IDao masterDao = masterForm.ControlManager.Dao;
            //MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
            //MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

            ArchiveDetailForm form = Feng.Windows.Utils.ArchiveFormFactory.GenerateArchiveDetailForm(ADInfoBll.Instance.GetWindowInfo("非业务财务_借贷"), null);
            form.ControlManager.AddNew();

            form.UpdateContent();
            if (form.ShowDialog() == DialogResult.OK)
            {
                费用实体 entity = form.ControlManager.DisplayManager.CurrentItem as 费用实体;
                if (entity == null)
                {
                    throw new ArgumentNullException("invalid 费用实体");
                }
                AddFees(masterForm.DisplayManager.CurrentItem as 凭证, entity.费用, detailCm);
            }
        }

        public static void 添加现有业务费用(ArchiveOperationForm masterForm)
        {
            IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
            //IDao masterDao = masterForm.ControlManager.Dao;
            //MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
            //MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_会计凭证_业务费用")) as ArchiveCheckForm;
            if (form.ShowDialog() == DialogResult.OK)
            {
                IList<费用> list = new List<费用>();
                foreach (object i in form.SelectedEntites)
                {
                    list.Add(i as 费用);
                }

                AddFees(masterForm.DisplayManager.CurrentItem as 凭证, list, detailCm);
            }
        }

        public static void 添加现有非业务费用(ArchiveOperationForm masterForm)
        {
            IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
            //IDao masterDao = masterForm.ControlManager.Dao;
            //MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
            //MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

            ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("选择_会计凭证_非业务费用")) as ArchiveCheckForm;
            if (form.ShowDialog() == DialogResult.OK)
            {
                IList<费用> list = new List<费用>();
                foreach (object i in form.SelectedEntites)
                {
                    list.Add(i as 费用);
                }

                AddFees(masterForm.DisplayManager.CurrentItem as 凭证, list, detailCm);
            }
        }

        //public static void 审核(ArchiveOperationForm masterForm)
        //{
        //    if (MessageForm.ShowYesNo("是否确认审核？", "确认") == System.Windows.Forms.DialogResult.No)
        //        return;

        //    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
        //    {
        //        (new 凭证Dao()).Audit(rep, masterForm.DisplayManager.CurrentItem as 凭证);
        //    }
        //    masterForm.ControlManager.RaiseEntityChangedEvent(true);
        //}

        //public static void 撤销审核(ArchiveOperationForm masterForm)
        //{
        //    if (MessageForm.ShowYesNo("是否确认撤销审核？", "确认") == System.Windows.Forms.DialogResult.No)
        //        return;
        //    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
        //    {
        //        (new 凭证Dao()).Unaudit(rep, masterForm.DisplayManager.CurrentItem as 凭证);
        //    }
        //    masterForm.ControlManager.RaiseEntityChangedEvent(true);
        //}

        #region "凭证费用明细审核"
        //public static void 审核费用(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //    IBaseDao masterDao = ((IArchiveGrid)masterForm.ActiveGrid).Bll as IBaseDao;
        //    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //    MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //    if (detailCm.DisplayManager.Position == -1)
        //    {
        //        MessageForm.ShowError("请选择要核销的费用项目！");
        //        return;
        //    }

        //    if (detailCm.DisplayManagerT.CurrentEntity.费用项编号 == "000")
        //    {
        //        审核不核销费用(masterForm, detailForm, windowMenuInfo);
        //    }
        //    else
        //    {
        //        RepositoryHelper.Initialize(detailCm.DisplayManagerT.CurrentEntity.费用项, detailCm.DisplayManagerT.CurrentEntity);

        //        detailCm.DisplayManagerT.CurrentEntity.费用项 = EntityBufferCollection.Instance["费用项"].Get(detailCm.DisplayManagerT.CurrentEntity.费用项编号) as 费用项;

        //        凭证费用明细 entity = detailCm.DisplayManagerT.CurrentEntity;
        //        int? 费用类别编号 = entity.收付标志 == 收付标志.收 ? entity.费用项.收入类别 : entity.费用项.支出类别;

        //        if (费用类别编号.HasValue)
        //        {
        //            费用类别 费用类别 = EntityBufferCollection.Instance["费用类别"].Get(费用类别编号.Value) as 费用类别;
        //            if (费用类别.大类 == "业务常规" || 费用类别.大类 == "业务额外")
        //            {
        //                审核业务核销费用(masterForm, detailForm, windowMenuInfo);
        //            }
        //            else
        //            {
        //                审核非业务核销费用(masterForm, detailForm, windowMenuInfo);
        //            }
        //        }
        //        else
        //        {
        //            throw new ArgumentException("无有效费用类别！");
        //        }
        //    }
        //}


        //public static void 审核不核销费用(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //    IBaseDao masterDao = ((IArchiveGrid)masterForm.ActiveGrid).Bll as IBaseDao;
        //    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //    MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //    if (detailCm.DisplayManager.Position == -1)
        //    {
        //        MessageForm.ShowError("请选择要核销的费用项目！");
        //        return;
        //    }
        //    if (detailCm.DisplayManagerT.CurrentEntity.费用项编号 != "000")
        //    {
        //        MessageForm.ShowError("此项目不为应收应付款，不能核销！");
        //        return;
        //    }
        //    detailCm.DisplayManagerT.CurrentEntity.审核状态 = true;
        //    detailCm.RaiseEntityChangedEvent(true);

        //    memoryBll.Update(detailCm.DisplayManagerT.CurrentEntity);
        //}

        //public static void 审核业务核销费用(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //    IBaseDao masterDao = ((IArchiveGrid)masterForm.ActiveGrid).Bll as IBaseDao;
        //    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //    MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //    if (detailCm.DisplayManager.Position == -1)
        //    {
        //        MessageForm.ShowError("请选择要核销的费用项目！");
        //        return;
        //    }
        //    if (detailCm.DisplayManagerT.CurrentEntity.费用项编号 == "000")
        //    {
        //        MessageForm.ShowError("此项目为应收应付款，不能核销！");
        //        return;
        //    }

        //    ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo(108)) as ArchiveCheckForm;
        //    ISearchControl fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["相关人编号"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.相关人编号 };
        //    fc.ReadOnly = true;

        //    fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["费用项编号"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.费用项编号 };
        //    fc.ReadOnly = true;

        //    fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["收付标志"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.收付标志 };
        //    fc.ReadOnly = true;

        //    if (form.ShowDialog() == DialogResult.OK)
        //    {
        //        IList<费用> list = new List<费用>();
        //        foreach (object i in form.SelectedEntites)
        //        {
        //            list.Add(i as 费用);
        //        }

        //        AddFees(masterForm.DisplayManager.CurrentItem as 凭证, list, detailCm, memoryBll, memoryBll2, false);
        //    }
        //}


        //public static void 审核非业务核销费用(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //    IBaseDao masterDao = ((IArchiveGrid)masterForm.ActiveGrid).Bll as IBaseDao;
        //    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //    MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //    if (detailCm.DisplayManager.Position == -1)
        //    {
        //        MessageForm.ShowError("请选择要核销的费用项目！");
        //        return;
        //    }
        //    if (detailCm.DisplayManagerT.CurrentEntity.费用项编号 == "000")
        //    {
        //        MessageForm.ShowError("此项目为应收应付款，不能核销！");
        //        return;
        //    }

        //    ArchiveCheckForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo(109)) as ArchiveCheckForm;
        //    ISearchControl fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["相关人编号"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.相关人编号 };
        //    fc.ReadOnly = true;

        //    fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["费用项编号"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.费用项编号 };
        //    fc.ReadOnly = true;

        //    fc = (form.DisplayManager.SearchManager as ISearchManagerControls).SearchControls["收付标志"];
        //    fc.SelectedDataValues = new ArrayList { detailCm.DisplayManagerT.CurrentEntity.收付标志 };
        //    fc.ReadOnly = true;

        //    if (form.ShowDialog() == DialogResult.OK)
        //    {
        //        IList<费用> list = new List<费用>();
        //        foreach (object i in form.SelectedEntites)
        //        {
        //            list.Add(i as 费用);
        //        }

        //        AddFees(masterForm.DisplayManager.CurrentItem as 凭证, list, detailCm, memoryBll, memoryBll2, false);
        //    }
        //}

        //public static void 清空审核数据(ArchiveOperationForm masterForm)
        //{
        //    IControlManager<凭证费用明细> detailCm = (((IArchiveDetailFormWithDetailGrids)detailForm).DetailGrids[0] as IArchiveGrid).ControlManager as IControlManager<凭证费用明细>;
        //    IBaseDao masterDao = ((IArchiveGrid)masterForm.ActiveGrid).Bll as IBaseDao;
        //    MemoryDao<凭证费用明细> memoryBll = (masterDao.GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<凭证费用明细>;
        //    MemoryDao<费用> memoryBll2 = (((masterDao.GetSubDao(0) as IMasterDao).DetailDao as IBaseDao).GetSubDao(0) as IMasterDao).DetailMemoryDao as MemoryDao<费用>;

        //    if (detailCm.DisplayManager.Position == -1)
        //    {
        //        MessageForm.ShowError("请选择要核销的费用项目！");
        //        return;
        //    }

        //    if (MessageForm.ShowYesNo("是否要清空审核数据?", "确认") == DialogResult.No)
        //    {
        //        return;
        //    }

        //    RepositoryHelper.Initialize(detailCm.DisplayManagerT.CurrentEntity.费用, detailCm.DisplayManagerT.CurrentEntity);
        //    foreach (费用 i in detailCm.DisplayManagerT.CurrentEntity.费用)
        //    {
        //        i.凭证费用明细 = null;
        //        memoryBll2.Delete(i);
        //    }
        //    detailCm.DisplayManagerT.CurrentEntity.费用.Clear();

        //    detailCm.DisplayManagerT.CurrentEntity.审核状态 = false;
        //    detailCm.RaiseEntityChangedEvent(true);

        //    memoryBll.Update(detailCm.DisplayManagerT.CurrentEntity);
        //}
        #endregion

        public static void 新增成本发票(ArchiveOperationForm masterForm)
        {
            ArchiveDetailForm form = Feng.Windows.Utils.ArchiveFormFactory.GenerateArchiveDetailForm(ADInfoBll.Instance.GetWindowInfo("非业务财务_成本发票"), null);
            form.ControlManager.AddNew();
            form.UpdateContent();
            form.ControlManager.DisplayManager.DataControls["金额"].SelectedDataValue = masterForm.ControlManager.DisplayManager.DataControls["金额.数额"].SelectedDataValue;
            form.ControlManager.DisplayManager.DataControls["凭证号"].SelectedDataValue = masterForm.ControlManager.DisplayManager.DataControls["凭证号"].SelectedDataValue;

            DialogResult ret = form.ShowDialog();
        }

        public static void AddFees(凭证 master, IList<费用> list, IControlManager<凭证费用明细> detailCm)
        {
            AddFees(master, list, detailCm, true, null);
        }

        public static void AddFees(凭证 master, IList<费用> list, IControlManager<凭证费用明细> detailCm, bool add, 收付标志? asDzd收付标志)
        {
            if (list == null)
                return;

            List<费用> newList = new List<费用>();
            foreach (费用 i in list)
            {
                if (i.凭证费用明细 == null)
                {
                    newList.Add(i);
                }
            }

            IList<凭证费用明细> ret = new List<凭证费用明细>();
            if (!asDzd收付标志.HasValue)
            {
                // 费用实体类型. 收付标志, 费用项编号, 相关人编号
                Dictionary<Tuple<int, 收付标志, string, string>, IList<费用>> dict = Utility.GroupFyToPzYsyf(newList);

                foreach (KeyValuePair<Tuple<int, 收付标志, string, string>, IList<费用>> kvp in dict)
                {
                    凭证费用明细 pzs1 = new 凭证费用明细();

                    decimal sum = 0;
                    foreach (费用 k4 in kvp.Value)
                    {
                        sum += k4.金额.Value;
                        k4.凭证费用明细 = pzs1;
                    }

                    //string s = NameValueMappingCollection.Instance.FindNameFromId("信息_业务类型_全部", kvp.Key.Item1);
                    //if (string.IsNullOrEmpty(s))
                    //{
                    //    pzs1.业务类型编号 = null;
                    //}
                    //else
                    //{
                    //    pzs1.业务类型编号 = kvp.Key.Item1;
                    //}
                    pzs1.业务类型编号 = kvp.Key.Item1;

                    pzs1.费用 = kvp.Value;
                    pzs1.费用项编号 = kvp.Key.Item3;
                    pzs1.金额 = sum;
                    pzs1.收付标志 = kvp.Key.Item2;
                    pzs1.相关人编号 = kvp.Key.Item4;

                    // pzs1.凭证 = pz;

                    ret.Add(pzs1);
                }
            }
            else
            {
                Dictionary<Tuple<int, string>, IList<费用>> dict = Utility.GroupFyToDzdYsyf(newList);

                foreach (KeyValuePair<Tuple<int, string>, IList<费用>> kvp in dict)
                {
                    凭证费用明细 pzs1 = new 凭证费用明细();

                    decimal sum = 0;
                    foreach (费用 k4 in kvp.Value)
                    {
                        if (k4.收付标志 == asDzd收付标志.Value)
                        {
                            sum += k4.金额.Value;
                        }
                        else
                        {
                            sum -= k4.金额.Value;
                        }
                        k4.凭证费用明细 = pzs1;
                    }

                    //string s = NameValueMappingCollection.Instance.FindNameFromId("信息_业务类型_全部", kvp.Key.First);
                    //if (string.IsNullOrEmpty(s))
                    //{
                    //    pzs1.业务类型编号 = null;
                    //}
                    //else
                    //{
                    //    pzs1.业务类型编号 = kvp.Key.First;
                    //}
                    pzs1.业务类型编号 = kvp.Key.Item1;

                    pzs1.费用 = kvp.Value;
                    pzs1.费用项编号 = "000";    // 常规应收应付
                    pzs1.金额 = sum;
                    pzs1.收付标志 = asDzd收付标志.Value;
                    pzs1.相关人编号 = kvp.Key.Item2;

                    // pzs1.凭证 = pz;

                    ret.Add(pzs1);
                }
            }

            if (add)
            {
                foreach (凭证费用明细 item in ret)
                {
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = item;
                    detailCm.EndEdit();

                    foreach (费用 i in item.费用)
                    {
                        i.凭证费用明细 = item;
                    }
                }
            }
            else
            {
                if (ret.Count == 0)
                    return;

                System.Diagnostics.Debug.Assert(ret.Count <= 1, "选出多个凭证费用明细，请查证！");
                System.Diagnostics.Debug.Assert(ret[0].费用项编号 == detailCm.DisplayManagerT.CurrentEntity.费用项编号, "凭证费用明细费用项和选择的费用项不同！");
                System.Diagnostics.Debug.Assert(ret[0].相关人编号 == detailCm.DisplayManagerT.CurrentEntity.相关人编号, "凭证费用明细费用项和选择的相关人不同！");
                System.Diagnostics.Debug.Assert(ret[0].收付标志 == detailCm.DisplayManagerT.CurrentEntity.收付标志, "凭证费用明细费用项和选择的相关人不同！");

                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用>())
                {
                    rep.Initialize(detailCm.DisplayManagerT.CurrentEntity.费用, detailCm.DisplayManagerT.CurrentEntity);
                }

                if (detailCm.DisplayManagerT.CurrentEntity.费用 == null)
                {
                    detailCm.DisplayManagerT.CurrentEntity.费用 = new List<费用>();
                }
                foreach (费用 i in ret[0].费用)
                {
                    i.凭证费用明细 = detailCm.DisplayManagerT.CurrentEntity;
                }

                detailCm.EditCurrent();
                detailCm.EndEdit();
            }
        }

        public static void 打印凭证(ArchiveOperationForm masterForm)
        {
            凭证 pz = masterForm.DisplayManager.CurrentItem as 凭证;
            if (pz == null)
            {
                MessageForm.ShowError("请选择要打印的凭证！");
                return;
            }

            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
            {
                rep.Initialize(pz.凭证费用明细, pz);
            }

            MyReportForm form = new MyReportForm("报表_凭证");

            form.FillDataSet(0, new List<凭证> { pz });
            form.FillDataSet(1, pz.凭证费用明细);

            //System.Data.DataSet ds = form.TemplateDataSet;
            //ds.Tables["凭证"].Rows[0]["大写金额"] = ChineseHelper.ConvertToChinese(pz.金额.数额.Value);
            //ds.Tables["凭证"].Rows[0]["金额.币制编号"] = NameValueMappingCollection.Instance.FindColumn2FromColumn1("财务_币制", "代码", "符号", pz.金额.币制编号);

            form.Show();

            //Hd.Report.凭证Ds ds = new Hd.Report.凭证Ds();
            //Hd.Report.凭证Ds.凭证Row row = ds.凭证.New凭证Row();
            //row["凭证号"] = pz.凭证号;
            //row["日期"] = pz.日期;
            //row["相关人编号"] = NameValueMappingCollection.Instance.FindNameFromId("人员单位_全部", pz.相关人编号);
            ////row["项目名称"] = "000";
            //row["金额.数额"] = pz.金额.数额;
            //row["大写金额"] = Feng.Utils.ChineseHelper.ConvertToChinese(pz.金额.数额.Value);
            //row["备注"] = pz.备注;
            //ds.凭证.Rows.Add(row);
            //FormRptPreView frmReport = new FormRptPreView(ds, new Hd.Report.凭证());
            //frmReport.Show();
        }

        public static void 应收应付出凭证(ArchiveSeeForm masterForm)
        {
            if (masterForm.DisplayManager.CurrentItem == null)
            {
                MessageForm.ShowError("请选择相关人！");
                return;
            }
            System.Data.DataRowView row = masterForm.DisplayManager.CurrentItem as System.Data.DataRowView;

            IControlManager cm = (masterForm.ArchiveDetailForm as IControlManagerContainer).ControlManager;
            cm.AddNew();
            凭证 pz = cm.DisplayManager.CurrentItem as 凭证;
            pz.凭证类别 = 凭证类别.付款凭证;  //row["收付标志"].ToString() == "1" ? 凭证类别.收款凭证 : 凭证类别.付款凭证;
            pz.自动手工标志 = 自动手工标志.手工;
            pz.操作人 = "会计";
            pz.相关人编号 = row["相关人"].ToString();

            masterForm.ShowArchiveDetailForm();
        }
    }
}
