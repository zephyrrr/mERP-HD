using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    //public enum 合同类型
    //{
    //    Invalid = 0
    //    委托人合同 = 1,
    //    付款合同 = 2
    //}

    [Serializable]
    [Auditable]
    [Class(NameType = typeof(合同费用项), Table = "参数备案_合同费用项", OptimisticLock = OptimisticLockMode.Version)]
    //[Discriminator(Column = "合同类型", TypeType = typeof(int))]
    public class 合同费用项 : BaseBOEntity,
        IMasterEntity<合同费用项, 费用理论值信息>
    {
        #region "interface"
        IList<费用理论值信息> IMasterEntity<合同费用项, 费用理论值信息>.DetailEntities
        {
            get { return 费用理论值; }
            set { 费用理论值 = value; }
        }
        #endregion

        [Property(Column = "费用项", NotNull = true, Length = 3)]
        public virtual string 费用项编号
        {
            get;
            set;
        }

        [ManyToOne(NotNull = true, Insert = false, Update = false, ForeignKey = "FK_合同费用项_费用项")]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true, OrderBy = "序号")]
        [Key(1, Column = "合同费用项")]
        //[Index(2, Column = "序号", TypeType = typeof(int))]
        [OneToMany(2, ClassType = typeof(费用理论值信息), NotFound = NotFoundMode.Ignore)]
        public virtual IList<费用理论值信息> 费用理论值
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否生成实际费用
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否空值全部生成
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否判断相关人
        {
            get;
            set;
        }
    }
}
