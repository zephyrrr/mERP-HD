using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ck2
{
    [Class(Table = "业务过程_出口只票不箱_商检查验", OptimisticLock = OptimisticLockMode.Version)]
    public class 出口只票不箱过程商检查验 : LogEntity,
        IOnetoOneChildEntity<出口只票不箱过程报检, 出口只票不箱过程商检查验>
    {
        #region "Interface"
        出口只票不箱过程报检 IOnetoOneChildEntity<出口只票不箱过程报检, 出口只票不箱过程商检查验>.ParentEntity
        {
            get { return 报检过程; }
            set { 报检过程 = value; }
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
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程商检查验_过程报检")]
        public virtual 出口只票不箱过程报检 报检过程
        {
            get; set;
        }

        #region 商检查验
        ///<summary>
        ///商检查验箱号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 商检查验箱号
        {
            get;
            set;
        }

        ///<summary>
        ///商检倒箱箱号
        ///</summary>
        [Property(Length = 500)]
        public virtual string 商检倒箱箱号
        {
            get;
            set;
        }

        ///<summary>
        ///商检查验时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 商检查验时间
        {
            get;
            set;
        }

        /// <summary>
        /// 商检查验结果
        /// </summary>
        [Property(NotNull = false)]
        public virtual 合格标志? 商检查验结果
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口只票不箱过程_商检查验场地")]
        public virtual 客户 商检查验场地
        {
            get;
            set;
        }

        ///<summary>
        ///商检查验场地
        ///</summary>
        [Property(Column = "商检查验场地", Length = 6)]
        public virtual string 商检查验场地编号
        {
            get;
            set;
        }
        #endregion
    }
}
