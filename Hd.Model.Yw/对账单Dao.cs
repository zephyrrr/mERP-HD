using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Windows.Utils;

namespace Hd.Model
{
    public enum 货代对账单类型
    {
        自动凭证应付对账单 = 1, //用于滞箱费付款，不分业务类型

        货代应收对账单 = 2, //用于委托人对账，并且各个业务类型分开
        货代应付对账单 = 3, //用于受托人对账，对外对账，可收可付
    }

    public class 对账单Dao : BaseSubmittedDao<对账单>
    {
        internal static void GenerateDzdYsyf(IRepository rep, 对账单 entity)
        {
            if (entity.对账单类型 == (int)货代对账单类型.自动凭证应付对账单)
                return;

            if (!entity.Submitted)
            {
                throw new InvalidUserOperationException("对账单还未提交！");
            }

            rep.Initialize(entity.费用, entity);

            entity.应收应付款 = new List<应收应付款>();

            // 业务类型编号, 相关人编号 // 费用项编号 = "000"
            Dictionary<Tuple<int, string, string>, IList<费用>> dict;

            dict = Feng.Utils.CollectionHelper.Group<费用, Tuple<int, string, string>>(entity.费用,
                            new Feng.Utils.CollectionHelper.GetGroupKey<费用, Tuple<int, string, string>>(delegate(费用 i)
                            {
                                if (!i.费用类别编号.HasValue
                                   || string.IsNullOrEmpty(i.相关人编号)
                                   || !i.金额.HasValue)
                                {
                                    return null;
                                }
                                return new Tuple<int, string, string>(i.费用实体.费用实体类型编号, i.相关人编号, i.费用类别.大类);
                            }));


            foreach (KeyValuePair<Tuple<int, string, string>, IList<费用>> kvp in dict)
            {
                decimal sum = 0;
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

                应收应付款 ysyfk = new 应收应付款();
                if (kvp.Key.Item3 == "业务常规")
                {
                    ysyfk.费用项编号 = "000";
                }
                else
                {
                    ysyfk.费用项编号 = "001";
                }
                ysyfk.结算期限 = entity.结算期限.HasValue ? entity.结算期限.Value : entity.关账日期.Value;
                ysyfk.金额 = sum;
                ysyfk.日期 = entity.关账日期.Value;
                ysyfk.收付标志 = entity.收付标志;
                ysyfk.相关人编号 = kvp.Key.Item2 ;
                ysyfk.业务类型编号 = kvp.Key.Item1;
                ysyfk.应收应付源 = entity;

                (new HdBaseDao<应收应付款>()).Save(rep, ysyfk);

                entity.应收应付款.Add(ysyfk);
            }
        }

        internal static void UngenerateDzdYsyf(IRepository rep, 对账单 dzd)
        {
            if (dzd.对账单类型 == (int)货代对账单类型.自动凭证应付对账单)
                return;

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
            if (!entity.关账日期.HasValue)
            {
                throw new InvalidUserOperationException("请输入 关账日期！");
            }

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
                case (int)货代对账单类型.自动凭证应付对账单:
                    Dictionary<string, object> dict = ProcessInfoHelper.ExecuteProcess("自动凭证应付对账单", new Dictionary<string, object> { { "entity", entity } }) as Dictionary<string, object>;
                    自动对账单生成凭证(rep, entity, dict);
                    break;
                case (int)货代对账单类型.货代应付对账单:
                case (int)货代对账单类型.货代应收对账单:
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
                case (int)货代对账单类型.自动凭证应付对账单:
                    自动对账单作废凭证(rep, entity);
                    break;
                case (int)货代对账单类型.货代应付对账单:
                case (int)货代对账单类型.货代应收对账单:
                    UngenerateDzdYsyf(rep, entity);
                    break;
                default:
                    throw new NotSupportedException("Not Supported 对账单类型 of " + entity.对账单类型 + "!");
            }
        }

