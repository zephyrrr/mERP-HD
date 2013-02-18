using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;

namespace Hd.Model
{
    [Component(ClassType = typeof(金额))]
    public class 金额
    {
        [ManyToOne(Insert = false, Update = false)]
        public virtual 币制 币制
        {
            get;
            set;
        }

        [Property(Column = "币制", NotNull = false, Length = 3)]
        public virtual string 币制编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 数额
        {
            get;
            set;
        }
    }
}
