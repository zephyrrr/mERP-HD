using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Ba
{
    public enum 学历
    {
        高中 = 1,
        本科 = 2,
        大专 = 3,
        硕士 = 4,
        博士 = 5
    }

    public enum 政治面貌
    {
        团员 = 1,
        党员 = 2,
        其它党派 = 3,
        群众 = 4
    }

    public enum 婚姻状况
    {
        未婚 = 1,
        已婚 = 2
    }

    [Serializable]
    [Auditable]
    [JoinedSubclass(NameType = typeof(员工), ExtendsType = typeof(人员), Table = "参数备案_人员单位_员工")]
    [Key(Column = "编号", ForeignKey = "FK_人员_员工")]
    [Cache(Usage = CacheUsage.NonStrictReadWrite)]
    public class 员工 : 人员, IOperatingEntity
	{
        void IOperatingEntity.PreparingOperate(OperateArgs e)
        {
            if (e.OperateType == OperateType.Save)
            {
                if (string.IsNullOrEmpty(this.编号))
                {
                    // Todo
                    int delta = Feng.Utils.RepositoryHelper.GetRepositoryDelta(e.Repository, typeof(员工).Name);

                    this.编号 = PrimaryMaxIdGenerator.GetMaxId("参数备案_人员单位", "编号", 6, "20", delta).ToString();
                }
            }
        }
        void IOperatingEntity.PreparedOperate(OperateArgs e)
        {
        }

        ///<summary>
        ///性别
        ///</summary>
        [Property(NotNull = false)]
        public virtual 性别? 性别
        {
            get;
            set;
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Property(NotNull = false)]
        public virtual DateTime? 出生日期
        {
            get;
            set;
        }

        ///<summary>
        ///籍贯
        ///</summaty>
        [Property(Length = 50, NotNull = false)]
        public virtual string 籍贯
        {
            get;
            set;
        }

        /// <summary>
        /// 照片
        /// </summary>
        [Property(Length = 4096, NotNull = false)]
        public virtual byte[] 照片
        {
            get;
            set;
        }

        ///<summary>
        ///身份证号
        ///</summaty>
        [Property(Length = 18, NotNull = false)]
        public virtual string 身份证号
        {
            get;
            set;
        }

        ///<summary>
        ///毕业院校
        ///</summary>
        [Property(Length = 50)]
        public virtual string 毕业院校
        {
            get;
            set;
        }

        ///<summary>
        ///学历
        ///</summary>
        [Property(NotNull = false)]
        public virtual 学历? 学历
        {
            get;
            set;
        }

        ///<summary>
        ///专业
        ///</summary>
        [Property(NotNull = false)]
        public virtual string 专业
        {
            get;
            set;
        }

        ///<summary>
        ///政治面貌
        ///</summary>
        [Property( NotNull = false)]
        public virtual 政治面貌? 政治面貌
        {
            get;
            set;
        }

        ///<summary>
        ///婚姻状况
        ///</summary>
        [Property(NotNull = false)]
        public virtual 婚姻状况? 婚姻状况
        {
            get;
            set;
        }

        /// <summary>
        /// 加入公司日期
        /// </summary>
        [Property(NotNull = false)]
        public virtual DateTime? 加入公司日期
        {
            get;
            set;
        }

        ///<summary>
        ///特长爱好
        ///</summary>
        [Property(Length = 200)]
        public virtual string 特长爱好
        {
            get;
            set;
        }

        ///<summary>
        ///工作简历
        ///</summary>
        [Property(Length = 200)]
        public virtual string 工作简历
        {
            get;
            set;
        }
	}
}
