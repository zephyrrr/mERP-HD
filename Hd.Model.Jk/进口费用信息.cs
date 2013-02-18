//using System;
//using System.Collections.Generic;
//using System.Text;
//using NHibernate.Mapping.Attributes;
//using Feng;

//namespace Hd.Model.Jk
//{
//    [Serializable]
//    [Auditable]
//    //[Subclass(Name="进口费用信息", ExtendsType = typeof(费用信息), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 业务类型.进口)]
//    //[JoinedSubclass(Name = "进口费用信息",ExtendsType = typeof(费用信息), Table = "财务_费用信息")]
//    //[Key(Column = "Id", ForeignKey = "FK_费用信息_进口费用信息")]
//    //[Class(Name = "进口费用信息", Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version, Polymorphism = PolymorphismType.Explicit, Where = "业务类型 = 11", SchemaAction = "none")]
//    //[UnionSubclass(Table = "财务_费用信息", ExtendsType = typeof(费用信息))]
//    public class 进口费用信息 : 费用信息
//    //, IDetailEntity<进口票, 进口费用信息>
//    {
//        //#region "Interface"
//        //进口票 IDetailEntity<进口票, 进口费用信息>.MasterEntity
//        //{
//        //    get { return 进口票; }
//        //    set { 进口票 = value; }
//        //}
//        //#endregion
//        //[ManyToOne(Column = "票", ClassType = typeof(进口票), Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_普通票")]
//        //public new 进口票 票
//        //{
//        //    get;
//        //    set;
//        //}

//        //[ManyToOne(ClassType = typeof(进口票), Column = "票", Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_普通票")]
//        //public override 普通票 票
//        //{
//        //    get;
//        //    set;
//        //}

//        //// OneToOne with Constrained = true Not Working!!!
//        //// ManyToOne: Set NotNull = true 才能LazyLoad。 对方Class必须要Lazy = true
//        //[ManyToOne(NotNull = true, Insert = false, Update = false, Column = "Id")]
//        //public virtual 进口费用信息附加字段 进口费用信息附加字段
//        //{
//        //    get;
//        //    set;
//        //}
//    }
//}
