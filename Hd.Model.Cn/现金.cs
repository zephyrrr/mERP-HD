using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using NHibernate.Mapping.Attributes;

namespace Hd.Model.Cn
{
    //[Class(Name="现金", Table = "财务_现金", OptimisticLock = OptimisticLockMode.Version)]
    //public class 现金 : LogEntity
    //{
    //    [Id(0, Name = "Id", Column = "Id")]
    //    [Generator(1, Class = "identity")]
    //    public virtual long Id
    //    {
    //        get;
    //        set;
    //    }

    //    [ManyToOne(Insert = false, Update = false, NotNull = true, ForeignKey = "FK_现金_币制")]
    //    public virtual 币制 币制
    //    {
    //        get;
    //        set;
    //    }

    //    [Property(Column = "币制", NotNull = true, Length = 3)]
    //    public virtual string 币制编号
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
