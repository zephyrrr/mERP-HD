using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Nmcg;
using Hd.Model.Jk;
using Hd.Model.Jk2;
using Hd.Model.Ck;

namespace Hd.Service
{
    public class process_fy_piao
    {
        public static void 货代自编号DoubleClick(object sender, EventArgs e)
        {
            Xceed.Grid.DataCell cell = sender as Xceed.Grid.DataCell;
            string hdzbh = (string)cell.Value;

            if (string.IsNullOrEmpty(hdzbh))
            {
                return;
            }
            process_watcher.OpenFileToAttachment(hdzbh);
        }

        public static void FyDoubleClick(object sender1, EventArgs e1)
        {
            Xceed.Grid.DataCell cell = sender1 as Xceed.Grid.DataCell; 
            Xceed.Grid.DataRow row = cell.ParentRow as Xceed.Grid.DataRow;

            //if (row.ParentGrid.ReadOnly)
            //    return;

            //IBoundGrid grid = (m_masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0];

            ArchiveSeeForm masterForm = cell.GridControl.FindForm() as ArchiveSeeForm;
            if (masterForm == null)
            {
                // 通过DetailForm来的
                masterForm = (cell.GridControl.FindForm() as ArchiveDetailForm).ParentForm as ArchiveSeeForm;
            }
            ArchiveOperationForm fydjForm = masterForm.Tag as ArchiveOperationForm;
            
            //if (cell.FieldName == "拟付金额" || cell.FieldName == "拟收金额" || cell.FieldName == "费用项")
            {
                if (fydjForm == null)
                {

                    if (masterForm.Name == "内贸出港_票费用")
                    {
                        fydjForm = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("内贸出港_票费用项费用登记")) as ArchiveOperationForm;
                    }
                    else
                    {
                        fydjForm = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("业务财务_票费用项费用登记")) as ArchiveOperationForm;         
                    }
                        
                    masterForm.Tag = fydjForm;

                    Dictionary<string, object> setDatanew = new Dictionary<string, object>();
                    fydjForm.Tag = setDatanew;

                    (fydjForm.ControlManager.Dao as 业务费用Dao).TransactionBeginning += new EventHandler<OperateArgs<业务费用>>(delegate(object sender, OperateArgs<业务费用> e)
                    {
                        if (e.Entity.费用实体 == null)
                        {
                            业务费用 fy = e.Entity as 业务费用;
                            fy.费用实体 = e.Repository.Get<费用实体>(setDatanew["费用实体"]);
                            fy.票 = fy.费用实体 as 普通票;
                            fy.费用项编号 = (string)setDatanew["费用项"];
                        }
                    });
                    fydjForm.DisplayManager.SearchManager.EnablePage = false;
                    fydjForm.DisplayManager.SearchManager.DataLoaded += new EventHandler<DataLoadedEventArgs>(delegate(object sender, DataLoadedEventArgs e)
                    {
                        fydjForm.TopMost = true;
                        fydjForm.Show();
                    });

                    fydjForm.FormClosing += new FormClosingEventHandler(delegate(object sender, FormClosingEventArgs e)
                    {
                        if (e.CloseReason == CloseReason.UserClosing)
                        {
                            if (!masterForm.IsDisposed)
                            {
                                if (masterForm is ArchiveOperationForm)
                                {
                                    //(masterForm as ArchiveOperationForm).ControlManager.DisplayManager.Items[(masterForm as ArchiveOperationForm).ControlManager.DisplayManager.Position] = (fydjForm.DisplayManager.CurrentItem as 费用).费用实体;
                                    (masterForm as ArchiveOperationForm).ControlManager.DisplayManager.SearchManager.ReloadItem((masterForm as ArchiveOperationForm).ControlManager.DisplayManager.Position);
                                    (masterForm as ArchiveOperationForm).ControlManager.OnCurrentItemChanged();
                                }
                                //IBoundGrid grid = (masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0];
                                //ISearchManager sm = grid.DisplayManager.SearchManager;
                                //System.Data.DataTable dt = (System.Data.DataTable)sm.FindData(new List<ISearchExpression> { }, null);
                                //foreach (System.Data.DataRow i in dt.Rows)
                                //{
                                //    if (i["费用项"].ToString() == setDatanew["费用项"].ToString())
                                //    {
                                //        object save = row.Cells["Submitted"].Value;
                                //        grid.SetDataRowsIListData(i, row);
                                //        row.Cells["Submitted"].Value = save;
                                //        break;
                                //    }
                                //}

                                e.Cancel = true;
                                fydjForm.Hide();
                            }
                        }
                    });
                }

                Dictionary<string, object> setData = fydjForm.Tag as Dictionary<string, object>;
                setData.Clear();

