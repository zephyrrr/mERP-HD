using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Kj
{
    public enum 固定资产分类
    {
        运输车辆资产 = 1,
        房屋资产 = 2,
        办公设备资产 = 3,
        生产设备资产 = 4,
        其他车辆资产 = 5,
        其它设备资产 = 9
    }
    public enum 固定资产状态
    {
        正常 = 1,
        报废 = 8,
        卖出 = 9
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "固定资产", ExtendsType = typeof(费用实体), Table = "财务_固定资产")]
    [Key(Column = "Id", ForeignKey = "FK_固定资产_费用实体")]
    public class 固定资产 : 费用实体
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

        [Property(NotNull = true)]
        public virtual 固定资产分类 分类
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

        [Property(NotNull = false)]
        public virtual int 使用年限
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

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 月折旧额
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

        [ManyToOne(NotNull = false, Insert = false, Update = false)]
        public virtual 人员 买方
        {
            get;
            set;
        }

        [Property(Column = "买方", Length = 6, NotNull = false)]
        public virtual string 买方编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 剩余折旧
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 其他未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 其他已确认
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 上次折旧日期
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 100)]
        public virtual string 警示状态
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

        //    固定资产 that = (固定资产)other;
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
        //    sb.Append("固定资产: ");
        //    sb.Append("编号").Append('=').Append(编号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
