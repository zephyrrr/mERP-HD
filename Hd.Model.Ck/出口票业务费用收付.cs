using System;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck
{
    [Serializable]
    [Auditable]
    //[Class(NameType = typeof(出口票业务费用收付), Table = "视图查询_财务费用_收付", Mutable = false, SchemaAction = "none")]
    public class 出口票业务费用收付 : Hd.Model.业务费用收付
    {
        //[OneToOne(ClassType = typeof(出口票))]
        [ManyToOne(Insert = false, Update = false, Column = "费用实体", NotNull = false)]
        public virtual 出口票 票
        {
            get;
            set;
        }
    }
}
