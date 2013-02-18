using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Feng.Net;

namespace Hd.NetRead.npedi
{
    /// <summary>
    /// npedi上的内容分析
    /// </summary>
    public class PageAnalyze
    {
        private static Regex m_regexBoxData = new Regex(@"<tr align=""center"" bgColor=""#FFFFFF""(.*?)<\/tr>");
        private static Regex m_regexBoxInnerData = new Regex(@"<td height=""24"" class=""dblue12"">(.*?)<\/td>");
        internal static IList<string> GetBoxData(string webContent)
        {
            /* <tr align="center" bgColor=#ffffff>
              <td height="24">1</td>
              <td height="24">UN9253014 </td>
              <td height="24">018E </td>
              <td height="24">TGHU2702678 </td>
              <td height="24">22GP </td>
              <td height="24">20071123010954 </td>
              <td height="24">20071130210800 </td>
              <td height="24">EMCWR32854 </td>
              <td height="24">EMC </td>
              <td height="24">ZJL-A1378 </td>
              <td height="24">BLCT3 </td>
            </tr>*/

            webContent = webContent.Trim();
            if (string.IsNullOrEmpty(webContent))
            {
                throw new ArgumentNullException("webContent");
            }
            webContent = webContent.Replace(System.Environment.NewLine, "");
            List<string> ret = new List<string>();
            MatchCollection myMc = m_regexBoxData.Matches(webContent);
            if (myMc.Count > 1)
            {
                throw new WebFormatChangedException("web format in npedi is changed");
            }
            foreach (Match m in myMc)
            {
                string box = m.Groups[1].Value;
                MatchCollection mc = m_regexBoxInnerData.Matches(box);

                if (mc.Count > 0)
                {
                    if (mc.Count == 11)
                    {
                        for (int i = 0; i < 11; ++i)
                        {
                            ret.Add(mc[i].Groups[1].Value.Trim());
                        }
                    }
                    else
                    {
                        throw new WebFormatChangedException("boxSource format is changed");
                    }
                }

            }
            return ret;
        }

        private static Regex m_regexBoxInnerData海关查验 = new Regex(@"<td height=""24"" nowrap=""nowrap"" class=""dblue12"">(.*?)<\/td>");
        internal static IList<string> GetBoxData海关查验结果(string webContent)
        {
            //<tr align="center" bgColor="#FFFFFF"}>  
            //  <td height="24" nowrap="nowrap" class="dblue12">1</td>  
            //  <td height="24" nowrap="nowrap" class="dblue12">MAERSKALGOL </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">1008 </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">MSKU2990124 </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">               
            //    北二集司(三期)
            //  </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">22 </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">
            //    查验                
            //  </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">Y </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">
            //  </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">2010-08-16 13:51:05 </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">
            //    完成归位
            //  </td>
            //  <td height="24" nowrap="nowrap" class="dblue12">2010-08-16 21:30:31 </td>
            //  <td height="24" nowrap="nowrap" class="dblue12"> </td>
            // </tr>

            webContent = webContent.Trim();
            if (string.IsNullOrEmpty(webContent))
            {
                throw new ArgumentNullException("webContent");
            }
            webContent = webContent.Replace(System.Environment.NewLine, "");
            List<string> ret = new List<string>();
            MatchCollection myMc = m_regexBoxData.Matches(webContent);
            if (myMc.Count > 1)
            {
                throw new WebFormatChangedException("web format in npedi is changed");
            }
            foreach (Match m in myMc)
            {
                string box = m.Groups[1].Value;
                MatchCollection mc = m_regexBoxInnerData海关查验.Matches(box);

                if (mc.Count > 0)
                {
                    if (mc.Count == 13)
                    {
                        for (int i = 0; i < 13; ++i)
                        {
                            ret.Add(mc[i].Groups[1].Value.Trim());
                        }
                    }
                    else
                    {
                        throw new WebFormatChangedException("boxSource format is changed");
                    }
                }
            }
            return ret;
        }

