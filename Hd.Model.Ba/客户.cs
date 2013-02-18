using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using NHibernate.Mapping.Attributes;
using Feng;
using Feng.Data;

namespace Hd.Model.Ba
{
	public enum 信誉情况
	{
		很好 = 1,
		良好 = 2,
		一般 = 3,
		差 = 4,
		极差 = 5
	}

	public enum 性别
	{
		男 = 1,
		女 = 2
	}

    [Serializable]
    [Auditable]
    [JoinedSubclass(NameType = typeof(客户), ExtendsType = typeof(人员), Table = "参数备案_人员单位_客户")]
    [Key(Column = "编号", ForeignKey = "FK_人员_客户")]
    [Cache(Usage = CacheUsage.NonStrictReadWrite)]
    public class 客户 : 人员, IOperatingEntity
    {
        void IOperatingEntity.PreparingOperate(OperateArgs e)
        {
            if (e.OperateType == OperateType.Save)
            {
                if (string.IsNullOrEmpty(this.编号))
                {
                    // Todo
                    int delta = Feng.Utils.RepositoryHelper.GetRepositoryDelta(e.Repository, typeof(客户).Name);

                    this.编号 = PrimaryMaxIdGenerator.GetMaxId("参数备案_人员单位", "编号", 6, "90", delta).ToString();
                }
            }
        }
        void IOperatingEntity.PreparedOperate(OperateArgs e)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public 客户()
        {
            信誉情况 = 信誉情况.一般;
        }

		///<summary>
		///首次交往
		///</summary>
        [Property(NotNull = false)]
        public virtual DateTime? 首次交往
        {
            get;
            set;
        }

		///<summary>
		///信誉情况
		///</summary>
        [Property(Column = "信誉情况", NotNull = true)]
		public virtual 信誉情况 信誉情况
		{
            get;
            set;
		}

		///<summary>
		///简况
		///</summary>
		[Property(Length = 200)]
		public virtual string 简况
		{
            get;
            set;
		}

		///<summary>
		///交往记录
		///</summary>
		[Property(Length = 200)]
		public virtual string 交往记录
		{
            get;
            set;
		}
    }
}
