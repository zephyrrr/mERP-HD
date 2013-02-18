using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    /// <summary>
    /// 根据收付标志可选择费用类别，根据费用类别可选择费用项和业务类型。
    /// </summary>
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(凭证费用明细), Table = "财务_凭证费用明细", OptimisticLock = OptimisticLockMode.Version)]
    //[Discriminator(Column = "收付标志", TypeType = typeof(int))]
    public class 凭证费用明细 : BaseBOEntity, //IOperatingEntity,
        IDetailEntity<凭证, 凭证费用明细>,
        IMasterEntity<凭证费用明细, 费用>
    {
        #region "Interface"
        //void IOperatingEntity.PreparingOperate(OperateArgs e)
        //{
        //    if (this.凭证.审核状态 && (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update))
        //    {
        //        e.Repository.Initialize(this.费用, this);

        //        if (费用 == null || 费用.Count == 0)
        //        {
        //            return;
        //        }
        //        // 应收应付，不检查金额
        //        if (this.费用项编号 != "000" && this.费用项编号 != "001" && this.费用项编号 != "002")
        //        {
        //            decimal sum = 0;                    
        //            foreach (费用 item in 费用)
        //            {
        //                // 任意凭证费用明细都是同一个MemoryDao<费用>
        //                if (item.凭证费用明细 != this)
        //                    continue;

        //                if (item.收付标志 == 收付标志.收)
        //                    sum -= item.金额.Value;
        //                else if (item.收付标志 == 收付标志.付)
        //                    sum += item.金额.Value;
        //                else
        //                    throw new InvalidUserOperationException("未填写有效的收付标志！");
        //            }

        //            decimal sum2 = 0;
        //            if (this.金额.HasValue)
        //            {
        //                sum2 = this.收付标志 == 收付标志.付 ? this.金额.Value : -this.金额.Value;
        //            }

        //            if (sum2 != sum)
        //            {
        //                throw new InvalidUserOperationException("凭证费用明细金额和费用总金额不付，请重新填写！");
        //            }
        //        }
        //    }
        //}
        //void IOperatingEntity.PreparedOperate(OperateArgs e)
        //{
        //}
        凭证 IDetailEntity<凭证, 凭证费用明细>.MasterEntity
        {
            get { return 凭证; }
            set { 凭证 = value; }
        }

        IList<费用> IMasterEntity<凭证费用明细, 费用>.DetailEntities
        {
            get { return 费用; }
            set { 费用 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_凭证费用明细_凭证", Cascade = "none")]
        public virtual 凭证 凭证
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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证费用明细_相关人")]
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

        [Property(NotNull = true, Precision = 19, Scale = 2)]
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
        [Key(1, Column = "凭证费用明细")]
        [OneToMany(2, ClassType = typeof(费用), NotFound = NotFoundMode.Ignore)]
        public virtual IList<费用> 费用
        {
            get;
            set;
        }

        // 当是一般应收款时，可以随便填，只是在凭证上显示
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_凭证费用明细_费用项")]
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

        [Property(Column = "凭证费用类别", NotNull = false)]
        public virtual int? 凭证费用类别编号
        {
            get;
            set;

            //string s = null;
            //if (this.收付标志 == 收付标志.收)
            //{
            //    s = NameValueMappingCollection.Instance.FindColumn2FromColumn1("费用项_全部", "编号", "收入类别", this.费用项编号);
            //}
            //else
            //{
            //    s = NameValueMappingCollection.Instance.FindColumn2FromColumn1("费用项_全部", "编号", "支出类别", this.费用项编号);
            //}
            //int? i = Feng.Utils.ConvertHelper.ToInt(s);
            //if (i.HasValue)
            //    return i.Value;
            //else
            //    return -1;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_凭证费用明细_费用类别")]
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

        [Property(Length = 200, NotNull = false)]
        public virtual string 备注
        {
            get;
            set;
        }

        /// <summary>
        /// 填写借款时用
        /// </summary>
        [Property(NotNull = false)]
        public virtual DateTime? 结算期限
        {
            get;
            set;
        }

        [Property()]
        public virtual 支付方式要求? 支付方式要求
        {
            get;
            set;
        }
    }
}
