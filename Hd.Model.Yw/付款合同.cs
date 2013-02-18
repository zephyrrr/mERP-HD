using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(付款合同), Table = "参数备案_付款合同", OptimisticLock = OptimisticLockMode.Version)]
    public class 付款合同 : BaseBOEntity,
        IMasterEntity<付款合同, 付款合同费用项>
    {
        #region "interface"
        IList<付款合同费用项> IMasterEntity<付款合同, 付款合同费用项>.DetailEntities
        {
            get { return 合同费用项; }
            set { 合同费用项 = value; }
        }
        #endregion

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_付款合同_费用类别")]
        public virtual 费用类别 业务类型
        {
            get;
            set;
        }

        [Property(Column = "业务类型", NotNull = true)]
        public virtual int 业务类型编号
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "付款合同")]
        [OneToMany(2, ClassType = typeof(付款合同费用项), NotFound = NotFoundMode.Ignore)]
        public virtual IList<付款合同费用项> 合同费用项
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 生效时间
        {
            get;
            set;
        }
    }
}
