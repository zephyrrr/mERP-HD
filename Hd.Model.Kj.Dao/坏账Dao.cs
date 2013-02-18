using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model.Kj
{
    public class 坏账Dao : BaseSubmittedDao<坏账>
    {
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 坏账 entity)
        {
            对账单 dzd = new 对账单();
            dzd.对账单类型 = (int)财务对账单类型.坏账;
            dzd.结束日期 = entity.日期;
            dzd.费用 = new List<费用>();
            dzd.费用项编号 = entity.费用项编号;
            dzd.关账日期 = entity.日期;
            dzd.结算期限 = entity.日期;
            dzd.金额 = -entity.金额;
            dzd.收付标志 = entity.收付标志;
            dzd.送出时间 = entity.日期;
            dzd.相关人编号 = entity.相关人编号;
            dzd.业务类型编号 = entity.业务类型编号;

            dzd.Submitted = true;
            (new HdBaseDao<对账单>()).Save(rep, dzd);

            非业务费用 fy = new 非业务费用();
            fy.费用项编号 = "352";  // 坏账
            fy.费用实体 = entity;
            fy.金额 = entity.金额;
            fy.收付标志 = entity.收付标志 == 收付标志.付 ? 收付标志.收 : 收付标志.付;
            fy.相关人编号 = entity.相关人编号;
            fy.对账单 = dzd;
            (new 非业务费用Dao()).Save(rep, fy);
            dzd.费用.Add(fy);

            entity.费用 = new List<费用>();
            entity.费用.Add(fy);

            应收应付款 ysyfk = new 应收应付款();
            ysyfk.费用项编号 = entity.费用项编号;
            ysyfk.结算期限 = dzd.结算期限.HasValue ? dzd.结算期限.Value : dzd.关账日期.Value;
            ysyfk.金额 = dzd.金额;
            ysyfk.日期 = dzd.关账日期.Value;
            ysyfk.收付标志 = dzd.收付标志;
            ysyfk.相关人编号 = dzd.相关人编号;
            ysyfk.业务类型编号 = entity.业务类型编号;
            ysyfk.应收应付源 = dzd;

            (new HdBaseDao<应收应付款>()).Save(rep, ysyfk);
            dzd.应收应付款 = new List<应收应付款>();
            dzd.应收应付款.Add(ysyfk);

            entity.对账单号 = dzd.编号;
            entity.Submitted = true;
            this.Update(rep, entity);
        }

        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 坏账 entity)
        {
            rep.Initialize(entity.费用, entity);
            System.Diagnostics.Debug.Assert(entity.费用.Count == 1, "坏账费用有且只有一条！");

            对账单 dzd = entity.费用[0].对账单;
            System.Diagnostics.Debug.Assert(dzd != null, "坏账费用肯定已自动出过对账单！");
            rep.Initialize(dzd, entity.费用[0]);
            rep.Initialize(dzd.费用, dzd);

            rep.Delete(entity.费用[0]);

            rep.Initialize(dzd.应收应付款, dzd);
            //System.Diagnostics.Debug.Assert(dzd.应收应付款.Count == 1, "坏账应收应付款有且只有一条！");
            //rep.Delete(dzd.应收应付款[0]);
            foreach (应收应付款 i in dzd.应收应付款)
            {
                rep.Delete(i);
            }
            dzd.应收应付款.Clear();

            rep.Delete(dzd);

            entity.Submitted = false;
            this.Update(rep, entity);
        }
    }
}
