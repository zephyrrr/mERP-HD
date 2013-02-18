using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Cn
{
    public enum 存取标志
    {
        存款 = 1,
        取款 = 2
    }

    [Serializable]
    [Auditable]
    [Class(Name = "银行存取款", Table = "财务_银行存取款", OptimisticLock = OptimisticLockMode.Version)]
    public class 银行存取款 : BaseBOEntity, IOperatingEntity
    {
        void IOperatingEntity.PreparingOperate(OperateArgs e)
        {
            if (string.IsNullOrEmpty(this.编号) && (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update))
            {
                this.编号 = PrimaryMaxIdGenerator.GetMaxId("财务_银行存取款", "编号", 8, "Y" + PrimaryMaxIdGenerator.GetIdYearMonth(日期)).ToString();
            }
        }
        void IOperatingEntity.PreparedOperate(OperateArgs e)
        {
        }

        ///<summary>
        ///编号
        ///</summary>
        [Property(Length = 8, NotNull = true, Unique = true, UniqueKey = "UK_银行存取款_编号", Index = "Idx_银行存取款_编号")]
        public virtual string 编号
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

        [Property(NotNull = true)]
        public virtual 存取标志 存取标志
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_银行存取款_银行账户")]
        public virtual 银行账户 银行账户
        {
            get;
            set;
        }

        [Property(Column = "银行账户", NotNull = true)]
        public virtual Guid 银行账户编号
        {
            get;
            set;
        }

        [Property(NotNull = true, Precision = 19, Scale = 2)]
        public virtual decimal 金额
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_银行存取款_员工")]
        public virtual 人员 经办人
        {
            get;
            set;
        }

        [Property(Column = "经办人", Length = 6)]
        public virtual string 经办人编号
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


        
    }
}
