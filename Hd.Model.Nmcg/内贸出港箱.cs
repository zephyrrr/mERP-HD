using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Nmcg
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "内贸出港箱", ExtendsType = typeof(普通箱), Table = "业务备案_内贸出港箱")]
    [Key(Column = "Id", ForeignKey = "FK_内贸出港箱_箱")]
    public class 内贸出港箱 : 普通箱, 
        IDetailEntity<内贸出港票, 内贸出港箱> 
        //IOnetoOneParentEntity<内贸出港箱, 内贸出港箱过程承运>
    {
        #region "Interface"
        内贸出港票 IDetailEntity<内贸出港票, 内贸出港箱>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }

        //内贸出港箱过程承运 IOnetoOneParentEntity<内贸出港箱, 内贸出港箱过程承运>.ChildEntity
        //{
        //    get { return 承运过程; }
        //    set { 承运过程 = value; }
        //}

        //Type IOnetoOneParentEntity<内贸出港箱, 内贸出港箱过程承运>.ChildType
        //{
        //    get { return typeof(内贸出港箱过程承运); }
        //}
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_内贸出港箱_内贸出港票", Cascade = "none", Lazy = Laziness.False, OuterJoin = OuterJoinStrategy.True)]
        public virtual 内贸出港票 票
        {
            get;
            set;
        }

        ///// <summary>
        ///// 过程信息
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        ////[ManyToOne(Column = "Id", NotNull = false, Insert = false, Update = false)]
        //public virtual 内贸出港箱过程承运 承运过程
        //{
        //    get;
        //    set;
        //}

        #region "业务信息"
        ///<summary>
        ///最终目的地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港箱_最终目的地")]
        public virtual 人员 最终目的地
        {
            get;
            set;
        }

        ///<summary>
        ///最终目的地
        ///</summary>
        [Property(Column = "最终目的地", Length = 6)]
        public virtual string 最终目的地编号
        {
            get;
            set;
        }

        ///<summary>
        ///件数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 件数
        {
            get;
            set;
        }

        #endregion

        #region 承运
        [Property(Length = 12, Index = "Idx_内贸出港箱_回货箱号")]
        public virtual string 回货箱号
        {
            get;
            set;
        }

        [Property(Length = 12)]
        public virtual string 车号
        {
            get;
            set;
        }

        [Property(Length = 20)]
        public virtual string 装货地
        {
            get;
            set;
        }

        //[ManyToOne(Insert = false, Update = false, ForeignKey = "FK_内贸出港箱_装货地")]
        //public virtual 客户 装货地
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////装货地
        /////</summary>
        //[Property(Column = "装货地", Length = 6)]
        //public virtual string 装货地编号
        //{
        //    get;
        //    set;
        //}

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
        ///装货时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 装货时间
        {
            get;
            set;
        }

        ///<summary>
        ///提箱时间      对港的
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 提箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///还箱时间      对港的
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 还箱时间
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
        #endregion

        #region 破损记录
        ///<summary>
        ///破损记录
        ///</summary>
        [Property(Length = 200)]
        public virtual string 破损记录
        {
            get;
            set;
        }

        ///<summary>
        ///破损责任人
        ///</summary>
        [Property(Length = 20)]
        public virtual string 破损责任人
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 内贸海运费
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

        //    内贸出港箱 that = (内贸出港箱)other;
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
        //    sb.Append("内贸出港箱: ");
        //    sb.Append("箱号").Append('=').Append(箱号);
        //    return sb.ToString();
        //}
        //#endregion
    }
}
