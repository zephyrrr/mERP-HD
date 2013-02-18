using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cx
{
    [Class(Name = "查询_承兑汇票", Table = "视图查询_承兑汇票", SchemaAction = "none", Mutable = false)]
    public class 查询_承兑汇票 : IEntity,
        IMasterEntity<查询_承兑汇票, 查询_承兑汇票_凭证收付>,
        IOnetoOneParentEntity<查询_承兑汇票, 查询_承兑汇票_托收贴现>
    {
        #region "Interface"
        IList<查询_承兑汇票_凭证收付> IMasterEntity<查询_承兑汇票, 查询_承兑汇票_凭证收付>.DetailEntities
        {
            get { return 查询_承兑汇票_凭证收付; }
            set { 查询_承兑汇票_凭证收付 = value; }
        }

        查询_承兑汇票_托收贴现 IOnetoOneParentEntity<查询_承兑汇票, 查询_承兑汇票_托收贴现>.ChildEntity
        {
            get { return 查询_承兑汇票_托收贴现; }
            set { 查询_承兑汇票_托收贴现 = value; }
        }
        #endregion

        [Id(0, Name = "Id", Column = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "承兑汇票")]
        [OneToMany(2, ClassType = typeof(查询_承兑汇票_凭证收付), NotFound = NotFoundMode.Ignore)]
        public virtual IList<查询_承兑汇票_凭证收付> 查询_承兑汇票_凭证收付
        {
            get;
            set;
        }
        
        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        public virtual 查询_承兑汇票_托收贴现 查询_承兑汇票_托收贴现
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
        public virtual String 出票银行
        {
            get;
            set;
        }
        [Property()]
        public virtual DateTime 承兑期限
        {
            get;
            set;
        }

        [Property()]
        public virtual Decimal 金额
        {
            get;
            set;
        }
        [Property()]
        public virtual String 付款人
        {
            get;
            set;
        }

        [Property()]
        public virtual int? 托收贴现
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
        public virtual Boolean Submitted
        {
            get;
            set;
        }
    }
}