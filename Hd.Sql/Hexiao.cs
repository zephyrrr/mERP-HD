using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Hd.Sql
{
    public class Hexiao
    {
        public static void Test()
        {
            //using (Repository rep = new Repository())
            //{
            //}

            //DateTime dt1 = System.DateTime.Now;
            //using (Repository rep = new Repository())
            //{
            //    for (int i = 0; i < 10000; ++i)
            //    {
            //        费用 fee = new 常规业务费用();
            //        fee.费用项编号 = "100";
            //        fee.金额 = new 金额("CNY", 100);
            //        fee.收付标志 = 收付标志.收;
            //        fee.应收应付款时间 = DateTime.Now;

            //        rep.BeginTransaction();
            //        rep.Save(fee);
            //        rep.CommitTransaction();
            //    }
            //}
            //DateTime dt2 = System.DateTime.Now;
            //TimeSpan time = dt2 - dt1;

            SqlCommand cmd = new SqlCommand(@"INSERT INTO 财务_费用 ([Id] ,[费用类别]
           ,[Version]
           ,[费用项]
           ,[收付标志]
           ,[币制]
           ,[数额]
           ,[完全标志]
			,[已核销数额]) 
            VALUES (NEWID(), '1',
            '1', '100', '1', 'CNY', 200, 'FALSE', 0)");
        }

        public static void Calculate()
        {
            string sql1 = @"SELECT A.收款人编号, A.费用项编号, A.数额, A.票, A.箱, B.币制编号 FROM 财务_凭证费用明细 A" +
                    " INNER JOIN 财务_凭证 B ON A.凭证 = B.Id" + 
                    " WHERE A.票 IS NOT NULL";

            string sql21 = @"SELECT * FROM 财务_费用 WHERE 票 = @票 AND 箱 = @箱";
            //string sql22 = @"SELECT * FROM 财务_费用 WHERE 票 = @票";
            DataTable dt1 = GetDataTable(sql1);
            foreach (DataRow row in dt1.Rows)
            {
                DataTable dt2 = GetDataTable(sql21);

            }

                
            
        }

        private static DataTable GetDataTable(string sql)
        {
            using (SqlConnection connection = new SqlConnection("context connection=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        reader.GetSqlInt32(0);
                    }
                }
            }

            return null;
        }
    }

}
