using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck2
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(ExtendsType = typeof(费用实体), Table = "业务备案_出口只票不箱")]
    [Key(Column = "Id", ForeignKey = "FK_出口只票不箱_费用实体")]
    public class 出口只票不箱 : 费用实体,
        IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报关>, 
        IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报检>, 
        IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程出保函>
    {
        #region "Interface"
        出口只票不箱过程报关 IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报关>.ChildEntity
        {
            get { return 报关过程; }
            set { 报关过程 = value; }
        }

        Type IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报关>.ChildType
        {
            get { return 报关标志 ? typeof(出口只票不箱过程报关) : null; }
        }

        出口只票不箱过程报检 IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报检>.ChildEntity
        {
            get { return 报检过程; }
            set { 报检过程 = value; }
        }

        Type IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程报检>.ChildType
        {
            get { return 报检标志 ? typeof(出口只票不箱过程报检) : null; }
        }

        出口只票不箱过程出保函 IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程出保函>.ChildEntity
        {
            get { return 出保函过程; }
            set { 出保函过程 = value; }
        }

        Type IOnetoOneParentEntity<出口只票不箱, 出口只票不箱过程出保函>.ChildType
        {
            get { return 出保函标志 ? typeof(出口只票不箱过程出保函) : null; }
        }
        #endregion

        /// <summary>
        /// 出保函过程
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 出口只票不箱过程出保函 出保函过程
        {
            get;
            set;
        }

        /// <summary>
        /// 报检过程
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 出口只票不箱过程报检 报检过程
        {
            get;
            set;
        }

        /// <summary>
        /// 报关过程
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 出口只票不箱过程报关 报关过程
        {
            get;
            set;
        }

        #region 基本信息备案
        /// <summary>
        /// 委托时间
        /// </summary>
        [Property(NotNull = false)]
        public virtual DateTime? 委托时间
        {
            get;
            set;
        }

        ///<summary>
        ///委托人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_委托人")]
        public virtual 客户 委托人
        {
            get;
            set;
        }

        ///<summary>
        ///委托人编号
        ///</summary>
        [Property(Column = "委托人", NotNull = true, Length = 6)]
        public virtual string 委托人编号
        {
            get;
            set;
        }


        /// <summary>
        /// 报关单号
        /// </summary>
        [Property(Length = 50, NotNull = true, Unique = true, UniqueKey = "UK_出口只票不箱_报关单号", Index = "Idx_出口只票不箱_报关单号")]
        public virtual string 报关单号
        {
            get;
            set;
        }

        ///<summary>
        ///提单号
        ///</summary>
        [Property(Length = 50, NotNull = true, Unique = true, UniqueKey = "UK_出口只票不箱_提单号")]
        public virtual string 提单号
        {
            get;
            set;
        }

        ///<summary>
        ///船名航次
        ///</summary>
        [Property(Length = 50)]
        public virtual string 船名航次
        {
            get;
            set;
        }

        ///<summary>
        ///箱量
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 箱量
        {
            get;
            set;
        }

        ///<summary>
        ///标箱量
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 标箱量
        {
            get;
            set;
        }

        ///<summary>
        ///船公司
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_船公司")]
        public virtual 客户 船公司
        {
            get;
            set;
        }

        ///<summary>
        ///船公司
        ///</summary>
        [Property(Column = "船公司", NotNull = false, Length = 6)]
        public virtual string 船公司编号
        {
            get;
            set;
        }

        ///<summary>
        ///总重量
        ///</summary>
        [Property(NotNull = false)]
        public virtual int 总重量
        {
            get;
            set;
        }

        ///<summary>
        ///总金额
        ///</summary>
        [Property(NotNull = false)]
        public virtual decimal? 总金额
        {
            get;
            set;
        }

        ///<summary>
        ///币制
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_币制")]
        public virtual 币制 币制
        {
            get;
            set;
        }

        ///<summary>
        ///币制
        ///</summary>
        [Property(Column = "币制", Length = 3)]
        public virtual string 币制编号
        {
            get;
            set;
        }

        ///<summary>
        ///品名
        ///</summary>
        [Property(Length = 500)]
        public virtual string 品名
        {
            get;
            set;
        }

        ///<summary>
        ///提箱期限
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 提箱期限
        {
            get;
            set;
        }

        ///<summary>
        ///转关标志
        ///</summary>
        [Property(NotNull = true)]
        public virtual 转关标志 转关标志
        {
            get;
            set;
        }

        ///<summary>
        ///离港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 离港时间
        {
            get;
            set;
        }

        ///<summary>
        ///进港地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_进港地")]
        public virtual 客户 进港地
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
        ///进港期限
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 进港期限
        {
            get;
            set;
        }

        ///<summary>
        ///核销单抬头
        ///</summary>
        [Property(Length = 100)]
        public virtual string 核销单抬头
        {
            get;
            set;
        }

        ///<summary>
        ///核销单号
        ///</summary>
        [Property(Length = 100)]
        public virtual string 核销单号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool 转船标志
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool 加载标志
        {
            get;
            set;
        }

        /// <summary>
        /// 出保函标志
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 出保函标志
        {
            get;
            set;
        }

        ///<summary>
        ///箱号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 箱号
        {
            get;
            set;
        }

        ///<summary>
        ///封志号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 封志号
        {
            get;
            set;
        }

        ///<summary>
        ///内部备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 内部备注
        {
            get;
            set;
        }

        ///<summary>
        ///对上备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 对上备注
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 报检标志
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 报检标志
        {
            get;
            set;
        }

        /// <summary>
        /// 报关标志
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 报关标志
        {
            get;
            set;
        }

        #region Overriden Methods

        /// <summary>
        /// Returns <code>true</code> if the argument is a Board instance and all identifiers for this entity
        /// equal the identifiers of the argument entity. Returns <code>false</code> otherwise.
        /// </summary>
        public override bool Equals(Object other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            出口只票不箱 that = (出口只票不箱)other;
            if (that == null)
            {
                return false;
            }
            if (this.Id == null || that.Id == null || !this.Id.Equals(that.Id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code based on this entity's identifiers.
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 14;
            hashCode = 29 * hashCode + (Id == null ? 0 : Id.GetHashCode());
            return hashCode;
        }

        /// <summary>
        /// Returns a string representation of this VersionedEntity
        /// </summary>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("出口只票不箱: ");
            sb.Append("提单号").Append('=').Append(提单号);
            return sb.ToString();
        }

        #endregion

    }
}
