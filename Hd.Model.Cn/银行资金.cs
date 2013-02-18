using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using NHibernate.Mapping.Attributes;

namespace Hd.Model.Cn
{
    //[Class(Name="银行资金", Table = "财务_银行资金", OptimisticLock = OptimisticLockMode.Version)]
    //public class 银行资金 : LogEntity
    //{
    //    [Id(0, Name = "Id", Column = "Id")]
    //    [Generator(1, Class = "identity")]
    //    public virtual long Id
    //    {
    //        get;
    //        set;
    //    }

    //    [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_银行资金_银行账户")]
    //    public virtual 银行账户 银行账户
    //    {
    //        get;
    //        set;
    //    }

    //    [Property(Column = "银行账户", NotNull = true)]
    //    public virtual Guid 银行账户编号
    //    {
    //        get;
    //        set;
    //    }

    //    [Property(NotNull = true, Length = 19, Precision = 19, Scale = 2)]
    //    public virtual decimal 数额
    //    {
    //        get;
    //        set;
    //    }
    //}
}
