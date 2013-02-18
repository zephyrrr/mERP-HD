using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    /// <summary>
    /// <example>a959a103-2b2c-4991-a7c3-810577dd90da	1	101	900285	40	True	0	0	100000	2009-9-1 0:00:00	NULL	NULL</example>
    /// </summary>
    [Serializable]
    [Auditable]
    [Class(NameType = typeof(费用理论值信息), Table = "参数备案_费用理论值信息", OptimisticLock = OptimisticLockMode.Version)]
    public class 费用理论值信息 : BaseBOEntity,
         IDetailEntity<合同费用项, 费用理论值信息>
    {
        #region "interface"
        合同费用项 IDetailEntity<合同费用项, 费用理论值信息>.MasterEntity
        {
            get { return 合同费用项; }
            set { 合同费用项 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_费用理论值信息_合同费用项", Cascade = "none")]
        public virtual 合同费用项 合同费用项
        {
            get;
            set;
        }


        [Property(NotNull = true)]
        public virtual int 序号
        {
            get;
            set;
        }

        /// <summary>
        /// 格式：iif[true, 20, 30]。
        /// 例如，大箱为30小箱为20，则公式为iif[箱型=大箱，30，20]
        /// </summary>
        [Property(NotNull = true, Length = 4000)]
        public virtual string 条件
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 4000)]
        public virtual string 结果
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 255)]
        public virtual string 备注
        {
            get;
            set;
        }
    }
}
  