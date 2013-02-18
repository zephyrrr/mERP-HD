using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cx
{
    [Class(Name = "查询_承兑汇票_凭证收付", Table = "视图查询_承兑汇票_凭证收付", SchemaAction = "none", Mutable = false)]
    public class 查询_承兑汇票_凭证收付 : IEntity,
        IDetailEntity<查询_承兑汇票, 查询_承兑汇票_凭证收付>
    {
        #region "Interface"
        查询_承兑汇票 IDetailEntity<查询_承兑汇票, 查询_承兑汇票_凭证收付>.MasterEntity
        {
            get { return 查询_承兑汇票; }
            set { 查询_承兑汇票 = value; }
        }
        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [ManyToOne(Column = "承兑汇票", Insert = false, Update = false)]
        public virtual 查询_承兑汇票 查询_承兑汇票
        {
            get;
            set;
        }

        [Property()]
        public virtual Int32 收付标志
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
        public virtual Guid 凭证
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
        [Property()]
        public virtual String 凭证摘要
        {
            get;
            set;
        }
        [Property()]
        public virtual DateTime 日期
        {
            get;
            set;
        }
    }
}