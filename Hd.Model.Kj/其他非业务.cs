using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Kj
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "其他非业务", ExtendsType = typeof(费用实体), Table = "财务_其他非业务")]
    [Key(Column = "Id", ForeignKey = "FK_其他非业务_费用实体")]
    public class 其他非业务 : 费用实体
    {
        [Property(Insert = false, Update = false, Formula = "(SELECT SUM(ISNULL((CASE A.收付标志 WHEN '1' THEN A.金额 ELSE NULL END), 0)) FROM 财务_费用 A WHERE A.费用实体 = Id) ")]
        public virtual decimal 收款小计
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT SUM(ISNULL((CASE A.收付标志 WHEN '2' THEN A.金额 ELSE NULL END), 0)) FROM 财务_费用 A WHERE A.费用实体 = Id) ")]
        public virtual decimal 付款小计
        {
            get;
            set;
        }

        //[Property(NotNull = true)]
        //public virtual DateTime 日期
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///简介
        ///</summary>
        [Property(Length = 500)]
        public virtual string 简介
        {
            get;
            set;
        }

        ///<summary>
        ///备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        ///<summary>
        ///摘要
        ///</summary>
        [Property(Length = 100)]
        public virtual string 摘要//产生应收款时的凭证号等
        {
            get;
            set;
        }

        //#region Overriden Methods

        ///// <summary>
        ///// Returns <code>true</code> if the argument is a Board instance and all identifiers for this entity
        ///// equal the identifiers of the argument entity. Returns <code>false</code> otherwise.
        ///// </summary>
        //public override bool Equals(Object other)
        //{
        //    if (object.ReferenceEquals(this, other))
        //    {
        //        return true;
        //    }

        //    其他非业务 that = (其他非业务)other;
        //    if (that == null)
        //    {
        //        return false;
        //    }
        //    if (this.ID == null || that.ID == null || !this.ID.Equals(that.ID))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Returns a hash code based on this entity's identifiers.
        ///// </summary>
        //public override int GetHashCode()
        //{
        //    int hashCode = 14;
        //    hashCode = 29 * hashCode + (Id == null ? 0 : Id.GetHashCode());
        //    return hashCode;
        //}

        ///// <summary>
        ///// Returns a string representation of this VersionedEntity
        ///// </summary>
        //public override string ToString()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    sb.Append("其他非业务: ");
        //    sb.Append("编号").Append('=').Append(编号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
