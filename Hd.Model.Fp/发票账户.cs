using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Hd.Model;

namespace Hd.Model.Fp
{
    [Serializable]
    [Auditable]
    [Class(Name = "发票账户", Table = "参数备案_发票账户", OptimisticLock = OptimisticLockMode.Version)]
    public class 发票账户 : BaseBOEntity
    {
        [Property(NotNull = true, Length = 10, Unique = true, UniqueKey = "UK_发票账户_简称")]
        public virtual string 简称
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string 全称
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 简介
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
