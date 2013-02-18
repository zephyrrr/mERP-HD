using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck
{
    [Class(Table = "业务过程_出口箱_承运", OptimisticLock = OptimisticLockMode.Version)]
    public class 出口箱过程承运 : LogEntity,
        IOnetoOneChildEntity<出口箱, 出口箱过程承运>
    {
        #region "Interface"
        出口箱 IOnetoOneChildEntity<出口箱, 出口箱过程承运>.ParentEntity
        {
            get { return this.箱; }
            set { this.箱 = value; }
        }

        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// 箱信息
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程承运_出口箱")]
        public virtual 出口箱 箱
        {
            get; set;
        }

        #region 运输


        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_出口箱_提箱地")]
        public virtual 客户 提箱地
        {
            get;
            set;
        }

        ///<summary>
        ///提箱地
        ///</summary>
        [Property(Column = "提箱地", Length = 6)]
        public virtual string 提箱地编号
        {
            get;
            set;
        }

        ///<summary>
        ///提箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 提箱时间
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口箱_装货地")]   
        public virtual 客户 装货地
        {
            get;
            set;
        }

        ///<summary>
        ///装货地
        ///</summary>
        [Property(Column = "装货地", Length = 6)]
        public virtual string 装货地编号
        {
            get;
            set;
        }

        ///<summary>
        ///装货时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 装货时间
        {
            get;
            set;
        }

        ///<summary>
        ///进港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 进港时间
        {
            get;
            set;
        }

        ///<summary>
        ///货代提箱时间要求止
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 货代提箱时间要求止
        {
            get;
            set;
        }

        ///<summary>
        ///对下备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 对下备注
        {
            get;
            set;
        }
        #endregion
    }
}
