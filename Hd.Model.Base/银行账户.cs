using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 账户类型
    {
        公司账户 = 0,
        个人账户 = 1
    }

    [Serializable]
    [Auditable]
    [Class(NameType = typeof(银行账户), Table = "参数备案_银行账户", OptimisticLock = OptimisticLockMode.Version)]
    public class 银行账户 : BaseBOEntity
    {
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_银行账户_币制")]
        public virtual 币制 币制
        {
            get;
            set;
        }

        [Property(Column = "币制", NotNull = true, Length = 3)]
        public virtual string 币制编号
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string 账号
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 10, Unique = true, UniqueKey = "UK_银行账户_简称")]
        public virtual string 简称
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string 开户行
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 开户时间
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 账户类型 账户类型
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string 户主
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
    }
}
