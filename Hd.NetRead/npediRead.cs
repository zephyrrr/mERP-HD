using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Globalization;
using System.Drawing;

namespace Hd.NetRead
{
    /// <summary>
    /// 在http://www.npedi.com上的查询
    /// </summary>
    public class npediRead : Feng.Net.WebProxy
    {
        private string Login()
        {
            string jspcookie;
            string htmlInfo;

            htmlInfo = npedi.HtmlInfoBase.GetHtmlInfo(npedi.HtmlInfoBase.LoginUrl, out jspcookie);
            jspcookie = jspcookie.Split(';')[0];// 获得cookie

            string header = "";
            byte[] b = { };
            Bitmap img = new Bitmap(
            npedi.HtmlInfoBase.GetStreamByBytes(npedi.HtmlInfoBase.Host, npedi.HtmlInfoBase.BmpUrl, b, jspcookie, out header));//获得验证码图片


            npedi.unCodeNbYang UnCheckobj = new npedi.unCodeNbYang(img);
            string strNum = UnCheckobj.getPicnum();     //识别图片,取得数字信息


            string postString = npedi.HtmlInfoBase.LoginPostData(null, null, strNum);// 组成Post数据

            string returnHtml = npedi.HtmlInfoBase.GetHtml(npedi.HtmlInfoBase.LoginUrl, postString, jspcookie, out header); // 登入

            return jspcookie;
        }
        #region "集装箱进门查询"
        private static string m_集装箱进门查询Url = "http://www.npedi.com/edi/scodecoIngateAction.do?pageIndex=1";
        /// <summary>
        /// 根据“集装箱进门查询”
        /// </summary>
        /// <param name="提单号">提单号</param>
        /// <returns>集装箱进门查询结果列表</returns>
        /// <exception cref="Feng.WebFormatChangedException">网页格式发生改变时抛出</exception>
        public IList<集装箱进门查询结果> 集装箱进门查询(string 提单号)
        {
            if (string.IsNullOrEmpty(提单号))
            {
                throw new ArgumentException("提单号");
            }

            string jspcookie = Login();

            IList<集装箱进门查询结果> ret =
                集装箱进门查询(提单号.Trim(), null, jspcookie);// 返回查询结果集

            return ret;
        }

        private IList<集装箱进门查询结果> 集装箱进门查询(string 提单号, string 集装箱号, string jspcookie)
        {
            List<集装箱进门查询结果> ret = new List<集装箱进门查询结果>();
            string postStr = npedi.HtmlInfoBase.SearchPostDataByblNo(提单号);
            string header;
            string returnHtml2 = npedi.HtmlInfoBase.GetHtml(m_集装箱进门查询Url, postStr, jspcookie, out header);
            string buf = returnHtml2.Replace(Environment.NewLine, "");

            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("zh-CN"); // Add by yl 2009/10/14

            int rowCellCnt = 13;
            IList<string> list = npedi.EdiPageResolve.FetchBoxData(buf, rowCellCnt);

            for (int i = 0; i < (list.Count / rowCellCnt); i++)
            {
                DateTime? receiveTime = null, import = null; //  信息收到时间  进门时间
                DateTime d;
                if (DateTime.TryParseExact(list[(rowCellCnt * i) + 1], "yyyy-MM-dd HH:mm:ss", cultureInfo, DateTimeStyles.None, out d))
                {
                    receiveTime = d;
                }
                if (DateTime.TryParseExact(list[(rowCellCnt * i) + 2], "yyyy-MM-dd HH:mm", cultureInfo, DateTimeStyles.None, out d))
                {
                    import = d;
                }

                ret.Add(new 集装箱进门查询结果(receiveTime, import, list[(rowCellCnt * i) + 3].Trim(),
                        list[(rowCellCnt * i) + 4].Trim(), list[(rowCellCnt * i) + 5].Trim(), list[(rowCellCnt * i) + 6].Trim(),
                        list[(rowCellCnt * i) + 7].Trim(), list[(rowCellCnt * i) + 8].Trim(), list[(rowCellCnt * i) + 9].Trim(),
                        list[(rowCellCnt * i) + 10].Trim(), list[(rowCellCnt * i) + 11].Trim(), list[(rowCellCnt * i) + 12].Trim()));
            }
            return ret;
        }
        #endregion

