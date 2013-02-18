using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Kj
{
    public class 调节款Dao : BaseSubmittedDao<调节款>
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 调节款 entity)
        {
            //rep.Initialize(entity, entity.调节款明细);
            //foreach (调节款明细 i in entity.调节款明细)
            //{
            //    应收应付款 j = new 应收应付款();
            //    j.备注 = entity.备注;
            //    j.费用项编号 = i.费用项编号;
            //    j.结算期限 = i.结算期限;
            //    j.金额 = i.金额;
            //    j.日期 = i.日期;
            //    j.收付标志 = i.收付标志;
            //    j.相关人编号 = i.相关人编号;
            //    j.业务类型编号 = i.业务类型编号;

            //    j.应收应付源 = entity;

            //    (new HdBaseDao<应收应付款>()).Save(rep, j);
            //}
           
            应收应付款 j = new 应收应付款();
            j.备注 = entity.备注;
            j.费用项编号 = entity.费用项编号;
            j.结算期限 = entity.结算期限.HasValue ? entity.结算期限.Value : entity.日期;
            j.金额 = entity.金额;
            j.日期 = entity.日期;
            j.收付标志 = entity.收付标志;
            j.相关人编号 = entity.相关人编号;
            j.业务类型编号 = entity.业务类型编号;

            j.应收应付源 = entity;

            (new HdBaseDao<应收应付款>()).Save(rep, j);

            entity.Submitted = true;
            this.Update(rep, entity);
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 调节款 entity)
        {
            IList<应收应付款> list = (rep as Feng.NH.INHibernateRepository).List<应收应付款>(NHibernate.Criterion.DetachedCriteria.For<应收应付款>()
                .Add(NHibernate.Criterion.Expression.Eq("应收应付源", entity)));

            entity.Submitted = false;
            this.Update(rep, entity);

            foreach (应收应付款 i in list)
            {
                rep.Delete(i);
            }

            // 如果先Delete后Update，不知为何会出现“a different object with the same identifier value was already associated with the session”错误
        }
    }
}
