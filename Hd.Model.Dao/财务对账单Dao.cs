using System;
using System.Collections.Generic;
using System.Text;
using Feng;

namespace Hd.Model
{
    public enum 财务对账单类型
    {
        应收调节款 = 4, //用于委托人应收款的调节款，各个业务类型分开
        坏账 = 5,       //用于委托人应收款的坏账，各个业务类型分开(可收可付，可业务可非业务) (收付标志需手工选)
        固定资产折旧 = 6    // 固定资产折旧（费用可自动生成）
    }


    public class 财务对账单Dao : BaseSubmittedDao<对账单>
    {
        internal static void GenerateDzdYsyf(IRepository rep, 对账单 entity)
        {
            
            if (!entity.Submitted)
            {
                throw new InvalidUserOperationException("对账单还未提交！");
            }

            rep.Initialize(entity.费用, entity);

            entity.应收应付款 = new List<应收应付款>();

            // 业务类型编号, 相关人编号 // 费用项编号 = "000"

            Dictionary<Tuple<int, string>, IList<费用>> dict = Utility.GroupFyToDzdYsyf(entity.费用);
            
            foreach (KeyValuePair<Tuple<int, string>, IList<费用>> kvp in dict)
            {
                decimal sum = 0;

                应收应付款 ysyfk = new 应收应付款();

                if (entity.对账单类型 == (int)财务对账单类型.固定资产折旧)
                {
                    foreach (费用 k4 in kvp.Value)
                    {
                        if (k4.收付标志 == 收付标志.付)
                        {
                            sum -= k4.金额.Value;
                        }
                        else
                        {
                            sum += k4.金额.Value;
                        }
                    }

                    ysyfk.费用项编号 = "004";
                    ysyfk.收付标志 = 收付标志.收;
                    ysyfk.业务类型编号 = 24;
                }
                else
                {
                    foreach (费用 k4 in kvp.Value)
                    {
                        if (k4.收付标志 == entity.收付标志)
                        {
                            sum += k4.金额.Value;
                        }
                        else
                        {
                            sum -= k4.金额.Value;
                        }
                    }

                    ysyfk.费用项编号 = "000";                    
                    ysyfk.收付标志 = entity.收付标志;
                    ysyfk.业务类型编号 = kvp.Key.Item1;
                }

                ysyfk.结算期限 = entity.结算期限.HasValue ? entity.结算期限.Value : entity.关账日期.Value;
                ysyfk.金额 = sum;
                ysyfk.日期 = entity.关账日期.Value;
                ysyfk.相关人编号 = kvp.Key.Item2;                
                ysyfk.应收应付源 = entity;

                (new HdBaseDao<应收应付款>()).Save(rep, ysyfk);

                entity.应收应付款.Add(ysyfk);
            }
        }

        internal static void UngenerateDzdYsyf(IRepository rep, 对账单 dzd)
        {
            rep.Initialize(dzd.应收应付款, dzd);

            //IList<应收应付款> ysyfk = rep.Session.CreateCriteria<应收应付款>()
            //    .Add(NHibernate.Criterion.Expression.Eq("应收应付源", dzd))
            //    .List<应收应付款>();
            foreach(应收应付款 i in dzd.应收应付款)
            {
                rep.Delete(i);
            }
            dzd.应收应付款.Clear();
        }

         /// <summary>
        /// 提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Submit(IRepository rep, 对账单 entity)
        {
            entity.Submitted = true;

            rep.Initialize(entity.费用, entity);
            decimal sum = 0;
            foreach (费用 i in entity.费用)
            {
                if (i.收付标志 == 收付标志.收)
                {
                    sum += i.金额.Value;
                }
                else
                {
                    sum -= i.金额.Value;
                }

                if (i.凭证费用明细 != null)
                {
                    throw new InvalidUserOperationException("费用已经出凭证！");
                }
            }
            if (entity.收付标志 == 收付标志.收)
            {
                entity.金额 = sum;
            }
            else
            {
                entity.金额 = -sum;
            }

            this.Update(rep, entity);

            费用Dao fyDao = new 费用Dao();
            switch (entity.对账单类型)
            {
                case (int)财务对账单类型.固定资产折旧:
                    foreach (费用 i in entity.费用)
                    {
                        fyDao.Update(rep, i);
                    }

                    GenerateDzdYsyf(rep, entity);
                    break;
                case (int)财务对账单类型.坏账:
                    break;
                case (int)财务对账单类型.应收调节款:
                    foreach (费用 i in entity.费用)
                    {
                        fyDao.Update(rep, i);
                    }

                    GenerateDzdYsyf(rep, entity);
                    break;
                default:
                    throw new NotSupportedException("Not Supported 对账单类型 of " + entity.对账单类型 + "!");
            }
        }


        /// <summary>
        /// 撤销提交
        /// </summary>
        /// <param name="rep"></param>
        /// <param name="entity"></param>
        public override void Unsubmit(IRepository rep, 对账单 entity)
        {
            entity.Submitted = false;
            entity.金额 = null;

            this.Update(rep, entity);

            rep.Initialize(entity.费用, entity);
            foreach (费用 i in entity.费用)
            {
                if (i.凭证费用明细 != null)
                {
                    throw new InvalidUserOperationException("费用已经出凭证！");
                }
            }

            switch (entity.对账单类型)
            {
                case (int)财务对账单类型.固定资产折旧:
                    UngenerateDzdYsyf(rep, entity);
                    break;
                case (int)财务对账单类型.坏账:
                    break;
                case (int)财务对账单类型.应收调节款:
                    UngenerateDzdYsyf(rep, entity);
                    break;
                default:
                    throw new NotSupportedException("Not Supported 对账单类型 of " + entity.对账单类型 + "!");
            }
        }
    }
}
