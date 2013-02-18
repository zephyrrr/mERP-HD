using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceMonitor
{
    /// <summary>
    /// 全局变量
    /// </summary>
    internal class Global
    {
        /// <summary>
        /// 默认创建的监视
        /// </summary>
        internal static Dictionary<string, string> DefaultMonitors = new Dictionary<string, string>()
        {
            // 监视名称，监视路径
            {"2版单证",@"\\192.168.0.10\2版电子文档\2版单证"},
            {"2版合同",@"\\192.168.0.10\2版电子文档\2版合同"},
            {"2版国内注册证书",@"\\192.168.0.10\2版电子文档\2版国内注册证书"},
            {"2版注册证书",@"\\192.168.0.10\2版电子文档\2版注册证书"},
            {"2版商检发票号",@"\\192.168.0.10\2版电子文档\2版商检发票号"},
            {"2版批文号",@"\\192.168.0.10\2版电子文档\2版批文号"}
        };

        /// <summary>
        /// 当前已创建的监视
        /// </summary>
        internal static Dictionary<string, string> CurrentMonitors = new Dictionary<string, string>();
    }
}
