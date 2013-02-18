using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Feng.Net;

namespace Hd.NetRead.nbedi
{
    internal class PageAnalyze
    {
        private static Regex m_regex1 = new Regex(@"<div align='center' style='cursor:hand' onclick=""jzx_diaoyong\('(.*?)'\);"">");
        public static IList<string> Parse查询报关单编号(string htmlInfo)
        {
            MatchCollection mc = m_regex1.Matches(htmlInfo);
            List<string> ret = new List<string>();
            foreach (Match m in mc)
            {
                ret.Add(m.Groups[1].Value);
            }
            return ret;
        }

        private static Regex m_regexDetail = new Regex(@"<div.*?>(.*?)</div>", RegexOptions.Singleline);
        private static Regex m_regexXh = new Regex(@"<td valign=""top"">.*集装箱号：(.*?)</td>", RegexOptions.Singleline);
        private static Regex m_regexTgdh = new Regex(@"随附单证号：B:(.*?)</td>", RegexOptions.Singleline);
        private static Regex m_regexBgy = new Regex(@"报关员：([0-9]{8})", RegexOptions.Singleline);
        internal static 报关单数据 Parse查询报关单数据(string htmlInfo, string 报关单编号)
        {
            int idx1 = htmlInfo.IndexOf(@"<div id=""mainlay""");
            int idx2 = htmlInfo.IndexOf(@"<table");
            int idx3 = 0;
            if (idx1 == -1 || idx1 == -1)
            {
                throw new WebFormatChangedException("Invalid Html");
            }
            string innerHtml = htmlInfo.Substring(idx1, idx2 - idx1);

            MatchCollection mc = m_regexDetail.Matches(innerHtml);

            List<string> ss = new List<string>();
            foreach (Match m in mc)
            {
                ss.Add(WebProxy.RemoveSpaces(m.Groups[1].Value));
            }

            int idx = ss.IndexOf(报关单编号);
            if (idx == -1)
            {
                throw new WebFormatChangedException("nbedi format changed!");
            }

            string xh = "";
            string s = ss[idx + 30];
            if (!string.IsNullOrEmpty(s))
            {
                idx1 = s.IndexOf('*');
                idx2 = s.IndexOf('(');
                idx3 = s.IndexOf(')');
                xh = s.Substring(0, idx1 - 1).Trim();
            }

            Match m1 = m_regexXh.Match(htmlInfo);
            if (m1 == null)
            {
                throw new WebFormatChangedException("nbedi format is changed");
            }
            Match m2 = m_regexTgdh.Match(htmlInfo);
            string tgdh = null;
            if (m2 != null && m2.Groups.Count > 0)
            {
                tgdh = WebProxy.RemoveSpaces(m2.Groups[1].Value);
            }

            string xh1 = WebProxy.RemoveSpaces(m1.Groups[1].Value);
            if (!string.IsNullOrEmpty(xh1))
            {
                xh += "," + xh1;
            }

            int xl = 0;
            int bxl = 0;
            if (s.Contains("("))
            {
                xl = string.IsNullOrEmpty(s) ? 0 : Convert.ToInt32(s.Substring(idx1 + 1, idx2 - idx1 - 1).Trim());
                bxl = string.IsNullOrEmpty(s) ? 0 : Convert.ToInt32(s.Substring(idx2 + 1, idx3 - idx2 - 1).Trim());
            }
            else
            {
                xl = string.IsNullOrEmpty(s) ? 0 : Convert.ToInt32(s.Substring(idx1 + 1).Trim());
            }

            //报关员
            string bgy = null;
            Match m3 = m_regexBgy.Match(htmlInfo);
            if (m3 != null && m3.Groups.Count > 0)
            {
                bgy = WebProxy.RemoveSpaces(m3.Groups[1].Value);
            }

            //报关公司            
            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);
            HtmlAgilityPack.HtmlNode node_bgdh = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[39]/div[1]");
            string bggs = WebProxy.RemoveSpaces(node_bgdh.InnerText);

            return new 报关单数据(ss[0], ss[idx], ss[idx + 11], xh,
                string.IsNullOrEmpty(ss[idx + 5]) ? null : new DateTime?(Convert.ToDateTime(ss[idx + 5])),
                ss[idx + 6], ss[idx + 12], ss[idx + 20], xl, bxl,
                htmlInfo, 1, tgdh, bgy, bggs);
        }

        //private static Regex m_regexBgdh = new Regex(@"type=""checkbox"" value=""(.*?)"">", RegexOptions.Singleline);
        internal static List<string> Parse查询所有报关单号(string htmlInfo)
        {
            List<string> bgdhList = new List<string>();

            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);

            HtmlAgilityPack.HtmlNode node_table = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[2]");

            // 判断table的<tr>确定每页的记录条数
            // 列头<tr class="table1">被忽略，当前页记录条数 + 一个分页<tr>
            int trCount = new Regex("<tr>").Matches(node_table.InnerHtml).Count;

