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
    public partial class frm_fkpz_kj : ArchiveDetailFormWithDetailGrids
    {
        public frm_fkpz_kj()
        {
            InitializeComponent();

            // 如果在Constructor中调用，还未有ControlManager，不能添加到StateControls中
            this.AddDetailGrid(grdFymx);
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
            //if (付款对象.SelectedDataValue != null)
            //{
            //    NameValueMappingCollection.Instance["信息_凭证费用类别_动态"].Params["@收付标志"] = Hd.Model.收付标志.付;
            //    NameValueMappingCollection.Instance.Reload("信息_凭证费用类别_动态");
            //    付款对象.ReadOnly = true;
            //}

            相关人编号.SelectedDataValue = 付款对象.SelectedDataValue;

            SetTopGrid();
        }

        protected override void Form_Load(object sender, EventArgs e)
        {
            grdFymx.VisibleColumns(false);
            //this.ControlManager.StateControls.Add(grdFymx);
            grdFymx.ReadOnly = true;

            base.Form_Load(sender, e);

            //GeneratedArchiveOperationForm.AddSelectedDataValuesChangedEvent(this.DisplayManager);
            //GeneratedArchiveOperationForm.AddSelectedDataValuesChangedEvent(grdFymx.DisplayManager);

            用途分类.SelectedDataValueChanged += new EventHandler(用途分类_SelectedDataValueChanged);
            付款对象.SelectedDataValueChanged += new EventHandler(付款对象_SelectedDataValueChanged);

            grdFymx.DataRowTemplate.EditEnded += new EventHandler(DataRowTemplate_EditEnded);
        }

        void DataRowTemplate_EditEnded(object sender, EventArgs e)
        {
            Xceed.Grid.DataRow row = sender as Xceed.Grid.DataRow;
            if (row.Cells["费用项"].Value != null)
            {
                if (row.Cells["费用项"].Value.ToString() == "011")
                {
                    ArchiveDetailForm detailForm = ArchiveFormFactory.GenerateArchiveDetailForm(ADInfoBll.Instance.GetWindowInfo("资金票据_凭证_会计付款_固定资产"));
                    if (detailForm != null)
                    {
                        detailForm.ControlManager.AddNew();
                        detailForm.UpdateContent();
                        detailForm.DisplayManager.DataControls["购入金额"].SelectedDataValue = row.Cells["金额"].Value;
                        Hd.Model.Kj.固定资产 entity = detailForm.DisplayManager.CurrentItem as Hd.Model.Kj.固定资产;
                        if (this.DisplayManager.DataControls["日期"].SelectedDataValue != null)
                        {
                            entity.购入时间 = (DateTime)this.DisplayManager.DataControls["日期"].SelectedDataValue;
                        }
                        else
                        {
                            entity.购入时间 = System.DateTime.Today;
                        }

                        detailForm.ShowDialog();
                    }
                }
                else if (row.Cells["费用项"].Value.ToString() == "012")
                {
                    ArchiveDataControlForm form = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("资金票据_凭证_会计付款_结算期限")) as ArchiveDataControlForm;
                    if (form != null)
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            row.Cells["结算期限"].Value = form.DataControls["结算期限"].SelectedDataValue;
                            row.Cells["业务分类"].Value = form.DataControls["业务分类"].SelectedDataValue;
                        }

                    }
                }
            }
        }

        private void SetTopGrid()
        {
            if (用途分类.SelectedDataValue == null)
                return;
            
            凭证用途分类 ytfl = (凭证用途分类)用途分类.SelectedDataValue;
            if (ytfl == 凭证用途分类.业务应付)
            {
                if (付款对象.SelectedDataValue == null)
                    return;
                ArchiveFormFactory.SetupDataUnboundGrid(grdTop, ADInfoBll.Instance.GetWindowTabInfo("资金票据_应收应付当前"));
                grdTop.DisplayManager.SearchManager.LoadData(SearchExpression.And(SearchExpression.Eq("相关人", 付款对象.SelectedDataValue), 
                    SearchExpression.Eq("收付标志", Hd.Model.收付标志.付)) , null);
            }
        }
        void 用途分类_SelectedDataValueChanged(object sender, EventArgs e)
        {
            //业务分类.ReadOnly = false;
            label10.Text = "业务分类";
            业务分类.Control.Text = string.Empty;

            MyGrid.CancelEditCurrentDataRow(grdFymx);
            grdFymx.DataRows.Clear();
            grdFymx.VisibleColumns(false);
           

            if (用途分类.SelectedDataValue == null)
                return;

            凭证用途分类 ytfl = (凭证用途分类)用途分类.SelectedDataValue;

            int? pzfylb = null;
            string[] neededColumns = null;
            if (ytfl == 凭证用途分类.业务应付)          
            {
                pzfylb = 102;   // 业务应付
                neededColumns = new string[] {"费用项", "金额", "支付方式要求", "备注"};
            }
            else if (ytfl == 凭证用途分类.业务报销)     // 业务报销
            {
                pzfylb = 102;
                neededColumns = new string[] { "费用项", "金额", "自编号", "箱号" };
            }
            else if (ytfl == 凭证用途分类.其他应付)     //其他应付
            {
                // "费用项" = "002"
                neededColumns = new string[] { "金额", "支付方式要求", "备注" };
            }
            //else if (ytfl == 凭证用途分类.代收代付)    // 借款套现押金
            //{
            //    // "费用项" = "012"
            //    // no pzfylb;
            //    neededColumns = new string[] { "凭证费用类别", "业务分类", "金额", "结算期限", "支付方式要求", "备注" };
            //}
            else if (ytfl == 凭证用途分类.其他报销)
            {
                // no pzfylb;
                neededColumns = new string[] { "凭证费用类别", "费用项", "金额", "支付方式要求", "备注" };
            }

            foreach(Xceed.Grid.Column column in grdFymx.Columns)
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

            if (pzfylb.HasValue)
            {
                //// only for 报销
                //NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].Params["@收付标志"] = Hd.Model.收付标志.付;
                //NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].Params["@凭证费用类别"] = pzfylb;
                //NameValueMappingCollection.Instance.Reload("信息_凭证业务类型_动态");

                //(业务分类.Control as MyComboBox).RefreshData();

                label10.Text = "业务分类";
                (业务分类.Control as MyComboBox).ValidateText = true;
                业务分类.ReadOnly = false;
                用途分类.Focus();
            }
            else
            {
                label10.Text = "部门";
                (业务分类.Control as MyComboBox).TextBoxArea.Text = "进口部";
                (业务分类.Control as MyComboBox).ValidateText = false;
                业务分类.ReadOnly = true;
                用途分类.Focus();
            }

            //if (!业务分类.ReadOnly)
            //{
            //    DataTable dt = NameValueMappingCollection.Instance.DataTable("信息_凭证业务类型_动态");
            //    if (dt.Rows.Count == 0)
            //    {
            //        业务分类.ReadOnly = true;
            //    }
            //    else
            //    {
            //        业务分类.ReadOnly = false;
            //        if (dt.Rows.Count == 0)
            //        {
            //            业务分类.SelectedDataValue = dt.Rows[0][NameValueMappingCollection.Instance["信息_凭证业务类型_动态"].ValueMember];
            //        }
            //    }
            //    //(业务分类.Control as MyComboBox).SetDataBinding(
            //    //        DbHelper.Instance.ExecuteDataTable("SELECT 代码, 类型 FROM 信息_凭证业务分类 WHERE 付 = 'true' AND 支出类别 = '" + ytfl + "'"), string.Empty);
            //}
            //else
            //{
            //    业务分类.SelectedDataValue = null;
            //}

            //if (!grdFymx.Columns["凭证费用类别"].ReadOnly)
            //{
            //    //if (ytfl != "999")
            //    {
            //        NameValueMappingCollection.Instance["信息_凭证费用类别_动态"].Params["@收付标志"] = Hd.Model.收付标志.付;
            //        NameValueMappingCollection.Instance.Reload("信息_凭证费用类别_动态");


            //        NameValueMappingCollection.Instance["信息_凭证费用项_动态"].Params["@收付标志"] = Hd.Model.收付标志.付;
            //        NameValueMappingCollection.Instance["信息_凭证费用项_动态"].Params["@凭证费用类别"] = ytfl;
            //        NameValueMappingCollection.Instance.Reload("信息_凭证费用项_动态");
            //    }
            //    //else
            //    //{

            //    //}
            //    //(grdFymx.Columns["费用项"].CellEditorManager as Feng.Grid.Editors.MyComboBoxEditor).SetDataBinding(
            //    //    DbHelper.Instance.ExecuteDataTable("SELECT 编号, 名称 FROM 信息_凭证费用项 WHERE 付 = 'true' AND 凭证支出类别 = '" + ytfl + "'"), string.Empty);
            //}

            用途分类.ReadOnly = true;
            grdFymx.ReadOnly = false;

            SetTopGrid();
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
            pz.操作人 = "会计";
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.凭证>() as Feng.NH.INHibernateRepository)
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
                        pzfymx.金额 =je;
                        pzfymx.凭证 = pz;
                        pzfymx.凭证费用类别编号 = (int?)row.Cells["凭证费用类别"].Value;
                        pzfymx.收付标志 = Hd.Model.收付标志.付;
                        pzfymx.相关人编号 = (string)付款对象.SelectedDataValue;
                        pzfymx.业务类型编号 = (int?)row.Cells["业务分类"].Value;
                        pzfymx.费用项编号 = (string)row.Cells["费用项"].Value;
                        pzfymx.费用 = new List<费用>();
                        pzfymx.结算期限 = (DateTime?)row.Cells["结算期限"].Value;

                        凭证用途分类 ytfl = (凭证用途分类)用途分类.SelectedDataValue;

                        if (ytfl == 凭证用途分类.业务应付)          // 业务应付
                        {
                            pzfymx.业务类型编号 = (int?)业务分类.SelectedDataValue;
                        }
                        else if (ytfl == 凭证用途分类.其他应付)
                        {
                            pzfymx.业务类型编号 = 111;
                            pzfymx.费用项编号 = "002";
                        }
                        else if (ytfl == 凭证用途分类.其他报销)
                        {
                            pzfymx.费用项编号 = (string)row.Cells["费用项"].Value;
                        }
                        else if (ytfl == 凭证用途分类.业务报销)
                        {
                            普通票 piao = null;
                            普通箱 xiang = null;

                            piao = rep.UniqueResult<Hd.Model.普通票>(NHibernate.Criterion.DetachedCriteria.For<Hd.Model.普通票>()
                                           .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value)));

                            if (piao == null)
                            {
                                throw new InvalidUserOperationException("自编号" + (string)row.Cells["自编号"].Value + "输入有误，请重新输入！");
                            }
                            switch (业务分类.SelectedDataValue.ToString())
                            {
                                case "11":
                                    using (var rep2 = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口箱>() as Feng.NH.INHibernateRepository)
                                    {
                                       
                                        if (row.Cells["箱号"].Value != null)
                                        {
                                            xiang = rep2.UniqueResult<Hd.Model.Jk.进口箱>(NHibernate.Criterion.DetachedCriteria.For<Hd.Model.Jk.进口箱>()
                                                .Add(NHibernate.Criterion.Expression.Eq("箱号", row.Cells["箱号"].Value))
                                                .CreateCriteria("票")
                                                .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value)));
                 
                                            if (xiang == null)
                                            {
                                                throw new InvalidUserOperationException("箱号" + (string)row.Cells["箱号"].Value + "输入有误，请重新输入！");
                                            }
                                        }
                                    }
                                    break;
                                case "15":
                                    using (var rep2 = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口箱>() as Feng.NH.INHibernateRepository)
                                    {
                                        //piao = rep2.Session.CreateCriteria<Hd.Model.普通票>()
                                        //        .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value))
                                        //        .UniqueResult<Hd.Model.普通票>();
                                        //if (piao == null)
                                        //{
                                        //    throw new InvalidUserOperationException("自编号" + (string)row.Cells["自编号"].Value + "输入有误，请重新输入！");
                                        //}
                                        if (row.Cells["箱号"].Value != null)
                                        {
                                            xiang = rep2.UniqueResult<Hd.Model.Nmcg.内贸出港箱>(NHibernate.Criterion.DetachedCriteria.For<Hd.Model.Nmcg.内贸出港箱>()
                                                .Add(NHibernate.Criterion.Expression.Eq("箱号", row.Cells["箱号"].Value))
                                                .CreateCriteria("票")
                                                .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value)));

                                            if (xiang == null)
                                            {
                                                throw new InvalidUserOperationException("箱号" + (string)row.Cells["箱号"].Value + "输入有误，请重新输入！");
                                            }
                                        }
                                    }
                                    break;
                                case "45":
                                    using (var rep2 = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<Hd.Model.Jk.进口票>() as Feng.NH.INHibernateRepository)
                                    {
                                        //piao = rep2.Session.CreateCriteria<Hd.Model.普通票>()
                                        //        .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value))
                                        //        .UniqueResult<Hd.Model.普通票>();
                                        //if (piao == null)
                                        //{
                                        //    throw new InvalidUserOperationException("自编号" + (string)row.Cells["自编号"].Value + "输入有误，请重新输入！");
                                        //}
                                        if (row.Cells["箱号"].Value != null)
                                        {
                                            xiang = rep2.UniqueResult<Hd.Model.Jk2.进口其他业务箱>(NHibernate.Criterion.DetachedCriteria.For<Hd.Model.Jk2.进口其他业务箱>()
                                               .Add(NHibernate.Criterion.Expression.Eq("箱号", row.Cells["箱号"].Value))
                                               .CreateCriteria("票")
                                               .Add(NHibernate.Criterion.Expression.Eq("货代自编号", row.Cells["自编号"].Value)));

                                            if (xiang == null)
                                            {
                                                throw new InvalidUserOperationException("箱号" + (string)row.Cells["箱号"].Value + "输入有误，请重新输入！");
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    throw new ArgumentException("不合理的业务类型分类！");

                            }

                            NHibernate.Criterion.DetachedCriteria cri = NHibernate.Criterion.DetachedCriteria.For<业务费用>()
                                .Add(NHibernate.Criterion.Expression.Eq("票.ID", piao.ID))
                                .Add(NHibernate.Criterion.Expression.Eq("费用项编号", row.Cells["费用项"].Value))
                                .Add(NHibernate.Criterion.Expression.Eq("收付标志", Hd.Model.收付标志.付));
                            if (xiang != null)
                            {
                                cri = cri.Add(NHibernate.Criterion.Expression.Eq("箱.ID", xiang.ID));
                            }
                            else
                            {
                                cri = cri.Add(NHibernate.Criterion.Expression.IsNull("箱"));
                            }

                            IList<业务费用> list = rep.List<业务费用>(cri);


                            cri = NHibernate.Criterion.DetachedCriteria.For<费用信息>()
                                                .Add(NHibernate.Criterion.Expression.Eq("票.ID", piao.ID))
                                                .Add(NHibernate.Criterion.Expression.Eq("费用项编号", row.Cells["费用项"].Value));
                            IList<费用信息> fyxxs = rep.List<费用信息>(cri);
                            if (fyxxs.Count > 0)
                            {
                                if (!fyxxs[0].完全标志付)
                                {
                                    fyxxs[0].完全标志付 = true;
                                    (new HdBaseDao<费用信息>()).Update(rep, fyxxs[0]);
                                }
                                else
                                {
                                    throw new InvalidUserOperationException("货代自编号" + piao.货代自编号 + "费用项" + row.Cells["费用项"].Value.ToString()
                                    + "已打完全标志，不能修改费用！");
                                }
                            }
                            bool exist = false;
                            if (list.Count > 0)
                            {
                                foreach (业务费用 i in list)
                                {
                                    if (i.金额 == je && i.相关人编号 == pzfymx.相关人编号 && i.凭证费用明细 == null)
                                    {
                                        i.凭证费用明细 = pzfymx;
                                        (new 业务费用Dao()).Update(rep, i);
                                        pzfymx.费用.Add(i);

                                        exist = true;
                                        break;
                                    }
                                }
                            }
                            if (!exist)
                            {
                                if (list.Count > 1)
                                {
                                    throw new InvalidUserOperationException("货代自编号" + piao.货代自编号 + "费用项" + row.Cells["费用项"].Value.ToString()
                                        + "已存在多条费用，且无费用金额一致，请先修改一致！");
                                }
                                else if (list.Count == 0)
                                {
                                    业务费用 fy = new 业务费用();
                                    fy.备注 = (string)row.Cells["备注"].Value;
                                    fy.费用实体 = piao;
                                    fy.费用项编号 = (string)row.Cells["费用项"].Value;
                                    fy.金额 = Feng.Utils.ConvertHelper.ToDecimal(row.Cells["金额"].Value).Value;
                                    fy.票 = piao;
                                    fy.凭证费用明细 = pzfymx;
                                    fy.收付标志 = Hd.Model.收付标志.付;
                                    fy.相关人编号 = (string)付款对象.SelectedDataValue;
                                    if (xiang != null)
                                    {
                                        fy.箱 = xiang;
                                        fy.箱Id = xiang.ID;
                                    }
                                    (new 业务费用Dao()).Save(rep, fy);
                                    pzfymx.费用.Add(fy);
                                }
                                else// if (list.Count == 1)
                                {
                                    if (list[0].相关人编号 == pzfymx.相关人编号 && list[0].凭证费用明细 == null)
                                    {
                                        if (MessageForm.ShowYesNo("货代自编号" + piao.货代自编号 + "费用项" + row.Cells["费用项"].Value.ToString()
                                            + "已存在费用，且费用金额不符，是否添加调节款？", "确认"))
                                        {
                                            调节业务款 tjk = new 调节业务款();
                                            tjk.备注 = (string)row.Cells["备注"].Value;
                                            tjk.费用实体 = piao;
                                            tjk.费用项编号 = (string)row.Cells["费用项"].Value;
                                            tjk.金额 = Feng.Utils.ConvertHelper.ToDecimal(row.Cells["金额"].Value).Value;
                                            tjk.票 = piao;
                                            tjk.凭证费用明细 = pzfymx;
                                            tjk.收付标志 = Hd.Model.收付标志.付;
                                            tjk.相关人编号 = (string)付款对象.SelectedDataValue;
                                            if (xiang != null)
                                            {
                                                tjk.箱 = xiang;
                                                tjk.箱Id = xiang.ID;
                                            }
                                            (new 费用Dao()).Save(rep, tjk);
                                            pzfymx.费用.Add(tjk);
                                        }
                                        else
                                        {
                                            throw new InvalidUserOperationException("请重新填写货代自编号！");
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidUserOperationException("货代自编号" + piao.货代自编号 + "费用项" + row.Cells["费用项"].Value.ToString()
                                        + "已存在的一条费用相关人不符或已经出国凭证，请先修改一致！");
                                    }

                                }
                            }
                        }
                        else
                        {
                            throw new ArgumentException("不合理的凭证用途分类！");
                        }
                        pz.凭证费用明细.Add(pzfymx);
                    }
                    
                    pz.会计编号 = SystemConfiguration.UserName;
                    pz.会计金额 = sum;
                    pz.金额.币制编号 = "CNY";
                    pz.金额.数额 = pz.会计金额;
                    pz.凭证类别 = 凭证类别.付款凭证;
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

                    // don't save to database
                    this.ControlManager.EndEdit(false);

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

