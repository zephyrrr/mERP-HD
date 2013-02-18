using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Hd.Model.Kj;

namespace Hd.Model.Cn
{
    public class 承兑汇票Bll : HdBaseDao<承兑汇票>, I支付方式, I收入方式
    {
        #region "Interface"
        public void 支付(IRepository rep, 凭证收支明细 entity)
        {
            if (string.IsNullOrEmpty(entity.票据号码))
            {
                throw new InvalidUserOperationException("请填写票据号码！");
            }

            承兑汇票 item = GetFrom票据号码(rep, entity.票据号码);
            if (item.金额 != entity.金额
                || "CNY" != entity.凭证.金额.币制编号
                || item.Submitted || item.托收贴现 != null)
            {
                throw new InvalidUserOperationException("承兑汇票金额、币制或者状态不对，请查证！");
            }
            item.Submitted = true;
            this.Update(rep, item);
        }

        public void 取消支付(IRepository rep, 凭证收支明细 entity)
        {
            承兑汇票 item = GetFrom票据号码(rep, entity.票据号码);
            if (item == null)
            {
                throw new InvalidOperationException("未找到票据号码为" + entity.票据号码 + "的承兑汇票!");
            }
            if (!item.Submitted || item.托收贴现 != null)
            {
                throw new InvalidUserOperationException("承兑汇票状态不对，请查证！");
            }
            item.Submitted = false;
            this.Update(rep, item);
        }

        public void 收入(IRepository rep, 凭证收支明细 entity)
        {
            if ("CNY" != entity.凭证.金额.币制编号)
            {
                throw new InvalidUserOperationException("币制不对，请查证！");
            }
            if (!entity.承兑期限.HasValue
                || string.IsNullOrEmpty(entity.出票银行)
                || string.IsNullOrEmpty(entity.票据号码)
                || string.IsNullOrEmpty(entity.付款人编号))
            {
                throw new InvalidUserOperationException("请填写承兑期限、出票银行、票据号码和付款人！");
            }

            承兑汇票 item = new 承兑汇票();
            item.承兑期限 = entity.承兑期限.Value;
            item.出票银行 = entity.出票银行;
            item.票据号码 = entity.票据号码;
            item.付款人编号 = entity.付款人编号;
            item.Submitted = false;
            item.金额 = entity.金额.Value;

            this.Save(rep, item);
        }

        public void 取消收入(IRepository rep, 凭证收支明细 entity)
        {
            承兑汇票 item = GetFrom票据号码(rep, entity.票据号码);
            if (item == null)
            {
                throw new InvalidOperationException("未找到票据号码为" + entity.票据号码 + "的承兑汇票!");
            }
            if (item.Submitted || item.托收贴现 != null)
            {
                throw new InvalidUserOperationException("承兑汇票状态不对，请查证！");
            }
            this.Delete(rep, item);
        }

        public 承兑汇票 GetFrom票据号码(IRepository rep, string 票据号码)
        {
            IList<承兑汇票> list = (rep as Feng.NH.INHibernateRepository).List<承兑汇票>(NHibernate.Criterion.DetachedCriteria.For<承兑汇票>()
                .Add(NHibernate.Criterion.Expression.Eq("票据号码", 票据号码)));
                
            if (list.Count == 0)
            {
                throw new InvalidOperationException(票据号码 + "的承兑汇票不存在！");
            }
            else if (list.Count == 1)
            {
                return list[0];
            }
            else
            {
                throw new InvalidOperationException(票据号码 + "的承兑汇票有重复记录！");
            }
        }
        #endregion

        public 承兑汇票Bll()
        {
            this.EntityOperating += new EventHandler<OperateArgs<承兑汇票>>(承兑汇票Bll_EntityOperating);
        }