                //进口_额外费用_委托人   费用项双击事件
                if (masterForm.Text.Equals("进口_额外费用_委托人"))
                {
                    //明细窗体
                    if (cell.ParentColumn.Title.Equals("费用项"))
                    {
                        setData["费用实体"] = (Guid)row.Cells["费用实体"].Value;
                        setData["费用项"] = (string)row.Cells["费用项"].Value;
                    }
                    //主窗体
                    else
                    {
                        using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用项>())
                        {
                            IList<费用项> list = (rep as Feng.NH.INHibernateRepository).List<费用项>(NHibernate.Criterion.DetachedCriteria.For<费用项>()
                                .Add(NHibernate.Criterion.Expression.Eq("名称", cell.ParentColumn.Title)));
                            if (list != null && list.Count > 0)
                            {
                                setData["费用实体"] = (row.Tag as 进口票).ID;
                                setData["费用项"] = list[0].编号;
                            }
                        }
                    }
                }
                 // 票费用登记窗体
                else if (row.Cells["费用实体"] != null)
                {
                    setData["费用实体"] = (Guid)row.Cells["费用实体"].Value;
                    setData["费用项"] = (string)row.Cells["费用项"].Value;
                    if (/*cell.FieldName == "已收金额" || cell.FieldName == "应收金额" || */cell.FieldName == "拟收金额")
                    {
                        setData["收付标志"] = 收付标志.收;
                    }
                    else if (/*cell.FieldName == "已付金额" || cell.FieldName == "应付金额" || */cell.FieldName == "拟付金额")
                    {
                        setData["收付标志"] = 收付标志.付;
                    }
                }
                // 滞箱费
                else
                {
                    setData["费用实体"] = (row.Tag as 费用信息).票Id;
                    setData["费用项"] = "167";
                }

                NameValueMappingCollection.Instance["信息_箱号_动态"].Params["@票"] = (Guid)setData["费用实体"];
                NameValueMappingCollection.Instance.Reload(fydjForm.DisplayManager.Name, "信息_箱号_动态");

