using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(受托人合同), Table = "参数备案_受托人合同", OptimisticLock = OptimisticLockMode.Version)]
    public class 受托人合同 : BaseBOEntity,
        IMasterEntity<受托人合同, 受托人合同费用项>
    {
        #region "interface"
        IList<受托人合同费用项> IMasterEntity<受托人合同, 受托人合同费用项>.DetailEntities
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

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_受托人合同_业务类型")]
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
        [Key(1, Column = "受托人合同")]
        [OneToMany(2, ClassType = typeof(受托人合同费用项), NotFound = NotFoundMode.Ignore)]
        public virtual IList<受托人合同费用项> 合同费用项
        {
            get;
            set;
        }

        ///<summary>
        ///委托人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_受托人合同_受托人")]
        public virtual 人员 受托人
        {
            get;
            set;
        }

        ///<summary>
        ///受托人编号
        ///</summary>
        [Property(Column = "受托人", NotNull = true, Length = 6)]
        public virtual string 受托人编号
        {
            get;
            set;
        }

        ///<summary>
        ///联系人
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_受托人合同_联系人")]
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

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_受托人合同_经手人")]
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

        /////<summary>
        /////默认相关人。表达式。 用票.承运人
        /////例如 $a := iif[%卸箱地编号% = \"900125\", \"900005\", %卸箱地编号%];iif[%卸箱地编号% = \"900125\", \"900005\", a]$
        /////</summary>
        //[Property(Length = 400)]
        //public virtual string 默认相关人
        //{
        //    get;
        //    set;
        //}
    }
}
