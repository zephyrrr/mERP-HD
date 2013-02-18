using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口箱", ExtendsType = typeof(普通箱), Table = "业务备案_进口箱")]
    [Key(Column = "Id", ForeignKey = "FK_进口箱_箱")]
    public class 进口箱 : 普通箱, 
        IDetailEntity<进口票, 进口箱> 
        //IDetailGenerateDetailEntity<进口票, 进口箱>,
        //IOnetoOneParentEntity<进口箱, 进口箱过程承运>
        //IOnetoOneParentEntity<进口箱, 进口箱过程滞箱费减免>

        //IMasterGenerateDetailEntity<费用实体, 费用>,
        //IMasterGenerateDetailEntity<费用实体, 费用信息>
    {
        #region "Interface"
        进口票 IDetailEntity<进口票, 进口箱>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }

        //进口箱过程承运 IOnetoOneParentEntity<进口箱, 进口箱过程承运>.ChildEntity
        //{
        //    get { return 承运过程; }
        //    set { 承运过程 = value; }
        //}

        //Type IOnetoOneParentEntity<进口箱, 进口箱过程承运>.ChildType
        //{
        //    get { return (票 as 进口票).承运标志 ? typeof(进口箱过程承运) : null; }
        //}

        //进口箱过程滞箱费减免 IOnetoOneParentEntity<进口箱, 进口箱过程滞箱费减免>.ChildEntity
        //{
        //    get { return 滞箱费减免过程; }
        //    set { 滞箱费减免过程 = value; }
        //}

        //Type IOnetoOneParentEntity<进口箱, 进口箱过程滞箱费减免>.ChildType
        //{
        //    get { return (票 as 进口票).滞箱费减免标志 ? typeof(进口箱过程滞箱费减免) : null; }
        //}

        //进口票 IDetailEntity<进口票, 进口箱>.MasterEntity
        //{
        //    get { return 票; }
        //    set { 票 = value; }
        //}

        //bool IDetailGenerateDetailEntity<进口票, 进口箱>.CopyIfMatch(进口箱 other)
        //{
        //    if (string.IsNullOrEmpty(other.箱号)
        //        || this.箱号 == other.箱号)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //IList<费用> IMasterGenerateDetailEntity<费用实体, 费用>.GenerateDetails()
        //{
        //    IList<费用项> feeItems = (new 费用项Bll()).Get费用项(this, this.费用实体类型);
        //    IList<费用> ret = new List<费用>();
        //    foreach (费用项 i in feeItems)
        //    {
        //        常规业务费用 fee1 = null, fee2 = null;
        //        if (i.收)
        //        {
        //            fee1 = new 常规业务费用();
        //            fee1.费用实体 = this;
        //            fee1.费用项编号 = i.编号;
        //            fee1.收付标志 = 收付标志.收;
        //            fee1.相关人编号 = this.票.委托人编号;
        //            fee1.理论值 = (new 费用理论值Bll()).Calculate理论值(this, i.编号);
        //            ret.Add(fee1);
        //        }
        //        if (i.付)
        //        {
        //            fee2 = new 常规业务费用();
        //            fee2.费用实体 = this;
        //            fee2.费用项编号 = i.编号;
        //            fee2.收付标志 = 收付标志.付;
        //            //fee.相关人编号 = this.委托人编号;
        //            fee2.理论值 = (new 费用理论值Bll()).Calculate理论值(this, i.编号);
        //            ret.Add(fee2);
        //        }

        //        if (i.收 && i.付)
        //        {
        //            fee1.关联费用 = fee2;
        //            fee2.关联费用 = fee1;
        //        }
        //    }
        //    return ret;
        //}

        //IList<额外费用信息> IMasterGenerateDetailEntity<费用实体, 额外费用信息>.GenerateDetails()
        //{
        //    IList<费用项> feeItems = (new 费用项Bll()).Get额外费用项(this, 费用实体类型.进口箱);
        //    IList<额外费用信息> ret = new List<额外费用信息>();
        //    foreach (费用项 i in feeItems)
        //    {
        //        额外费用信息 info = new 额外费用信息();
        //        info.费用实体 = this;
        //        info.费用项编号 = i.编号;
        //        ret.Add(info);
        //    }
        //    return ret;
        //}
        #endregion

        ///// <summary>
        ///// 是否已提交
        ///// </summary>
        //public virtual bool Submitted
        //{
        //    get { return this.票.Submitted; }
        //    set { }
        //}

        [ManyToOne(NotNull = true, ForeignKey = "FK_进口箱_进口票", Column = "票", Cascade = "none")]
        public virtual 进口票 票
        {
            get;
            set;
        }

        ///// <summary>
        ///// 过程信息
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        ////[ManyToOne(Column = "Id", NotNull = false, Insert = false, Update = false)]
        //public virtual 进口箱过程承运 承运过程
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 过程信息
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        ////[ManyToOne(Column = "Id", NotNull = false, Insert = false, Update = false)]
        //public virtual 进口箱过程滞箱费减免 滞箱费减免过程
        //{
        //    get;
        //    set;
        //}

        #region 基本信息备案
     


        #endregion

        #region 海关查验
        /// <summary>
        /// 海关查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual 查验标志? 海关查验
        {
            get;
            set;
        }
        #endregion

        #region 商检查验
        /// <summary>
        /// 商检查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual 查验标志? 商检查验
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool? 商检查验标志
        {
            get;
            set;
        }
        #endregion

        #region 承运
        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_进口箱_提箱地")]
        public virtual 人员 提箱地
        {
            get;
            set;
        }

        ///<summary>
        ///提箱地
        ///</summary>
        [Property(Column = "提箱地", Length = 6)]
        public virtual string 提箱地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口箱_卸货地")]
        public virtual 人员 卸货地
        {
            get;
            set;
        }

        ///<summary>
        ///卸货地
        ///</summary>
        [Property(Column = "卸货地", Length = 6)]
        public virtual string 卸货地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口箱_还箱地")]
        public virtual 人员 还箱地
        {
            get;
            set;
        }

        ///<summary>
        ///还箱地
        ///</summary>
        [Property(Column = "还箱地", Length = 6)]
        public virtual string 还箱地编号
        {
            get;
            set;
        }

        ///<summary>
        ///货代提箱时间要求止
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 货代提箱时间要求止
        {
            get;
            set;
        }

        ///<summary>
        ///货代还箱时间要求止
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 货代还箱时间要求止
        {
            get;
            set;
        }

        ///<summary>
        ///提箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 提箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///拉箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 拉箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///验封时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 验封时间
        {
            get;
            set;
        }

        ///<summary>
        ///还箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 还箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///卸货时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 卸货时间
        {
            get;
            set;
        }
        #endregion

        #region 滞箱费
        ///<summary>
        ///最终免箱天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 最终免箱天数
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 初始滞箱费
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        // OneToOne with Constrained = true Not Working!!!
        // ManyToOne: Set NotNull = true 才能LazyLoad。 对方Class必须要Lazy = true
        [ManyToOne(NotNull = true, Insert = false, Update = false, Column = "Id")]
        public virtual 进口箱滞箱费减免 进口箱滞箱费减免
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

        //    进口箱 that = (进口箱)other;
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
        //    sb.Append("进口箱: ");
        //    sb.Append("箱号").Append('=').Append(箱号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
