using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck2
{
    [Class(Table = "业务过程_出口只票不箱_报关", OptimisticLock = OptimisticLockMode.Version)]
    public class 出口只票不箱过程报关 : SubmittedEntity,
        IOnetoOneChildEntity<出口只票不箱, 出口只票不箱过程报关>,
        IOnetoOneParentEntity<出口只票不箱过程报关, 出口只票不箱过程海关查验>
    {
        #region "Interface"
        出口只票不箱 IOnetoOneChildEntity<出口只票不箱, 出口只票不箱过程报关>.ParentEntity
        {
            get { return 票; }
            set { 票 = value; }
        }

        出口只票不箱过程海关查验 IOnetoOneParentEntity<出口只票不箱过程报关, 出口只票不箱过程海关查验>.ChildEntity
        {
            get { return 海关查验过程; }
            set { 海关查验过程 = value; }
        }

        Type IOnetoOneParentEntity<出口只票不箱过程报关, 出口只票不箱过程海关查验>.ChildType
        {
            get { return 海关查验 ? typeof(出口只票不箱过程海关查验) : null; }
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
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程报关_出口只票不箱")]
        public virtual 出口只票不箱 票
        {
            get; set;
        }

        /// <summary>
        /// 海关查验过程
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 出口只票不箱过程海关查验 海关查验过程
        {
            get;
            set;
        }

        #region 报关
        /// <summary>
        /// 转关联系单号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 转关联系单号
        {
            get;
            set;
        }

        /// <summary>
        /// 报关单长号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 报关单长号
        {
            get;
            set;
        }

        ///<summary>
        ///报关员
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_报关员")]
        public virtual 人员 报关员
        {
            get;
            set;
        }

        ///<summary>
        ///报关员编号
        ///</summary>
        [Property(Column = "报关员", NotNull = false, Length = 6)]
        public virtual string 报关员编号
        {
            get;
            set;
        }

        ///<summary>
        ///申报时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 申报时间
        {
            get;
            set;
        }

        ///<summary>
        ///单证放行时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 单证放行时间
        {
            get;
            set;
        }

        ///<summary>
        ///港区放行时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 港区放行时间
        {
            get;
            set;
        }

        ///<summary>
        ///港区放行地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_港区放行地")]
        public virtual 客户 港区放行地
        {
            get;
            set;
        }

        ///<summary>
        ///港区放行地
        ///</summary>
        [Property(Column = "港区放行地", Length = 6)]
        public virtual string 港区放行地编号
        {
            get;
            set;
        }

        ///<summary>
        ///证明联领取时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 证明联领取时间
        {
            get;
            set;
        }

        ///<summary>
        ///退税时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 退税时间
        {
            get;
            set;
        }

        ///<summary>
        ///退税标志
        ///</summary>
        [Property(NotNull = false)]
        public virtual 退税标志? 退税标志
        {
            get;
            set;
        }

        /// <summary>
        /// 退税情况
        /// </summary>
        [Property(Length = 50)]
        public virtual string 退税情况
        {
            get;
            set;
        }

        ///<summary>
        ///异常标志
        ///</summary>
        [Property(NotNull = false)]
        public virtual 异常标志? 异常标志
        {
            get;
            set;
        }

        ///<summary>
        ///删改单标志
        ///</summary>
        [Property(NotNull = false)]
        public virtual 删改单标志? 删改单标志
        {
            get;
            set;
        }

        /// <summary>
        /// 旧报关单号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 旧报关单号
        {
            get;
            set;
        }

        ///<summary>
        ///责任人标志
        ///</summary>
        [Property(NotNull = false)]
        public virtual 责任人标志? 责任人标志
        {
            get;
            set;
        }

        /// <summary>
        /// 海关查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual bool 海关查验
        {
            get;
            set;
        }
        #endregion

    }
}
