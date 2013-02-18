using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using NHibernate.Mapping.Attributes;

namespace Hd.Model
{
    /// <summary>
    /// 可提交的记录
    /// </summary>
    public abstract class SubmittedEntity : BaseBOEntity, ISubmittedEntity
    {
        /// <summary>
        /// 是否已提交
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool Submitted
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 可提交的记录
    /// </summary>
    public abstract class SubmittedEntity2 : SubmittedEntity, ISubmittedEntity2
    {
        /// <summary>
        /// 是否已提交
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool Submitted2
        {
            get;
            set;
        }
    }
}
