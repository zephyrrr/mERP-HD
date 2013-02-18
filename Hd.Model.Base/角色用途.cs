using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(角色用途), Table = "信息_角色用途", OptimisticLock = OptimisticLockMode.Version)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "代码")]
    public class 角色用途 : BaseEntity<string>
    {
        public override string Identity
        {
            get { return this.代码; }
        }

        [Id(0, Name = "代码", Length = 2)]
        [Generator(1, Class = "assigned")]
        public virtual string 代码
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = true)]
        public virtual string 类型
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 客户
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 员工
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 报关组
        {
            get;
            set;
        }
    }
}
