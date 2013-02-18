using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Feng;
using Feng.Windows.Utils;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Kj;
using Hd.Model.Jk;

namespace Hd.Service
{
    public class process_dzd_dzfz
    {
        public static void 费用登记(ArchiveOperationForm masterForm)
        {
            int successCount = 0, skipCount = 0;

            ProgressAsyncHelper asyncHelper = new ProgressAsyncHelper(
               new Feng.Async.AsyncHelper.DoWork(delegate()
                   {
                       using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
                       {
                           rep.BeginTransaction();

                           foreach (Xceed.Grid.DataRow row in masterForm.MasterGrid.GridControl.SelectedRows)
                           {
                               // 60 = 未备案
                               // 61 = 箱号不合理
                               // 62 = 金额不同
                               // 63 = 已对账已凭证
                               // 64 = 未对账
                               // 65 = 未登记
                               // 66 = 费用不完整
                               if (!row.Cells["状态"].Value.ToString().Contains("65")) // 只有“未登记”的费用才会被登记
                               {
                                   skipCount++;
                                   continue;
                               }

                               if (row.Cells["相关人"].Value == null || row.Cells["费用项"].Value == null)
                               {
                                   if (MessageForm.ShowYesNo("第 " + row.Cells["Id"].Value.ToString() + " 条费用，填写不完整！" + Environment.NewLine
                                       + "提单号:" + row.Cells["提单号"].Value.ToString()
                                       + " 箱号:" + row.Cells["箱号"].Value.ToString()
                                       + Environment.NewLine + "“是”跳过继续登记，“否”取消所有登记？", "提示"))
                                   {
                                       skipCount++;
                                       continue;
                                   }
                                   else
                                   {
                                       successCount = skipCount = 0;
                                       rep.RollbackTransaction();
                                       return null;
                                   }
                               }

                               业务费用 ywfy = new 业务费用();
                               ywfy.相关人编号 = row.Cells["相关人"].Value.ToString();
                               ywfy.费用项编号 = row.Cells["费用项"].Value.ToString();
                               ywfy.金额 = Convert.ToDecimal(row.Cells["金额"].Value);
                               ywfy.收付标志 = 收付标志.付;
                               ywfy.票 = rep.Get<普通票>(new Guid(row.Cells["费用实体"].Value.ToString()));
                               ywfy.箱Id = new Guid(row.Cells["箱"].Value.ToString());
                               ywfy.费用实体 = ywfy.票;

                               new 业务费用Dao().Save(rep, ywfy);
                               successCount++;
                               //rep.Save(ywfy);                    
                           }

                           rep.CommitTransaction();
                       }
                       return null;
                   }),
                   new Feng.Async.AsyncHelper.WorkDone(delegate(object result)
                   {
                       MessageForm.ShowInfo("成功" + successCount + "条， 跳过" + skipCount + "条"); 
                   }), masterForm, "登记");      
        }

        public static void 自动填备注(ArchiveOperationForm masterForm)
        {
            int successCount = 0, skipCount = 0;

            ProgressAsyncHelper asyncHelper = new ProgressAsyncHelper(
                new Feng.Async.AsyncHelper.DoWork(delegate()
                    {
                        using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
                        {
                            rep.BeginTransaction();
                            foreach (Xceed.Grid.DataRow row in masterForm.MasterGrid.GridControl.SelectedRows)
                            {
                                if (!row.Cells["状态"].Value.ToString().Contains("64")) // 只有“未对账”的费用才会自动填备注
                                {
                                    if (MessageForm.ShowYesNo("第 " + row.Cells["Id"].Value.ToString() + " 条不是未对账费用！" + Environment.NewLine
                                        + "提单号:" + row.Cells["提单号"].Value.ToString()
                                        + " 箱号:" + row.Cells["箱号"].Value.ToString()
                                        + " 费用项:" + row.Cells["费用项"].Value.ToString()
                                        + Environment.NewLine + "“是”跳过继续，“否”取消所有操作？", "提示"))
                                    {
                                        skipCount++;
                                        continue;
                                    }
                                    else
                                    {
                                        successCount = skipCount = 0;
                                        rep.RollbackTransaction();
                                        return null;
                                    }
                                }

                                IList<业务费用> ywfy = (rep as Feng.NH.INHibernateRepository).List<业务费用>(NHibernate.Criterion.DetachedCriteria.For<业务费用>()
                                    .Add(NHibernate.Criterion.Expression.Eq("费用实体.ID", new Guid(row.Cells["费用实体"].Value.ToString())))
                                    .Add(NHibernate.Criterion.Expression.Eq("箱Id", new Guid(row.Cells["箱"].Value.ToString())))
                                    .Add(NHibernate.Criterion.Expression.Eq("费用项编号", row.Cells["费用项"].Value.ToString()))
                                    .Add(NHibernate.Criterion.Expression.Eq("相关人编号", row.Cells["相关人"].Value.ToString()))
                                    .Add(NHibernate.Criterion.Expression.Eq("金额", (decimal)row.Cells["金额"].Value)));

                                if (ywfy.Count > 1)
                                {
                                    rep.RollbackTransaction();
                                    throw new NullReferenceException("第 " + row.Cells["Id"].Value.ToString() + " 条对应了" + ywfy.Count + "条财务费用"
                                        + Environment.NewLine + "可能由于重复登记，请先查看删除多余记录");
                                }

                                ywfy[0].备注 += row.Cells["备注"].Value.ToString();
                                new 业务费用Dao().Update(rep, ywfy[0]);
                                successCount++;
                            }

                            rep.CommitTransaction();
                        }
                        return null;
                    }),
                    new Feng.Async.AsyncHelper.WorkDone(delegate(object result)
                        {
                            MessageForm.ShowInfo("成功" + successCount + "条， 跳过" + skipCount + "条");                           
                        }), masterForm, "执行");
        }
    }
}
