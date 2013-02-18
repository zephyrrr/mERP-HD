using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Kj
{
    /// <summary>
    /// 调节款，不进入费用，只进入应收应付。和业务调节款不同，它不和票箱关联，只是调节相关人
    /// </summary>
    [UnionSubclass(NameType = typeof(调节款), ExtendsType = typeof(应收应付源), Table = "财务_调节款")]
    public class 调节款 : 应收应付源
    {
        public override void PreparingOperate(Feng.OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.编号))
            {
                this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_调节款", "编号", 8, PrimaryMaxIdGenerator.GetIdYearMonth()).ToString();
            }
        }

        ///<summary>
        ///编号
        ///</summary>
        [Property(Length = 8, NotNull = true, Unique = true, UniqueKey = "UK_调节款_编号", Index = "Idx_调节款_编号")]
        public virtual string 编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 收付标志 收付标志
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_调节款_相关人")]
        public virtual 人员 相关人
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

        [Property(NotNull = true, Length = 19, Precision = 19, Scale = 2)]
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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_调节款_费用项")]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        [Property(Column = "费用项", NotNull = true, Length = 3)]
        public virtual string 费用项编号
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

        [Property(NotNull = false)]
        public virtual DateTime? 结算期限
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_调节款_业务类型")]
        public virtual 费用类别 业务类型
        {
            get;
            set;
        }

        [Property(Column = "业务类型", NotNull = true)]
        public virtual int 业务类型编号
        {
            get;
            set;
        }

        ///<summary>
        ///备注
        ///</summary>
        [Property(Length = 200)]
        public virtual string 备注
        {
            get;
            set;
        }
    }
}
