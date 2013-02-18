using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cn
{
    public enum 支票类型
    {
        //Invalid = 0,
        现金支票 = 1,
        转账支票 = 2
    }

    public enum 领用方式
    {
        空白支票 = 1,
        直接支付 = 2
    }

    //public enum 转账支票状态
    //{
    //    未使用 = 1,
    //    //空白支票已领用未反馈 = 2,
    //    已领用未支付 = 3,
    //    已支付 = 4,
    //    已转账 = 5,
    //    已作废 = 7,
    //}

    //public enum 现金支票状态
    //{
    //    未使用 = 1,
    //    //空白支票已领用未反馈 = 2,
    //    已领用未支付 = 3,
    //    已支付 = 4,
    //    已提现 = 5,
    //    已作废 = 7,
    //}

    [Serializable]
    [Auditable]
    [Class(Name = "支票", Table = "财务_支票", OptimisticLock = OptimisticLockMode.Version, DiscriminatorValue = "0")]
    [Discriminator(Column = "支票类型", TypeType = typeof(int))]
    public class 支票 : SubmittedEntity
    {
        [Property(NotNull = true, Length = 50, Unique = true, Index = "Idx_支票_票据号码", UniqueKey = "UK_支票_票据号码")]
        public virtual string 票据号码
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 买入时间
        {
            get;
            set;
        }

        [ManyToOne(NotNull = true, Insert = false, Update = false, ForeignKey = "FK_支票_银行账户")]
        public virtual 银行账户 银行账户
        {
            get;
            set;
        }

        [Property(Column = "银行账户", NotNull = true)]
        public virtual Guid 银行账户编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 19, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 领用时间
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_支票_经办人")]
        public virtual 人员 经办人
        {
            get;
            set;
        }

        [Property(Column = "经办人", Length = 6)]
        public virtual string 经办人编号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 领用方式? 领用方式
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Property(Length = 10)]
        public virtual string 支付凭证号
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 日期
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 摘要//出款账户、入款账户、支付凭证号、凭证收款人、费用项
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否作废
        {
            get;
            set;
        }
    }
}
