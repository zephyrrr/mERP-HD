using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Utils;

namespace Hd.Model.Nmcg
{
    public enum 条款
    {
        CYCY = 1,
        CYDR = 2
    }
    public enum 破损责任人
    {
        出厂 = 1,
        驾驶员 = 2,
        仓库 = 3,
        对港收货 = 4
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "内贸出港票", ExtendsType = typeof(普通票), Table = "业务备案_内贸出港票")]
    [Key(Column = "Id", ForeignKey = "FK_内贸出港票_票")]
    public class 内贸出港票 : 普通票,
        IMasterEntity<内贸出港票, 内贸出港箱>
    {
        #region "Interface"
        IList<内贸出港箱> IMasterEntity<内贸出港票, 内贸出港箱>.DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }

        //protected override IList<费用> Generate费用Details()
        //{
        //    IList<费用项> feeItems = Utility.Get费用项(费用实体类型.进口票);
        //    IList<费用> ret = new List<费用>();
        //    foreach (费用项 i in feeItems)
        //    {
        //        业务费用 fee1 = null, fee2 = null;
        //        if (i.收)
        //        {
        //            fee1 = new 业务费用();
        //            fee1.费用实体 = this;
        //            fee1.费用项编号 = i.编号;
        //            fee1.收付标志 = 收付标志.收;
        //            fee1.相关人编号 = this.委托人编号;
        //            //fee1.理论值 = (new 费用理论值Bll()).Calculate理论值(this, i.编号);
        //            ret.Add(fee1);
        //        }
        //        if (i.付)
        //        {
        //            fee2 = new 业务费用();
        //            fee2.费用实体 = this;
        //            fee2.费用项编号 = i.编号;
        //            fee2.收付标志 = 收付标志.付;
        //            //fee.相关人编号 = this.委托人编号;
        //            //fee2.理论值 = (new 费用理论值Bll()).Calculate理论值(this, i.编号);
        //            ret.Add(fee2);
        //        }
        //    }
        //    return ret;
        //}
        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(内贸出港箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<内贸出港箱> 箱
        {
            get;
            set;
        }

        ///// <summary>
        ///// 票最终的还箱时间
        ///// </summary>
        //public virtual DateTime? 票最终还箱时间
        //{
        //    get
        //    {
        //        RepositoryHelper.Initialize(箱, this);
        //        if (箱.Count == 0)
        //            return null;

        //        DateTime dt = DateTime.MinValue;
        //        foreach (内贸出港箱 i in 箱)
        //        {
        //            if (!i.还箱时间.HasValue)
        //                return null;
        //            dt = DateTimeHelper.MaxDateTime(dt, i.还箱时间.Value);
        //        }
        //        return dt;
        //    }
        //}

        ///// <summary>
        ///// 承运标志
        ///// </summary>
        //[Property(NotNull = true)]
        //public virtual bool 承运标志
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 对港承运标志
        ///// </summary>
        //[Property(NotNull = true)]
        //public virtual bool 对港承运标志
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 退舱标志
        ///// </summary>
        //[Property(NotNull = true)]
        //public virtual bool 退舱标志
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///操作完全标志
        ///</summary>
        [Property(NotNull = true)]
        public virtual bool 操作完全标志
        {
            get;
            set;
        }

        #region 基本信息备案
        ///<summary>
        ///预配提单号
        ///</summary>
        [Property(Length = 30)]
        public virtual string 预配提单号
        {
            get;
            set;
        }

        ///<summary>
        ///预配船名航次
        ///</summary>
        [Property(Length = 30)]
        public virtual string 预配船名航次
        {
            get;
            set;
        }

        ///<summary>
        ///条款
        ///</summary>
        [Property(NotNull = false)]
        public virtual 条款? 条款
        {
            get;
            set;
        }

        ///<summary>
        ///目的港
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港票_目的港")]
        public virtual 人员 目的港
        {
            get;
            set;
        }

        ///<summary>
        ///目的港
        ///</summary>
        [Property(Column = "目的港", Length = 6)]
        public virtual string 目的港编号
        {
            get;
            set;
        }

        ///<summary>
        ///预计开航日期
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 预计开航日期
        {
            get;
            set;
        }

        ///<summary>
        ///开航日期
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 开航日期
        {
            get;
            set;
        }

        ///<summary>
        ///进港地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港票_进港地")]
        public virtual 人员 进港地
        {
            get;
            set;
        }

        ///<summary>
        ///进港地
        ///</summary>
        [Property(Column = "进港地", Length = 6)]
        public virtual string 进港地编号
        {
            get;
            set;
        }

        ///<summary>
        ///预计到港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 预计到港时间
        {
            get;
            set;
        }

        ///<summary>
        ///到港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 到港时间
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港票_倒箱仓库")]
        public virtual 人员 倒箱仓库
        {
            get;
            set;
        }

        ///<summary>
        ///倒箱仓库
        ///</summary>
        [Property(Column = "倒箱仓库", Length = 6)]
        public virtual string 倒箱仓库编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港票_承运人")]
        public virtual 人员 承运人
        {
            get;
            set;
        }

        ///<summary>
        ///承运人
        ///</summary>
        [Property(Column = "承运人", Length = 6)]
        public virtual string 承运人编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港箱_对港承运人")]
        public virtual 人员 对港承运人
        {
            get;
            set;
        }

        ///<summary>
        ///承运人
        ///</summary>
        [Property(Column = "对港承运人", Length = 6)]
        public virtual string 对港承运人编号
        {
            get;
            set;
        }

        ///<summary>
        ///免箱天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 免箱天数
        {
            get;
            set;
        }
        #endregion

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

        //    内贸出港票 that = (内贸出港票)other;
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
        //    sb.Append("内贸出港票: ");
        //    sb.Append("货代自编号").Append('=').Append(货代自编号);
        //    return sb.ToString();
        //}

        //#endregion
    }
}
