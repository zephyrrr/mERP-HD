using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Hd.Model;

namespace Hd.Model.Fp
{

    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "成本发票", ExtendsType = typeof(费用实体), Table = "财务_成本发票")]
    [Key(Column = "Id", ForeignKey = "FK_成本发票_费用实体")]
    public class 成本发票 : 费用实体, 
         IMasterEntity<成本发票, 成本发票明细>
    {
        #region "Interface"
        IList<成本发票明细> IMasterEntity<成本发票, 成本发票明细>.DetailEntities
        {
            get { return this.成本发票明细; }
            set { this.成本发票明细 = value; }
        }

        public override void PreparedOperate(OperateArgs e)
        {
            base.PreparedOperate(e);

            if (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update)
            {
                if (this.成本发票明细 != null)
                {
                    e.Repository.Initialize(this.成本发票明细, this);
                    decimal sum = 0;
                    foreach (成本发票明细 i in this.成本发票明细)
                    {
                        sum += i.金额;
                    }
                    this.入账金额 = sum;
                }
            }
        }
        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "成本发票")]
        [OneToMany(2, ClassType = typeof(成本发票明细), NotFound = NotFoundMode.Ignore)]
        public virtual IList<成本发票明细> 成本发票明细
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 买入日期
        {
            get;
            set;
        }

        //[ManyToOne(Insert = false, Update = false, ForeignKey = "FK_成本发票_相关人")]
        //public virtual 人员 相关人
        //{
        //    get;
        //    set;
        //}

        //[Property(Column = "相关人", NotNull = false, Length = 6)]
        //public virtual string 相关人编号
        //{
        //    get;
        //    set;
        //}

        [Property(NotNull = true, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual decimal 入账金额
        {
            get;
            set;
        }

        public virtual string 大写金额
        {
            get
            {
                if (金额.HasValue)
                {
                    return Feng.Windows.Utils.ChineseHelper.ConvertToChinese(金额.Value);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
