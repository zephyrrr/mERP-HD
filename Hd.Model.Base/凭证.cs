using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model
{
    //public enum 费用明细状态//根据凭证费用明细的 审核标志  自动；凭证交互状态变[已作废]时要[未审核]
    //{
    //    未审核 = 0,   
    //    已部分审核 = 1,   
    //    已审核完全 = 2    
    //}

    //public enum 收支明细状态//根据凭证收支明细 自动；凭证交互状态变[已作废]时要[未出纳收付]
    //{
    //    未出纳收付 = 0,
    //    已出纳收付 = 1
    //}

    //public enum 凭证交互状态
    //{
    //    未提交 = 1,//只能修改、不能删除，因为要保证凭证号连续；不能打印。后续：已提交、已作废
    //    已提交 = 2,//前继：未提交、已作废；后续：允许作废
    //    允许作废 = 3,//收款凭证，会计置；付款凭证，出纳置。前继：已提交；后续：已作废、已提交
    //    已作废=4//收款凭证，出纳置；付款凭证，会计置。前继：未提交、允许作废；后续：已提交
    //}

    public enum 凭证用途分类
    {
        业务应付 = 1,
        业务报销 = 2,
        其他应付 = 3,
        其他报销 = 4
        //代收代付 = 5
    }

    public enum 支付方式要求
    {
        其他 = 0,
        承兑汇票 = 1,
    }

    public enum 凭证类别
    {
        收款凭证 = 1,
        付款凭证 = 2
    }

    public enum 自动手工标志
    {
        手工 = 1,
        承兑汇票 = 2,
        对账单=3
    }

    [Serializable]
    [Auditable]
    //[RepositoryConfigName("hd.model.cn.config")]
    //[Class(NameType = typeof(凭证), Table = "财务_凭证", OptimisticLock = OptimisticLockMode.Version)]
    [UnionSubclass(NameType = typeof(凭证), ExtendsType = typeof(应收应付源), Table = "财务_凭证")]
    public class 凭证 : 应收应付源, 
        IMasterEntity<凭证, 凭证费用明细>, 
        IMasterEntity<凭证, 凭证收支明细>,
        IDeletableEntity
    {
        #region "Interface"
        public override void PreparingOperate(OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.凭证号))
            {
                this.凭证号 = PrimaryMaxIdGenerator.GetMaxId("财务_凭证", "凭证号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(this.日期)).ToString();
            }
            //if (this.操作人 == "会计")
            //{
            //    this.费用明细状态 = 费用明细状态.未审核;

            //    e.Repository.Initialize(this.凭证费用明细, this);

            //    if (凭证费用明细 != null && 凭证费用明细.Count > 0)
            //    {
            //        bool allHave = true;
            //        foreach (凭证费用明细 sub in 凭证费用明细)
            //        {
            //            if (sub.审核标志)
            //            {
            //                this.费用明细状态 = 费用明细状态.已部分审核;
            //            }
            //            else
            //            {
            //                allHave = false;
            //            }
            //        }
            //        if (allHave)
            //        {
            //            this.费用明细状态 = 费用明细状态.已审核完全;
            //        }
            //    }
            // }
            //if (this.操作人 == "会计")
            //{
            //    this.会计编号 = SystemConfiguration.UserName;
            //}
            //else if (this.操作人 == "出纳")
            //{
            //    this.出纳编号 = SystemConfiguration.UserName;
            //}
            //else if (this.操作人 == "审核人")
            //{
            //    this.审核人编号 = SystemConfiguration.UserName;
            //}
        }

        IList<凭证费用明细> IMasterEntity<凭证, 凭证费用明细>.DetailEntities
        {
            get { return 凭证费用明细; }
            set { 凭证费用明细 = value; }
        }

        IList<凭证收支明细> IMasterEntity<凭证, 凭证收支明细>.DetailEntities
        {
            get { return 凭证收支明细; }
            set { 凭证收支明细 = value; }
        }

        bool IDeletableEntity.CanBeDelete(OperateArgs e)
        {
            return false;
        }
        #endregion

        public 凭证()
        {
        }

        [Property(NotNull = true)]
        public virtual 凭证类别 凭证类别
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 自动手工标志 自动手工标志
        {
            get;
            set;
        }

        /// <summary>
        /// 凭证号: 格式YYMMXXXX
        /// </summary>
        [Property(Length = 8, NotNull = true)]
        public virtual string 凭证号
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "凭证")]
        [OneToMany(2, ClassType = typeof(凭证费用明细), NotFound = NotFoundMode.Ignore)]
        public virtual IList<凭证费用明细> 凭证费用明细
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "凭证")]
        [OneToMany(2, ClassType = typeof(凭证收支明细), NotFound = NotFoundMode.Ignore)]
        public virtual IList<凭证收支明细> 凭证收支明细
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 日期
        {
            get;
            set;
        }

        [Property(Column = "相关人", NotNull = true, Length = 6)]
        public virtual string 相关人编号
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证_相关人")]
        public virtual 人员 相关人
        {
            get;
            set;
        }

        //[Property(NotNull = true)]
        //public virtual 费用明细状态 费用明细状态
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 收款凭证时，需会计审核
        /// </summary>
        [Property(NotNull = true)]
        public virtual bool 审核状态
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 收支状态
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 是否作废
        {
            get;
            set;
        }

        [Property(Length = 200)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Property(Length = 200)]
        public virtual string 摘要//收付款人、费用项、收付方式、票据号码
        {
            get;
            set;
        }

        ///<summary>
        ///金额    费用明细的汇总
        ///</summary>
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 会计金额
        {
            get;
            set;
        }

        private 金额 m_金额 = new 金额();
        //[ComponentProperty()]
        [RawXml(After = typeof(ComponentAttribute), Content = @"<component name=""金额"">
            <property name=""币制编号"" column = ""币制"" length=""3"" not-null=""false""/>
            <property name=""数额"" column = ""数额"" not-null=""false""/>
            <many-to-one name=""币制"" column = ""币制"" update=""false"" insert=""false"" foreign-key=""FK_凭证_币制""/>
            </component>")]
        public virtual 金额 金额
        {
            get { return m_金额; }
            set { if (value != null) m_金额 = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证_审核人")]
        public virtual 人员 审核人
        {
            get;
            set;
        }

        [Property(Column = "审核人", Length = 6)]
        public virtual string 审核人编号
        {
            get;
            set;
        }


        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证_会计")]
        public virtual 人员 会计
        {
            get;
            set;
        }

        [Property(Column = "会计", Length = 6)]
        public virtual string 会计编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证_出纳")]
        public virtual 人员 出纳
        {
            get;
            set;
        }

        [Property(Column = "出纳", Length = 6)]
        public virtual string 出纳编号
        {
            get;
            set;
        }

        //[Property()]
        //public virtual 收付款方式? 要求付款方式
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// "会计"，,"出纳", "审核人"
        /// </summary>
        public virtual string 操作人
        {
            get;
            set;
        }
    }
}
