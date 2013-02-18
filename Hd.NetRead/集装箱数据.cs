using System;
using System.Collections.Generic;
using System.Text;

namespace Hd.NetRead
{
    /// <summary>
    /// 
    /// </summary>
	public enum ImportExportType
	{
        /// <summary>
        /// 
        /// </summary>
		出口集装箱 = 1,
        /// <summary>
        /// 
        /// </summary>
		进口集装箱 = 0,
	}

	/// <summary>
	/// 集装箱数据
	/// 船名 船舶英文名称 航次 集装箱号 船舶UN代码 进场时间 堆场位置 提单号 堆场区 
	///	CSCCQ XIN CHONG QING  0103E CCLU3009908  UN9262118 2007年11月25日 03点01分33秒 L10732  SHHNBMTR3AH881   北仑三期 
	/// </summary>
	public class 集装箱数据
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="船名"></param>
		/// <param name="船舶英文名称"></param>
		/// <param name="航次"></param>
		/// <param name="集装箱号"></param>
		/// <param name="船舶UN代码"></param>
		/// <param name="进场时间"></param>
		/// <param name="堆场位置"></param>
		/// <param name="提单号"></param>
		/// <param name="堆场区"></param>
		/// <param name="箱型"></param>
		public 集装箱数据(string 船名, string 船舶英文名称, string 航次, string 集装箱号, string 船舶UN代码,
			DateTime 进场时间, DateTime? 提箱时间, string 堆场位置, string 提单号, string 堆场区, string 箱型)
		{
			m_船名 = 船名;
			m_船舶英文名称 = 船舶英文名称;
			m_航次 = 航次;
			m_集装箱号 = 集装箱号;
			m_船舶UN代码 = 船舶UN代码;
			m_Real进场时间 = 进场时间;
            m_Real提箱时间 = 提箱时间;
			m_堆场位置 = 堆场位置;
			m_提单号 = 提单号;
			m_堆场区 = 堆场区;
			m_箱型 = 箱型;
		}

		private string m_船名;
		/// <summary>
		/// 船名
		/// </summary>
		public string 船名
		{
			get { return m_船名; }
		}

		private string m_船舶英文名称;
		/// <summary>
		/// 船舶英文名称
		/// </summary>
		public string 船舶英文名称
		{
			get { return m_船舶英文名称; }
		}

		private string m_航次;
		/// <summary>
		/// 　航次
		/// </summary>
		///
		public string 航次
		{
			get { return m_航次; }
		}

		private string m_集装箱号;
		/// <summary>
		/// 　集装箱号
		/// </summary>
		///
		public string 集装箱号
		{
			get { return m_集装箱号; }
		}

		private string m_船舶UN代码;
		/// <summary>
		/// 　船舶UN代码
		/// </summary>
		///
		public string 船舶UN代码
		{
			get { return m_船舶UN代码; }
		}

		/// <summary>
		/// 实际进场时间的年月日
		/// </summary>
		///
		public DateTime 进场时间
		{
			get { return new DateTime(m_Real进场时间.Year, m_Real进场时间.Month, m_Real进场时间.Day); }
		}

        public DateTime? 提箱时间
        {
            get { return new DateTime(m_Real提箱时间.Value.Year, m_Real提箱时间.Value.Month, m_Real提箱时间.Value.Day); }
        }

		private DateTime m_Real进场时间;
		/// <summary>
		/// 进场时间
		/// 输入格式：yyyy-MM-dd； 输出格式：2007年11月25日 03点01分33秒）
		/// </summary>
		///
		public DateTime Real进场时间
		{
			get { return m_Real进场时间; }
		}

        private DateTime? m_Real提箱时间;

        public DateTime? Real提箱时间
        {
            get { return m_Real提箱时间; }
            set { m_Real提箱时间 = value; }
        }

		private string m_堆场位置;
		/// <summary>
		/// 堆场位置
		/// </summary>
		public string 堆场位置
		{
			get { return m_堆场位置; }
		}

		private string m_提单号;
		/// <summary>
		/// 提单号
		/// </summary>
		public string 提单号
		{
			get { return m_提单号; }
		}

		private string m_堆场区;
		/// <summary>
		/// 堆场区
		/// </summary>
		public string 堆场区
		{
			get { return m_堆场区; }
            set { m_堆场区 = value; }
		}

		private string m_箱型;
		/// <summary>
		/// 箱型
		/// </summary>
		public string 箱型
		{
			get { return m_箱型; }
            set { m_箱型 = value; }
		}
	}
}
