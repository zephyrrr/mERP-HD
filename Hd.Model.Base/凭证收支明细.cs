using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 收付款方式
    {
        现金 = 1,
        现金支票 = 2,       //票号
        转账支票 = 3,       //票号、银行账号
        银行承兑汇票 = 4,   //票号、出票银行、承兑期限  注：相关信息参考《出纳_承兑汇票》
        银行本票汇票 = 5,   //票号、银行账号
        银行收付 = 6,        //银行账号
        电汇 = 7,               //银行账号
    }

    //[Serializable]
    //[Auditable]
    //[Subclass(Name="凭证收入明细", ExtendsType = typeof(凭证收支明细), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 收付标志.收)]
    //public class 凭证收入明细 : 凭证收支明细
    //{
    //    [Property(NotNull = true)]
    //    public virtual 收款方式 收款方式
    //    {
    //        get;
    //        set;
    //    }

    //    [Property(Length = 50, NotNull = false)]
    //    public virtual string 出票银行
    //    {
    //        get;
    //        set;
    //    }

    //    [Property(NotNull = false)]
    //    public virtual DateTime? 承兑期限
    //    {
    //        get;
    //        set;
    //    }
    //}

    //[Serializable]
    //[Auditable]
    //[Subclass(Name="凭证支付明细", ExtendsType = typeof(凭证收支明细), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 收付标志.付)]
    //public class 凭证支付明细 : 凭证收支明细
    //{
    //    [Property(NotNull = true)]
    //    public virtual 付款方式 付款方式
    //    {
    //        get;
    //        set;
    //    }

    //    //[Property(Column = "支票", Length = 50)]
    //    //public virtual string 付款支票号码
    //    //{
    //    //    get;
    //    //    set;
    //    //}

    //    //[ManyToOne(NotNull = false, Insert = false, Update = false, ForeignKey = "FK_凭证收入明细_支票")]
    //    //public virtual 支票 支票
    //    //{
    //    //    get;
    //    //    set;
    //    //}
    //}

    [Serializable]
    [Auditable]
    [Class(NameType = typeof(凭证收支明细), Table = "财务_凭证收支明细", OptimisticLock = OptimisticLockMode.Version)]
    //[Discriminator(Column = "收付标志", TypeType = typeof(int))]
    public class 凭证收支明细 : BaseBOEntity, 
        IDetailEntity<凭证, 凭证收支明细>
    {
        #region "Interface"
        凭证 IDetailEntity<凭证, 凭证收支明细>.MasterEntity
        {
            get { return 凭证; }
            set { 凭证 = value; }
        }
        #endregion

        public virtual decimal? 收款金额
        {
            get { return 收付标志 == 收付标志.收 ? (decimal?)金额 : null; }
            set
            {
                if (value.HasValue)
                {
                    收付标志 = 收付标志.收; 金额 = value.Value;
                }
                else
                {
                    if (收付标志 == 收付标志.收)
                    {
                        金额 = null;
                    }
                }
            }
        }
            
        public virtual decimal? 付款金额
        {
            get { return 收付标志 == 收付标志.付 ? (decimal?)金额 : null; }
            set
            {
                if (value.HasValue)
                {
                    收付标志 = 收付标志.付; 金额 = value.Value;
                }
                else
                {
                    if (收付标志 == 收付标志.付)
                    {
                        金额 = null;
                    }
                }
            }
        }


        [Property(NotNull = true)]
        public virtual 收付标志 收付标志
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 收付款方式 收付款方式
        {
            get;
            set;
        }

        #region "only to 收承兑汇票
        [Property(Length = 50, NotNull = false)]
        public virtual string 出票银行
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 承兑期限
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证承兑汇票_付款人")]
        public virtual 人员 付款人
        {
            get;
            set;
        }

        [Property(Column = "付款人", Length = 6, NotNull = false)]
        public virtual string 付款人编号
        {
            get;
            set;
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_凭证收支明细_凭证", Cascade = "none")]
        public virtual 凭证 凭证
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 19, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
        {
            get;
            set;
        }

        [Property(Length = 50)]
        public virtual string 票据号码
        {
            get;
            set;
        }

        [ManyToOne(NotNull = false, Insert = false, Update = false, ForeignKey = "FK_凭证收支明细_银行账户")]
        public virtual 银行账户 银行账户
        {
            get;
            set;
        }

        [Property(Column = "银行账户", NotNull = false)]
        public virtual Guid? 银行账户编号
        {
            get;
            set;
        }
    }
}