        #region "集装箱出门查询"
        private static string m_集装箱出门查询Url = "http://www.npedi.com/edi/scodecoOutgateAction.do?pageIndex=1";
        /// <summary>
        /// 根据“集装箱出门查询”
        /// </summary>
        /// <param name="提单号">提单号</param>
        /// <returns>集装箱出门查询结果列表</returns>
        /// <exception cref="Feng.WebFormatChangedException">网页格式发生改变时抛出</exception>
        public IList<集装箱出门查询结果> 集装箱出门查询(string 提单号)
        {
            if (string.IsNullOrEmpty(提单号))
            {
                throw new ArgumentException("提单号");
            }
            string jspcookie = Login();

            IList<集装箱出门查询结果> ret =
                集装箱出门查询(提单号.Trim(), null, jspcookie);// 返回查询结果集

            return ret;
        }

        private IList<集装箱出门查询结果> 集装箱出门查询(string 提单号, string 集装箱号, string jspcookie)
        {
            List<集装箱出门查询结果> ret = new List<集装箱出门查询结果>();
            string postStr = npedi.HtmlInfoBase.SearchPostDataByblNo(提单号);
            string header;
            string returnHtml2 = npedi.HtmlInfoBase.GetHtml(m_集装箱出门查询Url, postStr, jspcookie, out header);
            string buf = returnHtml2.Replace(Environment.NewLine, "");
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("zh-CN"); // Add by yl 2009/10/14

            int rowCellCnt = 11;
            IList<string> list = npedi.EdiPageResolve.FetchBoxData(buf, rowCellCnt);

            for (int i = 0; i < (list.Count / rowCellCnt); i++)
            {
                DateTime? outTime = null; //  出门时间
                DateTime d;
                if (DateTime.TryParseExact(list[(rowCellCnt * i) + 1], "yyyyMMddHHmm", cultureInfo, DateTimeStyles.None, out d))
                {
                    outTime = d;
                }

                ret.Add(new 集装箱出门查询结果(outTime, list[(rowCellCnt * i) + 2].Trim(), list[(rowCellCnt * i) + 3].Trim(),
                        list[(rowCellCnt * i) + 4].Trim(), list[(rowCellCnt * i) + 5].Trim(), list[(rowCellCnt * i) + 6].Trim(),
                        list[(rowCellCnt * i) + 7].Trim(), list[(rowCellCnt * i) + 8].Trim(), list[(rowCellCnt * i) + 9].Trim(),
                        list[(rowCellCnt * i) + 10].Trim()));
            }
            return ret;
        }
        #endregion

        #region "集装箱进口综合查询"
        private static string m_集装箱进口综合查询url = "http://www.npedi.com/edi/q_CtnqueryImportAction.do";
        /// <summary>
        /// 通用查询（）
        /// </summary>
        /// <returns></returns>
        public IList<集装箱数据> 集装箱进口综合查询(ImportExportType 进出口类型, string 集装箱号)
        {
            string jspcookie = Login();
            string header;

            List<集装箱数据> totalBoxData = new List<集装箱数据>();
            List<string> boxStringData = new List<string>();

            // 第一页
            string firstPostData = CreatePostData(进出口类型, 集装箱号);
            //string webContent = GetToString(m_集装箱进口综合查询url + "?" + firstPostData);
            string webContent = npedi.HtmlInfoBase.GetHtml(m_集装箱进口综合查询url, firstPostData, jspcookie, out header);

            boxStringData.AddRange(npedi.PageAnalyze.GetBoxData(webContent));
            // NO. 船舶UN 航次 箱号 箱型 卸船时间 出门时间 铅封号 箱主 集卡 
            // 1 UN9253014  018E  TGHU2702678  22GP  20071123010954  20071130210800  EMCWR32854  EMC  ZJL-A1378  
            for (int i = 0; i < boxStringData.Count / 11; ++i)
            {
                DateTime jcsj = DateTime.ParseExact(boxStringData[9 * i + 5], "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                DateTime txsj = DateTime.ParseExact(boxStringData[9 * i + 6], "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);

                if (!string.IsNullOrEmpty(集装箱号) && 集装箱号 != boxStringData[9 * i + 3].Trim())
                    continue;

                totalBoxData.Add(new 集装箱数据(null,
                    null,
                    boxStringData[9 * i + 2].Trim(),
                    boxStringData[9 * i + 3].Trim(),
                    boxStringData[9 * i + 1].Trim(),
                    jcsj,
                    txsj,
                    null,
                    null,
                    null,
                    boxStringData[9 * i + 4].Trim()));
            }
            return totalBoxData;
        }

