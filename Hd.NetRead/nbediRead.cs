using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Feng.Net;

namespace Hd.NetRead
{
    /// <summary>
    /// nbedi上的内容分析
    /// </summary>
    public class nbediRead : Feng.Net.WebProxy
    {
        private void Login()
        {
            string postData = m_loginData.Replace("#USERNAME#", m_userName).Replace("#PASSWORD#", m_passWord);
            string ret = PostToString(m_loginUrl, postData);
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

        private static string m_loginUrl = "http://www.nbedi.com/h2000eport/default.asp";
        private static string m_loginData = "uid=#USERNAME#&pwd=#PASSWORD#&session_uid=&session_co_id=&login=yes";
        private static string m_detailUrl = "http://www.nbedi.com/h2000eport/pre_bgd/bgd_print/normal_print.asp?pre_entry_id=#报关单编号#&sign=#页数#";
        // 不知为什么导致网页过期
        private static string m_detailUrl2 = "http://www.nbedi.com/h2000eport/pre_bgd/bgd_print/normal_print_new.asp?pre_entry_id=#报关单pre_entry_id#&soft_flag=&sign=#页数#";

        private static string m_searchUrl = "http://www.nbedi.com/h2000eport/pre_bgd/entry_main/default.asp";
        private static string m_searchData = "i_e_type=all&b_date=2008-01-01&e_date=2037-01-01&stats_code=all&pre_entry_id=#报关单短号#&Submit2=%B2%E9%D1%AF&intCur=1&goto=";
        private static string m_printPageUrl = "http://www.nbedi.com/h2000eport/pre_bgd/bgd_print/print_main.asp?pre_entry_id=#报关单编号#";
        private static Regex m_regexPrintPage = new Regex(@"<a href=""normal_print.asp\?pre_entry_id=(.*?)&sign=(.*?)""");
        private static Regex m_regexPrintPage2 = new Regex("pre_entry_id=(.*?)&soft_flag=&sign=(.*?)\"");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="报关单短号"></param>
        /// <returns></returns>
        public IList<string> 查询报关单编号(string 报关单短号)
        {
            string postData = m_searchData.Replace("#报关单短号#", 报关单短号);
            string htmlInfo = GetLoginedPostString(m_searchUrl, postData);

            return nbedi.PageAnalyze.Parse查询报关单编号(htmlInfo);
        }

        private string GetLoginedString(string url)
        {
            string htmlInfo = base.GetToString(url);
            if (htmlInfo.Contains("网页已过期，请重新登录")
                || htmlInfo.Contains(@"<input name=""Submit2"" type=""button"" class=""button"" value=""登 录"" onClick=""log_submit();"">"))
            {
                Login();
                htmlInfo = base.GetToString(url);
            }
            return htmlInfo;
        }

        private string GetLoginedPostString(string url, string postData)
        {
            string htmlInfo = base.PostToString(url, postData);
            if (htmlInfo.Contains("网页已过期，请重新登录")
                || htmlInfo.Contains(@"<input name=""Submit2"" type=""button"" class=""button"" value=""登 录"" onClick=""log_submit();"">"))
            {
                Login();
                htmlInfo = base.PostToString(url, postData);
            }
            return htmlInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string 网页快照分隔符 = System.Environment.NewLine + "网页快照" + System.Environment.NewLine;

        /// <summary>
        /// 通过网页快照查询报关单数据
        /// </summary>
        /// <param name="报关单编号"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public 报关单数据 查询报关单数据(string 报关单编号, string buffer)
        {
            if (string.IsNullOrEmpty(报关单编号) || string.IsNullOrEmpty(buffer))
                return null;

            报关单数据 bgd = null;

            string[] ss = buffer.Split(new string[] { nbediRead.网页快照分隔符 }, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length == 0)
                return null;

            bgd = nbedi.PageAnalyze.Parse查询报关单数据(ss[0], 报关单编号);

            return bgd;
        }

        /// <summary>
        /// 通过网络查询报关单数据
        /// </summary>
        /// <param name="报关单编号"></param>
        /// <returns></returns>
        public 报关单数据 查询报关单数据(string 报关单编号)
        {
            报关单数据 bgd = null;
            try
            {
                string url = m_printPageUrl.Replace("#报关单编号#", 报关单编号);
                string htmlInfo = GetLoginedString(url);
                MatchCollection mc = m_regexPrintPage.Matches(htmlInfo);
                if (mc.Count == 0)
                {
                    bgd = nbedi.PageAnalyze.Parse查询报关单数据(htmlInfo, 报关单编号);
                }
                else
                {
                    for (int i = 0; i < mc.Count; ++i)
                    {
                        url = m_detailUrl.Replace("#报关单编号#", mc[i].Groups[1].Value).Replace("#页数#", mc[i].Groups[2].Value);
                        htmlInfo = GetLoginedString(url);

                        if (i == 0)
                        {
                            bgd = nbedi.PageAnalyze.Parse查询报关单数据(htmlInfo, 报关单编号);
                        }
                        else
                        {
                            bgd.网页快照 += 网页快照分隔符 + htmlInfo;
                            bgd.页数++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
            }
            return bgd;
        }

        public 报关单数据 查询报关单数据2(string 报关单编号, string 报关单pre_entry_id)
        {
            报关单数据 bgd = null;
            try
            {
                string url = m_printPageUrl.Replace("#报关单编号#", 报关单pre_entry_id);
                string htmlInfo = GetLoginedString(url);
                MatchCollection mc = m_regexPrintPage2.Matches(htmlInfo);
                if (mc.Count == 0)
                {
                    bgd = nbedi.PageAnalyze.Parse查询报关单数据(htmlInfo, 报关单编号);
                }
                else
                {
                    for (int i = 0; i < mc.Count; ++i)
                    {
                        url = m_detailUrl2.Replace("#报关单pre_entry_id#", mc[i].Groups[1].Value).Replace("#页数#", mc[i].Groups[2].Value);
                        htmlInfo = GetLoginedString(url);

                        if (i == 0)
                        {
                            bgd = nbedi.PageAnalyze.Parse查询报关单数据(htmlInfo, 报关单编号);
                        }
                        else
                        {
                            bgd.网页快照 += 网页快照分隔符 + htmlInfo;
                            bgd.页数++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
            }
            return bgd;
        }


        /// <summary>
        /// 无论用报关单长号、短号，都可查询到报关单数据
        /// </summary>
        /// <param name="bgdh"></param>
        /// <returns></returns>
        public 报关单数据 长短号查询报关单数据(string 报关单编号)
        {
            if (报关单编号.Length < 10)   // 根据短号查报关单号
            {
                IList<string> bgdh_ch = 查询报关单编号(报关单编号);

                if (bgdh_ch != null && bgdh_ch.Count > 0)
                {
                    报关单编号 = bgdh_ch[0];
                }
            }

            return 查询报关单数据(报关单编号);
        }

        private static string m_jdjdData_first = "i_e_type=all&b_date=#查询时间始#&e_date=#查询时间末#&stats_code=#最新状态#&pre_entry_id=&Submit2=%B2%E9%D1%AF&intCur=1&goto=";
        private static string m_jdjdData_next = "i_e_type=all&b_date=#查询时间始#&e_date=#查询时间末#&stats_code=#最新状态#&pre_entry_id=&intCur=#当前页#&goto=#跳转页#&sbtpn=GO";
        /// <summary>
        /// 查询当天报关单号
        /// </summary>
        /// <returns>返回当天所有报关单号</returns>
        public List<string> 查询报关单号(DateTime dt1)
        {
            //all = 全部
            //7 = 该单已被电子口岸入库
            //Z = 报关单放行前删除或者异常处理
            //9 = 未知错误
            //Y = 电子口岸退单
            //P = 报关单放行
            //L = 成功入海关预录入库
            //B = 担保验放
            //G = 接单交单
            //F = 放行交单
            //E = 退单或入库失败
            //H = 挂起
            //M = 报关单重审
            //R = 结关
            //A = 报关单放行前删除或者异常处理
            //W = 无纸验放通知（审结）
            //I = 无纸放行通知（放行）
            //C = 无纸验放查验通知书（放行）
            //D = 报关单放行后删除
            //1 = 该单已保存、未申报
            //2 = 该单已修改、未申报
            //8 = 该单未申报被用户删除
            //3 = 该单已申报

            // 过滤，只读取最新状态 = 接单交单;,  模拟网页查询按钮
            string postData = m_jdjdData_first.Replace("#最新状态#", "G");
            // 查询时间范围为当天
            postData = postData.Replace("#查询时间始#", dt1.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            postData = postData.Replace("#查询时间末#", DateTime.Today.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            string htmlInfo = GetLoginedPostString(m_searchUrl, postData);

            // 获取总页数
            int pageCount = GetPageCount(htmlInfo);

            // 第一页
            List<string> bgdhList = nbedi.PageAnalyze.Parse查询所有报关单号(htmlInfo);

            // 翻页读取  模拟网页翻页GO按钮            
            for (int i = 1; i < pageCount; i++)
            {
                postData = m_jdjdData_next.Replace("#最新状态#", "G");
                postData = postData.Replace("#查询时间始#", dt1.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                postData = postData.Replace("#查询时间末#", DateTime.Today.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                postData = postData.Replace("#当前页#", i.ToString());
                postData = postData.Replace("#跳转页#", (i + 1).ToString());
                htmlInfo = GetLoginedPostString(m_searchUrl, postData);

                bgdhList.AddRange(nbedi.PageAnalyze.Parse查询所有报关单号(htmlInfo));
            }

            return bgdhList;
        }

        public Dictionary<string, string> 查询报关单号2(DateTime dt1)
        {
            string postData = m_jdjdData_first.Replace("#最新状态#", "G");
            postData = postData.Replace("#查询时间始#", dt1.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            postData = postData.Replace("#查询时间末#", DateTime.Today.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            string htmlInfo = GetLoginedPostString(m_searchUrl, postData);
            int pageCount = GetPageCount(htmlInfo);
            Dictionary<string, string> dic_bgdh = nbedi.PageAnalyze.Parse查询所有报关单号2(htmlInfo);
            for (int i = 1; i < pageCount; i++)
            {
                postData = m_jdjdData_next.Replace("#最新状态#", "G");
                postData = postData.Replace("#查询时间始#", dt1.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                postData = postData.Replace("#查询时间末#", DateTime.Today.ToString("yyyy-MM-d", System.Globalization.DateTimeFormatInfo.InvariantInfo));
                postData = postData.Replace("#当前页#", i.ToString());
                postData = postData.Replace("#跳转页#", (i + 1).ToString());
                htmlInfo = GetLoginedPostString(m_searchUrl, postData);

                foreach (KeyValuePair<string, string> item in nbedi.PageAnalyze.Parse查询所有报关单号2(htmlInfo))
                {
                    dic_bgdh.Add(item.Key, item.Value);
                }
            }
            return dic_bgdh;
        }

        private static Regex m_regexPage = new Regex("页数：([0-9]+)/([0-9]+)");
        /// <summary>
        /// 得到网页总页数
        /// </summary>
        /// <param name="pageSource"></param>
        /// <returns></returns>
        internal static int GetPageCount(string pageSource)
        {
            MatchCollection mc = m_regexPage.Matches(pageSource);
            if (mc.Count == 1)
            {
                return Convert.ToInt32(mc[0].Groups[2].Value);
            }
            else
            {
                throw new WebFormatChangedException("没有查询到数据或者网站的样式发送改变");
            }
        }


        private static Regex m_regexTotalBox = new Regex("共([0-9]+)条");
        /// <summary>
        /// 得到网页的总信息条数
        /// </summary>
        /// <param name="pageSource"></param>
        /// <returns></returns>
        internal static int GetBoxCount(string pageSource)
        {
            MatchCollection mx = m_regexTotalBox.Matches(pageSource);
            if (mx.Count == 1)
            {
                return Convert.ToInt32(mx[0].Groups[1].Value);
            }
            else
            {
                throw new WebFormatChangedException("PageSource format for BoxCount is changed");
            }
        }

        #region 运抵报告

        private string m_PostData运抵报告_first = "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=#VIEWSTATE#&__VIEWSTATEENCRYPTED=#VIEWSTATEENCRYPTED#&__EVENTVALIDATION=#EVENTVALIDATION#&txt_ContainerNO=&txt_BillNO=#提单号#&txt_ShipName=&txt_VoyageNo=&txt_StartDate=&txt_EndDate=&btn_Search=%E5%BC%80%E5%A7%8B%E6%9F%A5%E8%AF%A2&TextBox1=20&Pager1_input=1";
        private string m_PostData运抵报告_next = "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=#VIEWSTATE#&__VIEWSTATEENCRYPTED=#VIEWSTATEENCRYPTED#&__EVENTVALIDATION=#EVENTVALIDATION#&txt_ContainerNO=&txt_BillNO=#提单号#&txt_ShipName=&txt_VoyageNo=&txt_StartDate=&txt_EndDate=&TextBox1=20&Pager1_input=#当前页数#&Pager1=%E7%BF%BB%E9%A1%B5";
        private string m_运抵报告url = "http://arrival.nbedi.com/MsFlat/ArrivalReportnew.aspx";
        /// <summary>
        /// 查询英文船名、航次、箱号、箱型、进港时间(运抵时间)、提单号
        /// </summary>
        /// <param name="提单号"></param>
        /// <returns></returns>
        public IList<集装箱数据> 查询集装箱数据(string 提单号, string 英文船名, string 航次)
        {
            List<集装箱数据> jzx = new List<集装箱数据>();

            WebProxy webProxy = new WebProxy();
            webProxy.Encoding = Encoding.UTF8;

            string htmlInfo = webProxy.GetToString(m_运抵报告url);

            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);

            HtmlAgilityPack.HtmlNode node_viewState = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/input[1]/@value[1]");
            string viewState = node_viewState.Attributes["value"].Value;
            string postData = m_PostData运抵报告_first.Replace("#VIEWSTATE#", System.Web.HttpUtility.UrlEncode(viewState));

            HtmlAgilityPack.HtmlNode node_viewStateEncrypted = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/input[2]/@value[1]");
            string viewStateEncrypted = node_viewStateEncrypted.Attributes["value"].Value;
            postData = postData.Replace("#VIEWSTATEENCRYPTED#", System.Web.HttpUtility.UrlEncode(viewStateEncrypted));

            HtmlAgilityPack.HtmlNode node_eventValidation = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/input[3]/@value[1]");
            string eventValidation = node_eventValidation.Attributes["value"].Value;
            postData = postData.Replace("#EVENTVALIDATION#", System.Web.HttpUtility.UrlEncode(eventValidation));

            postData = postData.Replace("#提单号#", 提单号);
            postData = postData.Replace("#当前页数#", "1");

            htmlInfo = webProxy.PostToString(m_运抵报告url, postData);
            foreach (集装箱数据 item in nbedi.PageAnalyze.Parse查询集装箱数据(htmlInfo))
            {
                if (item.提单号 == 提单号 && item.船舶英文名称 == 英文船名 && item.航次 == 航次)
                {
                    jzx.Add(item);
                }
            }

            int pageCount = GetPageCount运抵报告(htmlInfo);

            if (pageCount > 1)
            {
                postData = m_PostData运抵报告_next.Replace("#VIEWSTATE#", System.Web.HttpUtility.UrlEncode(viewState));
                postData = postData.Replace("#VIEWSTATEENCRYPTED#", System.Web.HttpUtility.UrlEncode(viewStateEncrypted));
                postData = postData.Replace("#EVENTVALIDATION#", System.Web.HttpUtility.UrlEncode(eventValidation));
                postData = postData.Replace("#提单号#", 提单号);
            }

            for (int i = 2; i <= pageCount; i++)
            {
                System.Threading.Thread.Sleep(1000);

                postData = postData.Replace("#当前页数#", i.ToString());

                htmlInfo = webProxy.PostToString(m_运抵报告url, postData);
                foreach (集装箱数据 item in nbedi.PageAnalyze.Parse查询集装箱数据(htmlInfo))
                {
                    if (item.提单号 == 提单号 && item.船舶英文名称 == 英文船名 && item.航次 == 航次)
                    {
                        jzx.Add(item);
                    }
                }
            }

            foreach (集装箱数据 x in jzx)
            {
                x.堆场区 = 查询进场码头(x.船舶英文名称, x.航次, x.提单号);
            }

            return jzx;
        }

        private string m_运抵报告对比进场码头url = "http://arrival.nbedi.com/MsFlat/arrivalReport.aspx?BillNO=#英文船名#|#航次#|#提单号#";
        public string 查询进场码头(string 英文船名, string 航次, string 提单号)
        {
            string 进场码头url = m_运抵报告对比进场码头url.Replace("#英文船名#", 英文船名);
            进场码头url = 进场码头url.Replace("#航次#", 航次);
            进场码头url = 进场码头url.Replace("#提单号#", 提单号);

            WebProxy webProxy = new WebProxy();
            webProxy.Encoding = Encoding.UTF8;
            string htmlInfo = webProxy.GetToString(进场码头url);
            return nbedi.PageAnalyze.Parse查询进场码头(htmlInfo);
        }

        private Regex m_regexPageCount运抵报告 = new Regex(@"<span id=""lbl_AllCount"">(.*?)</span>");
        private int GetPageCount运抵报告(string pageSource)
        {
            MatchCollection mx = m_regexPageCount运抵报告.Matches(pageSource);
            if (mx.Count == 1)
            {
                return Convert.ToInt32(string.IsNullOrEmpty(mx[0].Groups[1].Value) ? "0" : mx[0].Groups[1].Value);
            }
            else
            {
                throw new WebFormatChangedException("PageSource format for BoxCount is changed");
            }
        }

        #endregion
    }
}
