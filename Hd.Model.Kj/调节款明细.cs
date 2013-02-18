//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Kj
//{
//    [Serializable]
//    [Auditable]
//    [Class(NameType = typeof(调节款明细), Table = "财务_调节款_明细", OptimisticLock = OptimisticLockMode.Version)]
//    public class 调节款明细 : BaseBOEntity,
//        IDetailEntity<调节款, 调节款明细>
//    {
//        #region "Interface"
//        调节款 IDetailEntity<调节款, 调节款明细>.MasterEntity
//        {
//            get { return 调节款; }
//            set { 调节款 = value; }
//        }
//        #endregion

//        [Property(NotNull = true)]
//        public virtual 收付标志 收付标志
//        {
//            get;
//            set;
//        }

//        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_调节款明细_相关人")]
//        public virtual 人员 相关人
//        {
//            get;
//            set;
//        }

//        [Property(Column = "相关人", NotNull = true, Length = 6)]
//        public virtual string 相关人编号
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = true, Length = 19, Precision = 19, Scale = 2)]
//        public virtual decimal? 金额
//        {
//            get;
//            set;
//        }

//        public virtual decimal? 收款金额
//        {
//            get { return 收付标志 == 收付标志.收 && 金额 != null ? (decimal?)金额 : null; }
//            set
//            {
//                if (value.HasValue)
//                {
//                    收付标志 = 收付标志.收; 金额 = value.Value;
//                }
//                else
//                {
//                    if (收付标志 == 收付标志.收)
//                    {
//                        金额 = null;
//                    }
//                }
//            }
//        }

//        public virtual decimal? 付款金额
//        {
//            get { return 收付标志 == 收付标志.付 && 金额 != null ? (decimal?)金额 : null; }
//            set
//            {
//                if (value.HasValue)
//                {
//                    收付标志 = 收付标志.付; 金额 = value.Value;
//                }
//                else
//                {
//                    if (收付标志 == 收付标志.付)
//                    {
//                        金额 = null;
//                    }
//                }
//            }
//        }

//        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_调节款明细_费用项")]
//        public virtual 费用项 费用项
//        {
//            get;
//            set;
//        }

//        [Property(Column = "费用项", NotNull = true, Length = 3)]
//        public virtual string 费用项编号
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = true)]
//        public virtual DateTime 日期
//        {
//            get;
//            set;
//        }

//        [Property(NotNull = true)]
//        public virtual DateTime 结算期限
//        {
//            get;
//            set;
//        }

//        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_调节款明细_业务类型")]
//        public virtual 业务类型 业务类型
//        {
//            get;
//            set;
//        }

//        [Property(Column = "业务类型", NotNull = true)]
//        public virtual int 业务类型编号
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///备注
//        ///</summary>
//        [Property(Length = 200)]
//        public virtual string 备注
//        {
//            get;
//            set;
//        }

//        //[ManyToOne(NotNull = true, ForeignKey = "FK_应收应付款_应收应付源", Cascade = "none")]
//        //public virtual 应收应付源 应收应付源
//        //{
//        //    get;
//        //    set;
//        //}

//        [ManyToOne(NotNull = true, ForeignKey = "FK_调节款明细_调节款", Cascade = "none")]
//        public virtual 调节款 调节款
//        {
//            get;
//            set;
//        }
//    }
//}
