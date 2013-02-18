using System;
using System.Collections.Generic;
using System.Text;

namespace Hd.NetRead
{
	/// <summary>
	/// 报关单数据
    /// 报关单号 报关单长号 船名航次 箱号 申报日期 经营单位 提运单号 批准文号 页数
	/// </summary>
	public class 报关单数据
	{
		/// <summary>
		/// Constructor
		/// </summary>
        /// <param name="报关单号"></param>
        /// <param name="报关单长号"></param>
        /// <param name="船名航次"></param>
        /// <param name="箱号"></param>
        /// <param name="申报日期"></param>
        /// <param name="经营单位"></param>
        /// <param name="提运单号"></param>
        /// <param name="批准文号"></param>
        /// <param name="标箱量"></param>
        /// <param name="箱量"></param>
        /// <param name="网页快照"></param>
        /// <param name="页数"></param>
        /// <param name="通关单号"></param>
        public 报关单数据(string 报关单号, string 报关单长号, string 船名航次, string 箱号, DateTime? 申报日期, string 经营单位,
            string 提运单号, string 批准文号, int 箱量, int 标箱量, string 网页快照, int 页数, string 通关单号, string 报关员, string 报关公司)
		{
            m_报关单号 = 报关单号;
            m_报关单长号 = 报关单长号;
            m_船名航次 = 船名航次;
            m_箱号 = 箱号;
            m_申报日期 = 申报日期;
            m_经营单位 = 经营单位;
            m_提运单号 = 提运单号;
            m_批准文号 = 批准文号;
            m_箱量 = 箱量;
            m_标箱量 = 标箱量;
            m_网页快照 = 网页快照;
            m_页数 = 页数;
            m_通关单号 = 通关单号;
            m_报关员 = 报关员;
            m_报关公司 = 报关公司;
		}

		private string m_报关单号;
		/// <summary>
        /// 报关单号
		/// </summary>
        public string 报关单号
		{
            get { return m_报关单号; }
		}

        private string m_报关单长号;
		/// <summary>
        /// 报关单长号
		/// </summary>
        public string 报关单长号
		{
            get { return m_报关单长号; }
		}

		private string m_船名航次;
		/// <summary>
        /// 　船名航次
		/// </summary>
        public string 船名航次
		{
            get { return m_船名航次; }
		}

		private string m_箱号;
		/// <summary>
		/// 　箱号
		/// </summary>
		///
		public string 箱号
		{
			get { return m_箱号; }
		}

		private string m_经营单位;
		/// <summary>
        /// 　经营单位
		/// </summary>
		///
        public string 经营单位
		{
            get { return m_经营单位; }
		}

        private DateTime? m_申报日期;
		/// <summary>
		/// 申报日期
		/// </summary>
		///
        public DateTime? 申报日期
		{
            get { return m_申报日期; }
		}

		private string m_提运单号;
		/// <summary>
        /// 提运单号
		/// </summary>
        public string 提运单号
		{
            get { return m_提运单号; }
		}

		private string m_批准文号;
		/// <summary>
        /// 批准文号
		/// </summary>
        public string 批准文号
		{
            get { return m_批准文号; }
		}

        private int m_标箱量;
        /// <summary>
        /// 标箱量
        /// </summary>
        public int 标箱量
        {
            get { return m_标箱量; }
        }

        private int m_箱量;
        /// <summary>
        /// 箱量
        /// </summary>
        public int 箱量
        {
            get { return m_箱量; }
        }

		private string m_网页快照;
		/// <summary>
        /// 网页快照
		/// </summary>
        public string 网页快照
		{
            get { return m_网页快照; }
            set { m_网页快照 = value; }
		}

        private int m_页数;
        /// <summary>
        /// 页数
        /// </summary>
        public int 页数
        {
            get { return m_页数; }
            set { m_页数 = value; }
        }

        private string m_通关单号;
        /// <summary>
        /// 通关单号
        /// </summary>
        public string 通关单号
        {
            get { return m_通关单号; }
        }

        private string m_报关员;
        /// <summary>
        /// 报关员
        /// </summary>
        public string 报关员
        {
            get { return m_报关员; }
        }

        private string m_报关公司;
        /// <summary>
        /// 报关公司
        /// </summary>
        public string 报关公司
        {
            get { return m_报关公司; }
        }
	}
}
