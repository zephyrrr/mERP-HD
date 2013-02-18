using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feng;
using Hd.Model;

namespace Hd.Model.Cn
{
    public class 银行转账Dao : HdBaseDao<换汇>
    {
        protected override void DoSave(Feng.IRepository rep, 换汇 entity)
        {
            银行账户 ck_zh = Get银行账户(entity.出款账户编号);//出款账户
            银行账户 rk_zh = Get银行账户(entity.入款账户编号);//入款账户
            if (!ck_zh.币制编号.Equals(rk_zh.币制编号))
            {
                throw new InvalidUserOperationException("转账业务：" + Environment.NewLine + "银行账户币制必须相同");
            }
            entity.出款金额.币制编号 = entity.入款金额.币制编号 = ck_zh.币制编号;
            entity.入款金额 = entity.出款金额;
            base.DoSave(rep, entity);
        }

        protected override void DoUpdate(Feng.IRepository rep, 换汇 entity)
        {
            银行账户 ck_zh = Get银行账户(entity.出款账户编号);//出款账户
            银行账户 rk_zh = Get银行账户(entity.入款账户编号);//入款账户
            if (!ck_zh.币制编号.Equals(rk_zh.币制编号))
            {
                throw new InvalidUserOperationException("转账业务：" + Environment.NewLine + "银行账户币制必须相同");
            }
            entity.出款金额.币制编号 = entity.入款金额.币制编号 = ck_zh.币制编号;
            entity.入款金额 = entity.出款金额;
            base.DoUpdate(rep, entity);
        }

        protected override void DoSaveOrUpdate(Feng.IRepository rep, 换汇 entity)
        {
            银行账户 ck_zh = Get银行账户(entity.出款账户编号);//出款账户
            银行账户 rk_zh = Get银行账户(entity.入款账户编号);//入款账户
            if (!ck_zh.币制编号.Equals(rk_zh.币制编号))
            {
                throw new InvalidUserOperationException("转账业务：" + Environment.NewLine + "银行账户币制必须相同");
            }
            entity.出款金额.币制编号 = entity.入款金额.币制编号 = ck_zh.币制编号;
            entity.入款金额 = entity.出款金额;
            base.DoSaveOrUpdate(rep, entity);
        }

        private 银行账户 Get银行账户(Guid guid)
        {
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<银行账户>())
            {
                return (rep as Feng.NH.INHibernateRepository).List<银行账户>(NHibernate.Criterion.DetachedCriteria.For<银行账户>()
                    .Add(NHibernate.Criterion.Expression.Eq("Id", guid)))[0];
            }
        }
    }
}