                ISearchExpression se = SearchExpression.And(SearchExpression.Eq("费用实体.ID", (Guid)setData["费用实体"]), 
                    SearchExpression.Eq("费用项编号", (string)setData["费用项"]));
                if (setData.ContainsKey("收付标志"))
                {
                    se = SearchExpression.And(se, SearchExpression.Eq("收付标志", setData["收付标志"]));
                }
                fydjForm.ControlManager.DisplayManager.SearchManager.LoadData(se, null);
            }

            //Dictionary<string, bool?> submitted = new Dictionary<string, bool?>();
            //foreach (Xceed.Grid.DataRow i in grid.DataRows)
            //{
            //    submitted[i.Cells["费用项"].Value.ToString()] = (bool?)i.Cells["Submitted"].Value;
            //}
            //(row.GridControl as IBoundGrid).ReloadData();
            //foreach (Xceed.Grid.DataRow i in grid.DataRows)
            //{
            //    i.Cells["Submitted"].Value = submitted[i.Cells["费用项"].Value.ToString()];
            //}
        }

        public static void 进口_要求承运天数(object sender, EventArgs e)
        {
            Xceed.Grid.Cell cell = sender as Xceed.Grid.Cell;
            Xceed.Grid.DataRow row = cell.ParentRow as Xceed.Grid.DataRow;

            if (row.Cells["放行时间"].Value == null 
                || row.Cells["到港时间"].Value == null 
                || row.Cells["免箱天数"].Value == null
                || cell.ReadOnly)
            {
                return;
            }

            TimeSpan ts = Convert.ToDateTime(row.Cells["放行时间"].Value).Subtract(Convert.ToDateTime(row.Cells["到港时间"].Value));
            cell.CellEditorControl.Text = (Convert.ToInt32(row.Cells["免箱天数"].Value) - ts.Days).ToString();
        }

        public static bool 票费用保存(ArchiveOperationForm masterForm)
        {
            IBoundGrid grid = (masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0];
            if (grid.DataRows.Count == 0)
                return true;

            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用信息>())
            {
                try
                {
                    rep.BeginTransaction();

                    IList<费用信息> list = (rep as Feng.NH.INHibernateRepository).List<费用信息>(NHibernate.Criterion.DetachedCriteria.For<费用信息>()
                            .Add(NHibernate.Criterion.Expression.Eq("票Id", (Guid)grid.DataRows[0].Cells["费用实体"].Value)));
                    Dictionary<string, 费用信息> fyxxs = new Dictionary<string, 费用信息>();
                    foreach (费用信息 i in list)
                    {
                        fyxxs[i.费用项编号] = i;
                    }

                    // 收
                    foreach (Xceed.Grid.DataRow row in grid.DataRows)
                    {
                        bool? b = Feng.Utils.ConvertHelper.ToBoolean(row.Cells["Submitted"].Value);
                        if (!b.HasValue)
                            continue;

                        string fyx = (string)(row.Cells["费用项"].Value);
                        if (!fyxxs.ContainsKey(fyx))
                        {
                            费用信息 fyxx = new 费用信息();
                            fyxx.票Id = (Guid)row.Cells["费用实体"].Value;
                            fyxx.费用项编号 = (string)(row.Cells["费用项"].Value);
                            fyxx.业务类型编号 = (int)(row.Cells["业务类型"].Value);
                            fyxx.Submitted = b.Value;

                            (new HdBaseDao<费用信息>()).Save(rep, fyxx);

                            fyxxs[fyxx.费用项编号] = fyxx;
                            // MessageForm.ShowInfo("此费用项下还未有费用，不需要打完全标志！");
                        }
                        else
                        {
                            费用信息 fyxx = fyxxs[fyx];
                            if (fyxx.Submitted != b.Value)
                            {
                                fyxx.Submitted = b.Value;
                                (new HdBaseDao<费用信息>()).Update(rep, fyxx);
                            }
                        }
                    }

                    // 付
                    foreach (Xceed.Grid.DataRow row in grid.DataRows)
                    {
                        bool? b = Feng.Utils.ConvertHelper.ToBoolean(row.Cells["完全标志付"].Value);
                        if (!b.HasValue)
                            continue;

                        string fyx = (string)(row.Cells["费用项"].Value);
                        if (!fyxxs.ContainsKey(fyx))
                        {
                            费用信息 fyxx = new 费用信息();
                            fyxx.票Id = (Guid)row.Cells["费用实体"].Value;
                            fyxx.费用项编号 = (string)(row.Cells["费用项"].Value);
                            fyxx.业务类型编号 = (int)(row.Cells["业务类型"].Value);
                            fyxx.完全标志付 = b.Value;

                            (new HdBaseDao<费用信息>()).Save(rep, fyxx);

                            fyxxs[fyxx.费用项编号] = fyxx;
                            // MessageForm.ShowInfo("此费用项下还未有费用，不需要打完全标志！");
                        }
                        else
                        {
                            费用信息 fyxx = fyxxs[fyx];
                            if (fyxx.完全标志付 != b.Value)
                            {
                                fyxx.完全标志付 = b.Value;
                                (new HdBaseDao<费用信息>()).Update(rep, fyxx);
                            }
                        }
                    }

                    rep.CommitTransaction();


                    masterForm.ControlManager.OnCurrentItemChanged();

                    grid.ReloadData();

                    masterForm.ControlManager.State = Feng.StateType.View;
                    if (masterForm.ControlManager.ControlCheckExceptionProcess != null)
                        masterForm.ControlManager.ControlCheckExceptionProcess.ClearAllError();
                    masterForm.ControlManager.EndEdit(false);

                    return true;
                }
                catch (Exception ex)
                {
                    rep.RollbackTransaction();
                    ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                }
                return false;
            }
        }

        public static void 自动生成费用(ArchiveOperationForm masterForm)
        {
            Dictionary<string, object> setData = masterForm.Tag as Dictionary<string, object>;
            费用实体 entity;
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
            {
                rep.BeginTransaction();
                entity = rep.Get<费用实体>(setData["费用实体"]);
                rep.CommitTransaction();
            }

            if (entity.费用实体类型编号 == 11/*费用实体类型.进口*/)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口票>())
                {
                    进口票 piao = rep.Get<进口票>(setData["费用实体"]);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, (string)setData["费用项"], !setData.ContainsKey("收付标志") ? null : (收付标志?)setData["收付标志"]);
                }
            }
            else if (entity.费用实体类型编号 == 15/*费用实体类型.内贸出港*/)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<内贸出港票>())
                {
                    内贸出港票 piao = rep.Get<内贸出港票>(setData["费用实体"]);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, (string)setData["费用项"], !setData.ContainsKey("收付标志") ? null : (收付标志?)setData["收付标志"]);
                }
            }
            else if (entity.费用实体类型编号 == 45/*费用实体类型.进口其他业务*/)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口其他业务票>())
                {
                    进口其他业务票 piao = rep.Get<进口其他业务票>(setData["费用实体"]);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, (string)setData["费用项"], !setData.ContainsKey("收付标志") ? null : (收付标志?)setData["收付标志"]);
                }
            }
            else if (entity.费用实体类型编号 == 13/*费用实体类型.出口*/)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<出口票>())
                {
                    出口票 piao = rep.Get<出口票>(setData["费用实体"]);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, (string)setData["费用项"], !setData.ContainsKey("收付标志") ? null : (收付标志?)setData["收付标志"]);
                }
            }

            (masterForm.MasterGrid as IBoundGrid).ReloadData();
        }

        public static void 自动生成全部费用(ArchiveOperationForm masterForm)
        {
            if (!MessageForm.ShowYesNo("是否要自动生成全部费用？", "确认"))
            {
                return;
            }
            ProgressForm progressForm = new ProgressForm();
            progressForm.Start(masterForm, "生成");

            Feng.Async.AsyncHelper asyncHelper = new Feng.Async.AsyncHelper(
                new Feng.Async.AsyncHelper.DoWork(delegate()
                    {
                        费用实体 entity = masterForm.DisplayManager.CurrentItem as 费用实体;
                        if (entity == null)
                        {
                            throw new ArgumentException("请选择要生成费用的票！");
                        }

                        生成票费用(entity);
                        return null;
                    }), 
                new Feng.Async.AsyncHelper.WorkDone(delegate(object result)
                    {
                        Feng.Grid.BoundGridExtention.ReloadData((masterForm as ArchiveOperationForm).MasterGrid as IBoundGrid);
                        progressForm.Stop();
                    }));
        }

        /// <summary>
        /// 与“自动生成全部费用”功能一样
        /// 当2个不同window，用DetailWindow配置时，用这个函数
        /// </summary>
        /// <param name="masterForm"></param>
        public static void 自动生成全部费用DetailWindow(ArchiveOperationForm masterForm)
        {
            if (!MessageForm.ShowYesNo("是否要自动生成全部费用？", "确认"))
            {
                return;
            }
            ProgressForm progressForm = new ProgressForm();
            progressForm.Start(masterForm, "生成");

            Feng.Async.AsyncHelper asyncHelper = new Feng.Async.AsyncHelper(
                new Feng.Async.AsyncHelper.DoWork(delegate()
                {
                    费用实体 entity = (masterForm.ParentForm as ArchiveOperationForm).DisplayManager.CurrentItem as 费用实体;
                    if (entity == null)
                    {
                        throw new ArgumentException("请选择要生成费用的票！");
                    }

                    生成票费用(entity);
                    return null;
                }),
                new Feng.Async.AsyncHelper.WorkDone(delegate(object result)
                {
                    Feng.Grid.BoundGridExtention.ReloadData((masterForm.ParentForm as ArchiveOperationForm).MasterGrid as IBoundGrid);
                    progressForm.Stop();
                }));
        }

        public static void 自动生成全部票费用(ArchiveOperationForm masterForm)
        {
            if (!MessageForm.ShowYesNo("是否要自动生成全部票费用？", "确认"))
            {
                return;
            }

            ProgressForm progressForm = new ProgressForm();
            progressForm.Start(masterForm, "生成");

            Feng.Async.AsyncHelper asyncHelper = new Feng.Async.AsyncHelper(
                new Feng.Async.AsyncHelper.DoWork(delegate()
                {
                    foreach (object obj in masterForm.DisplayManager.Items)
                    {
                        费用实体 entity = obj as 费用实体;
                        if (entity == null)
                        {
                            throw new ArgumentException("费用实体 is null！");
                        }

                        生成票费用(entity);
                    }
                    return null;
                }),
                    new Feng.Async.AsyncHelper.WorkDone(delegate(object result)
                    {
                        Feng.Grid.BoundGridExtention.ReloadData(masterForm.MasterGrid as IBoundGrid);
                        progressForm.Stop();
                    }));
        }

        public static void 生成票费用(费用实体 entity)
        {
            生成票费用(entity, false);        
        }

        public static void 生成票费用(费用实体 entity, bool service)
        {
            if (entity.费用实体类型编号 == 11)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口票>())
                {
                    进口票 piao = rep.Get<进口票>(entity.ID);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, null, null, service);
                }
            }
            else if (entity.费用实体类型编号 == 15)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<内贸出港票>())
                {
                    内贸出港票 piao = rep.Get<内贸出港票>(entity.ID);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, null, null, service);
                }
            }
            else if (entity.费用实体类型编号 == 45)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口其他业务票>())
                {
                    进口其他业务票 piao = rep.Get<进口其他业务票>(entity.ID);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, null, null, service);
                }
            }
            else if (entity.费用实体类型编号 == 13)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<出口票>())
                {
                    出口票 piao = rep.Get<出口票>(entity.ID);
                    rep.Initialize(piao.箱, piao);
                    process_fy_yw.批量生成费用(rep, entity.费用实体类型编号, piao, piao.箱, null, null, service);
                }
            }
        }
    }
}
