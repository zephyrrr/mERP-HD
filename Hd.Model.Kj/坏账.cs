using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Kj
{
    /// <summary>
    /// 坏账进入财务费用，进入应收应付
    /// </summary>
    [Serializable]
    [Auditable]
    [JoinedSubclass(NameType = typeof(坏账), ExtendsType = typeof(费用实体), Table = "财务_坏账")]
    [Key(Column = "Id", ForeignKey = "FK_坏账_费用实体")]
    public class 坏账 : 费用实体
    {
        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.编号 FROM 财务_对账单 A INNER JOIN 财务_费用 B ON B.对账单 = A.ID AND B.费用实体 = Id)")]
        public virtual string 对账单号
        {
            get;
            set;
        }

        [ManyToOne(NotNull = true, Insert = false, Update = false, ForeignKey = "FK_坏账_相关人")]
        public virtual 人员 相关人
        {
            get;
            set;
        }

        ///<summary>
        ///相关人
        ///</summary>
        [Property(Column = "相关人", Length = 6, NotNull = true)]
        public virtual string 相关人编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 日期
        {
            get;
            set;
        }

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

        [Property(NotNull = true)]
        public virtual 收付标志 收付标志
        {
            get;
            set;
        }

        [Property(NotNull = true, Precision = 19, Scale = 2)]
        public virtual decimal 金额
        {
            get;
            set;
        }

        // 当是一般应收款时，可以随便填，只是在凭证上显示
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_坏账_费用项")]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        [Property(Column = "费用项", NotNull = true, Length = 3)]
        public virtual string 费用项编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_坏账_费用类别")]
        public virtual 费用类别 业务类型
        {
            get;
            set;
        }

        [Property(Column = "业务类型", NotNull = true)]
        public virtual int 业务类型编号
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

        //    坏账 that = (坏账)other;
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
        //    sb.Append("坏账: ");
        //    sb.Append("坏账").Append('=').Append(this.编号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
