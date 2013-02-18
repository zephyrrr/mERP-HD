using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Feng.Windows.Utils;
using Feng.Data;
using Hd.Model;

namespace Hd.Forms
{
    public partial class frm_skpz_cn : ArchiveDetailFormWithDetailGrids
    {
        public frm_skpz_cn()
        {
            InitializeComponent();

            // 如果在Constructor中调用，还未有ControlManager，不能添加到StateControls中
            this.AddDetailGrid(grdFymx);
            this.AddDetailGrid(grdZfmx);

            用途分类.SelectedDataValueChanged += new EventHandler(用途分类_SelectedDataValueChanged);
            付款对象.SelectedDataValueChanged += new EventHandler(付款对象_SelectedDataValueChanged);

            日期.SelectedDataValue = System.DateTime.Today;
        }
        private void myCurrencyTextBox1_TextChanged(object sender, EventArgs e)
        {
            decimal? d = Feng.Utils.ConvertHelper.ToDecimal(myCurrencyTextBox1.Text);
            if (d.HasValue)
            {
                大写金额.SelectedDataValue = ChineseHelper.ConvertToChinese(d.Value);
            }
            else
            {
                大写金额.SelectedDataValue = null;
            }
        }

        void 付款对象_SelectedDataValueChanged(object sender, EventArgs e)
        {
            IDisplayManager dm = sender as IDisplayManager;
            if (付款对象.SelectedDataValue != null)
            {
                NameValueMappingCollection.Instance["信息_凭证费用类别_动态"].Params["@收付标志"] = Hd.Model.收付标志.收;
                NameValueMappingCollection.Instance.Reload(dm.Name, "信息_凭证费用类别_动态");
                付款对象.ReadOnly = true;
            }

            相关人编号.SelectedDataValue = 付款对象.SelectedDataValue;
        }

        protected override void Form_Load(object sender, EventArgs e)
        {
            grdFymx.VisibleColumns(false);
            //this.ControlManager.StateControls.Add(grdFymx);
            grdFymx.ReadOnly = true;

            this.ControlManager.StateControls.Add(grdZfmx);

            grdZfmx.DisplayManager.SelectedDataValueChanged += new EventHandler<SelectedDataValueChangedEventArgs>(DisplayManagerPzsfmx_SelectedDataValueChanged);
            grdZfmx.InsertionRow.EditBegun += new EventHandler(InsertionRow2_EditBegun);

            base.Form_Load(sender, e);
        }

        static void InsertionRow2_EditBegun(object sender, EventArgs e)
        {
            Xceed.Grid.InsertionRow row = sender as Xceed.Grid.InsertionRow;
            row.Cells["金额"].Value = ((row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.CurrentItem as 凭证).金额.数额;
            row.Cells["收付标志"].Value = 收付标志.收;
            (row.GridControl as IArchiveGrid).DisplayManager.OnSelectedDataValueChanged(new SelectedDataValueChangedEventArgs("收付标志", row.Cells["收付标志"]));
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

        void 用途分类_SelectedDataValueChanged(object sender, EventArgs e)
        {
            业务分类.ReadOnly = false;
            业务分类.SelectedDataValue = null;
            grdFymx.DataRows.Clear();
            grdFymx.VisibleColumns(false);
            MyGrid.CancelEditCurrentDataRow(grdFymx);

            if (用途分类.SelectedDataValue == null)
                return;

            IDisplayManager dm = sender as IDisplayManager;

            int ytfl = (int)用途分类.SelectedDataValue;

            string[] neededColumns = null;
            if (ytfl == 101)          // 业务应收
            {
                neededColumns = new string[] { "费用项", "金额", "备注" };
            }
            else if (ytfl == 103)     //其他应付
            {
                // "费用项" = "002"
                neededColumns = new string[] { "金额", "备注" };
            }
            else if (ytfl == 203 || ytfl == 204 || ytfl == 205)    // 借款套现押金
            {
                // "费用项" = "012"
                neededColumns = new string[] { "金额", "备注" };
            }
            else
            {
                neededColumns = new string[] { "费用项", "金额", "备注" };
            }

            foreach (Xceed.Grid.Column column in grdFymx.Columns)
            {
                if (Array.IndexOf<string>(neededColumns, column.FieldName) != -1)
                {
                    column.Visible = true;
                    column.ReadOnly = false;
                }
                else
                {
                    column.Visible = false;
                    column.ReadOnly = true;
                }
            }
            grdFymx.AutoAdjustColumnWidth();

            NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].Params["@收付标志"] = Hd.Model.收付标志.收;
            NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].Params["@凭证费用类别"] = ytfl;
            NameValueMappingCollection.Instance.Reload(dm.Name, "信息_凭证业务类型_动态");

            //(业务分类.Control as MyComboBox).RefreshData();

            if (!业务分类.ReadOnly)
            {
                DataView dv = NameValueMappingCollection.Instance.GetDataSource(dm.Name, "信息_凭证业务类型_动态");
                if (dv.Count == 0)
                {
                    业务分类.ReadOnly = true;
                }
                else
                {
                    业务分类.ReadOnly = false;
                    if (dv.Count == 0)
                    {
                        业务分类.SelectedDataValue = dv[0][NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].ValueMember];
                    }
                }
                //(业务分类.Control as MyComboBox).SetDataBinding(
                //        DbHelper.Instance.ExecuteDataTable("SELECT 代码, 类型 FROM 信息_凭证业务分类 WHERE 付 = 'true' AND 支出类别 = '" + ytfl + "'"), string.Empty);
            }
            else
            {
                业务分类.SelectedDataValue = null;
            }

