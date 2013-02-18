using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 收付标志
    {
        收 = 1,
        付 = 2
    }

    public enum 费用类型
    {
        业务费用 = 1,
        调节业务款 = 3,
        //坏账 = 4,
        非业务费用 = 11
    }

    [Class(NameType = typeof(费用), Table = "财务_费用", OptimisticLock = OptimisticLockMode.Version, DiscriminatorValue = "0")]
    [Discriminator(Column = "费用类型", TypeType = typeof(int))]
    public class 费用 : BaseBOEntity, IDeletableEntity, IOperatingEntity,
        IDetailGenerateDetailEntity<费用实体, 费用>,
        IDetailEntity<凭证费用明细, 费用>,
        IDetailEntity<对账单, 费用>
    {
        public 费用()
        {
            this.收付标志 = 收付标志.收;
        }

        #region "Interface"
        public virtual void PreparedOperate(OperateArgs e)
        {
        }
        public virtual void PreparingOperate(OperateArgs e)
        {
        }

        public virtual bool CanBeDelete(OperateArgs e)
        {
            费用 entity = e.Entity as 费用;
            if (entity.凭证费用明细 != null)
            {
                throw new InvalidUserOperationException("此费用已出凭证，不能删除!");
            }
            if (entity.对账单 != null && entity.对账单.Submitted)
            {
                throw new InvalidUserOperationException("此费用已出对帐单且已确认，不能删除!");
            }
            return true;
        }

        费用实体 IDetailEntity<费用实体, 费用>.MasterEntity
        {
            get { return 费用实体; }
            set { 费用实体 = value; }
        }

        对账单 IDetailEntity<对账单, 费用>.MasterEntity
        {
            get { return 对账单; }
            set { 对账单 = value; }
        }

        bool IDetailGenerateDetailEntity<费用实体, 费用>.CopyIfMatch(费用 other)
        {
            if (this.费用项编号 == other.费用项编号
                && this.收付标志 == other.收付标志)
            {
                this.相关人编号 = other.相关人编号;
                return true;
            }
            return false;
        }

        //IList<并列费用> IMasterEntity<费用, 并列费用>.DetailEntities
        //{
        //    get { return null; }
        //    set { }
        //}
        凭证费用明细 IDetailEntity<凭证费用明细, 费用>.MasterEntity
        {
            get { return 凭证费用明细; }
            set { 凭证费用明细 = value; }
        }
        #endregion

        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_费用_费用类别")]
        public virtual 费用类别 费用类别
        {
            get;
            set;
        }

        [Property(Column = "费用类别", NotNull = false)]
        public virtual int? 费用类别编号
        {
            get;
            set;
        }

        [ManyToOne(NotNull = true, ForeignKey = "FK_费用_费用实体", Index = "Idx_费用_费用实体", Cascade = "none")]
        public virtual 费用实体 费用实体
        {
            get;
            set;
        }

        //Insert = false, Update = false, 
        //[Property(Column = "费用实体", NotNull = true)]
        //public virtual Guid 费用实体Id
        //{
        //    get;
        //    set;
        //}

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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_费用_费用项")]
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

        [Property(NotNull = false)]
        public virtual 收付标志 收付标志
        {
            get;
            set;
        }


        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_费用_相关人")]
        public virtual 人员 相关人
        {
            get;
            set;
        }

        [Property(Column = "相关人", NotNull = false, Length = 6)]
        public virtual string 相关人编号
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 金额
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

        ///// <summary>
        ///// 计算时产生，不保存（****全部核销）
        ///// </summary>
        //[Property(NotNull = true)]
        //public virtual decimal 已核销数额
        //{
        //    get;
        //    set;
        //}

        [ManyToOne(NotNull = false, ForeignKey = "FK_费用_对账单", Cascade = "none")]
        public virtual 对账单 对账单
        {
            get;
            set;
        }

        /// <summary>
        /// 用于核销的凭证
        /// </summary>
        [ManyToOne(NotNull = false, ForeignKey = "FK_费用_凭证费用明细", Cascade = "none")]
        public virtual 凭证费用明细 凭证费用明细
        {
            get;
            set;
        }
    }
}
