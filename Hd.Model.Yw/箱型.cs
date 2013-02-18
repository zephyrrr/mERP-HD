using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(箱型), Table = "参数备案_箱型", OptimisticLock = OptimisticLockMode.Version)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "编号")]
    public class 箱型 : BaseEntity<int>
    {
        public override int Identity
        {
            get { return this.编号; }
        }

        [Id(0, Name = "编号")]
        [Generator(1, Class = "assigned")]
        public virtual int 编号
        {
            get;
            set;
        }

        [Property(Length = 10, NotNull = true, Unique = true, UniqueKey = "UK_箱型_名称")]
        public virtual string 名称
        {
            get;
            set;
        }

        ///<summary>
        ///中文名称
        ///</summary>
        [Property(Length = 20)]
        public virtual string 中文名称
        {
            get;
            set;
        }

        ///<summary>
        ///备注
        ///</summary>
        [Property(Length = 100)]
        public virtual string 备注
        {
            get;
            set;
        }
    }
}
