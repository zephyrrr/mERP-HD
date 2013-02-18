using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck2
{
    [Class(Table = "业务过程_出口只票不箱_报检", OptimisticLock = OptimisticLockMode.Version)]
    public class 出口只票不箱过程报检 : SubmittedEntity, 
        IOnetoOneChildEntity<出口只票不箱, 出口只票不箱过程报检>,
        IOnetoOneParentEntity<出口只票不箱过程报检, 出口只票不箱过程商检查验>
    {
        #region "Interface"
        出口只票不箱 IOnetoOneChildEntity<出口只票不箱, 出口只票不箱过程报检>.ParentEntity
        {
            get { return 票; }
            set { 票 = value; }
        }

        出口只票不箱过程商检查验 IOnetoOneParentEntity<出口只票不箱过程报检, 出口只票不箱过程商检查验>.ChildEntity
        {
            get { return 商检查验过程; }
            set { 商检查验过程 = value; }
        }

        Type IOnetoOneParentEntity<出口只票不箱过程报检, 出口只票不箱过程商检查验>.ChildType
        {
            get { return 商检查验 ? typeof(出口只票不箱过程商检查验) : null; }
        }
        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 票信息
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程报检_出口只票不箱")]
        public virtual 出口只票不箱 票
        {
            get; set;
        }

        /// <summary>
        /// 商检查验过程
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 出口只票不箱过程商检查验 商检查验过程
        {
            get;
            set;
        }

        #region 报检
        /// <summary>
        /// 报检号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 报检号
        {
            get;
            set;
        }

        ///<summary>
        ///报检时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 报检时间
        {
            get;
            set;
        }

        /// <summary>
        /// 转单号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 转单号
        {
            get;
            set;
        }

        ///<summary>
        ///商检出证时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 商检出证时间
        {
            get;
            set;
        }

        ///<summary>
        ///报检员
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_报检员")]
        public virtual 人员 报检员
        {
            get;
            set;
        }

        ///<summary>
        ///报检员编号
        ///</summary>
        [Property(Column = "报检员", NotNull = false, Length = 6)]
        public virtual string 报检员编号
        {
            get;
            set;
        }

        /// <summary>
        /// 商检查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual bool 商检查验
        {
            get;
            set;
        }
        #endregion

    }
}
