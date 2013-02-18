using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cn
{
    public enum 承兑汇票返回方式
    {
        现金 = 1,
        银行 = 2
    }

    public enum 托收贴现
    {
        托收 = 1,
        贴现 = 2
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "承兑汇票", ExtendsType = typeof(费用实体), Table = "财务_承兑汇票")]
    [Key(Column = "Id", ForeignKey = "FK_承兑汇票_费用实体")]
    public class 承兑汇票 : 费用实体
    {
        [Property(NotNull = true, Length = 50, Unique = true, Index = "Idx_承兑汇票_承兑汇票号码", UniqueKey = "UK_承兑汇票_承兑汇票号码")]
        public virtual string 票据号码
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 20)]
        public virtual string 出票银行
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 承兑期限
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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_承兑汇票_付款人")]
        public virtual 人员 付款人
        {
            get;
            set;
        }

        [Property(Column = "付款人", Length = 6, NotNull = true)]
        public virtual string 付款人编号
        {
            get;
            set;
        }

        [Property()]
        public virtual 托收贴现? 托收贴现
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 出去时间
        {
            get;
            set;
        }

        //[ManyToOne(ForeignKey = "FK_承兑汇票_托收凭证", Cascade = "none")]
        //public virtual 付款凭证 托收凭证
        //{
        //    get;
        //    set;
        //}

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_承兑汇票_经办人")]
        public virtual 人员 经办人
        {
            get;
            set;
        }

        [Property(Column = "经办人", Length = 6)]
        public virtual string 经办人编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_承兑汇票_出去经手人")]
        public virtual 人员 出去经手人
        {
            get;
            set;
        }

        [Property(Column = "出去经手人", Length = 6)]
        public virtual string 出去经手人编号
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 返回时间
        {
            get;
            set;
        }

        [Property()]
        public virtual 承兑汇票返回方式? 返回方式
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_承兑汇票_入款账户")]
        public virtual 银行账户 入款账户
        {
            get;
            set;
        }
        [Property(Column = "入款账户")]
        public virtual Guid? 入款账户编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_承兑汇票_返回经手人")]
        public virtual 人员 返回经手人
        {
            get;
            set;
        }

        [Property(Column = "返回经手人", Length = 6)]
        public virtual string 返回经手人编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 返回金额
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

        [Property(Length = 500)]
        public virtual string 摘要//收入时的凭证号、付款人、费用项
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.凭证号 FROM 视图查询_承兑汇票_凭证收付 A WHERE A.收付标志 = 1 AND A.承兑汇票 = Id)")]
        public virtual string 凭证号收
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.日期 FROM 视图查询_承兑汇票_凭证收付 A WHERE A.收付标志 = 1 AND A.承兑汇票 = Id)")]
        public virtual DateTime? 凭证日期收
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.凭证号 FROM 视图查询_承兑汇票_凭证收付 A WHERE A.收付标志 = 2 AND A.承兑汇票 = Id)")]
        public virtual string 凭证号付
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.日期 FROM 视图查询_承兑汇票_凭证收付 A WHERE A.收付标志 = 2 AND A.承兑汇票 = Id)")]
        public virtual DateTime? 凭证日期付
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

        //    承兑汇票 that = (承兑汇票)other;
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
        //    sb.Append("承兑汇票: ");
        //    sb.Append("票据号码").Append('=').Append(票据号码);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
