using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ba
{
    [Class(NameType = typeof(地名), Table = "参数备案_地名", OptimisticLock = OptimisticLockMode.Version)]
    public class 地名 : BaseBOEntity
    {
        /// <summary>
        /// 地名
        /// </summary>
        [Property(Length = 50, NotNull = true, Unique = true, Index = "Idx_地名_简称", UniqueKey = "UK_地名_简称")]
        public virtual string 简称
        {
            get;
            set;
        }

        /// <summary>
        /// 全称
        /// </summary>
        [Property(Length = 50, NotNull = true, Unique = true, UniqueKey = "UK_地名_全称")]
        public virtual string 全称
        {
            get;
            set;
        }

        /// <summary>
        ///备注
        /// </summary>
        [Property(Length = 200)]
        public virtual string 备注
        {
            get;
            set;
        }

        /// <summary>
        /// 省
        /// </summary>
        [Property(Length = 6, NotNull = true)]
        public virtual string 省
        {
            get;
            set;
        }

        /// <summary>
        /// 市
        /// </summary>
        [Property(Length = 6, NotNull = true)]
        public virtual string 市
        {
            get;
            set;
        }

        /// <summary>
        /// 区县
        /// </summary>
        [Property(Length = 6, NotNull = true)]
        public virtual string 区县
        {
            get;
            set;
        }
    }
}
