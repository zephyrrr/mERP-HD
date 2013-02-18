//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Jk
//{
//    [Serializable]
//    [Auditable]
//    [Class(NameType = typeof(进口费用信息附加字段), Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version, SchemaAction = "none")]
//    public class 进口费用信息附加字段 : IEntity
//    {
//        [Id(0, Name = "Id", Column = "Id")] //only for when length is meaningful
//        [Generator(1, Class = "assigned")]
//        public virtual Guid Id
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = false)]
//        public virtual decimal? 委托人承担
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = false)]
//        public virtual decimal? 车队承担
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = false)]
//        public virtual decimal? 对外付款
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = false)]
//        public virtual decimal? 自己承担
//        {
//            get;
//            set;
//        }
//    }
//}
