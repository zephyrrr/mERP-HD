using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ky
{
    // 跟出口一样
    [Serializable]
    [Auditable]
    [JoinedSubclass(Name = "空运票", ExtendsType = typeof(普通票), Table = "业务备案_空运票")]
    [Key(Column = "Id", ForeignKey = "FK_空运票_票")]
    public class 空运票 : 普通票, IMasterEntity<空运票, 空运箱>
    {
        #region IMasterEntity<空运票,空运箱> 成员

        public IList<空运箱> DetailEntities
        {
            get { return 箱; }
            set { 箱 = value; }
        }

        #endregion

        [Bag(0, Cascade = "none", Inverse = true)]
        [Key(1, Column = "票")]
        [OneToMany(2, ClassType = typeof(空运箱), NotFound = NotFoundMode.Ignore)]
        public virtual IList<空运箱> 箱
        {
            get;
            set;
        }

        [Property(Column = "起飞机场", Length = 6)]
        public virtual string 起飞机场编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_空运票_起飞机场")]
        public virtual 人员 起飞机场
        {
            get;
            set;
        }

        [Property(Column = "目的机场", Length = 6)]
        public virtual string 目的机场编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_空运票_目的机场")]
        public virtual 人员 目的机场
        {
            get;
            set;
        }

        [Property(Column = "提货地", Length = 6)]
        public virtual string 提货地编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_空运票_提货地")]
        public virtual 人员 提货地
        {
            get;
            set;
        }

        [Property(Column = "承运人", Length = 6)]
        public virtual string 承运人编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_空运票_承运人")]
        public virtual 人员 承运人
        {
            get;
            set;
        }

        [Property(Column = "收货人", Length = 6)]
        public virtual string 收货人编号
        {
            get;
            set;
        }

        [ManyToOne(Insert = false, Update = false, ForeignKey = "FK_空运票_收货人")]
        public virtual 人员 收货人
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime 起飞时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime 到达时间
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual string 特殊情况 // 是否危险品、是否急件
        {
            get;
            set;
        }
    }
}
