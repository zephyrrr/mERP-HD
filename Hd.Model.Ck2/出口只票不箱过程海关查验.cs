using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck2
{
    [Class(Table = "业务过程_出口只票不箱_海关查验", OptimisticLock = OptimisticLockMode.Version)]
    public class 出口只票不箱过程海关查验 : LogEntity,
        IOnetoOneChildEntity<出口只票不箱过程报关, 出口只票不箱过程海关查验>
    {
        #region "Interface"
        出口只票不箱过程报关 IOnetoOneChildEntity<出口只票不箱过程报关, 出口只票不箱过程海关查验>.ParentEntity
        {
            get { return 报关过程; }
            set { 报关过程 = value; }
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
        /// 票信息
        /// </summary>
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程海关查验_过程报关")]
        public virtual 出口只票不箱过程报关 报关过程
        {
            get; set;
        }

        #region 海关查验
        ///<summary>
        ///海关查验箱号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 海关查验箱号
        {
            get;
            set;
        }

        ///<summary>
        ///海关倒箱箱号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 海关倒箱箱号
        {
            get;
            set;
        }
        ///<summary>
        ///海关查验下单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关查验下单时间
        {
            get;
            set;
        }

        ///<summary>
        ///海关查验时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关查验时间
        {
            get;
            set;
        }

        /// <summary>
        /// 海关查验结果
        /// </summary>
        [Property(NotNull = false)]
        public virtual 海关查验结果? 海关查验结果
        {
            get;
            set;
        }

        /// <summary>
        /// 海关处理结果
        /// </summary>
        [Property(NotNull = false)]
        public virtual 海关处理结果? 海关处理结果
        {
            get;
            set;
        }

        /// <summary>
        /// 海关查验老师
        /// </summary>
        [Property(Length = 50)]
        public virtual string 海关查验老师
        {
            get;
            set;
        }

        ///<summary>
        ///海关查验完成时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关查验完成时间
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱过程_海关查验场地")]
        public virtual 客户 海关查验场地
        {
            get;
            set;
        }

        ///<summary>
        ///海关查验场地
        ///</summary>
        [Property(Column = "海关查验场地", Length = 6)]
        public virtual string 海关查验场地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱过程_海关查验陪同人")]
        public virtual 人员 海关查验陪同人
        {
            get;
            set;
        }

        ///<summary>
        ///海关查验陪同人
        ///</summary>
        [Property(Column = "海关查验陪同人", Length = 6)]
        public virtual string 海关查验陪同人编号
        {
            get;
            set;
        }
        #endregion

        #region 海关查验 移交
        /// <summary>
        /// 违规性质
        /// </summary>
        [Property(NotNull = false)]
        public virtual 违规性质? 违规性质
        {
            get;
            set;
        }

        ///<summary>
        ///处罚时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 处罚时间
        {
            get;
            set;
        }

        /// <summary>
        /// 处罚结果
        /// </summary>
        [Property(NotNull = false)]
        public virtual 处罚结果? 处罚结果
        {
            get;
            set;
        }

        /// <summary>
        /// 处罚决定书号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 处罚决定书号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_移交处理部门")]
        public virtual 客户 移交处理部门
        {
            get;
            set;
        }

        ///<summary>
        ///移交处理部门
        ///</summary>
        [Property(Column = "移交处理部门", Length = 6)]
        public virtual string 移交处理部门编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱_移交经手人")]
        public virtual 人员 移交经手人
        {
            get;
            set;
        }

        ///<summary>
        ///移交经手人
        ///</summary>
        [Property(Column = "移交经手人", Length = 6)]
        public virtual string 移交经手人编号
        {
            get;
            set;
        }
        #endregion
    }
}
