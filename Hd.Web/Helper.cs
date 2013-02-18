using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using ComponentArt.Web.UI;
using Feng;

namespace Hd.Web
{
    public class Helper
    {
        public static string DictionaryToString(Dictionary<string, object> dict)
        {
            if (dict.Count == 0)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                sb.Append(kvp.Key + ": " + kvp.Value.ToString());
                sb.Append("; ");
            }
            return sb.ToString(0, sb.Length - 2);
        }

        public static Dictionary<string, string> StringToDictionary(string s)
        {
            string[] ss = s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string i in ss)
            {
                string[] s2 = i.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (s2.Length != 2)
                    continue;
                dict[s2[0].Trim()] = s2[1].Trim();
            }
            return dict;
        }

        private static Regex s_regexExpression = new Regex(@"\$(.*?)\$", RegexOptions.Compiled);
        private static Regex s_regexEntityParamter = new Regex(@"\%(.*?)\%", RegexOptions.Compiled);

        /// <summary>
        /// 替换字符串里的%变量%(用entity内变量）（只有entity变量）
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ReplaceParameterizedEntity(string expression, GridItem entity)
        {
            try
            {
                Match m = s_regexEntityParamter.Match(expression);
                while (m.Success)
                {
                    object v = entity[m.Groups[1].Value];

                    string s = "\"\"";
                    if (v != null && v != System.DBNull.Value)
                    {
                        if (v is DateTime)
                        {
                            s = ((DateTime)v).ToString("MM.dd.yyyy");
                        }
                        else if (v is string || v.GetType().IsEnum || v is Boolean
                            || v.GetType().IsClass) //如果和Class直接比较，一般是和null比较
                        {
                            s = "\"" + v.ToString() + "\"";
                        }
                        else
                        {
                            string s1 = v.ToString();
                            if (!string.IsNullOrEmpty(s1))
                            {
                                s = s1;
                            }
                        }
                    }
                    expression = expression.Replace(m.Groups[0].Value, s);

                    m = s_regexEntityParamter.Match(expression);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("expression of \"" + expression + "\" 's format is invalid!", ex);
            }
            return expression;
        }

        public enum 人员类型
        {
            委托人 = 01,
            承运人 = 02,
            收货人 = 07,
            货代 = 99
        }

        public enum 业务类型
        {
            进口 = 11,
            出口 = 13,
            内贸出港 = 15,

            进口其他业务 = 45
        }

        /// <summary>
        /// 根据人员类型(委托人，承运人，收货人)和业务类型（进口，出口。。）生成查询条件
        /// 在"参数备案_人员单位"中有一列是Role，为此客户对应的数据库角色名。
        /// 例如，用户admin属于角色（货代角色1，收货人角色2），则990001（货代1）Role=货代角色1。当用admin登录后，可按照货代1查询
        /// </summary>
        /// <param name="人员类型"></param>
        /// <param name="业务类型"></param>
        /// <returns></returns>
        public static ISearchExpression GetConstraitByRole(人员类型 人员类型, 业务类型 业务类型)
        {
            if (System.Web.HttpContext.Current.User.Identity == null)
                return null;

            string name = System.Web.HttpContext.Current.User.Identity.Name;
            //name = "100000";

            string[] roles;
            using (var pm = new Feng.UserManager.ProviderManager())
            {
                roles = pm.DefaultProvider.CreateUserManager().GetRoles(System.Web.Security.Membership.ApplicationName, name);
            }

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("SELECT * FROM 参数备案_人员单位 WHERE Role IN (");

            if (roles.Length == 0)
                return null;

            for (int i = 0; i < roles.Length; ++i)
            {
                string paraNameI = "@Role" + i.ToString();
                cmd.CommandText += paraNameI;
                if (i != roles.Length - 1)
                {
                    cmd.CommandText += ",";
                }

                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(paraNameI, roles[i]));
            }
            cmd.CommandText += ')';

            System.Data.DataTable dt = Feng.Data.DbHelper.Instance.ExecuteDataTable(cmd);

            //for (int i = 0; i < roles.Length; ++i)
            //{
            //    string paraNameI = "'" + roles[i].ToString() + "'";
            //    cmd.CommandText += paraNameI;
            //    if (i != roles.Length - 1)
            //    {
            //        cmd.CommandText += ",";
            //    }
            //}
            //cmd.CommandText += ')';
            //System.Data.DataTable dt = DBManager.GetDateTable(cmd);

            Dictionary<人员类型, string> dict = new Dictionary<人员类型, string> { { 人员类型.委托人, "委托人" },
                { 人员类型.承运人, "承运人" }, { 人员类型.收货人, "收货人" }};

            ISearchExpression se = null;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (!row["业务类型"].ToString().Contains(((int)业务类型).ToString("00") + ","))
                    continue;
                if (!row["角色用途"].ToString().Contains(((int)人员类型).ToString("00") + ","))
                    continue;

                ISearchExpression sse = null;// SearchExpression.Eq("ClientId", row["ClientId"].ToString());
                if (人员类型 != 人员类型.货代)
                {
                    sse = SearchExpression.Eq(dict[人员类型], row["编号"]); // SearchExpression.And(sse, SearchExpression.Eq(dict[人员类型], row["编号"]));
                }
                if (se == null)
                {
                    se = sse;
                }
                else
                {
                    if (sse != null)
                    {
                        se = SearchExpression.Or(se, sse);
                    }
                }
            }
            return se;
        }
    }
}
