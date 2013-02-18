using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(进口箱滞箱费减免), Table = "视图查询_业务备案_进口箱", OptimisticLock = OptimisticLockMode.Version, SchemaAction = "none")]
    public class 进口箱滞箱费减免 : IEntity
    {
        [Id(0, Name = "Id", Column = "Id")] //only for when length is meaningful
        [Generator(1, Class = "assigned")]
        public virtual Guid Id
        {
            get;
            set;
        }

        //[Property(Insert = false, Update = false, Formula = "(SELECT SUM(CASE A.收付标志 WHEN '1' THEN A.金额 ELSE -A.金额 END) FROM 财务_费用 A WHERE A.费用项 = '168' AND A.相关人 = (SELECT TOP 1 船公司 FROM 业务备案_普通票 B WHERE B.ID = 票) AND A.费用实体 = 票 AND A.箱 = Id)")] 
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 退滞箱费
        {
            get;
            set;
        }

        //[Property(Insert = false, Update = false, Formula = "(SELECT SUM(CASE A.收付标志 WHEN '2' THEN A.金额 ELSE -A.金额 END) FROM 财务_费用 A WHERE A.费用项 = '167' AND A.相关人 = (SELECT TOP 1 船公司 FROM 业务备案_普通票 B WHERE B.ID = 票) AND A.费用实体 = 票 AND A.箱 = Id)")]
        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 最终滞箱费
        {
            get;
            set;
        }
    }
}
