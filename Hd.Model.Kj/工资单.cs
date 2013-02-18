using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Kj
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "工资单", ExtendsType = typeof(费用实体), Table = "财务_工资单")]
    [Key(Column = "Id", ForeignKey = "FK_工资单_费用实体")]
    public class 工资单 : 费用实体
    {
        public virtual decimal? 工资小计
        {
            get 
            {
                decimal dXj = 0;
                if (基本工资.HasValue)
                    dXj += 基本工资.Value;
                if (餐费.HasValue)
                    dXj += 餐费.Value;
                if (通讯费.HasValue)
                    dXj += 通讯费.Value;
                if (福利.HasValue)
                    dXj += 福利.Value;
                if (补助.HasValue)
                    dXj += 补助.Value;
                if (违纪扣款.HasValue)
                    dXj += 违纪扣款.Value;
                if (养老扣款.HasValue)
                    dXj += 养老扣款.Value;
                if (医疗扣款.HasValue)
                    dXj += 医疗扣款.Value;
                if (失业扣款.HasValue)
                    dXj += 失业扣款.Value;
                if (其他扣款.HasValue)
                    dXj += 其他扣款.Value;
                return dXj;
            }
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.凭证号 FROM 财务_凭证 A INNER JOIN 财务_凭证费用明细 B ON B.凭证 = A.ID INNER JOIN 财务_费用 C ON C.凭证费用明细 = B.ID WHERE C.费用实体 = Id)")]
        public virtual string 凭证号
        {
            get;
            set;
        }

        [Property(Insert = false, Update = false, Formula = "(SELECT TOP 1 A.金额 FROM 财务_费用 A WHERE A.费用实体 = Id)")]
        public virtual decimal? 登记金额
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_工资_员工")]
        public virtual 人员 员工
        {
            get;
            set;
        }

        [Property(Column = "员工", NotNull = true, Length = 6)]
        public virtual string 员工编号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 日期
        {
            get;
            set;
        }

        ///<summary>
        ///简介
        ///</summary>
        [Property(Length = 500)]
        public virtual string 简介
        {
            get;
            set;
        }

        [Property(Length = 500)]
        public virtual string 备注
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 基本工资
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 餐费
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 通讯费
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 福利
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 补助
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 违纪扣款
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 养老扣款
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 医疗扣款
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 失业扣款
        {
            get;
            set;
        }

        [Property(NotNull = false, Precision = 19, Scale = 2)]
        public virtual decimal? 其他扣款
        {
            get;
            set;
        }
    }
}
