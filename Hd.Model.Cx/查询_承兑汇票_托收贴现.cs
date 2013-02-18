using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Cx
{
    [Class(Name = "查询_承兑汇票_托收贴现", Table = "视图查询_承兑汇票_托收贴现", SchemaAction = "none", Mutable = false)]
    public class 查询_承兑汇票_托收贴现 : IEntity,
        IOnetoOneChildEntity<查询_承兑汇票, 查询_承兑汇票_托收贴现>
    {
        #region "Interface"
        查询_承兑汇票 IOnetoOneChildEntity<查询_承兑汇票, 查询_承兑汇票_托收贴现>.ParentEntity
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

        [OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = true)]
        public virtual 查询_承兑汇票 查询_承兑汇票
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
        public virtual DateTime? 出去时间
        {
            get;
            set;
        }
        [Property()]
        public virtual String 经办人
        {
            get;
            set;
        }
        [Property()]
        public virtual String 出去经手人
        {
            get;
            set;
        }
        [Property()]
        public virtual DateTime? 返回时间
        {
            get;
            set;
        }
        [Property()]
        public virtual Int32? 返回方式
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
        public virtual String 返回经手人
        {
            get;
            set;
        }
        [Property()]
        public virtual Decimal? 返回金额
        {
            get;
            set;
        }
    }
}