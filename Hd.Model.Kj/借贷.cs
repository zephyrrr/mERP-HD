//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Kj
//{
//    [Serializable]
//    [Auditable]
//    [JoinedSubclass(Name = "借贷", ExtendsType = typeof(费用实体), Table = "财务_借贷")]
//    [Key(Column = "Id", ForeignKey = "FK_借贷_费用实体")]
//    public class 借贷 : 费用实体, IOperatingEntity
//    {
//        void IOperatingEntity.PreparingOperate(OperateArgs e)
//        {
            
//        }
//        void IOperatingEntity.PreparedOperate(OperateArgs e)
//        {
//            if (e.OperateType == OperateType.Save || e.OperateType == OperateType.Update)
//            {
//                if (this.费用 != null)
//                {
//                    e.Repository.Initialize(this.费用, this);
//                    foreach (费用 i in this.费用)
//                    {
//                        if (string.IsNullOrEmpty(i.相关人编号))
//                        {
//                            i.相关人编号 = this.相关人编号;
//                            i.相关人 = this.相关人;
//                        }
//                    }
//                }
//            }
//        }

//        [Property(Insert = false, Update = false, Formula = "(SELECT SUM(ISNULL((CASE A.收付标志 WHEN '1' THEN A.金额 ELSE NULL END), 0)) FROM 财务_费用 A WHERE A.费用实体 = Id) ")]
//        public virtual decimal 收款小计
//        {
//            get;
//            set;
//        }

//        [Property(Insert = false, Update = false, Formula = "(SELECT SUM(ISNULL((CASE A.收付标志 WHEN '2' THEN A.金额 ELSE NULL END), 0)) FROM 财务_费用 A WHERE A.费用实体 = Id) ")]
//        public virtual decimal 付款小计
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

//        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_借贷_相关人")]
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

//        ///<summary>
//        ///简介
//        ///</summary>
//        [Property(Length = 500)]
//        public virtual string 简介
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///备注
//        ///</summary>
//        [Property(Length = 500)]
//        public virtual string 备注
//        {
//            get;
//            set;
//        }

//        ///<summary>
//        ///摘要
//        ///</summary>
//        [Property(Length = 100)]
//        public virtual string 摘要//产生应收款时的凭证号等
//        {
//            get;
//            set;
//        }
//    }
//}
