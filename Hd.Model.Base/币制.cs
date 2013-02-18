using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(币制), Table = "参数备案_币制", OptimisticLock = OptimisticLockMode.Version, SchemaAction = "none")]
    //[Cache(Usage = CacheUsage.ReadOnly, Include = CacheInclude.All)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "代码")]
    public class 币制 : BaseEntity<string>
    {
        public override string Identity
        {
            get { return this.代码; }
        }
        [Id(0, Name = "代码", Length = 3)]
        [Generator(1, Class = "assigned")]
        public virtual string 代码
        {
            get;
            set;
        }

        [Property(Length = 10, NotNull = true, Unique = true, UniqueKey = "UK_币制_名称")]
        public virtual string 名称
        {
            get;
            set;
        }

        [Property(Length = 3, NotNull = true)]
        public virtual string 符号
        {
            get;
            set;
        }
    }
}