        /// <summary>
        /// 生成初次进入交互数据子项
        /// </summary>
        /// <returns></returns>
        private string CreatePostData(ImportExportType 进出口类型, string 集装箱号)
        {
            // q_vessel=UN8100052%2F0303N&querychoice=bycontainerno&q_containerno=TGHU2702678
            StringBuilder postData = new StringBuilder();
            postData.Append("q_vessel=UN8100052%2F0303N&querychoice=bycontainerno");

            if (!string.IsNullOrEmpty(集装箱号))
            {
                postData.Append("&q_containerno=" + 集装箱号);
            }

            string s = postData.ToString();
            return s;
        }
        #endregion

        #region "查询海关查验结果、放行时间"
        private string m_查询海关查验结果url = "http://www.npedi.com/edi/cusmovAction.do?pageIndex=1";
        private string m_PostData海关查验结果 = "ctnNo=#箱号#&s_VelInfo=&companyCode=&beginOrderSendDate=&endOrderSendDate=";
        public 海关查验查询结果 查询海关查验结果(string 箱号)
        {
            //NO. 船名 航次 箱号 码头 箱型尺寸 移箱类型 H986 目的场站 指令时间 移箱标志 处理时间 反馈信息 
            //1 MAERSKALGOL  1008  MSKU2990124  北二集司(三期)  22  查验  Y   2010-08-16 13:51:05  完成归位  2010-08-16 21:30:31  

            string jspcookie = Login();
            string header;

            string PostData = m_PostData海关查验结果.Replace("#箱号#", 箱号);
            IList<string> boxStringData = npedi.PageAnalyze.GetBoxData海关查验结果(npedi.HtmlInfoBase.GetHtml(m_查询海关查验结果url, PostData, jspcookie, out header));

            if (boxStringData.Count == 0)
            {
                return null;
            }
            else
            {
                // 去除第一列No.
                return new 海关查验查询结果(boxStringData[1].Trim(),
                    boxStringData[2].Trim(), boxStringData[3].Trim(),
                    boxStringData[4].Trim(), boxStringData[5].Trim(),
                    boxStringData[6].Trim(), boxStringData[7].Trim(),
                    boxStringData[8].Trim(), DateTime.Parse(boxStringData[9].Trim()),
                    boxStringData[10].Trim(), DateTime.Parse(boxStringData[11].Trim()),
                    boxStringData[12].Trim());
            }
        }