            if (!grdFymx.Columns["费用项"].ReadOnly)
            {
                //if (ytfl != "999")
                {
                    NameValueMappingCollection.Instance["信息_凭证费用项_动态"].Params["@收付标志"] = Hd.Model.收付标志.收;
                    NameValueMappingCollection.Instance["信息_凭证费用项_动态"].Params["@凭证费用类别"] = ytfl;
                    NameValueMappingCollection.Instance.Reload(dm.Name, "信息_凭证费用项_动态");
                }
                //else
                //{

                //}
                //(grdFymx.Columns["费用项"].CellEditorManager as Feng.Grid.Editors.MyComboBoxEditor).SetDataBinding(
                //    DbHelper.Instance.ExecuteDataTable("SELECT 编号, 名称 FROM 信息_凭证费用项 WHERE 付 = 'true' AND 凭证支出类别 = '" + ytfl + "'"), string.Empty);
            }

            用途分类.ReadOnly = true;
            grdFymx.ReadOnly = false;
        }

        public override bool DoSave()
        {
            //if (!this.ControlManager.CheckControlValue())
            //{
            //    return false;
            //}
            this.ValidateChildren();
            if (!this.ControlManager.SaveCurrent())
                return false;

            凭证 pz = this.ControlManager.DisplayManager.CurrentItem as 凭证;
            pz.操作人 = "出纳";
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.凭证>())
            {
                try
                {
                    rep.BeginTransaction();

                    decimal sum = 0;

                    pz.凭证费用明细 = new List<凭证费用明细>();
                    foreach (Xceed.Grid.DataRow row in grdFymx.DataRows)
                    {
                        decimal je = Feng.Utils.ConvertHelper.ToDecimal(row.Cells["金额"].Value).Value;
                        sum += je;

                        凭证费用明细 pzfymx = new 凭证费用明细();
                        pzfymx.备注 = (string)row.Cells["备注"].Value;
                        pzfymx.金额 = je;
                        pzfymx.凭证 = pz;
                        pzfymx.凭证费用类别编号 = (int?)用途分类.SelectedDataValue;
                        pzfymx.收付标志 = 收付标志.收;
                        pzfymx.相关人编号 = (string)付款对象.SelectedDataValue;
                        pzfymx.业务类型编号 = (int?)业务分类.SelectedDataValue;
                        pzfymx.费用项编号 = (string)row.Cells["费用项"].Value;
                        pzfymx.费用 = new List<费用>();

                        int ytfl = (int)用途分类.SelectedDataValue;

                        if (ytfl == 101)          // 业务应付
                        {
                            pzfymx.费用项编号 = (string)row.Cells["费用项"].Value;
                        }
                        else if (ytfl == 103)     //其他应付
                        {
                            pzfymx.费用项编号 = "002";
                        }
                        else if (ytfl == 203 || ytfl == 204 || ytfl == 205)    // 借款套现押金
                        {
                            pzfymx.费用项编号 = "012";
                        }
                        else
                        {
                            pzfymx.费用项编号 = (string)row.Cells["费用项"].Value;
                        }

                        pz.凭证费用明细.Add(pzfymx);
                    }

                    pz.出纳编号 = SystemConfiguration.UserName;
                    pz.会计金额 = sum;
                    pz.金额.币制编号 = "CNY";
                    pz.金额.数额 = pz.会计金额;
                    pz.凭证类别 = 凭证类别.收款凭证;
                    pz.相关人编号 = (string)付款对象.SelectedDataValue;
                    pz.自动手工标志 = 自动手工标志.手工;

                    (new 凭证Dao()).Save(rep, pz);

                    foreach (凭证费用明细 pzfymx in pz.凭证费用明细)
                    {
                        (new HdBaseDao<凭证费用明细>()).Save(rep, pzfymx);
                    }

                    (new 凭证Dao()).Submit(rep, pz);

                    rep.CommitTransaction();

                    if (this.ControlManager.ControlCheckExceptionProcess != null)
                    {
                        this.ControlManager.ControlCheckExceptionProcess.ClearAllError();
                    }
                    this.ControlManager.State = StateType.View;
                    this.ControlManager.OnCurrentItemChanged();

                    return true;
                }
                catch (Exception ex)
                {
                    rep.RollbackTransaction();
                    ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                    return false;
                }
            }
        }
    }
}
