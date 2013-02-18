using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 转关标志
    {
        清关 = 1,
        转关 = 2,
    }

    public enum 申报类型//进口转关的 卸箱地 大榭招商的 需要填写
    {
        预申报 = 1,
        未申报 = 2,
    }

    public enum 货物标志
    {
        整箱 = 1,
        拼箱 = 2,
        散装 = 3
    }

    public enum 合格标志
    {
        合格 = 1,
        不合格 = 2
    }
    public enum 海关查验结果
    {
        正常 = 1,
        归类不符 = 2,
        货名不符 = 3,
        侵权 = 4
    }
    public enum 海关处理结果
    {
        放行 = 1,
        移交缉私 = 2,
        移交法规 = 3
    }
    public enum 退税标志
    {
        T = 1,//表示退给客户了
        W = 2,//表示未退
        H = 3,//有异常情况，去核仓单
        D = 4//表示异常情况已经解决
    }
    public enum 异常标志
    {
        无法结关 = 1,
        结关后改单 = 2,
        正常 = 3
    }
    public enum 处罚结果
    {
        罚款 = 1,
        没收 = 2
    }
    public enum 违规性质
    {
        逃通关单 = 1,
        逃许可证 = 2,
        侵权 = 3,
        走私 = 4
    }
    public enum 删改单标志
    {
        删单 = 1,
        改单 = 2,
        不需要 = 3
    }

    public enum 责任人标志
    {
        报关组 = 1,
        委托人 = 2
    }

    public enum 进口业务类型//进口合同的最基本模版根据这个类型；用于报表分类型统计
    {
        包干 = 11,
        YYH型 = 12,
        零星 = 13
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(NameType = typeof(普通票), Table = "业务备案_普通票", ExtendsType = typeof(费用实体))]
    [Key(Column = "Id", ForeignKey = "FK_普通票_费用实体")]
    public class 普通票 : 费用实体,
        IMasterEntity<普通票, 费用信息>,
        IMasterGenerateDetailEntity<普通票, 费用信息>,
        IMasterEntity<普通票, 业务费用>
    {
    	#region "Interface"
        IList<费用信息> IMasterEntity<普通票, 费用信息>.DetailEntities
        {
            get { return 费用信息; }
            set { 费用信息 = value; }
        }
        IList<业务费用> IMasterEntity<普通票, 业务费用>.DetailEntities
        {
            get { return 业务费用; }
            set { 业务费用 = value; }
        }
        //IList<费用> IMasterGenerateDetailEntity<费用实体, 费用>.GenerateDetails()
        //{
        //    IList<费用> list = Utility.Get费用By合同(this, this.合同);
        //    foreach (费用 i in list)
        //    {
        //        if (i.收付标志 == 收付标志.收)
        //        {
        //            i.相关人编号 = this.委托人编号;
        //        }
        //    }
        //    return list;
        //}
        IList<费用信息> IMasterGenerateDetailEntity<普通票, 费用信息>.GenerateDetails()
        {
            return Generate费用信息Details();
        }

        protected virtual IList<费用信息> Generate费用信息Details()
        {
            return new List<费用信息>();
        }
    	#endregion
 
        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(费用信息), NotFound = NotFoundMode.Ignore)]
        public virtual IList<费用信息> 费用信息
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "费用实体")]
        [OneToMany(2, ClassType = typeof(业务费用), NotFound = NotFoundMode.Ignore)]
        public virtual IList<业务费用> 业务费用
        {
            get;
            set;
        }

        ///<summary>
        ///品名   箱的品名汇总并排重
        ///</summary>
        [Property(Insert = false, Update = false, Formula = "(SELECT dbo.Concatenate(DISTINCT a.品名) FROM dbo.业务备案_普通箱 AS a INNER JOIN dbo.视图信息_箱票 AS b ON a.ID = b.ID WHERE b.票 = Id) ")]
        public virtual string 品名
        {
            get;
            set;
        }

        ///<summary>
        ///代表性箱号   箱的箱号取前2个
        ///</summary>
        //[Property(Insert = false, Update = false, Formula = "(SELECT a.代表性箱号 FROM dbo.视图信息_代表性箱号 AS a WHERE a.票 = Id) ")]
        [Property(Length = 50)]
        public virtual string 代表性箱号
        {
            get;
            set;
        }

        ///<summary>
        ///货代自编号
        ///</summary>
        [Property(Length = 30, /*NotNull = false, Unique = true, UniqueKey = "UK_普通票_货代自编号", */Index = "Idx_普通票_货代自编号")]
        public virtual string 货代自编号
        {
            get;
            set;
        }

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
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_普通票_委托人")]
        public virtual 人员 委托人
        {
            get;
            set;
        }

        ///<summary>
        ///委托人编号
        ///</summary>
        [Property(Column = "委托人", NotNull = false, Length = 6)]
        public virtual string 委托人编号
        {
            get;
            set;
        }

        ///<summary>
        ///提单号
        ///</summary>
        [Property(Length = 50, Index = "Idx_普通票_提单号")]
        public virtual string 提单号
        {
            get;
            set;
        }

        //[ManyToOne(Insert = false, Update = false, ForeignKey = "FK_普通票_合同")]
        //public virtual 合同 合同
        //{
        //    get;
        //    set;
        //}
        
        /////<summary>
        /////合同号
        /////</summary>
        //[Property(NotNull = true)]
        //public virtual Guid 合同编号
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 合同号
        /// </summary>
        [Property(Length = 50, Index = "Idx_普通票_合同号")]
        public virtual string 合同号
        {
            get;
            set;
        }

        /// <summary>
        /// 报检号
        /// </summary>
        [Property(Length = 50, Index = "Idx_普通票_报检号")]
        public virtual string 报检号
        {
            get;
            set;
        }

        /// <summary>
        /// 报关单号
        /// </summary>
        [Property(Length = 50, Index = "Idx_普通票_报关单号")]
        public virtual string 报关单号
        {
            get;
            set;
        }

        ///<summary>
        ///船公司
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_普通票_船公司")]
        public virtual 人员 船公司
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
        ///船名航次
        ///</summary>
        [Property(Length = 50, Index = "Idx_普通票_船名航次")]
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
        ///总重量
        ///</summary>
        [Property(NotNull = false, Index = "Idx_普通票_总重量")]
        public virtual int? 总重量
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int? 件数
        {
            get;
            set;
        }

        ///<summary>
        ///单价
        ///</summary>
        [Property(NotNull = false, Length = 19, Precision = 19, Scale = 2)]
        public virtual decimal? 单价
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

        ///<summary>
        ///对下备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 对下备注
        {
            get;
            set;
        }

        /// <summary>
        /// 允许应收对账
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 允许应收对账
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 50)]
        public virtual string 货物类别
        {
            get;
            set;
        }

        [Property(NotNull=false)]
        public virtual bool 自备箱
        {
            get;
            set;
        }

        ///<summary>
        ///介绍人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_普通票_介绍人")]
        public virtual 人员 介绍人
        {
            get;
            set;
        }

        ///<summary>
        ///介绍人
        ///</summary>
        [Property(Column = "介绍人", NotNull = false, Length = 6)]
        public virtual string 介绍人编号
        {
            get;
            set;
        }
    }
}
