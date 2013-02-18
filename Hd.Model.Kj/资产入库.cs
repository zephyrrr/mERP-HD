using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Kj
{
    /// <summary>
    /// 资产入库，不进入费用，只进入应收应付；应付款增加、库存增加
    /// </summary>
    [UnionSubclass(NameType = typeof(资产入库), ExtendsType = typeof(应收应付源), Table = "财务_资产入库")]
    public class 资产入库 : 应收应付源
    {
        public override void PreparingOperate(Feng.OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.编号))
            {
                this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_资产入库", "编号", 8, PrimaryMaxIdGenerator.GetIdYearMonth()).ToString();
            }
        }

        ///<summary>
        ///编号
        ///</summary>
        [Property(Length = 8, NotNull = true, Unique = true, UniqueKey = "UK_资产入库_编号", Index = "Idx_资产入库_编号")]
        public virtual string 编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_资产入库_相关人")]
        public virtual 人员 相关人//销售商
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

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_资产入库_业务类型")]
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