            for (int i = 2; i <= trCount; i++)  // 去掉列头<tr>和分页<tr>
            {
                HtmlAgilityPack.HtmlNode node_bgdh = _html.DocumentNode.SelectSingleNode(
                    string.Format("/html[1]/body[1]/table[2]/tr[{0}]/td[1]/div[1]/input[1]/@value[1]", i));
                bgdhList.Add(Feng.Net.WebProxy.RemoveSpaces(node_bgdh.Attributes["value"].Value));
            }

            //foreach (Match match in m_regexBgdh.Matches(htmlInfo))
            //{
            //    bgdhList.Add(match.Groups[1].Value);
            //}
            return bgdhList;
        }

        /// <summary>
        /// 返回 报关单号, 报关单pre_entry_id
        /// </summary>
        /// <param name="htmlInfo"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> Parse查询所有报关单号2(string htmlInfo)
        {
            Dictionary<string, string> dic = null;
            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);

            HtmlAgilityPack.HtmlNode node_table = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[2]");
            if (node_table != null)
            {
                dic = new Dictionary<string, string>();
                int trCount = new Regex("<tr>").Matches(node_table.InnerHtml).Count;
                for (int i = 2; i <= trCount; i++)
                {
                    //报关单号
                    HtmlAgilityPack.HtmlNode node_td1 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/table[2]/tr[{0}]/td[2]/div[1]/a[1]", i));
                    //地址
                    HtmlAgilityPack.HtmlNode node_td2 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/table[2]/tr[{0}]/td[7]/div[1]/a[1]", i));
                    string url = null;
                    if (node_td2 != null)
                    {
                        //去掉“../”，拼成报关单快照url
                        //url = "http://www.nbedi.com/h2000eport/pre_bgd/" + node_td2.InnerText.Substring(3);

                        //报关单pre_entry_id
                        string att = node_td2.Attributes["href"].Value;
                        url = att.Substring(att.LastIndexOf("=") + 1);
                    }
                    dic.Add(node_td1.InnerText, url);
                }
            }
            return dic;
        }

        internal static IList<集装箱数据> Parse查询集装箱数据(string htmlInfo)
        {
            List<集装箱数据> xhs = new List<集装箱数据>();

            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);

            HtmlAgilityPack.HtmlNode node_table = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]");
            int trCount = new Regex("<tr>").Matches(node_table.InnerHtml).Count;

            HtmlAgilityPack.HtmlNode node_tr = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[1]");
            int thCount = new Regex("<th").Matches(node_tr.InnerHtml).Count;

            /* <td width="120">MANHATTAN</td>
             * <td width="60">1042S</td>             
             * <td width="100">CLHU8379490</td>
             * <td width="30">45</td>
             * <td width="40">整箱</td>
             * <td>NLBDS046542</td>
             * <td>11/12/2010 10:37:00 PM</td>
             * <td>5H1863</td>
             * <td>BLCT</td>
             * <td>11/12/2010 10:44:03 PM</td>
             * <td class="table_content text_center" align="center" width="70">
                      <a id="GridView1_ctl02_lbt_YP" href="javascript:__doPostBack('GridView1$ctl02$lbt_YP','')">预配舱单</a>                          
             * </td> 
             */
            for (int i = 2; i <= trCount; i++)    // 去掉列头行
            {
                HtmlAgilityPack.HtmlNode node_td船名 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[1]", i));
                HtmlAgilityPack.HtmlNode node_td航次 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[2]", i));
                HtmlAgilityPack.HtmlNode node_td箱号 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[3]", i));
                HtmlAgilityPack.HtmlNode node_td箱型 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[4]", i));
                HtmlAgilityPack.HtmlNode node_td提单号 = null;
                HtmlAgilityPack.HtmlNode node_td运抵时间 = null;
                // 未知原因，整箱那列时有时无
                if (thCount == 11)
                {
                    node_td提单号 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[6]", i));
                    node_td运抵时间 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[7]", i));
                }
                else
                {
                    node_td提单号 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[5]", i));
                    node_td运抵时间 = _html.DocumentNode.SelectSingleNode(string.Format("/html[1]/body[1]/center[1]/div[3]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[{0}]/td[6]", i));
                }

                xhs.Add(new 集装箱数据(null,
                    WebProxy.RemoveSpaces(node_td船名.InnerText),
                    WebProxy.RemoveSpaces(node_td航次.InnerText),
                    WebProxy.RemoveSpaces(node_td箱号.InnerText),
                    null,
                    DateTime.Parse(node_td运抵时间.InnerText),
                    null,
                    null,
                    WebProxy.RemoveSpaces(node_td提单号.InnerText),
                    null,
                    WebProxy.RemoveSpaces(node_td箱型.InnerText)));
            }
            return xhs;
        }

        internal static string Parse查询进场码头(string htmlInfo)
        {
            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(htmlInfo);

            HtmlAgilityPack.HtmlNode node_td = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/div[2]/fieldset[1]/table[1]/tr[1]/td[1]/div[1]/table[1]/tr[2]/td[6]");

            if (node_td == null)
            {
                throw new WebFormatChangedException("PageSource format for 运抵报告对比进场码头 is changed");
            }

            return WebProxy.RemoveSpaces(node_td.InnerText);
        }
    }
}