        private static Regex m_tr = new Regex(@"<tr align=""center"" bgcolor=""#FFFFFF""(.*?)</tr>", RegexOptions.Singleline);
        private static Regex m_tr1 = new Regex(@"<tr align=""center"" bgcolor=""#E2EBF1""(.*?)</tr>", RegexOptions.Singleline);
        /// <summary>
        /// 返回提单号、 放行时间、报关单号
        /// </summary>
        /// <param name="webContent"></param>
        /// <returns></returns>
        internal static List<List<string>> 查询海关放行时间(string webContent)
        { 
            //<tr align="center">
            //  <TD height="24" bgcolor="#E9F1FC" class="dblue12">NO.</TD>
            //  <TD height="24" valign="middle" bgcolor="#FFFFFF" class="dblue12">集装箱号</TD>
            //  <TD height="24" valign="middle" bgcolor="#E9F1FC" class="dblue12">英文船名</TD>
            //  <TD height="24" valign="middle" bgcolor="#FFFFFF" class="dblue12">航次</TD>
            //  <TD height="24" valign="middle" bgcolor="#E9F1FC" class="dblue12">航向</TD>
            //  <TD height="24" valign="middle" bgcolor="#FFFFFF" class="dblue12">船舶UN代码</TD>
            //  <TD height="24" valign="middle" bgcolor="#E9F1FC" class="dblue12">提单号</TD>
            //  <TD height="24" valign="middle" bgcolor="#FFFFFF" class="dblue12">放行时间</TD>
            //  <TD height="24" valign="middle" bgcolor="#E9F1FC" class="dblue12">海关放行号</TD>
            //  <TD height="24" valign="middle" bgcolor="#FFFFFF" class="dblue12">码头/堆场</TD>
            //</TR>
            //<TR align="center" bgColor="#FFFFFF"}>
            //    <td height="24" class="dblue12">1</td>
            //  <TD height="24" class="dblue12">IMTU9085962 </TD>
            //  <TD height="24" class="dblue12">EVERUNIQUE </TD>
            //  <TD height="24" class="dblue12">0167W</TD>
            //  <TD height="24" class="dblue12">E </TD>
            //  <TD height="24" class="dblue12">UN9116606</TD>
            //  <TD height="24" class="dblue12">EGLV143083852506</TD>
            //  <TD height="24" class="dblue12">2010-11-09 13:54:41</TD>
            //  <TD height="24" class="dblue12">310120100518987733 </TD>
            //  <TD height="24" class="dblue12">港吉(四期)</TD>
            //</TR>
            //<TR align="center" bgColor="#E2EBF1">
            //    <td height="24" class="dblue12">2</td>
            //  <TD height="24" class="dblue12">IMTU9085962 </TD>
            //  <TD height="24" class="dblue12">CSCLASIA </TD>
            //  <TD height="24" class="dblue12">0086E</TD>
            //  <TD height="24" class="dblue12">I </TD>
            //  <TD height="24" class="dblue12">UN9285976</TD>
            //  <TD height="24" class="dblue12"></TD>
            //  <TD height="24" class="dblue12">2010-11-02 15:02:30</TD>
            //  <TD height="24" class="dblue12">31042010I170002364 </TD>
            //  <TD height="24" class="dblue12">北二集司(三期)
            //  </TD>
            //</TR>

            List<List<string>> returnList = new List<List<string>>();           

            HtmlAgilityPack.HtmlDocument _html = new HtmlAgilityPack.HtmlDocument();
            _html.LoadHtml(webContent);

            HtmlAgilityPack.HtmlNode node_table = _html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[4]/td[1]/table[1]");            
                        
            int trCount = m_tr.Matches(node_table.InnerHtml).Count + m_tr1.Matches(node_table.InnerHtml).Count;
            // 数据行数，需再加上列头行和翻页行
            for (int i = 2; i < trCount + 2; i++)
			{
                // 提单号
                HtmlAgilityPack.HtmlNode node_td_tdh = _html.DocumentNode.SelectSingleNode(
                    string.Format("/html[1]/body[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[7]", i));

                // 放行时间
                HtmlAgilityPack.HtmlNode node_td_fxsj = _html.DocumentNode.SelectSingleNode(
                    string.Format("/html[1]/body[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[8]", i));

                // 报关单号
                HtmlAgilityPack.HtmlNode node_td_bgdh = _html.DocumentNode.SelectSingleNode(
                    string.Format("/html[1]/body[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[9]", i));

                 List<string> dataList = new List<string>();
                 dataList.Add(WebProxy.RemoveSpaces(node_td_tdh.InnerHtml));
                 dataList.Add(WebProxy.RemoveSpaces(node_td_fxsj.InnerHtml));
                 dataList.Add(WebProxy.RemoveSpaces(node_td_bgdh.InnerHtml));
                 returnList.Add(dataList);
			}

            return returnList;
        }
    }
}