        private static void 自动对账单生成凭证(IRepository rep, 对账单 dzd, Dictionary<string, object> properties)
        {
            凭证 pz = new 凭证();
            pz.Submitted = true;
            pz.备注 = null;
            pz.出纳编号 = null;
            pz.审核状态 = true;
            pz.收支状态 = false;
            pz.会计编号 = SystemConfiguration.UserName;
            pz.会计金额 = dzd.金额;
            //pz.金额.币制编号 = properties["凭证币制编号"].ToString();
            //pz.金额.数额 = (decimal)properties["凭证金额"];
            pz.金额.币制编号 = "CNY";
            pz.金额.数额 = dzd.金额;
            pz.凭证类别 = 凭证类别.付款凭证;
            pz.日期 = dzd.关账日期.Value;
            pz.审核人编号 = null;
            pz.相关人编号 = dzd.相关人编号;
            pz.自动手工标志 = 自动手工标志.对账单;
            pz.凭证费用明细 = new List<凭证费用明细>();
            pz.审核状态 = true;

            Dictionary<int, Dictionary<收付标志, Dictionary<string, Dictionary<string, List<费用>>>>> dict =
                new Dictionary<int, Dictionary<收付标志, Dictionary<string, Dictionary<string, List<费用>>>>>();

            foreach (费用 item in dzd.费用)
            {
                if (string.IsNullOrEmpty(item.费用项编号)
                    || string.IsNullOrEmpty(item.相关人编号)
                    || !item.金额.HasValue)
                {
                    continue;
                }

                if (!dict.ContainsKey(item.费用实体.费用实体类型编号))
                {
                    dict[item.费用实体.费用实体类型编号] = new Dictionary<收付标志, Dictionary<string, Dictionary<string, List<费用>>>>();
                }
                if (!dict[item.费用实体.费用实体类型编号].ContainsKey(item.收付标志))
                {
                    dict[item.费用实体.费用实体类型编号][item.收付标志] = new Dictionary<string, Dictionary<string, List<费用>>>();
                }
                if (!dict[item.费用实体.费用实体类型编号][item.收付标志].ContainsKey(item.费用项编号))
                {
                    dict[item.费用实体.费用实体类型编号][item.收付标志][item.费用项编号] = new Dictionary<string, List<费用>>();
                }
                if (!dict[item.费用实体.费用实体类型编号][item.收付标志][item.费用项编号].ContainsKey(item.相关人编号))
                {
                    dict[item.费用实体.费用实体类型编号][item.收付标志][item.费用项编号][item.相关人编号] = new List<费用>();
                }
                dict[item.费用实体.费用实体类型编号][item.收付标志][item.费用项编号][item.相关人编号].Add(item);
            }

            IList<凭证费用明细> ret = new List<凭证费用明细>();
            foreach (KeyValuePair<int, Dictionary<收付标志, Dictionary<string, Dictionary<string, List<费用>>>>> k0 in dict)
            {
                foreach (KeyValuePair<收付标志, Dictionary<string, Dictionary<string, List<费用>>>> k1 in k0.Value)
                {
                    foreach (KeyValuePair<string, Dictionary<string, List<费用>>> k2 in k1.Value)
                    {
                        foreach (KeyValuePair<string, List<费用>> k3 in k2.Value)
                        {
                            凭证费用明细 pzs1 = new 凭证费用明细();

                            decimal sum = 0;
                            foreach (费用 k4 in k3.Value)
                            {
                                sum += k4.金额.Value;
                                k4.凭证费用明细 = pzs1;
                            }

                            pzs1.业务类型编号 = k0.Key;

                            pzs1.费用 = k3.Value;
                            pzs1.费用项编号 = k2.Key;
                            pzs1.金额 = sum;
                            pzs1.收付标志 = k1.Key;
                            pzs1.相关人编号 = k3.Key;

                            pzs1.凭证 = pz;

                            pz.凭证费用明细.Add(pzs1);
                        }
                    }
                }
            }

            (new HdBaseDao<凭证>()).Save(rep, pz);
            foreach (凭证费用明细 pzs1 in pz.凭证费用明细)
            {
                (new HdBaseDao<凭证费用明细>()).Save(rep, pzs1);

                foreach (费用 fee in pzs1.费用)
                {
                    (new 费用Dao()).Update(rep, fee);
                }
            }

            // dzd.凭证 = pz;
        }

        private static void 自动对账单作废凭证(IRepository rep, 对账单 dzd)
        {
            IList<凭证> pzs = (rep as Feng.NH.INHibernateRepository).List<凭证>(NHibernate.Criterion.DetachedCriteria.For<凭证>()
                .CreateCriteria("凭证费用明细")
                .CreateCriteria("费用")
                .Add(NHibernate.Criterion.Expression.Eq("对账单", dzd)));
            

            if (pzs.Count == 0)
            {
                throw new ArgumentException("对账单" + dzd.编号 + "无对应凭证，请重新确认！");
            }

            (new 凭证Dao()).Cancellate(rep, pzs[0]);

            //dzd.凭证 = null;
        }
    }
}
