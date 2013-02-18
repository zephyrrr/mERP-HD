using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Cn
{
    public class 银行收付Bll : I支付方式, I收入方式
    {
        public void 支付(IRepository rep, 凭证收支明细 entity)
        {
            if (!entity.银行账户编号.HasValue)
            {
                throw new InvalidUserOperationException("请填写银行账户！");
            }
            银行账户 item = rep.Get<银行账户>(entity.银行账户编号.Value);

            if (item.币制编号 != entity.凭证.金额.币制编号)
            {
                throw new InvalidUserOperationException("银行帐户币制不符，请重新填写！");
            }
        }

        public void 取消支付(IRepository rep, 凭证收支明细 entity)
        {
        }

        public void 收入(IRepository rep, 凭证收支明细 entity)
        {
            if (!entity.银行账户编号.HasValue)
            {
                throw new InvalidUserOperationException("请填写银行账户！");
            }
            银行账户 item = rep.Get<银行账户>(entity.银行账户编号.Value);

            if (item.币制编号 != entity.凭证.金额.币制编号)
            {
                throw new InvalidUserOperationException("银行帐户币制不符，请重新填写！");
            }
        }

        public void 取消收入(IRepository rep, 凭证收支明细 entity)
        {
        }
    }

    //public class 银行资金Bll : BaseDao<银行资金>, I支付方式, I收入方式
    //{
    //    public void 支付(Repository rep, 凭证收支明细 entity)
    //    {
    //        银行资金 item = GetFrom银行账户编号(rep, entity.银行账户编号);
    //        if (item.数额 < entity.数额
    //            || item.银行账户.币制编号 != entity.凭证.金额.币制编号)
    //        {
    //            throw new InvalidUserOperationException("银行资金数额不够或者币值不符，请重新填写！");
    //        }
    //        item.数额 -= entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 取消支付(Repository rep, 凭证收支明细 entity)
    //    {
    //        银行资金 item = GetFrom银行账户编号(rep, entity.银行账户编号);
    //        item.数额 += entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 收入(Repository rep, 凭证收支明细 entity)
    //    {
    //        银行资金 item = GetFrom银行账户编号(rep, entity.银行账户编号);
    //        if (item.银行账户.币制编号 != entity.凭证.金额.币制编号)
    //        {
    //            throw new InvalidUserOperationException("银行账户币值不符，请重新填写！");
    //        }
    //        item.数额 += entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 取消收入(Repository rep, 凭证收支明细 entity)
    //    {
    //        银行资金 item = GetFrom银行账户编号(rep, entity.银行账户编号);
    //        item.数额 -= entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public 银行资金 GetFrom银行账户编号(Repository rep, Guid? 银行账户编号)
    //    {
    //        if (银行账户编号.HasValue)
    //        {
    //            throw new InvalidUserOperationException("请填写银行账户！");
    //        }

    //        IList<银行资金> list = rep.Session.CreateCriteria(typeof(银行资金))
    //            .Add(NHibernate.Criterion.Expression.Eq("银行账户编号", 银行账户编号))
    //            .List<银行资金>();
    //        if (list.Count == 0)
    //        {
    //            银行资金 entity = new 银行资金();
    //            entity.银行账户编号 = 银行账户编号.Value;
    //            entity.数额 = 0;
    //            this.Save(rep, entity);
    //            return entity;
    //        }
    //        else if (list.Count == 1)
    //        {
    //            return list[0];
    //        }
    //        else
    //        {
    //            throw new InvalidUserOperationException(银行账户编号 + "的银行资金记录超过一条，请联系数据库管理员！");
    //        }
    //    }
    //}
}
