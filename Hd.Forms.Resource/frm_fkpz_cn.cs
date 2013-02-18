using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Feng.Windows.Utils;
using Hd.Model;

namespace Hd.Forms
{
    public partial class frm_fkpz_cn : ArchiveDetailFormWithDetailGrids
    {
        public frm_fkpz_cn()
        {
            InitializeComponent();

            // 如果在Constructor中调用，还未有ControlManager，不能添加到StateControls中
            this.AddDetailGrid(grdFymx);
            this.AddDetailGrid(grdZfmx);
        }

        protected override void Form_Load(object sender, EventArgs e)
        {
            this.ControlManager.StateControls.Add(grdFymx);
            this.ControlManager.StateControls.Add(grdZfmx);

            grdZfmx.DisplayManager.SelectedDataValueChanged += new EventHandler<SelectedDataValueChangedEventArgs>(DisplayManagerPzsfmx_SelectedDataValueChanged);
            grdZfmx.InsertionRow.EditBegun += new EventHandler(InsertionRow2_EditBegun);

            base.Form_Load(sender, e);
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

        static void InsertionRow2_EditBegun(object sender, EventArgs e)
        {
            Xceed.Grid.InsertionRow row = sender as Xceed.Grid.InsertionRow;
            row.Cells["金额"].Value = ((row.GridControl.FindForm() as ArchiveDetailForm).DisplayManager.CurrentItem as 凭证).金额.数额;
            row.Cells["收付标志"].Value = 收付标志.付;
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

        public override bool DoSave()
        {
            凭证 pz = this.ControlManager.DisplayManager.CurrentItem as 凭证;
            pz.出纳编号 = SystemConfiguration.UserName;
            pz.操作人 = "出纳";

            return base.DoSave();
        }
    }
}
