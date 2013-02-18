using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Utils;

namespace Hd.Model.Jk
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口票", ExtendsType = typeof(普通票), Table = "业务备案_进口票")]
    [Key(Column = "Id", ForeignKey = "FK_进口票_票")]
    //[Discriminator(Column = "费用主体类型", TypeType = typeof(int))]
    public class 进口票 : 普通票,
        IMasterEntity<进口票, 进口箱>,
        //IMasterGenerateDetailEntity<进口票, 进口箱>, 
        //IOnetoOneParentEntity<进口票, 进口票过程滞箱费减免>, 
        IOnetoOneParentEntity<进口票, 进口票过程转关标志>,
        IOnetoOneParentGenerateChildEntity<进口票, 进口票过程转关标志>
    {
        #region "Interface"
        IList<进口箱> IMasterEntity<进口票, 进口箱>.DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }

        //进口票过程滞箱费减免 IOnetoOneParentEntity<进口票, 进口票过程滞箱费减免>.ChildEntity
        //{
        //    get { return 滞箱费减免过程; }
        //    set { 滞箱费减免过程 = value; }
        //}

        //Type IOnetoOneParentEntity<进口票, 进口票过程滞箱费减免>.ChildType
        //{
        //    get { return 滞箱费减免标志 ? typeof(进口票过程滞箱费减免) : null; }
        //}

        进口票过程转关标志 IOnetoOneParentEntity<进口票, 进口票过程转关标志>.ChildEntity
        {
            get { return 转关过程; }
            set { 转关过程 = value; }
        }

        Type IOnetoOneParentGenerateChildEntity<进口票, 进口票过程转关标志>.ChildType
        {
            get
            {
                if (this.转关标志 == 转关标志.清关)
                {
                    return typeof(进口票过程转关标志清关);
                }
                else if (this.转关标志 == 转关标志.转关)
                {
                    return typeof(进口票过程转关标志转关);
                }
                else
                {
                    throw new ArgumentException("Invalid 转关标志");
                }
            }
        }

        //IList<进口箱> IMasterGenerateDetailEntity<进口票, 进口箱>.GenerateDetails()
        //{
        //    IList<进口箱> ret = new List<进口箱>();
        //    if (this.箱量.HasValue)
        //    {
        //        for (int i = 0; i < this.箱量.Value; ++i)
        //        {
        //            ret.Add(new 进口箱());
        //        }
        //    }
        //    return ret;
        //}

        //IList<费用> IMasterGenerateDetailEntity<费用实体, 费用>.GenerateDetails()
        //{
        //    IList<费用项> feeItems = (new 费用项Bll()).Get费用项(this, 费用实体类型.进口票);
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
        //            fee1.相关人编号 = this.委托人编号;
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

        protected override IList<费用信息> Generate费用信息Details()
        {
            IList<费用信息> ret = new List<费用信息>();
            // 不再判断，无用
            //if (this.滞箱费减免标志)
            {
                费用信息 item = new 费用信息();
                item.票Id = this.ID;
                item.费用项编号 = "167";   // 滞箱, 类别25
                item.业务类型编号 = 11;       // 进口

                ret.Add(item);
            }
            return ret;
        }

        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(进口箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<进口箱> 箱
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
        //        foreach (进口箱 i in 箱)
        //        {
        //            if (!i.还箱时间.HasValue)
        //                return null;
        //            dt = DateTimeHelper.MaxDateTime(dt, i.还箱时间.Value);
        //        }
        //        return dt;
        //    }
        //}

        ///// <summary>
        ///// 滞箱费减免过程信息
        ///// </summary>
        //[OneToOne(Cascade = "none", Constrained = false)]
        ////[ManyToOne(Column = "Id", NotNull = false, Insert = false, Update = false)]
        //public virtual 进口票过程滞箱费减免 滞箱费减免过程
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 滞箱费减免标志
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 滞箱费减免标志
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, Column = "Id")]
        public virtual 进口票额外费用 额外费用
        {
            get;
            set;
        }

        /// <summary>
        /// 转关过程信息
        /// </summary>
        [OneToOne(Cascade = "none", Constrained = false)]
        //[ManyToOne(Column = "Id", NotNull = false, Insert = false, Update = false)]
        public virtual 进口票过程转关标志 转关过程
        {
            get;
            set;
        }

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
        ///转关标志
        ///</summary>
        [Property(NotNull = true)]
        public virtual 转关标志 转关标志
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

        [Property(NotNull = false)]
        public virtual int? 报关天数
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int? 通关天数
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual int? 承运超期
        {
            get;
            set;
        }

        #region 基本信息备案
        ///<summary>
        ///船名
        ///</summary>
        [Property(Length = 30)]
        public virtual string 船名
        {
            get;
            set;
        }

        ///<summary>
        ///航次
        ///</summary>
        [Property(Length = 20)]
        public virtual string 航次
        {
            get;
            set;
        }

        ///<summary>
        ///总金额
        ///</summary>
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 总金额
        {
            get;
            set;
        }

        ///<summary>
        ///币制
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_币制")]
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
        ///产地
        ///</summary>
        [Property(Length = 20)]
        public virtual string 产地
        {
            get;
            set;
        }

        ///<summary>
        ///单证齐全时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 单证齐全时间
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

        ///<summary>
        ///卸箱地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_卸箱地")]
        public virtual 人员 卸箱地
        {
            get;
            set;
        }

        ///<summary>
        ///卸箱地
        ///</summary>
        [Property(Column = "卸箱地", Length = 6)]
        public virtual string 卸箱地编号
        {
            get;
            set;
        }

        ///<summary>
        ///转关指运地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_转关指运地")]
        public virtual 人员 转关指运地
        {
            get;
            set;
        }

        ///<summary>
        ///转关指运地
        ///</summary>
        [Property(Column = "转关指运地", Length = 6)]
        public virtual string 转关指运地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_承运人")]
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

        ///<summary>
        ///要求承运天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 要求承运天数
        {
            get;
            set;
        }

        ///<summary>
        ///申报类型
        ///</summary>
        [Property()]
        public virtual 申报类型? 申报类型
        {
            get;
            set;
        }

        ///<summary>
        ///注册证书号
        ///</summary>
        [Property(Length = 20)]
        public virtual string 注册证书号
        {
            get;
            set;
        }
        #endregion

        #region 跑单
        ///<summary>
        ///换单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 换单时间
        {
            get;
            set;
        }

        ///<summary>
        ///开箱单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 开箱单时间
        {
            get;
            set;
        }
        #endregion

        #region 报检
        ///<summary>
        ///卫生处理时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 卫生处理时间
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
        ///商检支票号
        ///</summary>
        [Property(Length = 20)]
        public virtual string 商检支票号
        {
            get;
            set;
        }

        ///<summary>
        ///报检员
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_报检员")]
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
        #endregion

        #region 商检查验
        ///<summary>
        ///商检查验时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 商检查验时间
        {
            get;
            set;
        }

        /////<summary>
        /////商检倒箱时间
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual DateTime? 商检倒箱时间
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///查验场地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_查验场地")]
        public virtual 人员 查验场地
        {
            get;
            set;
        }

        ///<summary>
        ///查验场地编号
        ///</summary>
        [Property(Column = "查验场地", NotNull = false, Length = 6)]
        public virtual string 查验场地编号
        {
            get;
            set;
        }

        ///<summary>
        ///查验员
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_查验员")]
        public virtual 人员 查验员
        {
            get;
            set;
        }

        ///<summary>
        ///查验员编号
        ///</summary>
        [Property(Column = "查验员", NotNull = false, Length = 6)]
        public virtual string 查验员编号
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT COUNT(*) FROM 业务备案_进口箱 A WHERE A.商检查验 > 0 AND A.票 = Id)")]
        public virtual int 商检查验箱量
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT COUNT(*) FROM 业务备案_进口箱 A WHERE (A.商检查验 = 2 OR A.商检查验 = 3) AND A.票 = Id)")]
        public virtual int 商检倒箱箱量
        {
            get;
            set;
        }
        #endregion

        #region 报关

        ///// <summary>
        ///// 报关单长号
        ///// </summary>
        //[Property(Length = 50)]
        //public virtual string 报关单长号
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///报关员
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_报关员")]
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
        ///放行时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 放行时间
        {
            get;
            set;
        }

        ///<summary>
        ///结关时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 结关时间
        {
            get;
            set;
        }
        #endregion

        #region 出纳
        ///<summary>
        ///结汇单交给货主时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 结汇单交给货主时间
        {
            get;
            set;
        }

        ///<summary>
        ///结汇单接受人
        ///</summary>
        [Property(Length = 50)]
        public virtual string 结汇单接受人
        {
            get;
            set;
        }
        #endregion

        #region 滞箱费减免
        ///<summary>
        ///免箱天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 免箱天数
        {
            get;
            set;
        }

        ///<summary>
        ///用箱天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 用箱天数
        {
            get;
            set;
        }

        ///<summary>
        ///用箱天数
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 预计用箱天数
        {
            get;
            set;
        }

        ///<summary>
        ///免箱船公司查询时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 免箱船公司查询时间
        {
            get;
            set;
        }

        ///<summary>
        ///免箱联系货主时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 免箱联系货主时间
        {
            get;
            set;
        }

        ///<summary>
        ///免箱船公司确认时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 免箱船公司确认时间
        {
            get;
            set;
        }

        ///<summary>
        ///滞箱费警示状态
        ///</summary>
        [Property(Length = 50)]
        public virtual string 滞箱费警示状态
        {
            get;
            set;
        }
        #endregion

        #region 异常情况
        ///<summary>
        ///异常情况
        ///</summary>
        [Property(Length = 50)]
        public virtual string 异常情况
        {
            get;
            set;
        }
        #endregion
        //[Property(Insert = false, Update = false, Formula = "(SELECT CASE WHEN (SELECT COUNT(*) FROM 财务_费用 A WHERE A.对账单 IS NOT NULL AND A.费用实体 = Id) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END)")]
        //public virtual bool 常规已对账
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///结束时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 结束时间
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

        //    进口票 that = (进口票)other;
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
        //    sb.Append("进口票: ");
        //    sb.Append("货代自编号").Append('=').Append(货代自编号);
        //    return sb.ToString();
        //}

        //#endregion

    }
}
