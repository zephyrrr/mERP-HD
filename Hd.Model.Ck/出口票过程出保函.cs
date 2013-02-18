//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Ck
//{
//    [Class(Table = "业务过程_出口票_出保函", OptimisticLock = OptimisticLockMode.Version)]
//    public class 出口票过程出保函 : LogEntity, 
//        IOnetoOneChildEntity<出口票, 出口票过程出保函>
//    {
//        #region "Interface"
//        出口票 IOnetoOneChildEntity<出口票, 出口票过程出保函>.ParentEntity
//        {
//            get { return 票; }
//            set { 票 = value; }
//        }
//        #endregion

//        [Id(0, Name = "Id", Column = "Id")]
//        [Generator(1, Class = "assigned")]
//        public virtual Guid Id
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 票信息
//        /// </summary>
//        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程出保函_出口票")]
//        public virtual 出口票 票
//        {
//            get; set;
//        }

//        #region 出保函
//        /// <summary>
//        /// 出具保函
//        /// </summary>
//        [Property(NotNull = false)]
//        public virtual bool 出具保函
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 保函编号
//        /// </summary>
//        [Property(Length = 50)]
//        public virtual string 保函编号
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 保函内容
//        /// </summary>
//        [Property(Length = 500)]
//        public virtual string 保函内容
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///保函时间
//        ///</summary>
//        [Property(NotNull = false)]
//        public virtual DateTime? 保函时间
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///保函期限
//        ///</summary>
//        [Property(NotNull = false)]
//        public virtual DateTime? 保函期限
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///保函核销时间
//        ///</summary>
//        [Property(NotNull = false)]
//        public virtual DateTime? 保函核销时间
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///出保函经手人
//        ///</summary>
//        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_出口票_出保函经手人")]
//        public virtual 人员 出保函经手人
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///出保函经手人编号
//        ///</summary>
//        [Property(Column = "出保函经手人", NotNull = false, Length = 6)]
//        public virtual string 出保函经手人编号
//        {
//            get;
//            set;
//        }
//        #endregion

//    }
//}
