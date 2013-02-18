using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cn
{
    [Serializable]
    [Auditable]
    [Subclass(Name = "现金支票", ExtendsType = typeof(支票), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 支票类型.现金支票)]
    public class 现金支票 : 支票
    {
 

    }
}
