//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Nmcg
//{
//    [Class(Name="内贸出港箱过程承运", Table = "业务过程_内贸出港箱_承运", OptimisticLock = OptimisticLockMode.Version)]
//    public class 内贸出港箱过程承运 : LogEntity,
//        IOnetoOneChildEntity<内贸出港箱, 内贸出港箱过程承运>
//    {
//        #region "Interface"
//        内贸出港箱 IOnetoOneChildEntity<内贸出港箱, 内贸出港箱过程承运>.ParentEntity
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
//        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true, ForeignKey = "FK_过程承运_内贸出港箱")]
//        public virtual 内贸出港箱 箱
//        {
//            get; set;
//        }

//        #region 运输




//        #endregion
//    }
//}
