using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Hd.NetRead.npedi
{
    #region 查询页面分析
    public class EdiPageResolve
    {
        #region private attributes

        private static Regex _rxBoxOuterInfo = new Regex("<tr align=\"center\" bgColor=\".*?\".*?>(.*?)<\\/tr>");
        private static Regex _rxBoxInnerInfo = new Regex("<td height=\"24\"(.*?)<\\/td>");
        private static Regex _rxBoxCountInfo = new Regex("<tr align=\"center\" bgColor=");
           
        #endregion

        #region public method FetchBoxData(string initialData) 获取查询信息
        /// <summary>
        /// 获取查询信息
        /// </summary>
        /// <param name="initialData">初始页面信息</param>
        /// <returns></returns>
        public static IList<string> FetchBoxData(string initialData, int rwCellCnt) // 获取查询信息
        {
            
            List<String> boxList = new List<string>();
            initialData = initialData.Trim().Replace(Environment.NewLine,"");
           // initialData = System.Text.RegularExpressions.Regex.Replace(initialData.Trim().Replace(Environment.NewLine,""), "<[^>]+>", "").Replace("nowrap=\"nowrap\" class=\"dblue12\">", "").Trim();
            if ((initialData == null) || (initialData.Length==0))
            {
                throw new Feng.Net.WebFormatChangedException("无法取得数据,请稍后重新操作!");

            }

            MatchCollection mymatch = _rxBoxOuterInfo.Matches(initialData);

           try
           {
               foreach (Match m in mymatch)
               {
                   string item = m.Groups[1].Value;
                   MatchCollection boxcount = _rxBoxCountInfo.Matches(item);
                   int count = boxcount.Count+1;
                   MatchCollection boxinfo = _rxBoxInnerInfo.Matches(item);
                   if (boxinfo.Count != rwCellCnt * count)
                   {
                      throw new Feng.Net.WebFormatChangedException("网页格式发生改变");

                   }

                   for (int i = 0; i < boxinfo.Count; i++)
                   {
                       string tmp = System.Text.RegularExpressions.Regex.Replace(boxinfo[i].Groups[1].Value,"<[^>]+>", "").Replace("nowrap=\"nowrap\" class=\"dblue12\">", "").Trim();
                       boxList.Add(tmp);
                   }

               }
           }
           catch (System.Exception ex)
           {
               ex.Message.ToString();
           }

           return boxList;

        }

        #endregion
    }

    #endregion

}
