using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Hd.Model;

namespace Hd.Model.Fp
{
    public enum 开票类别
    {
        业务款 = 1,
        代开 = 2
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "发票", ExtendsType = typeof(费用实体), Table = "财务_发票")]
    [Key(Column = "Id", ForeignKey = "FK_发票_费用实体")]
    public class 发票 : 费用实体, IDeletableEntity
    {
        #region "interface"
        public virtual bool CanBeDelete(OperateArgs e) 
        {
            return !(this.Submitted || this.是否作废);
        }
        #endregion

        [Property(NotNull = true, Length = 50, Unique = true, Index = "Idx_发票_票据号码", UniqueKey = "UK_发票_票据号码")]
        public virtual string 票据号码
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 买入时间
        {
            get;
            set;
        }

        [ManyToOne(NotNull = true, Insert = false, Update = false, ForeignKey = "FK_发票_发票账户")]
        public virtual 发票账户 发票账户
        {
            get;
            set;
        }

        [Property(Column = "发票账户", NotNull = true)]
        public virtual Guid 发票账户编号
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 日期
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 单位
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_发票_相关人")]
        public virtual 人员 相关人
        {
            get;
            set;
        }

        [Property(Column = "相关人", NotNull = false, Length = 6)]
        public virtual string 相关人编号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 开票类别? 开票类别付
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 开票类别? 开票类别收
        {
            get;
            set;
        }

        [Property(Length = 50)]
        public virtual string 对账单
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 内容//对账单 关账日期 对账单类型(出口\进口\常规\额外)
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

        [Property()]
        public virtual DateTime? 入账日期
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否作废
        {
            get;
            set;
        }

        public virtual string 大写金额
        {
            get
            {
                if (金额.HasValue)
                {
                    return Feng.Windows.Utils.ChineseHelper.ConvertToChinese(金额.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
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

        //    发票 that = (发票)other;
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
        //    sb.Append("发票: ");
        //    sb.Append("票据号码").Append('=').Append(票据号码);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
