using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model
{
    //public enum 对账单状态//用于 应收对账单
    //{
    //    未送出 = 1,
    //    已送出未返回 = 2,
    //    已返回 = 3,
    //    //已确认 = 4
    //}

    //public enum 对账单状态2//用于应付、应收调节、应收坏账
    //{
    //    未确认 = 3,
    //    已确认 = 4
    //}

    //public enum 对账费用类型 //用于应收（对账单、调节款、坏账   同一业务类型相同
    //{
    //    常规费用=1,
    //    额外费用=2
    //}

    [Serializable]
    [Auditable]
    //[Class(NameType = typeof(对账单), Table = "财务_对账单", OptimisticLock = OptimisticLockMode.Version)]
    //[JoinedSubclass(0, NameType = typeof(对账单), ExtendsType = typeof(应收应付源), Table = "财务_对账单")]
    //[Key(1, Column = "Id")]
    [UnionSubclass(0, NameType = typeof(对账单), ExtendsType = typeof(应收应付源),  Table = "财务_对账单")]
    //[Discriminator(1, Column = "对账单分类", TypeType = typeof(int))]
    public class 对账单 : 应收应付源,
        IMasterEntity<对账单, 费用>, IDeletableEntity
    {
        #region "Interface"
        public override void PreparingOperate(OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.编号))
            {
                if (this.结束日期.HasValue)
                {
                    this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_对账单", "编号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(this.结束日期.Value)).ToString();
                }
                if (this.关账日期.HasValue)
                {
                    this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_对账单", "编号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(this.关账日期.Value)).ToString();
                }
            }
        }

        IList<费用> IMasterEntity<对账单, 费用>.DetailEntities
        {
            get { return 费用; }
            set { 费用 = value; }
        }

        bool IDeletableEntity.CanBeDelete(OperateArgs e)
        {
            return !this.Submitted;
        }
        #endregion 

        [Property(NotNull = true)]
        public virtual int 对账单类型
        {
            get;
            set;
        }

        [Property(Length = 8, NotNull = true, Unique = true, UniqueKey = "UK_对账单_编号", Index = "Idx_对账单_编号")]
        public virtual string 编号
        {
            get;
            set;
        }

        [ManyToOne(NotNull = false, Insert = false, Update = false, ForeignKey = "FK_对账单_相关人")]
        public virtual 人员 相关人
        {
            get;
            set;
        }

        [Property(Column = "相关人", Length = 6, NotNull = false)]
        public virtual string 相关人编号
        {
            get;
            set;
        }

        ///<summary>
        ///收付标志
        ///</summary>
        [Property(NotNull = true)]
        public virtual 收付标志 收付标志
        {
            get;
            set;
        }

        ///<summary>
        ///备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        ///<summary>
        ///送出时间
        ///</summary>
        [Property()]
        public virtual DateTime? 送出时间
        {
            get;
            set;
        }

        ///<summary>
        ///返回时间
        ///</summary>
        [Property()]
        public virtual DateTime? 结束日期
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_对账单_费用项")]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        [Property(Column = "费用项", NotNull = false, Length = 3)]
        public virtual string 费用项编号
        {
            get;
            set;
        }

        ///<summary>
        ///起始日期
        ///</summary>
        [Property()]
        public virtual DateTime? 起始日期
        {
            get;
            set;
        }

        ///<summary>
        ///关账日期
        ///</summary>
        [Property()]
        public virtual DateTime? 关账日期
        {
            get;
            set;
        }

        ///<summary>
        ///结算期限
        ///</summary>
        [Property()]
        public virtual DateTime? 结算期限
        {
            get;
            set;
        }

        ///<summary>
        ///金额   对账单已确认状态 费用明细的汇总
        ///</summary>
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
        {
            get;
            set;
        }

        public virtual decimal? 收款金额
        {
            get { return 收付标志 == 收付标志.收 && 金额 != null ? (decimal?)金额 : null; }
            set
            {
                if (value.HasValue)
                {
                    收付标志 = 收付标志.收; 金额 = value.Value;
                }
                else
                {
                    if (收付标志 == 收付标志.收)
                    {
                        金额 = null;
                    }
                }
            }
        }

        public virtual decimal? 付款金额
        {
            get { return 收付标志 == 收付标志.付 && 金额 != null ? (decimal?)金额 : null; }
            set
            {
                if (value.HasValue)
                {
                    收付标志 = 收付标志.付; 金额 = value.Value;
                }
                else
                {
                    if (收付标志 == 收付标志.付)
                    {
                        金额 = null;
                    }
                }
            }
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "对账单")]
        [OneToMany(2, ClassType = typeof(费用), NotFound = NotFoundMode.Ignore)]
        public virtual IList<费用> 费用
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_对账单_业务类型")]
        public virtual 费用类别 业务类型
        {
            get;
            set;
        }

        [Property(Column = "业务类型", NotNull = false)]
        public virtual int? 业务类型编号
        {
            get;
            set;
        }

        /// <summary>
        /// 专款专用凭证
        /// </summary>
        [ManyToOne(NotNull = false, ForeignKey = "FK_对账单_凭证", Cascade = "none")]
        public virtual 凭证 凭证
        {
            get;
            set;
        }

        //[Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.凭证号 FROM 视图查询_业务费用明细 A WHERE A.对账单 = Id)")]
        //public virtual string 凭证号
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///凭证号
        ///</summary>
        [Property(Length = 8)]
        public virtual string 凭证号
        {
            get;
            set;
        }
    }
}
