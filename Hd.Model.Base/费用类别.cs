using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(费用类别), Table = "信息_费用类别", OptimisticLock = OptimisticLockMode.Version)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "代码")]
    public class 费用类别 : BaseEntity<int>
    {
        public override int Identity
        {
            get { return this.代码; }
        }

        [Id(0, Name = "代码")]
        [Generator(1, Class = "assigned")]
        public virtual int 代码
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = false)]
        public virtual string 类型
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = false)]
        public virtual string 大类
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = false)]
        public virtual string 小类
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool 收
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool 付
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int 收入类别
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int 支出类别
        {
            get;
            set;
        }


        [Property(Length = 2, NotNull = false)]
        public virtual string 前缀
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool? 业务
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool? 非业务
        {
            get;
            set;
        }

        [Property(Length = 50, NotNull = false)]
        public virtual string 代码类型名
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool? 费用实体类型
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool? 业务类型
        {
            get;
            set;
        }

        [Property(Column = "费用类别", NotNull = false)]
        public virtual bool? 费用类别标志
        {
            get;
            set;
        }
    }
}
