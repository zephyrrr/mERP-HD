using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    [Serializable]
    [Auditable]
    //[Subclass(NameType = typeof(委托人合同费用项), ExtendsType = typeof(合同费用项), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 合同类型.委托人合同)]
    [JoinedSubclass(NameType = typeof(受托人合同费用项), Table = "参数备案_受托人合同费用项", ExtendsType = typeof(合同费用项))]
    [Key(Column = "Id", ForeignKey = "FK_受托人合同费用项_合同费用项")]
    public class 受托人合同费用项 : 合同费用项,
        IDetailEntity<受托人合同, 受托人合同费用项>
    {
        #region "interface"
        受托人合同 IDetailEntity<受托人合同, 受托人合同费用项>.MasterEntity
        {
            get { return 受托人合同; }
            set { 受托人合同 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_受托人合同费用项_受托人合同")]
        public virtual 受托人合同 受托人合同
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

        ///// <summary>
        ///// 公式。如为真则生成
        ///// $%票.委托人编号% = "9002851"$
        ///// 结果为true, false
        ///// </summary>
        //[Property(NotNull = true, Length = 400)]
        //public virtual string 是否生成
        //{
        //    get;
        //    set;
        //}

        [Property(NotNull = true)]
        public virtual 付款合同费用项类型 付款合同费用项类型
        {
            get;
            set;
        }
    }
}
