using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cn
{
    [Serializable]
    [Auditable]
    [Subclass(Name = "转账支票", ExtendsType = typeof(支票), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 支票类型.转账支票)]
    public class 转账支票 : 支票
    {
        [ManyToOne(NotNull = false, Insert = false, Update = false, ForeignKey = "FK_支票_入款账户")]
        public virtual 银行账户 入款账户
        {
            get;
            set;
        }

        [Property(Column = "入款账户", NotNull = false)]
        public virtual Guid? 入款账户编号
        {
            get;
            set;
        }
    }
}
