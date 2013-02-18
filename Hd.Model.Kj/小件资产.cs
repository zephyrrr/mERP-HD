using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Kj
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "小件资产", ExtendsType = typeof(费用实体), Table = "财务_小件资产")]
    [Key(Column = "Id", ForeignKey = "FK_小件资产_费用实体")]
    public class 小件资产 : 费用实体
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

        [Property(NotNull = false)]
        public virtual DateTime 购入时间
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 简介
        {
            get;
            set;
        }

        [Property(Length = 50)]
        public virtual string 发票号码
        {
            get;
            set;
        }

        [Property(Length = 200)]
        public virtual string 购入卖出信息
        {
            get;
            set;
        }

        [Property(Length = 200)]
        public virtual string 售前售后信息
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 固定资产状态 状态
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 购入金额
        {
            get;
            set;
        }

        [Property(Length = 19, Precision = 19, Scale = 2)]
        public virtual decimal? 卖出金额
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 卖出时间
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 备注
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

        //    小件资产 that = (小件资产)other;
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
        //    sb.Append("小件资产: ");
        //    sb.Append("编号").Append('=').Append(编号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