        private string m_查询商检查验结果url = "http://www.npedi.com/edi/containerSearchAction.do";
        private string m_PostData商检查验结果 = "q_containerno=#箱号#";
        public bool? 查询商检查验结果(string 箱号)
        {
            string jspcookie = Login();
            string header;

            string PostData = m_PostData商检查验结果.Replace("#箱号#", 箱号);
            string htmlInfo = npedi.HtmlInfoBase.GetHtml(m_查询商检查验结果url, PostData, jspcookie, out header);

            if (htmlInfo.Contains("移箱"))// 两种 “移箱指令”“移箱到位”
            {
                return true;
            }
            else if (!htmlInfo.Contains("移箱") && htmlInfo.Contains("放行成功"))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

        private string m_海关放行时间url = "http://www.npedi.com/edi/passingInfoAction.do?pageIndex=1";
        private string m_PostData箱号 = "passno=&options=ctnno&ctnno=#箱号#&s_VelInfo=UN9224532%2F048E";
        private string m_PostData报关单号 = "options=passno&passno=#报关单号#&ctnno=&s_VelInfo=UN9224532%2F048E";
        public DateTime? 查询海关放行时间(string 箱号, string 提单号)   // 进口
        {
            string postData = m_PostData箱号.Replace("#箱号#", 箱号);
            string htmlInfo = new Feng.Net.WebProxy().PostToString(m_海关放行时间url, postData);

            DateTime? fxsj = null;
            List<List<string>> datas = npedi.PageAnalyze.查询海关放行时间(htmlInfo);
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][0] == 提单号)
                {
                    fxsj = DateTime.Parse(datas[i][1]);
                    break;
                }
            }
            return fxsj;
        }

        public DateTime? 查询海关放行时间(string 报关单号)  // 出口
        {
            string postData = m_PostData报关单号.Replace("#报关单号#", 报关单号);
            string htmlInfo = new Feng.Net.WebProxy().PostToString(m_海关放行时间url, postData);

            DateTime? fxsj = null;
            List<List<string>> datas = npedi.PageAnalyze.查询海关放行时间(htmlInfo);
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i][2] == 报关单号)
                {
                    fxsj = DateTime.Parse(datas[i][1]);
                    break;
                }
            }
            return fxsj;
        }
        #endregion

        #region "进口卸箱查询"
        private static string m_进口卸箱查询Url = "http://www.npedi.com/edi/scoarriDischargeAction.do?pageIndex=1";
        public IList<进口卸箱查询结果> 进口卸箱查询(string 提单号)
        {
            if (string.IsNullOrEmpty(提单号))
            {
                throw new ArgumentException("提单号");
            }
            string jspcookie = Login();

            IList<进口卸箱查询结果> ret = 进口卸箱查询(提单号.Trim(), null, jspcookie);

            return ret;
        }

        private IList<进口卸箱查询结果> 进口卸箱查询(string 提单号, string 集装箱号, string jspcookie)
        {
            List<进口卸箱查询结果> ret = new List<进口卸箱查询结果>();
            string postStr = npedi.HtmlInfoBase.SearchPostDataByblNo(提单号);
            string header;
            string returnHtml2 = npedi.HtmlInfoBase.GetHtml(m_进口卸箱查询Url, postStr, jspcookie, out header);
            string buf = returnHtml2.Replace(Environment.NewLine, "");
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("zh-CN");

            int rowCellCnt = 11;
            IList<string> list = npedi.EdiPageResolve.FetchBoxData(buf, rowCellCnt);

            for (int i = 0; i < (list.Count / rowCellCnt); i++)
            {
                DateTime? outTime = null; // 卸船时间
                DateTime d;
                if (DateTime.TryParseExact(list[(rowCellCnt * i) + 4], "yyyy-MM-dd HH:mm", cultureInfo, DateTimeStyles.None, out d))
                {
                    outTime = d;
                }

                ret.Add(new 进口卸箱查询结果(list[(rowCellCnt * i) + 1].Trim(), list[(rowCellCnt * i) + 2].Trim(), list[(rowCellCnt * i) + 3].Trim(), outTime, list[(rowCellCnt * i) + 5].Trim(), list[(rowCellCnt * i) + 6].Trim(),
                        list[(rowCellCnt * i) + 7].Trim(), list[(rowCellCnt * i) + 8].Trim(), list[(rowCellCnt * i) + 9].Trim(),
                        list[(rowCellCnt * i) + 10].Trim()));
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// 获取Edi网页源码(为了Python避免调用带out参数的函数)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public string GetHtmlInfo(string url, string postData)
        {
            string jspcookie = Login();
            string header;
            return npedi.HtmlInfoBase.GetHtml(url, postData, jspcookie, out header);
        }
    }
}
