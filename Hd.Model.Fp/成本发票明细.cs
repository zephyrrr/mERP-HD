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
    [Class(NameType = typeof(成本发票明细), Table = "财务_成本发票_明细", OptimisticLock = OptimisticLockMode.Version)]
    public class 成本发票明细 : BaseBOEntity,
         IDetailEntity<成本发票, 成本发票明细>
    {
        #region "Interface"
        成本发票 IDetailEntity<成本发票, 成本发票明细>.MasterEntity
        {
            get { return this.成本发票; }
            set { this.成本发票 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_成本发票明细_成本发票", Cascade = "none")]
        public virtual 成本发票 成本发票
        {
            get;
            set;
        }

        [ManyToOne(NotNull = false, Insert = false, Update = false, ForeignKey = "FK_成本发票_发票账户")]
        public virtual 发票账户 发票账户
        {
            get;
            set;
        }

        [Property(Column = "发票账户", NotNull = true)]
        public virtual Guid 发票账户编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 入账日期
        {
            get;
            set;
        }

        [Property(NotNull = true, Precision = 19, Scale = 2)]
        public virtual decimal 金额
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
