using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;

namespace Hd.Web
{
    public class DBManager
    {
        public static WeiTuoInfo SelectWeiTuoInfo(string HDId)
        {
            SqlConnection con = null;
            SqlDataReader reader = null;
            WeiTuoInfo weiTuoInfo = null;
            try
            {
                con = GetSqlConnection();
                con.Open();

                string sql = "select * from 网页查询_客户委托情况_进口 where 货代自编号 = '" + HDId + "'";
                SqlCommand com = new SqlCommand(sql, con);
                reader = com.ExecuteReader();
                if (reader.Read())
                {
                    weiTuoInfo = new WeiTuoInfo();
                    weiTuoInfo.货代自编号1 = reader["货代自编号"].ToString();
                    weiTuoInfo.委托时间1 = Convert.ToDateTime(reader["委托时间"]);
                    weiTuoInfo.委托人1 = reader["委托人"].ToString(); 
                    weiTuoInfo.通关类别1 = reader["通关类别"].ToString();
                    weiTuoInfo.委托分类1 = reader["委托分类"].ToString();
                    weiTuoInfo.提单号1 = reader["提单号"].ToString();
                    weiTuoInfo.船名航次1 = reader["船名航次"].ToString();
                    weiTuoInfo.停靠码头1 = reader["停靠码头"].ToString();                   
                    weiTuoInfo.到港时间1 = Convert.ToDateTime(reader["到港时间"]);
                    weiTuoInfo.箱量1 = Convert.ToInt32(reader["箱量"]);
                    weiTuoInfo.报关状态1 = reader["报关状态"].ToString();
                    weiTuoInfo.报检状态1 = reader["报检状态"].ToString();
                    weiTuoInfo.承运状态1 = reader["承运状态"].ToString();
                    weiTuoInfo.报关单号1 = reader["报关单号"].ToString();
                }
                return weiTuoInfo;
            }
            catch (Exception)
            {

                throw;
            }
            finally 
            {
                reader.Close();
                con.Close();
            }
        }

        public static DataTable GetDateTable(SqlCommand com) 
        {
            SqlConnection con = null;
            try
            {
                con = GetSqlConnection();
                con.Open();
                SqlDataAdapter ad = new SqlDataAdapter(com.CommandText, con);
                DataTable dt = new DataTable("Role");
                ad.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        private static SqlConnection GetSqlConnection() 
        {
            string strCon = "Data Source=192.168.0.10,8033;Initial Catalog=HDCX;User ID=hdcx;Password=hdcx";
            SqlConnection con = new SqlConnection(strCon);
            return con;
        }
    }
}
