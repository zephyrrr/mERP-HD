using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Class(NameType = typeof(费用信息附加字段), Table = "视图查询_费用信息_承担", Mutable = false)]
    public class 费用信息附加字段 : IEntity
    {
        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 车队已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 车队未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 委托人已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 委托人未确认
        {
            get;
            set;
        }
    }
}
