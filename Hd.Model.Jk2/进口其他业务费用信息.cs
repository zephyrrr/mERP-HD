using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk2
{
    [Serializable]
    [Auditable]
    //[Class(Name = "进口其他业务费用信息", Table = "财务_费用信息", OptimisticLock = OptimisticLockMode.Version, Where = "业务类型 = '45'", SchemaAction = "none")]
    public class 进口其他业务费用信息 : 费用信息
        //, IDetailEntity<进口其他业务票, 进口其他业务费用信息>
    {
        //#region "Interface"
        //进口其他业务票 IDetailEntity<进口其他业务票, 进口其他业务费用信息>.MasterEntity
        //{
        //    get { return 进口其他业务票; }
        //    set { 进口其他业务票 = value; }
        //}
        //#endregion
        //[ManyToOne(Column = "票", Insert = false, Update = false, NotNull = true, ForeignKey = "FK_费用信息_普通票")]
        //public virtual 进口其他业务票 进口其他业务票
        //{
        //    get;
        //    set;
        //}
    }
}
