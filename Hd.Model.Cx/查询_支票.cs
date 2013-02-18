using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cx
{
    [Class(Name = "查询_支票", Table = "视图查询_支票", SchemaAction = "none", Mutable = false)]
    public class 查询_支票 : IEntity, 
        IOnetoOneParentEntity<查询_支票, 查询_支票_过程在途>,
        IOnetoOneParentEntity<查询_支票, 查询_支票_过程结束>
    {
        #region "Interface"
        查询_支票_过程在途 IOnetoOneParentEntity<查询_支票, 查询_支票_过程在途>.ChildEntity
        {
            get { return 查询_支票_过程在途; }
            set { 查询_支票_过程在途 = value; }
        }

        查询_支票_过程结束 IOnetoOneParentEntity<查询_支票, 查询_支票_过程结束>.ChildEntity
        {
            get { return 查询_支票_过程结束; }
            set { 查询_支票_过程结束 = value; }
        }
        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 查询_支票_过程在途 查询_支票_过程在途
        {
            get;
            set;
        }
        
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 查询_支票_过程结束 查询_支票_过程结束
        {
            get;
            set;
        }

        [Property()]
        public virtual String 源
        {
            get;
            set;
        }

        

        [Property()]
        public virtual String 支票类型
        {
            get;
            set;
        }

        [Property()]
        public virtual String 票据号码
        {
            get;
            set;
        }

        [Property()]
        public virtual Guid? 银行账户
        {
            get;
            set;
        }

        [Property()]
        public virtual Decimal? 金额
        {
            get;
            set;
        }

        [Property()]
        public virtual String 币制
        {
            get;
            set;
        }

        [Property()]
        public virtual String 备注
        {
            get;
            set;
        }

        [Property()]
        public virtual String 摘要
        {
            get;
            set;
        }

        [Property()]
        public virtual DateTime? 日期
        {
            get;
            set;
        }

        [Property()]
        public virtual Boolean 是否作废
        {
            get;
            set;
        }

        [Property()]
        public virtual Boolean Submitted
        {
            get;
            set;
        }
    }
}