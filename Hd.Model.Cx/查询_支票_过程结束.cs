using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cx
{
    [Class(Name = "查询_支票_过程结束", Table = "视图查询_支票_过程结束", SchemaAction = "none", Mutable = false)]
    public class 查询_支票_过程结束 : IEntity,
        IOnetoOneChildEntity<查询_支票, 查询_支票_过程结束>
    {
        #region "Interface"
        查询_支票 IOnetoOneChildEntity<查询_支票, 查询_支票_过程结束>.ParentEntity
        {
            get { return 查询_支票; }
            set { 查询_支票 = value; }
        }
        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true)]
        public virtual 查询_支票 查询_支票
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
        public virtual String 源
        {
            get;
            set;
        }
        [Property()]
        public virtual Guid? 入款账户
        {
            get;
            set;
        }
        [Property()]
        public virtual String 凭证摘要
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
        public virtual Guid? 凭证
        {
            get;
            set;
        }

        [Property()]
        public virtual String 凭证号
        {
            get;
            set;
        }
    }
}