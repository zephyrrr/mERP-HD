using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Feng.Utils;
using Feng.Net;

namespace Hd.NetRead
{
    /// <summary>
    /// nbeport上的内容读取
    /// </summary>
    public sealed class nbeportRead : WebProxy
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public nbeportRead()
        {
        }

        private string m_url = "http://60.190.1.176/nbedipub/search/new_search/search_list.asp"; //"http://www.nbeport.gov.cn/nbedi/search/new_search/search_list.asp";
        //private string m_loginUrl = "http://www.nbeport.gov.cn/pkmslogin.form";
        //private string m_loginDisplaceUrl = "http://www.nbeport.gov.cn/pkmsdisplace";

        private string m_报关单流转Url = "http://www.nbeport.gov.cn/nbedi/search/new_search/moni_search.asp";
        private string m_报关单流转Data = "moni_entry_id=#报关单编号#&button=%BF%AA%CA%BC%B2%E9%D1%AF";
        private Regex m_报关单流转regex = new Regex(@"<div align=""center"">(.*?)</div>", RegexOptions.Singleline);
        private Regex m_HtmlTagRegex = new Regex(@"<.*?>", RegexOptions.Singleline);

        private void Login()
        {
            //string ret = GetPostString(m_loginUrl, "login-form-type=pwd&username=" + m_userName + "&password=" + m_passWord);
            //if (ret.Contains("PKMS Administration: Session Displacement"))
            //{
            //    GetString(m_loginDisplaceUrl);
            //}
        }
        private string m_userName, m_passWord;
        /// <summary>
        /// 设置登录的用户名密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        public void SetLoginInfo(string userName, string passWord)
        {
            m_userName = userName;
            m_passWord = passWord;
        }

        private int m_maxResult = 200;
        /// <summary>
        /// MaxResult
        /// </summary>
        public int MaxResult
        {
            get { return m_maxResult; }
            set { m_maxResult = value; }
        }

        private const int m_resultPerPage = 20;

        private bool m_incompleteBoxData;
        /// <summary>
        /// 集装箱信息未读取完全
        /// </summary>
        public bool IncompleteBoxData
        {
            get { return m_incompleteBoxData; }
        }

        #region "Interface"
        /// <summary>
        /// 查询集装箱数据
        /// </summary>
        /// <param name="进出口标志"></param>
        /// <param name="提单号"></param>
        /// <returns></returns>
        public IList<集装箱数据> 查询集装箱数据(ImportExportType 进出口标志, string 提单号)
        {
            return Grab(进出口标志, null, null, null, null, null, null, null, 提单号);
        }

