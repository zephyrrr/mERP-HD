using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 应收应付类型
    {
        业务 = 1,               // 应收应付减少，（常规，额外，其他）
        //专款专用明细 = 2,       // 不影响应收应付（选择已存在费用）（包括投资）
        借款类型 = 11,          // 应收应付增加，需填结算期限
        管理费用类型 = 12,       // 不影响应收应付，要需相应生成费用
        待分摊类型 = 13,        // 应收应付增加 004，用于资产
        其他 = 99               // 不影响应收应付，也不生成费用 （固定资产 + 凭证费用明细 有费用的（即专款专用））
    }
 
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(费用项), Table = "参数备案_费用项", OptimisticLock = OptimisticLockMode.Version)]
    //[AttributeIdentifier("Id.Column", Name = "Id.Column", Value = "编号")]
    public class 费用项 : BaseEntity<string>
    {
        public override string Identity
        {
            get { return this.编号; }
        }

        ///<summary>
        ///编号
        ///</summary>
        [Id(0, Name = "编号", Length = 3)]
        [Generator(1, Class = "assigned")]
        public virtual string 编号
        {
            get;
            set;
        }

        ///<summary>
        ///名称
        ///</summary>
        [Property(Length = 10, NotNull = true, Unique = true, UniqueKey = "UK_费用项_名称")]
        public virtual string 名称
        {
            get;
            set;
        }


        //[Property(NotNull = true)]
        //public virtual bool 收
        //{
        //    get;
        //    set;
        //}

        //[Property(NotNull = true)]
        //public virtual bool 付
        //{
        //    get;
        //    set;
        //}

        ///<summary>
        ///应收应付类型
        ///</summary>
        [Property(NotNull = false)]
        public virtual 应收应付类型 应收应付类型
        {
            get;
            set;
        }

        /////<summary>
        /////添加费用实体类型
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual int? 添加费用实体类型
        //{
        //    get;
        //    set;
        //}

        /////<summary>
        /////现有费用实体类型
        /////</summary>
        //[Property(Length = 100)]
        //public virtual string 现有费用实体类型
        //{
        //    get;
        //    set;
        //}

        //[Property(NotNull = true)]
        //public virtual bool 调节款
        //{
        //    get;
        //    set;
        //}

        //[Property(NotNull = true)]
        //public virtual bool 坏账
        //{
        //    get;
        //    set;
        //}

        [Property()]
        public virtual int? 收入类别
        {
            get;
            set;
        }

        [Property()]
        public virtual int? 支出类别
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 票
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 箱
        {
            get;
            set;
        }
    }
}
