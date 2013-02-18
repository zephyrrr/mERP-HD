using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.NH;

namespace Hd.Model.Cn
{
    //public class 现金Bll : BaseDao<现金>, I支付方式, I收入方式
    //{
    //    public void 支付(Repository rep, 凭证收支明细 entity)
    //    {
    //        现金 item = GetFrom币制(rep, entity.凭证.金额.币制编号);
    //        if (item.数额 < entity.数额)
    //        {
    //            throw new InvalidUserOperationException("现金数额不够，请重新填写！");
    //        }
    //        item.数额 -= entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 取消支付(Repository rep, 凭证收支明细 entity)
    //    {
    //        现金 item = GetFrom币制(rep, entity.凭证.金额.币制编号);
    //        item.数额 += entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 收入(Repository rep, 凭证收支明细 entity)
    //    {
    //        现金 item = GetFrom币制(rep, entity.凭证.金额.币制编号);
    //        item.数额 += entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public void 取消收入(Repository rep, 凭证收支明细 entity)
    //    {
    //        现金 item = GetFrom币制(rep, entity.凭证.金额.币制编号);
    //        item.数额 -= entity.数额;
    //        this.Update(rep, item);
    //    }

    //    public 现金 GetFrom币制(Repository rep, string 币制编号)
    //    {
    //        IList<现金> list = rep.Session.CreateCriteria(typeof(现金))
    //            .Add(NHibernate.Criterion.Expression.Eq("币制编号", 币制编号))
    //            .List<现金>();
    //        if (list.Count == 0)
    //        {
    //            现金 entity = new 现金();
    //            entity.币制编号 = 币制编号;
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
    //            throw new InvalidOperationException(币制编号 + "的现金记录超过一条，请联系数据库管理员！");
    //        }
    //    }
    //}
}
