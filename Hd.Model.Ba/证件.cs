using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;

namespace Hd.Model.Ba
{
    public enum 证件类型
    {
        公司 = 0,
        个人 = 1
    }

    [Serializable]
    [Auditable]
    [Class(NameType = typeof(证件), Table = "参数备案_证件", OptimisticLock = OptimisticLockMode.Version)]
    public class 证件 : BaseBOEntity
    {
        [Property(NotNull = true)]
        public virtual 证件类型 证件类型
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 50)]
        public virtual string 名称
        {
            get;
            set;
        }

        [Property(NotNull = false, Length = 200)]
        public virtual string 简介
        {
            get;
            set;
        }

        [Property(NotNull = true)]
        public virtual DateTime 发证日期
        {
            get;
            set;
        }

        [Property(NotNull = true, Length = 100)]
        public virtual string 发证单位
        {
            get;
            set;
        }

        /// <summary>
        /// 下次期限    如果早于当前时间，并且 IsActive = 'true'，提醒
        /// </summary>
        [Property(NotNull = true)]
        public virtual DateTime 下次期限
        {
            get;
            set;
        }

        /// <summary>
        /// 提醒天数  下次期限前这么多天开始提醒
        /// </summary>
        [Property(NotNull = true)]
        public virtual Int32 提醒天数
        {
            get;
            set;
        }

        [Property(NotNull = false)]
        public virtual DateTime? 上次日期
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
    }
}
