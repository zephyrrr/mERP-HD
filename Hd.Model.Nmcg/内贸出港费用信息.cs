using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Nmcg
{
    [Serializable]
    [Auditable]
    //[Class(Name = "内贸出港费用信息", Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version, Where = "业务类型 = '15'", SchemaAction = "none")]
    public class 内贸出港费用信息 : 费用信息
        //, IDetailEntity<内贸出港票, 内贸出港费用信息>
    {
        //#region "Interface"
        //内贸出港票 IDetailEntity<内贸出港票, 内贸出港费用信息>.MasterEntity
        //{
        //    get { return 内贸出港票; }
        //    set { 内贸出港票 = value; }
        //}
        //#endregion

        //[ManyToOne(Column = "票", Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_普通票")]
        //public virtual 内贸出港票 内贸出港票
        //{
        //    get;
        //    set;
        //}
    }
}
