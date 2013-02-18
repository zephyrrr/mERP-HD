using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Px
{
    // 类似于出口一样
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "拼箱票", ExtendsType = typeof(普通票), Table = "业务备案_拼箱票")]
    [Key(Column = "Id", ForeignKey = "FK_拼箱票_票")]
    public class 拼箱票 : 普通票, IMasterEntity<拼箱票, 拼箱箱>
    {
        #region IMasterEntity<拼箱票,拼箱箱> 成员

        public IList<拼箱箱> DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }

        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(拼箱箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<拼箱箱> 箱
        {
            get;
            set;
        }

        /// <summary>
        /// 承运标志
        /// </summary>
        [Property(NotNull = false)]
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
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_拼箱票_进港地")]
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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_拼箱票_承运人")]
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

        #region 拼箱

        //[ManyToOne(Insert = false, Update = false, ForeignKey = "FK_拼箱票_拼箱委托人")]
        //public virtual 人员 拼箱委托人
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///拼箱委托人
        ///</summary>
        [Property(Column = "拼箱委托人", Length = 50)]
        public virtual string 拼箱委托人编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 抬头
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 退税时间
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 拼箱提单号
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
        [Property(Insert = false, Update = false, Formula = "(SELECT dbo.Concatenate(DISTINCT b.箱号) FROM dbo.业务备案_拼箱箱 AS a INNER JOIN dbo.视图信息_箱票 AS b ON a.ID = b.ID WHERE b.票 = Id) ")]
        public virtual string 箱号
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 4001)]
        public virtual string 报关单快照
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 10, Column = "报关员")]
        public virtual string 报关员
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

        #endregion
    }
}
