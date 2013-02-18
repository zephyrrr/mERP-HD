using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Class(NameType = typeof(滞箱费费用信息), Table = "视图查询_费用信息_承担_滞箱费", Mutable = false, SchemaAction = "none")]
    public class 滞箱费费用信息 : IEntity
    {
        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 退滞箱费已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 退滞箱费未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 最终滞箱费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 未确认滞箱费
        {
            get;
            set;
        }
    }
}
