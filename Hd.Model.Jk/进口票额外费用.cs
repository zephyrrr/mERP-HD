using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    //[JoinedSubclass(NameType = typeof(进口票额外费用), ExtendsType = typeof(进口票), Table = "视图财务过程_进口票_额外费用_委托人")]
    //[Key(Column = "Id", ForeignKey = "FK_财务过程_进口票_额外费用_业务备案_进口票")]
    [Class(NameType = typeof(进口票额外费用), Table = "视图财务过程_进口票_额外费用_委托人", Mutable = false)]
    public class 进口票额外费用 : IEntity
    {
        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 额外其他费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 滞箱费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 额外修洗箱费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 额外查验费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 倒箱二次开箱费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 疏港费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 堆存费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 额外指运地其他费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 改舱单费
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 滞报金
        {
            get;
            set;
        }

        [Property(NotNull = false, Insert = false, Update = false, Precision = 19, Scale = 2)]
        public virtual decimal? 小计
        {
            get;
            set;
        }
    }
}
