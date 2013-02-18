using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck
{
    public enum 拼箱标志
    {
        整箱 = 1, 拼箱 = 2
    }

    public enum 委托类别
    {
        P = 1, T = 2
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "出口票", ExtendsType = typeof(普通票), Table = "业务备案_出口票")]
    [Key(Column = "Id", ForeignKey = "FK_出口票_票")]
    public class 出口票 : 普通票, IMasterEntity<出口票, 出口箱>
    //IOnetoOneParentGenerateChildEntity<出口票, 出口票过程报关>,
    //IOnetoOneParentGenerateChildEntity<出口票, 出口票过程报检>,
    //IOnetoOneParentGenerateChildEntity<出口票, 出口票过程出保函>
    {
        #region "Interface"
        //出口票过程报关 IOnetoOneParentEntity<出口票, 出口票过程报关>.ChildEntity
        //{
        //    get { return 报关过程; }
        //    set { 报关过程 = value; }
        //}

        //Type IOnetoOneParentGenerateChildEntity<出口票, 出口票过程报关>.ChildType
        //{
        //    get { return typeof(出口票过程报关); }
        //}

        //出口票过程报检 IOnetoOneParentEntity<出口票, 出口票过程报检>.ChildEntity
        //{
        //    get { return 报检过程; }
        //    set { 报检过程 = value; }
        //}

        //Type IOnetoOneParentGenerateChildEntity<出口票, 出口票过程报检>.ChildType
        //{
        //    get { return typeof(出口票过程报检); }
        //}

        //出口票过程出保函 IOnetoOneParentEntity<出口票, 出口票过程出保函>.ChildEntity
        //{
        //    get { return 出保函过程; }
        //    set { 出保函过程 = value; }
        //}

        //Type IOnetoOneParentGenerateChildEntity<出口票, 出口票过程出保函>.ChildType
        //{
        //    get { return 出保函标志 ? typeof(出口票过程出保函) : null; }
        //}

        IList<出口箱> IMasterEntity<出口票, 出口箱>.DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }
        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(出口箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<出口箱> 箱
        {
            get;
            set;
        }

        ///// <summary>
        ///// 出保函过程
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        //public virtual 出口票过程出保函 出保函过程
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 报检过程
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        //public virtual 出口票过程报检 报检过程
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 报关过程
        ///// </summary>
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        //public virtual 出口票过程报关 报关过程
        //{
        //    get;
        //    set;
        //}


        /// <summary>
        /// 承运标志
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 承运标志
        {
            get;
            set;
        }


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
        /////<summary>
        /////总金额
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual decimal? 总金额
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////币制
        /////</summary>
        //[ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口票_币制")]
        //public virtual 币制 币制
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////币制
        /////</summary>
        //[Property(Column = "币制", Length = 3)]
        //public virtual string 币制编号
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////提箱期限
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual DateTime? 提箱期限
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////转关标志
        /////</summary>
        //[Property(NotNull = true)]
        //public virtual 转关标志 转关标志
        //{
        //    get;
        //    set;
        //}

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
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口票_进港地")]
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

        /////<summary>
        /////进港期限
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual DateTime? 进港期限
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////核销单抬头
        /////</summary>
        //[Property(Length = 100)]
        //public virtual string 核销单抬头
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////核销单号
        /////</summary>
        //[Property(Length = 100)]
        //public virtual string 核销单号
        //{
        //    get;
        //    set;
        //}

        //[Property(NotNull = false)]
        //public virtual bool 转船标志
        //{
        //    get;
        //    set;
        //}

        //[Property(NotNull = false)]
        //public virtual bool 加载标志
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 出保函标志
        ///// </summary>
        //[Property(NotNull = true)]
        //public virtual bool 出保函标志
        //{
        //    get;
        //    set;
        //}

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口票_承运人")]
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
        #endregion

        #region 报关

        [Property(NotNull = false, Length = 50)]
        public virtual string 抬头
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 退税海关
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 退税客户
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 10)]
        public virtual string 核销单号
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 通关单号
        {
            get;
            set;
        }

        ///<summary>
        ///箱号   箱号汇总并排重
        ///</summary>
        [Property(Insert = false, Update = false, Formula = "(SELECT dbo.Concatenate(DISTINCT b.箱号) FROM dbo.业务备案_出口箱 AS a INNER JOIN dbo.视图信息_箱票 AS b ON a.ID = b.ID WHERE b.票 = Id) ")]
        public virtual string 箱号
        {
            get;
            set;
        }

        //[Property(NotNull = false, Length = 4001)]
        //public virtual string 报关单快照
        //{
        //    get;
        //    set;
        //}

        [Property(NotNull = false, Length = 10, Column = "报关员")]
        public virtual string 报关员编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 报关公司
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 海关查验时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 商检查验时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 放行时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 拼箱标志? 拼箱标志
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 委托类别? 委托类别
        {
            get;
            set;
        }       
        #endregion
    }
}
