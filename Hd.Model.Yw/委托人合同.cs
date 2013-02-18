using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(委托人合同), Table = "参数备案_委托人合同", OptimisticLock = OptimisticLockMode.Version)]
    public class 委托人合同 : BaseBOEntity,
        IMasterEntity<委托人合同, 委托人合同费用项>
    {
        #region "interface"
        IList<委托人合同费用项> IMasterEntity<委托人合同, 委托人合同费用项>.DetailEntities
        {
            get { return 合同费用项; }
            set { 合同费用项 = value; }
        }
        #endregion
        ///// <summary>
        ///// 对账单号: 格式YYYYMMXXXX
        ///// </summary>
        //[Property(Length = 8, NotNull = true, Unique = true, Index = "Idx_合同_合同号", UniqueKey = "UK_合同_合同号")]
        //public virtual string 合同号
        //{
        //    get;
        //    set;
        //}

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_委托人合同_业务类型")]
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

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "委托人合同")]
        [OneToMany(2, ClassType = typeof(委托人合同费用项), NotFound = NotFoundMode.Ignore)]
        public virtual IList<委托人合同费用项> 合同费用项
        {
            get;
            set;
        }

        ///<summary>
        ///委托人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_合同_委托人")]
        public virtual 人员 委托人
        {
            get;
            set;
        }

        ///<summary>
        ///委托人编号
        ///</summary>
        [Property(Column = "委托人", NotNull = true, Length = 6)]
        public virtual string 委托人编号
        {
            get;
            set;
        }

        ///<summary>
        ///联系人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_合同_联系人")]
        public virtual 人员 联系人
        {
            get;
            set;
        }

        ///<summary>
        ///委托人编号
        ///</summary>
        [Property(Column = "联系人", NotNull = true, Length = 6)]
        public virtual string 联系人编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_合同_经手人")]
        public virtual 人员 经手人
        {
            get;
            set;
        }

        [Property(Column = "经手人", NotNull = true, Length = 6)]
        public virtual string 经手人编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 签约时间
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期始
        /// </summary>
        [Property(NotNull = true)]
        public virtual DateTime 有效期始
        {
            get;
            set;
        }

        /// <summary>
        /// 有效期止
        /// </summary>
        [Property(NotNull = true)]
        public virtual DateTime 有效期止
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual int 允许单证晚到天数
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual int 常规费用结账时间
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual int 额外费用结账时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 收付款方式? 支付方式
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual int 卸箱能力每天
        {
            get;
            set;
        }

       
    }
}
