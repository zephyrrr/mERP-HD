using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(业务类型), Table = "信息_业务类型", OptimisticLock = OptimisticLockMode.Version)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "代码")]
    public class 业务类型 : BaseAbstractEntity<int>
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

        [Property(NotNull = true)]
        public virtual bool 收
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 付
        {
            get;
            set;
        }

        //[ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_业务类型_收入类别")]
        //public virtual 费用类别 收入类别
        //{
        //    get;
        //    set;
        //}

        [Property(Column = "收入类别", NotNull = true)]
        public virtual int 凭证收入类别编号
        {
            get;
            set;
        }

        //[ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_业务类型_支出类别")]
        //public virtual 费用类别 支出类别
        //{
        //    get;
        //    set;
        //}

        [Property(Column = "支出类别", NotNull = true)]
        public virtual int 凭证支出类别编号
        {
            get;
            set;
        }
    }
}
