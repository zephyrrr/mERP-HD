using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Subclass(NameType = typeof(调节业务款), ExtendsType = typeof(费用), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 费用类型.调节业务款)]
    [Discriminator(Column = "费用类型", TypeType = typeof(int))]
    public class 调节业务款 : 费用
    {
        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_业务费用_普通箱")]
        public virtual 普通箱 箱
        {
            get;
            set;
        }

        [Property(Column = "箱", NotNull = false)]
        public virtual Guid? 箱Id
        {
            get;
            set;
        }

        [ManyToOne(Column = "费用实体", Insert = false, Update = false, NotNull = true)]
        public virtual 普通票 票
        {
            get;
            set;
        }
    }

}
