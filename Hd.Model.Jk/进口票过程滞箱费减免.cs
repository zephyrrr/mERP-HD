//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Jk
//{
//    [Class(Name="进口票过程滞箱费减免", Table = "业务过程_进口票_滞箱费减免", OptimisticLock = OptimisticLockMode.Version)]
//    public class 进口票过程滞箱费减免 : LogEntity, 
//        IOnetoOneChildEntity<进口票, 进口票过程滞箱费减免>
//    {
//        #region "Interface"
//        进口票 IOnetoOneChildEntity<进口票, 进口票过程滞箱费减免>.ParentEntity
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
//        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程滞箱费减免_进口票")]
//        public virtual 进口票 票
//        {
//            get; set;
//        }

//        //[Bag(0, Cascade = "none", Inverse = true)]
//        //[Key(1, Column = "票")]
//        //[OneToMany(2, ClassType = typeof(进口箱过程滞箱费减免), NotFound = NotFoundMode.Ignore)]
//        //public virtual IList<进口箱过程滞箱费减免> 进口箱过程滞箱费减免
//        //{
//        //    get;
//        //    set;
//        //}

//        #region 滞箱费减免
//        ///<summary>
//        ///船公司查询时间
//        ///</summary>
//        [Property(NotNull = false)]
//        public virtual DateTime? 船公司查询时间
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///与货主联系时间
//        ///</summary>
//        [Property(NotNull = false)]
//        public virtual DateTime? 与货主联系时间
//        {
//            get;
//            set;
//        }

//        /////<summary>
//        /////船公司最终确认时间
//        /////</summary>
//        //[Property(NotNull = false)]
//        //public virtual DateTime? 船公司最终确认时间
//        //{
//        //    get;
//        //    set;
//        //}
//        #endregion
//        //理论单箱最大滞箱天数、票理论疏港天数、放行时间、报关天数、单证晚到天数、用箱天数
//    }
//}
