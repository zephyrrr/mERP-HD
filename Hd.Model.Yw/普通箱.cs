using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model
{
    //public enum 箱型
    //{
    //    [Description("20'平箱")]
    //    小箱 = 20,
    //    [Description("20'高箱")]
    //    小高箱 = 21,
    //    [Description("20'冷冻箱")]
    //    小冷冻箱 = 22,
    //    [Description("40'平箱")]
    //    大箱 = 40,
    //    [Description("40'高箱")]
    //    大高箱 = 41,
    //    [Description("40'冷冻箱")]
    //    大冷冻箱 = 42,
    //    //其它 = 99
    //}

    public enum 查验标志
    {
        不查验 = 0,
        开箱门 = 1,
        半倒箱 = 2,
        全倒箱 = 3,
        不开箱门 = 4
    }

    [Serializable]
    [Auditable]
    [Class(NameType = typeof(普通箱), Table = "业务备案_普通箱", OptimisticLock = OptimisticLockMode.Version)]
    public class 普通箱 : BaseBOEntity
    {
    	#region "Interface"
        //IList<费用> IMasterGenerateDetailEntity<费用实体, 费用>.GenerateDetails()
        //{
        //    IList<费用> list = Utility.Get费用By合同(this, this.普通票.合同);
        //    foreach (费用 i in list)
        //    {
        //        if (i.收付标志 == 收付标志.收)
        //        {
        //            i.相关人编号 = this.普通票.委托人编号;
        //        }
        //    }
        //    return list;
        //}
    	#endregion

        [Property(Length = 12, Index = "Idx_普通箱_箱号")]
        public virtual string 箱号
        {
            get;
            set;
        }

        ///<summary>
        ///箱型
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_普通箱_箱型")]
        public virtual 箱型 箱型
        {
            get;
            set;
        }

        [Property(Column = "箱型", NotNull = false)]
        public virtual int? 箱型编号
        {
            get;
            set;
        }

        ///<summary>
        ///重量
        ///</summary>
        [Property(NotNull = false)]
        public virtual int? 重量
        {
            get;
            set;
        }

        ///<summary>
        ///封志号
        ///</summary>
        [Property(Length = 20)]
        public virtual string 封志号
        {
            get;
            set;
        }

        ///<summary>
        ///品名
        ///</summary>
        [Property(Length = 500)]
        public virtual string 品名
        {
            get;
            set;
        }

        ///<summary>
        ///内部备注
        ///</summary>
        [Property(Length = 500)]
        public virtual string 内部备注
        {
            get;
            set;
        }
    }
}
