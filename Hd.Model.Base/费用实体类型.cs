using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(费用实体类型), Table = "信息_费用实体类型", OptimisticLock = OptimisticLockMode.Version)]
    public class 费用实体类型 : BaseAbstractEntity<int>
    {
        [Id(0, Name = "代码")]
        [Generator(1, Class = "assigned")]
        public virtual int 代码
        {
            get { return Id; }
            set { Id = value; }
        }

        [Property(Length = 50, NotNull = true)]
        public virtual string 类型
        {
            get;
            set;
        }

        [Property(Length = 2, NotNull = true, Unique = true)]
        public virtual string 前缀
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 业务
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 非业务
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = true)]
        public virtual string 代码类型名
        {
            get;
            set;
        }
    }
}
