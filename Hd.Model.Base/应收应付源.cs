using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(应收应付源), Table = "财务_应收应付源", OptimisticLock = OptimisticLockMode.Version, Abstract = true)]
    public abstract class 应收应付源 : SubmittedEntity, IOperatingEntity
    {
        public virtual void PreparingOperate(OperateArgs e) { }

        public virtual void PreparedOperate(OperateArgs e) { }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "应收应付源")]
        [OneToMany(2, ClassType = typeof(应收应付款), NotFound = NotFoundMode.Ignore)]
        public virtual IList<应收应付款> 应收应付款
        {
            get;
            set;
        }
    }
}
