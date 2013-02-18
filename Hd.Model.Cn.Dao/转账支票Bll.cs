using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Cn
{
    public class 转账支票Bll : HdBaseDao<转账支票>, I支付方式, I收入方式
    {
        #region "Interface"
        public void 支付(IRepository rep, 凭证收支明细 entity)
        {
            if (string.IsNullOrEmpty(entity.票据号码))
            {
                throw new InvalidUserOperationException("请输入支票号码！");
            }
            转账支票 item = GetFrom票据号码(rep, entity.票据号码);
            if (item.银行账户.币制编号 == entity.凭证.金额.币制编号
               && !item.Submitted && !item.是否作废)
            {
                if ((item.金额 == entity.金额 && item.领用方式.HasValue && item.领用方式.Value == 领用方式.直接支付)
                    || (item.领用方式.HasValue && item.领用方式.Value == 领用方式.空白支票))
                {
                    item.Submitted = true;
                    item.金额 = entity.金额;
                    item.支付凭证号 = entity.凭证.凭证号;
                    item.日期 = entity.凭证.日期;
                    this.Update(rep, item);
                    return;
                }
            }

            throw new InvalidUserOperationException("支票金额、币制或者状态不对，请查证！");
        }

        public void 取消支付(IRepository rep, 凭证收支明细 entity)
        {
            转账支票 item = GetFrom票据号码(rep, entity.票据号码);
            item.Submitted = false;
            item.支付凭证号 = null;
            item.日期 = null;
            this.Update(rep, item);
        }

        public void 收入(IRepository rep, 凭证收支明细 entity)
        {
            // do nothing，不纳入支票管理
            if (string.IsNullOrEmpty(entity.票据号码))
            {
                throw new InvalidUserOperationException("请输入支票号码！");
            }
            if (!entity.银行账户编号.HasValue)
            {
                throw new InvalidUserOperationException("请输入银行账户！");
            }
            银行账户 yhzh = rep.Get<银行账户>(entity.银行账户编号);

            if (yhzh == null
                || yhzh.币制编号 != entity.凭证.金额.币制编号)
            {
                throw new InvalidUserOperationException("银行账户输入错误！");
            }
        }

        public void 取消收入(IRepository rep, 凭证收支明细 entity)
        {
        }


        public 转账支票 GetFrom票据号码(IRepository rep, string 票据号码)
        {
            IList<转账支票> list = (rep as Feng.NH.INHibernateRepository).List<转账支票>(NHibernate.Criterion.DetachedCriteria.For<转账支票>()
                .Add(NHibernate.Criterion.Expression.Eq("票据号码", 票据号码)));
            if (list.Count == 0)
            {
                throw new InvalidUserOperationException(票据号码 + "的转账支票不存在！");
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                throw new InvalidUserOperationException(票据号码 + "的转账支票有重复记录！");
            }
        }
        #endregion

        public void 批量添加(DateTime 买入时间, Guid 出票账户, string preCode, int start, int end)
        {
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<转账支票>())
            {
                try
                {
                    rep.BeginTransaction();
                    for (int i = start; i <= end; ++i)
                    {
                        转账支票 zp = new 转账支票();
                        zp.银行账户编号 = 出票账户;
                        zp.买入时间 = 买入时间;
                        zp.票据号码 = preCode + i.ToString().PadLeft(end.ToString().Length, '0');
                        zp.Submitted = false;
                        zp.是否作废 = false;

                        this.Save(rep, zp);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                    throw;
                }
            }
        }

        public void 批量删除(DateTime 买入时间, Guid 出票账户, string preCode, int start, int end)
        {
            using (var rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<转账支票>())
            {
                try
                {
                    rep.BeginTransaction();
                    for (int i = start; i <= end; ++i)
                    {
                        string code = preCode + i.ToString().PadLeft(end.ToString().Length, '0');

                        IList<转账支票> zps = (rep as Feng.NH.INHibernateRepository).List<转账支票>(NHibernate.Criterion.DetachedCriteria.For<转账支票>()
                             .Add(NHibernate.Criterion.Expression.Eq("票据号码", code)));

                        if (zps.Count != 1 || zps[0].银行账户编号 != 出票账户 || zps[0].买入时间 != 买入时间
                            || zps[0].Submitted || zps[0].是否作废)
                        {
                            throw new InvalidUserOperationException("支票" + code + "数据有误，请查证是否可以删除！");
                        }

                        this.Delete(rep, zps[0]);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                    throw;
                }
            }
        }

        protected override void DoDelete(IRepository rep, 转账支票 entity)
        {
            if (entity.Submitted || entity.是否作废)
            {
                throw new InvalidUserOperationException("支票" + entity.票据号码 + "不能删除!");
            }

            base.DoDelete(rep, entity);
        }

        //protected override void DoUpdate(Repository rep, 转账支票 entity)
        //{
        ////    if (entity.票据状态 == 转账支票状态.未使用)
        ////    {
        ////        if (!entity.金额.HasValue)
        ////        {
        ////            entity.票据状态 = 转账支票状态.已领用未支付;
        ////            entity.领用方式 = 领用方式.空白支票;
        ////        }
        ////        else
        ////        {
        ////            entity.票据状态 = 转账支票状态.已领用未支付;
        ////            entity.领用方式 = 领用方式.直接支付;
        ////        }
        ////    }

        ////    base.DoUpdate(rep, entity);
        //}


        //public void 撤销(转账支票 entity)
        //{
        //    //switch (entity.票据状态)
        //    //{
        //    //    //case 转账支票状态.空白支票已领用未反馈:
        //    //    //    entity.票据状态 = 转账支票状态.未使用;
        //    //    //    entity.经办人编号 = null;
        //    //    //    entity.领用方式 = null;
        //    //    //    entity.领用时间 = null;
        //    //    //    break;
        //    //    case 转账支票状态.已领用未支付:
        //    //        switch (entity.领用方式.Value)
        //    //        {
        //    //            case 领用方式.直接支付:
        //    //                entity.票据状态 = 转账支票状态.未使用;
        //    //                entity.经办人编号 = null;
        //    //                entity.领用方式 = null;
        //    //                entity.领用时间 = null;
        //    //                entity.金额 = null;
        //    //                break;
        //    //            case 领用方式.空白支票:
        //    //                entity.票据状态 = 转账支票状态.未使用;
        //    //                entity.经办人编号 = null;
        //    //                entity.领用方式 = null;
        //    //                entity.领用时间 = null;
        //    //                entity.金额 = null;
        //    //                break;
        //    //            default:
        //    //                throw new InvalidOperationException("Invalid 领用方式");
        //    //        }
        //    //        break;
        //    //    case 转账支票状态.已转账:
        //    //        entity.票据状态 = 转账支票状态.已领用未支付;
        //    //        break;
        //    //    case 转账支票状态.已作废:
        //    //        entity.票据状态 = 转账支票状态.已领用未支付;
        //    //        break;
        //    //    case 转账支票状态.未使用:
        //    //        throw new InvalidUserOperationException("未使用支票不能撤销！");
        //    //    case 转账支票状态.已支付:
        //    //        throw new InvalidUserOperationException("已支付支票不能在此撤销，请撤销相应凭证！");
        //    //    default:
        //    //        throw new InvalidOperationException("Invalid 支票状态");
        //    //}

        //    //base.Update(entity);
        //}

        //public void 作废(转账支票 entity)
        //{
        //    //if (entity.票据状态 == 转账支票状态.已领用未支付)
        //    //{
        //    //    entity.票据状态 = 转账支票状态.已作废;
        //    //}
        //    //else
        //    //{
        //    //    throw new InvalidUserOperationException("只能作废已领用未支付支票！");
        //    //}
        //    //base.Update(entity);
        //}

        //public void 转账(转账支票 entity)
        //{
        //    //if (entity.票据状态 == 转账支票状态.已领用未支付)
        //    //{
        //    //    entity.票据状态 = 转账支票状态.已转账;
        //    //}
        //    //else
        //    //{
        //    //    throw new InvalidUserOperationException("只能转账已领用未支付支票！");
        //    //}

        //    //base.Update(entity);
        //}
    }
}
