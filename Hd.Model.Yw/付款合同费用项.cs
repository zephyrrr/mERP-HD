using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    public enum 付款合同费用项类型
    {
        /// <summary>
        /// 按照 费用理论值信息 中计算
        /// </summary>
        理论值计算 = 1,
        /// <summary>
        /// 报销（按照实际支出金额计算）
        /// </summary>
        报销 = 2
    }

    [Serializable]
    [Auditable]
    //[Subclass(NameType = typeof(付款合同费用项), ExtendsType = typeof(合同费用项), DiscriminatorValueEnumFormat = "d", DiscriminatorValueObject = 合同类型.付款合同)]
    [JoinedSubclass(NameType = typeof(付款合同费用项), Table = "参数备案_付款合同费用项", ExtendsType = typeof(合同费用项))]
    [Key(Column = "Id", ForeignKey = "FK_付款合同费用项_合同费用项")]
    public class 付款合同费用项 : 合同费用项,
        IDetailEntity<付款合同, 付款合同费用项>
    {
        #region "interface"
        付款合同 IDetailEntity<付款合同, 付款合同费用项>.MasterEntity
        {
            get { return 付款合同; }
            set { 付款合同 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_付款合同费用项_付款合同")]
        public virtual 付款合同 付款合同
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

        ///<summary>
        ///默认相关人。表达式。
        ///例如 $a := iif[%卸箱地编号% = \"900125\", \"900005\", %卸箱地编号%];iif[%卸箱地编号% = \"900125\", \"900005\", a]$
        ///</summary>
        [Property(Length = 400)]
        public virtual string 默认相关人
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual 付款合同费用项类型 付款合同费用项类型
        {
            get;
            set;
        }

    }
}
