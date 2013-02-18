//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Jk
//{
//    [Class(Name="进口箱过程滞箱费减免", Table = "业务过程_进口箱_滞箱费减免", OptimisticLock = OptimisticLockMode.Version)]
//    public class 进口箱过程滞箱费减免 : LogEntity,
//         IOnetoOneChildEntity<进口箱, 进口箱过程滞箱费减免>
//    {
//        #region "Interface"
//        进口箱 IOnetoOneChildEntity<进口箱, 进口箱过程滞箱费减免>.ParentEntity
//        {
//            get { return 箱; }
//            set { 箱 = value; }
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
//        /// 箱信息
//        /// </summary>
//        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程滞箱费减免_进口箱")]
//        public virtual 进口箱 箱
//        {
//            get; set;
//        }


//    }
//}
