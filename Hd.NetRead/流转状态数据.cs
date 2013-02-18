using System;
using System.Collections.Generic;
using System.Text;

namespace Hd.NetRead
{
    /// <summary>
    /// 流转状态数据
    /// </summary>
    public class 流转状态数据
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="状态"></param>
        /// <param name="时间"></param>
        public 流转状态数据(string 状态, DateTime 时间)
        {
            this.状态 = 状态;
            this.时间 = 时间;
        }

        /// <summary>
        /// 
        /// </summary>
        public string 状态 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime 时间 { get; set; }
    }
}
