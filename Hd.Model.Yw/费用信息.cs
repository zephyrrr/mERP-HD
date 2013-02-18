using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{


    public enum 费用过程状态//额外费用过程管理中用到
    {
        未分离 = 1,//Submitted=false 的付（收）款记录有的，收（付）款记录无的
        未凭证对账 = 2,//付（收）款记录中存在 对账单=null and 凭证费用明细=null 的
        已凭证对账 = 3//付（收）款记录 全部都对过账或者凭证过的
    }

    //public enum 费用过程警示//额外费用过程管理中用到
    //{
    //    待处理提醒 = 1,//Submitted=false 的费用的处理时间(创建时间修改时间凭证时间)在近2天的 并且 费用信息的修改时间null或者在处理时间前的
    //    凭证对账超时 = 2,//未对账凭证的费用 修改时间创建时间 较早的
    //    费用不合理 = 3//费用金额和理论值差距大的
    //}

    /// <summary>
    /// 以这种方式A : B, A,B都是以Class Mapping，如果加Polymorphism = PolymorphismType.Explicit，则只会读取对应的Class，如果不加，则会对A，B都读取
    /// 另外，JoinedSubClass为连接Subclass，和subClass+Discriminator+Join功能一样
    /// UnionSubClass为每个类一个表。
    /// </summary>
    //[Class(Name="费用信息统计", Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version)]
    //public class 费用信息统计 : 费用信息
    //{
    //    [Id(0, Name = "Id", Column = "Id")]
    //    [Generator(1, Class = "guid.comb")]
    //    public override Guid Id
    //    {
    //        get;
    //        set;
    //    }
    //}

    [Class(NameType = typeof(费用信息), Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version, Polymorphism = PolymorphismType.Explicit)]
    public class 费用信息 : SubmittedEntity, IDeletableEntity,
        //IDetailEntity<费用实体, 费用信息>,
        //IDetailGenerateDetailEntity<费用实体, 费用信息>,
        IMasterEntity<费用信息, 业务费用>,
        IDetailEntity<普通票, 费用信息>,
        IDetailGenerateDetailEntity<普通票, 费用信息>
    {
        #region "Interface"
        bool IDeletableEntity.CanBeDelete(OperateArgs e)
        {
            e.Repository.Initialize(this.费用, this);
            return (this.费用.Count == 0);
        }
        //bool IDetailGenerateDetailEntity<费用实体, 费用信息>.CopyIfMatch(费用信息 other)
        //{
        //    if (this.费用实体.ID == other.费用实体.ID
        //        && this.费用分类 == other.费用分类)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //费用实体 IDetailEntity<费用实体, 费用信息>.MasterEntity
        //{
        //    get { return 费用实体; }
        //    set { 费用实体 = value; }
        //}

        IList<业务费用> IMasterEntity<费用信息, 业务费用>.DetailEntities
        {
            get { return 费用; }
            set { 费用 = value; }
        }

        普通票 IDetailEntity<普通票, 费用信息>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }

        bool IDetailGenerateDetailEntity<普通票, 费用信息>.CopyIfMatch(费用信息 newEntity)
        {
            if (this.费用项编号 == newEntity.费用项编号
                && this.票Id == newEntity.票Id)
            {
                return true;
            }
            return false;
        }
        #endregion

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_费用类别")]
        public virtual 费用类别 业务类型
        {
            get;
            set;
        }

        [Property(Column = "业务类型", NotNull = false)]
        public virtual int 业务类型编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = true, Column = "Id")]
        public virtual 滞箱费费用信息 滞箱费费用信息
        {
            get;
            set;
        }


        //[Property(Insert = false, Update = false, Formula = "(SELECT SUM(A.金额) FROM 财务_费用 A WHERE A.费用实体 = 票 and A.费用项 = 费用项 and A.收付标志 = 1) ")]
        //[OneToOne(Cascade = "none", Fetch = NHibernate.Mapping.Attributes.FetchMode.Join, Constrained = false)]
        [ManyToOne(Insert = false, Update = false, NotNull = true, Column = "Id")]
        public virtual 费用信息附加字段 费用信息附加字段
        {
            get;
            set;
        }

        //public virtual decimal 收款小计
        //{
        //    get;
        //    set;
        //}

        //[Property(Insert = false, Update = false, Formula = "(SELECT SUM(A.金额) FROM 财务_费用 A WHERE A.费用实体 = 票 and A.费用项 = 费用项 and A.收付标志 = 2) ")]
        //public virtual decimal 付款小计
        //{
        //    get;
        //    set;
        //}

        //[ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_费用信息_费用类别")]
        //public virtual 费用类别 费用类别
        //{
        //    get;
        //    set;
        //}

        //[Property(Column = "费用类别", NotNull = false)]
        //public virtual int? 费用类别编号
        //{
        //    get;
        //    set;
        //}

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_费用项")]
        public virtual 费用项 费用项
        {
            get;
            set;
        }

        [Property(Column = "费用项", Length = 3, NotNull = true)]
        public virtual string 费用项编号
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 完全标志付
        {
            get;
            set;
        }

        ///<summary>
        ///备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_普通票")]
        public virtual 普通票 票
        {
            get;
            set;
        }

        [Property(Column = "票", NotNull = true)]
        public virtual Guid 票Id
        {
            get;
            set;
        }

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "费用信息")]
        [OneToMany(2, ClassType = typeof(业务费用), NotFound = NotFoundMode.Ignore)]
        //[ManyToMany(2, ClassType = typeof(费用), Column = "Id", NotFound = NotFoundMode.Ignore)]
        public virtual IList<业务费用> 费用
        {
            get;
            set;
        }

        #region "统计信息"
        [Property(NotNull = false)]
        public virtual decimal? 委托人承担
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual decimal? 车队承担
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual decimal? 对外付款
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual decimal? 自己承担
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual bool 填全状态
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 费用过程状态? 收款对账凭证状态
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual 费用过程状态? 付款对账凭证状态
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 对外理论值
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 车队已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 车队未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 车队理论值
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 委托人已确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 委托人未确认
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 委托人理论值
        {
            get;
            set;
        }
        #endregion
    }
}
