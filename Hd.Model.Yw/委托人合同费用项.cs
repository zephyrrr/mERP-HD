using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 委托人合同费用项类型
    {
        /// <summary>
        /// 按照 费用理论值信息 中计算
        /// </summary>
        理论值计算 = 1,
        /// <summary>
        /// 代垫（付多少收多少）
        /// </summary>
        代垫 = 2
    }

    [Serializable]
    [Auditable]
    //[Subclass(NameType = typeof(委托人合同费用项), ExtendsType = typeof(合同费用项), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 合同类型.委托人合同)]
    [JoinedSubclass(NameType = typeof(委托人合同费用项), Table = "参数备案_委托人合同费用项", ExtendsType = typeof(合同费用项))]
    [Key(Column = "Id", ForeignKey = "FK_委托人合同费用项_合同费用项")]
    public class 委托人合同费用项 : 合同费用项,
        IDetailEntity<委托人合同, 委托人合同费用项>
    {
        #region "interface"
        委托人合同 IDetailEntity<委托人合同, 委托人合同费用项>.MasterEntity
        {
            get { return 委托人合同; }
            set { 委托人合同 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_委托人合同费用项_委托人合同", Cascade = "none")]
        public virtual 委托人合同 委托人合同
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 委托人合同费用项类型 委托人合同费用项类型
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 可开票标志
        {
            get;
            set;
        }
    }
}