        public IList<集装箱数据> 查询集装箱数据(ImportExportType 进出口标志, string 提单号, string 航次)
        {
            return Grab(进出口标志, null, null, 航次, null, null, null, null, 提单号);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="进出口标志"></param>
        /// <param name="箱号"></param>
        /// <returns></returns>
        public IList<集装箱数据> 查询集装箱数据通过箱号(ImportExportType 进出口标志, string 箱号)
        {
            return Grab(进出口标志, null, null, null, 箱号, null, null, null, null);
        }

        /// <summary>
        /// 通用查询（）
        /// </summary>
        /// <returns></returns>
        public IList<集装箱数据> Grab(ImportExportType 进出口类型, string 船名, string 船舶英文名称, string 航次, string 集装箱号,
            DateTime? 进场时间起, DateTime? 进场时间止, string 船舶UN代码, string 提单号)
        {
            Login();

            if (进场时间起.HasValue)
            {
                进场时间起 = DateTimeHelper.GetDateTimeStartofDay(进场时间起.Value);
            }
            if (进场时间止.HasValue)
            {
                进场时间止 = DateTimeHelper.GetDateTimeEndofDay(进场时间止.Value);
            }

            List<集装箱数据> totalBoxData = new List<集装箱数据>();
            List<string> boxStringData = new List<string>();

            string pageSource, boxSource;

            // 第一页
            string firstPostData = CreatePostData(进出口类型, 船名, 船舶英文名称, 航次, 集装箱号, 进场时间起, 进场时间止, 船舶UN代码, 提单号);
            string webContent = PostToString(m_url, firstPostData);

            nbeport.PageAnalyze.SplitWebContent(webContent, out pageSource, out boxSource);

            if (pageSource == null || boxSource == null)
            {
                return totalBoxData;
            }

            int totalPages = nbeport.PageAnalyze.GetPageCount(pageSource);
            int totalBoxs = nbeport.PageAnalyze.GetBoxCount(pageSource);
            boxStringData.AddRange(nbeport.PageAnalyze.GetBoxData(boxSource));

            int needPage = (m_maxResult - 1) / m_resultPerPage + 1;
            if (totalPages > needPage)
            {
                m_incompleteBoxData = true;
                totalPages = needPage;
            }

            for (int page = 1; page < totalPages; page++)
            {
                System.Threading.Thread.Sleep(1000);

                string postData = CreateGotoPostData(page, firstPostData);

                webContent = PostToString(m_url, postData);

                nbeport.PageAnalyze.SplitWebContent(webContent, out pageSource, out boxSource);

                boxStringData.AddRange(nbeport.PageAnalyze.GetBoxData(boxSource));
            }

            if (boxStringData.Count % 9 != 0)
            {
                throw new WebFormatChangedException("invalid box data count");
            }

            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("zh-CN");
            for (int i = 0; i < boxStringData.Count / 9; ++i)
            {
                DateTime jcsj = DateTime.ParseExact(boxStringData[9 * i + 5], "yyyy年MM月dd日HH点mm分ss秒", culture);

                if (!string.IsNullOrEmpty(船名) && 船名 != boxStringData[9 * i].Trim())
                    continue;
                if (!string.IsNullOrEmpty(船舶英文名称) && 船舶英文名称 != boxStringData[9 * i + 1].Trim())
                    continue;
                if (!string.IsNullOrEmpty(航次) && 航次 != boxStringData[9 * i + 2].Trim())
                    continue;
                if (!string.IsNullOrEmpty(集装箱号) && 集装箱号 != boxStringData[9 * i + 3].Trim())
                    continue;
                if (!string.IsNullOrEmpty(船舶UN代码) && 船舶UN代码 != boxStringData[9 * i + 4].Trim())
                    continue;
                if (进场时间起 != null && jcsj < 进场时间起.Value)
                    continue;
                if (进场时间止 != null && jcsj > 进场时间止.Value)
                    continue;
                if (!string.IsNullOrEmpty(提单号) && 提单号 != boxStringData[9 * i + 7].Trim())
                    continue;

                totalBoxData.Add(new 集装箱数据(boxStringData[9 * i].Trim(),
                    boxStringData[9 * i + 1].Trim(),
                    boxStringData[9 * i + 2].Trim(),
                    boxStringData[9 * i + 3].Trim(),
                    boxStringData[9 * i + 4].Trim(),
                    jcsj,
                    null,
                    boxStringData[9 * i + 6].Trim(),
                    boxStringData[9 * i + 7].Trim(),
                    boxStringData[9 * i + 8].Trim(),
                    null));
            }
            return totalBoxData;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="进出口标志"></param>
        ///// <param name="tdh"></param>
        ///// <returns></returns>
        //private string GetBoxUrlByTdh(ImportExportType 进出口标志, string tdh)
        //{
        //    return m_url + "?" + CreatePostData(进出口标志, null, null, null, null, null, null, null, tdh);
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="进出口标志"></param>
        ///// <param name="xh"></param>
        ///// <returns></returns>
        //private string GetBoxUrlByXh(ImportExportType 进出口标志, string xh)
        //{
        //    return m_url + "?" + CreatePostData(进出口标志, null, null, null, xh, null, null, null, null);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="报关单编号"></param>
        /// <returns></returns>
        public IList<流转状态数据> 查询流转状态数据(string 报关单编号)
        {
            IList<流转状态数据> ret = new List<流转状态数据>();

            try
            {
                string postData = m_报关单流转Data.Replace("#报关单编号#", 报关单编号);
                string htmlInfo = base.PostToString(m_报关单流转Url, postData);
                if (htmlInfo.Contains("/portalframework/um/Login.do"))
                {
                    Login();
                    htmlInfo = base.PostToString(m_报关单流转Url, postData);
                }
                MatchCollection mc = m_报关单流转regex.Matches(htmlInfo);
                if (mc.Count % 5 != 0)
                {
                    throw new WebFormatChangedException("nbeport html format is changed");
                }

                for (int i = 1; i < mc.Count / 5; ++i)
                {
                    ret.Add(new 流转状态数据(WebProxy.RemoveSpaces(mc[i * 5 + 2].Groups[1].Value),
                        Convert.ToDateTime(WebProxy.RemoveSpaces(mc[i * 5 + 4].Groups[1].Value))));
                    //s = base.RemoveSpaces(s);
                }
            }
            catch (Exception)
            {
                //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
            }
            return ret;
        }

        #endregion



        /// <summary>
        /// 生成初次进入交互数据子项
        /// </summary>
        /// <returns></returns>
        private string CreatePostData(ImportExportType 进出口类型, string 船名, string 船舶英文名称, string 航次, string 集装箱号,
            DateTime? 进场时间起, DateTime? 进场时间止, string 船舶UN代码, string 提单号)
        {
            StringBuilder postData = new StringBuilder();

            if (!string.IsNullOrEmpty(提单号))
            {
                postData.Append("BILL_NO=" + 提单号);
            }

            if (!string.IsNullOrEmpty(船名))
            {
                postData.Append("&SHIP_NAME=" + 船名);
            }

            if (!string.IsNullOrEmpty(船舶英文名称))
            {
                postData.Append("&SHIP_NAME_EN=" + 船舶英文名称);
            }

            if (!string.IsNullOrEmpty(航次))
            {
                postData.Append("&FLIGHT_NO=" + 航次);
            }

            if (!string.IsNullOrEmpty(集装箱号))
            {
                postData.Append("&JZX_NO=" + 集装箱号);
            }

            if (进场时间起.HasValue)
            {
                postData.Append("&JC_DATE_B=" + 进场时间起.Value.ToString("yyyy-MM-dd"));
            }

            if (进场时间止.HasValue)
            {
                postData.Append("&JC_DATE_E=" + 进场时间止.Value.ToString("yyyy-MM-dd"));
            }

            if (!string.IsNullOrEmpty(船舶UN代码))
            {
                postData.Append("&SHIP_UN_NO=" + 船舶UN代码);
            }

            postData.Append("&button=%BF%AA%CA%BC%B2%E9%D1%AF");

            switch (进出口类型)
            {
                case ImportExportType.出口集装箱:
                    postData.Append("&IE_FLAG=" + "E");
                    break;
                case ImportExportType.进口集装箱:
                    postData.Append("&IE_FLAG=" + "I");
                    break;
                default:
                    throw new NotSupportedException("Invalid ImportExport Type");
            }

            string s = postData.ToString();
            if (s[0] == '&')
            {
                s = s.Remove(0, 1);
            }
            return s;
        }

        /// <summary>
        /// 与网站交互数据实现翻页功能
        /// </summary>
        /// <param name="intcur">当前页数</param>
        /// <param name="postdata">交互的原数据</param>
        /// <returns>返回翻页功能交互数据</returns>
        private string CreateGotoPostData(int intcur, string postdata)
        {
            string GotoPostData = postdata;
            int intCur = intcur;
            int nextCur = intcur + 1;
            GotoPostData += "&intCur=" + intCur + "&goto=" + nextCur + "&sbtpn=GO";
            return GotoPostData;
        }
    }
}
