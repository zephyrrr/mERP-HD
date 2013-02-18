using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Jk
{
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "进口票过程转关标志清关", ExtendsType = typeof(进口票过程转关标志), Table = "业务过程_进口票_清关")]
    [Key(Column = "Id", ForeignKey = "FK_进口票过程转关标志清关_进口票过程转关标志")]
    public class 进口票过程转关标志清关 : 进口票过程转关标志
    {
        #region 报关
        ///<summary>
        ///审单中心电子审结时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 审单中心电子审结时间
        {
            get;
            set;
        }

        ///<summary>
        ///现场交单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 现场交单时间
        {
            get;
            set;
        }

        ///<summary>
        ///海关审单时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关审单时间
        {
            get;
            set;
        }

        ///<summary>
        ///海关征税时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关征税时间
        {
            get;
            set;
        }

        ///<summary>
        ///海关实物放行时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关实物放行时间
        {
            get;
            set;
        }
        #endregion

        #region 海关查验
        ///<summary>
        ///海关查验时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 海关查验时间
        {
            get;
            set;
        }

        /////<summary>
        /////海关倒箱时间
        /////</summary>
        //[Property(NotNull = false)]
        //public virtual DateTime? 海关倒箱时间
        //{
        //    get;
        //    set;
        //}
        #endregion

        #region 出纳
        ///<summary>
        ///税单交给货主时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 税单交给货主时间
        {
            get;
            set;
        }
        ///<summary>
        ///税单接受人
        ///</summary>
        [Property(Length = 50)]
        public virtual string 税单接受人
        {
            get;
            set;
        }
        #endregion
    }
}