        void 承兑汇票Bll_EntityOperating(object sender, OperateArgs<承兑汇票> e)
        {
            switch (e.OperateType)
            {
                case OperateType.Update:
                    if (e.Entity.托收贴现.HasValue)
                    {
                        if (e.Entity.返回方式.HasValue)
                        {
                            e.Repository.Initialize(e.Entity.费用, e.Entity);
                            if (e.Entity.费用.Count == 0)
                            {
                                decimal? txf = e.Entity.金额 - e.Entity.返回金额;
                                if (!ServiceProvider.GetService<IMessageBox>().ShowYesNo("贴息费为" + txf.Value.ToString("N2") + "元，是否正确？", "确认"))
                                {
                                    throw new InvalidUserOperationException("金额填写错误，请重新填写！");
                                }

                                非业务费用 fee = new 非业务费用();
                                fee.费用实体 = e.Entity;
                                fee.费用项编号 = "333"; // 贴息费
                                fee.金额 = e.Entity.金额 - e.Entity.返回金额;
                                fee.收付标志 = 收付标志.付;
                                fee.相关人编号 = e.Entity.经办人编号;

                                凭证 pz = new 凭证();
                                pz.Submitted = true;
                                pz.备注 = e.Entity.备注;
                                pz.出纳编号 = SystemConfiguration.UserName;// e.Entity.返回经手人编号;
                                pz.收支状态 = true;
                                pz.会计编号 = SystemConfiguration.UserName;// e.Entity.返回经手人编号;
                                pz.会计金额 = fee.金额;
                                pz.金额.币制编号 = "CNY";
                                pz.金额.数额 = pz.会计金额.Value;
                                pz.凭证类别 = 凭证类别.付款凭证;
                                pz.日期 = e.Entity.返回时间.Value;
                                pz.审核人编号 = null;
                                pz.相关人编号 = fee.相关人编号;
                                pz.自动手工标志 = 自动手工标志.承兑汇票;
                                pz.凭证费用明细 = new List<凭证费用明细>();
                                pz.凭证收支明细 = new List<凭证收支明细>();
                                pz.审核状态 = true;

                                凭证费用明细 pzs1 = new 凭证费用明细();
                                pzs1.费用项编号 = "333";
                                pzs1.金额 = pz.会计金额;
                                pzs1.凭证 = pz;

                                pzs1.收付标志 = 收付标志.付;
                                pzs1.相关人编号 = pz.相关人编号;
                                pzs1.费用 = new List<费用>();

                                pzs1.费用.Add(fee);
                                fee.凭证费用明细 = pzs1;

                                pz.凭证费用明细.Add(pzs1);


                                凭证收支明细 pzs2 = new 凭证收支明细();
                                pzs2.凭证 = pz;
                                pzs2.金额 = e.Entity.金额;
                                pzs2.收付标志 = 收付标志.付;
                                pzs2.收付款方式 = 收付款方式.银行承兑汇票;
                                pzs2.票据号码 = e.Entity.票据号码;
                                pz.凭证收支明细.Add(pzs2);

                                凭证收支明细 pzs3 = new 凭证收支明细();
                                pzs3.凭证 = pz;
                                pzs3.金额 = e.Entity.返回金额;
                                pzs3.收付标志 = 收付标志.收;
                                if (e.Entity.返回方式.Value == 承兑汇票返回方式.银行)
                                {
                                    pzs3.收付款方式 = 收付款方式.银行收付;
                                    pzs3.银行账户编号 = e.Entity.入款账户编号;
                                }
                                else
                                {
                                    pzs3.收付款方式 = 收付款方式.现金;
                                }
                                pz.凭证收支明细.Add(pzs3);


                                (new HdBaseDao<凭证>()).Save(e.Repository, pz);
                                (new HdBaseDao<凭证费用明细>()).Save(e.Repository, pzs1);
                                (new HdBaseDao<凭证收支明细>()).Save(e.Repository, pzs2);
                                (new HdBaseDao<凭证收支明细>()).Save(e.Repository, pzs3);

                                (new 非业务费用Dao()).Save(e.Repository, fee);
                                e.Entity.费用.Add(fee);
                            }
                        }
                        else
                        {
                            e.Repository.Initialize(e.Entity.费用, e.Entity);
                            System.Diagnostics.Debug.Assert(e.Entity.费用.Count <= 1, "承兑汇票费用只有贴息费一项！");
                            if (e.Entity.费用.Count == 1)
                            {
                                非业务费用 fee = e.Entity.费用[0] as 非业务费用;
                                凭证费用明细 pzs1 = fee.凭证费用明细;
                                if (pzs1 != null)
                                {
                                    凭证 pz = pzs1.凭证;
                                    e.Repository.Initialize(pz.凭证收支明细, pz);
                                    foreach (凭证收支明细 pzs2 in pz.凭证收支明细)
                                    {
                                        (new HdBaseDao<凭证收支明细>()).Delete(e.Repository, pzs2);
                                    }
                                    pz.Submitted = false;
                                    pz.是否作废 = true;
                                    pz.审核状态 = false;
                                    pz.收支状态 = false;
                                    (new HdBaseDao<凭证>()).Update(e.Repository, pz);

                                    (new HdBaseDao<凭证费用明细>()).Update(e.Repository, pzs1);
                                }

                                fee.凭证费用明细 = null;
                                (new 非业务费用Dao()).Delete(e.Repository, fee);
                                e.Entity.费用.Remove(e.Entity.费用[0]);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
