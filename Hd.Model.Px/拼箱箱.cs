using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Px
{
    // 跟出口一样
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "拼箱箱", ExtendsType = typeof(普通箱), Table = "业务备案_拼箱箱")]
    [Key(Column = "Id", ForeignKey = "FK_拼箱箱_箱")]
    public class 拼箱箱 : 普通箱,
        IDetailEntity<拼箱票, 拼箱箱>
    {
        #region "Interface"
        拼箱票 IDetailEntity<拼箱票, 拼箱箱>.MasterEntity
        {
            get { return 票; }
            set { 票 = value; }
        }
        #endregion

        [ManyToOne(NotNull = true, ForeignKey = "FK_拼箱箱_拼箱票", Column = "票", Cascade = "none")]
        public virtual 拼箱票 票
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual bool 查验标志
        {
            get;
            set;
        }

        #region 基本信息备案
       
        #endregion
      
        #region 海关查验
        /// <summary>
        /// 海关查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual 查验标志? 海关查验
        {
            get;
            set;
        }
        #endregion

        #region 商检查验
        /// <summary>
        /// 商检查验
        /// </summary>
        [Property(NotNull = false)]
        public virtual 查验标志? 商检查验
        {
            get;
            set;
        }
        #endregion

        #region 承运
        [ManyToOne(Insert = false, Update = false, NotNull = false, ForeignKey = "FK_拼箱箱_提箱地")]
        public virtual 人员 提箱地
        {
            get;
            set;
        }

        ///<summary>
        ///提箱地
        ///</summary>
        [Property(Column = "提箱地", Length = 6)]
        public virtual string 提箱地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_拼箱箱_装货地")]
        public virtual 人员 装货地
        {
            get;
            set;
        }

        ///<summary>
        ///装货地
        ///</summary>
        [Property(Column = "装货地", Length = 6)]
        public virtual string 装货地编号
        {
            get;
            set;
        }

        ///<summary>
        ///货代提箱时间要求止
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 货代提箱时间要求止
        {
            get;
            set;
        }

        ///<summary>
        ///提箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 提箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///拉箱时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 拉箱时间
        {
            get;
            set;
        }

        ///<summary>
        ///进港时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 进港时间
        {
            get;
            set;
        }

        ///<summary>
        ///装货时间
        ///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 装货时间
        {
            get;
            set;
        }
        #endregion

    }
}
