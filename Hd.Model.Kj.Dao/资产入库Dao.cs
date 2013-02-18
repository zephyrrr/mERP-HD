using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Kj
{
    public class 资产入库Dao : BaseSubmittedDao<资产入库>
    {
        public override void Submit(IRepository rep, 资产入库 entity)
        {
            应收应付款 ysyfk = new 应收应付款();
            ysyfk.备注 = entity.备注;
            ysyfk.结算期限 = entity.结算期限.HasValue ? entity.结算期限.Value : entity.日期;
            ysyfk.金额 = entity.金额;
            ysyfk.日期 = entity.日期;
            ysyfk.相关人编号 = entity.相关人编号;
            ysyfk.业务类型编号 = entity.业务类型编号;
            ysyfk.应收应付源 = entity;
            if (entity.业务类型编号 == 24) // 业务类型=固定资产  费用项 002 其他的  000
            {
                ysyfk.费用项编号 = "002";
            }
            else
            {
                ysyfk.费用项编号 = "000";
            }            
            ysyfk.收付标志 = 收付标志.付;
            new HdBaseDao<应收应付款>().Save(rep, ysyfk);
           
            应收应付款 ysyf = new 应收应付款();
            ysyf.备注 = entity.备注;
            ysyf.结算期限 = entity.结算期限.HasValue ? entity.结算期限.Value : entity.日期;
            ysyf.金额 = entity.金额;
            ysyf.日期 = entity.日期;
            ysyf.相关人编号 = "900031";
            ysyf.业务类型编号 = entity.业务类型编号;
            ysyf.应收应付源 = entity;
            ysyf.费用项编号 = "004";
            ysyf.收付标志 = 收付标志.收;
            new HdBaseDao<应收应付款>().Save(rep, ysyf);
           
            entity.Submitted = true;
            this.Update(rep, entity);
        }

        public override void Unsubmit(IRepository rep, 资产入库 entity)
        {
            IList<应收应付款> list = (rep as Feng.NH.INHibernateRepository).List<应收应付款>(NHibernate.Criterion.DetachedCriteria.For<应收应付款>()
                .Add(NHibernate.Criterion.Expression.Eq("应收应付源", entity)));

            entity.Submitted = false;
            this.Update(rep, entity);

            foreach (应收应付款 i in list)
            {
                rep.Delete(i);
            }
        }
    }
}
