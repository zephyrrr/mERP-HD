using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model
{
    // can't inherit 费用Dao. 否则Repository会按照[费用]来生成
    public class 业务费用Dao : HdBaseDao<业务费用>
    {
        public 业务费用Dao()
        {
            this.EntityOperating += new EventHandler<OperateArgs<业务费用>>(业务费用Dao_EntityOperating);
        }

        private Guid m_费用实体Id;
        public void Set费用实体(Guid 费用实体Id)
        {
            m_费用实体Id = 费用实体Id;
        }

        void 业务费用Dao_EntityOperating(object sender, OperateArgs<业务费用> e)
        {
            if (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update)
            {
                业务费用 entity = e.Entity;
                if (entity.费用实体 == null && entity.费用信息 != null)
                {
                    entity.费用实体 = entity.费用信息.票;
                }

                // 用于票费用_费用登记，新增业务费用需要关联费用实体
                if (entity.费用实体 == null && entity.费用信息 == null)
                {
                    entity.费用实体 = e.Repository.Get<费用实体>(m_费用实体Id);
                    entity.票 = entity.费用实体 as 普通票;
                }

                // 费用类别
                if (string.IsNullOrEmpty(entity.费用项编号))
                {
                    entity.费用类别编号 = null;
                }
                else
                {
                    // 当费用项变换时，要重新设置费用类别编号
                    entity.费用项 = EntityBufferCollection.Instance.Get<费用项>(entity.费用项编号);
                    int? lbb = entity.收付标志 == 收付标志.收 ? entity.费用项.收入类别 : entity.费用项.支出类别;
                    if (!lbb.HasValue)
                    {
                        throw new InvalidUserOperationException("您选择的费用项和收付有误，请重新选择！");
                    }

                    string lb = (EntityBufferCollection.Instance.Get<费用类别>(lbb.Value)).大类;
                    if (lb != "业务常规" && lb != "业务额外")
                    {
                        throw new ArgumentException("费用项编号" + entity.费用项编号 + "的费用类别有误，只能是业务费用!");
                    }
                    entity.费用类别编号 = lbb;

                    //e.Repository.Initialize(entity.费用实体, entity);
                    //string fystlx = (EntityBufferCollection.Instance.Get<费用类别>(entity.费用实体.费用实体类型编号)).代码类型名;
                    //switch (fystlx)
                    //{
                    //    case "Hd.Model.Jk.进口票":
                    //        entity.费用类别编号 = lb == "业务常规" ? 999 : 999;
                    //        break;
                    //    case "Hd.Model.Jk2.进口其他票":
                    //        entity.费用类别编号 = lb == "业务常规" ? 999 : 999;
                    //        break;
                    //    case "Hd.Model.Nmcg.内贸出港票":
                    //        entity.费用类别编号 = lb == "业务常规" ? 999 : 999;
                    //        break;
                    //    default:
                    //        throw new ArgumentException("Invalid 费用实体类型代码类型名 of " + fystlx);
                    //}
                }

                // 费用信息
                if (string.IsNullOrEmpty(entity.费用项编号))
                {
                    entity.费用信息 = null;
                }
                else
                {
                    HdBaseDao<费用信息> daoFyxx = new HdBaseDao<费用信息>();

                    // 当费用项变换时，要重新设置费用信息
                    entity.费用类别 = EntityBufferCollection.Instance.Get<费用类别>(entity.费用类别编号) as 费用类别;
                    entity.费用项 = EntityBufferCollection.Instance.Get<费用项>(entity.费用项编号);

                    if (entity.费用类别.大类 == "业务额外" || entity.费用类别.大类 == "业务常规")
                    {
                        IList<费用信息> list = (e.Repository as Feng.NH.INHibernateRepository).List<费用信息>(NHibernate.Criterion.DetachedCriteria.For<费用信息>()
                            .Add(NHibernate.Criterion.Expression.Eq("费用项编号", entity.费用项.编号))
                            .Add(NHibernate.Criterion.Expression.Eq("票.ID", entity.票.ID)));
                        if (list.Count == 0)
                        {
                            费用信息 item = new 费用信息();
                            item.票Id = entity.票.ID;
                            item.费用项编号 = entity.费用项编号;
                            item.业务类型编号 = entity.票.费用实体类型编号;

                            daoFyxx.Save(e.Repository, item);

                            entity.费用信息 = item;
                        }
                        else if (list.Count == 1)
                        {
                             //修改的时候，和完全标志无关?? && e.OperateType == OperateType.Save
                            if (e.OperateType == OperateType.Save && ((entity.收付标志 == 收付标志.收 && list[0].Submitted)
                                || (entity.收付标志 == 收付标志.付 && list[0].完全标志付)))
                            {
                                throw new InvalidUserOperationException("票" + entity.票.货代自编号 + " 费用项" + entity.费用项编号 + "已打完全标志，不能操作费用！");
                            }
                            entity.费用信息 = list[0];
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "费用信息对同一费用主体同一费用项有多条！");
                        }
                    }
                }
            }
        }
    }
}
