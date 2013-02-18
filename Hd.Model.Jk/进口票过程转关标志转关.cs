using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口票过程转关标志转关", ExtendsType = typeof(进口票过程转关标志), Table = "业务过程_进口票_转关")]
    [Key(Column = "Id", ForeignKey = "FK_进口票过程转关标志转关_进口票过程转关标志")]
    public class 进口票过程转关标志转关 : 进口票过程转关标志
    {
        ///<summary>
        ///白卡排车时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 白卡排车时间
        {
            get;
            set;
        }

        ///<summary>
        ///口岸海关申报时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 口岸海关申报时间
        {
            get;
            set;
        }

        /// <summary>
        /// 转关联系单号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 转关联系单号
        {
            get;
            set;
        }

        /// <summary>
        /// 指运地报关单号
        /// </summary>
        [Property(Length = 50)]
        public virtual string 指运地报关单号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 指运地报关时间
        {
            get;
            set;
        }
     
        ///<summary>
        ///指运地海关查验时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 指运地海关查验时间
        {
            get;
            set;
        }

        ///<summary>
        ///指运地查验场地
        ///</summary>
        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_进口票_指运地查验场地")]
        public virtual 人员 指运地查验场地
        {
            get;
            set;
        }

        ///<summary>
        ///指运地查验场地编号
        ///</summary>
        [Property(Column = "指运地查验场地", NotNull = false, Length = 6)]
        public virtual string 指运地查验场地编号
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 指运地结关时间
        {
            get;
            set;
        }
    }
}
