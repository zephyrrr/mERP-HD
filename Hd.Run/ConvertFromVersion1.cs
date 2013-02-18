//using System;
//using System.Collections.Generic;
//using System.Text;
//using Feng;
//using Feng.Data;
//using Hd.Model;
//using Hd.Model.Kj;
//using Feng.NH;
//using System.Threading;
//using System.Security.Principal;
//namespace Hd.Run
//{
//    public sealed class ConvertFromVersion1
//    {
//        private static Dictionary<string, string> feeMappings = new Dictionary<string, string>();
//        private static Dictionary<string, string> feeMappingsNmcg = new Dictionary<string, string>();

//        static ConvertFromVersion1()
//        {
//            IIdentity identity = new GenericIdentity("200001", "ownset");
//            IPrincipal principal = new GenericPrincipal(identity, new string[0]);
//            Thread.CurrentPrincipal = principal;

//            // 
//            //feeMappingsNmcg["80100103"] = "184";
//            //feeMappingsNmcg["80100123"] = "165";
//            //feeMappingsNmcg["80100129"] = "184";
//            //feeMappingsNmcg["80100139"] = "184";
//            //feeMappingsNmcg["80100228"] = "136";
//            //feeMappingsNmcg["80100229"] = "136";

//            // 中远
//            //feeMappingsNmcg["80100103"] = "184";
//            //feeMappingsNmcg["80100122"] = "161";
//            //feeMappingsNmcg["80100123"] = "165";
//            //feeMappingsNmcg["80100125"] = "167";
//            //feeMappingsNmcg["80100129"] = "181";
//            //feeMappingsNmcg["80100134"] = "116";
//            //feeMappingsNmcg["80100139"] = "181";
//            //feeMappingsNmcg["80100149"] = "181";
//            //feeMappingsNmcg["80100227"] = "136";
//            //feeMappingsNmcg["80100228"] = "136";
//            //feeMappingsNmcg["80100229"] = "136";

//            ////中海船务代理
//            //feeMappingsNmcg["80100103"] = "181";
//            //feeMappingsNmcg["80100129"] = "136";
//            //feeMappingsNmcg["80100134"] = "116";

//            ////对港车队
//            //feeMappingsNmcg["80100129"] = "184";
//            //feeMappingsNmcg["80100139"] = "184";

//            ////倒箱仓库
//            //feeMappingsNmcg["80100104"] = "182";
//            //feeMappingsNmcg["80100134"] = "116";
//            //保险公司
//            feeMappingsNmcg["80100139"] = "185";
//            feeMappingsNmcg["80100228"] = "136";

//            feeMappings["80100101"] = "101";//单箱代理费
//            feeMappings["80100102"] = "102";//基准陆运费
//            feeMappings["80100103"] = "136";//常规其他费
//            feeMappings["80100104"] = "103";//卸箱地费
//            feeMappings["80100111"] = "111";//吊机费
//            feeMappings["80100112"] = "112";//箱检费
//            feeMappings["80100113"] = "128";//安保费
//            feeMappings["80100114"] = "124";//商检开倒箱费
//            feeMappings["80100116"] = "121";//转关施封费_现金
//            feeMappings["80100117"] = "218";//单证费
//            feeMappings["80100118"] = "123";//商检查验费
//            feeMappings["80100119"] = "125";//海关查验费
//            feeMappings["80100120"] = "131";//喷洒费
//            feeMappings["80100122"] = "161";//堆存费
//            feeMappings["80100123"] = "165";//额外修洗箱费
//            feeMappings["80100124"] = "162";//疏港费
//            feeMappings["80100125"] = "167";//滞箱费
//            feeMappings["80100126"] = "168";//退滞箱费
//            feeMappings["80100127"] = "163";//倒箱二次开箱费
//            feeMappings["80100128"] = "164";//额外查验费
//            feeMappings["80100129"] = "136";//常规其他费
//            feeMappings["80100131"] = "113";//外理费
//            feeMappings["80100132"] = "127";//港务港建费
//            feeMappings["80100133"] = "114";//开箱单代理费
//            feeMappings["80100134"] = "116";//箱单费
//            feeMappings["80100136"] = "115";//卫生处理费
//            feeMappings["80100137"] = "132";//码头操作费
//            feeMappings["80100138"] = "122";//转关施封费_支票
//            feeMappings["80100139"] = "136";//常规其他费
//            feeMappings["80100149"] = "169";//额外其他费
//            feeMappings["80100201"] = "219";//证明联打印费
//            feeMappings["80100211"] = "211";//商检输单费
//            feeMappings["80100212"] = "212";//海关输单费
//            feeMappings["80100213"] = "213";//换单费
//            feeMappings["80100214"] = "214";//电放换单费
//            feeMappings["80100215"] = "215";//检验检疫费
//            feeMappings["80100216"] = "241";//滞报金
//            feeMappings["80100217"] = "231";//海关关税
//            feeMappings["80100219"] = "216";//指运地报关费
//            feeMappings["80100220"] = "217";//指运地其他费
//            feeMappings["80100227"] = "242";//改舱单费
//            feeMappings["80100228"] = "136";//常规其他费
//            feeMappings["80100229"] = "136";//常规其他费
//            feeMappings["80100239"] = "169";//额外其他费

//            feeMappings["80100135"] = "134";//木拖消毒费
//            feeMappings["80000135"] = "134";//木拖消毒费

//            feeMappings["80000101"] = "101";
//            feeMappings["80000102"] = "102";
//            feeMappings["80000103"] = "136";
//            feeMappings["80000104"] = "103";
//            feeMappings["80000111"] = "111";
//            feeMappings["80000112"] = "112";
//            feeMappings["80000113"] = "128";
//            feeMappings["80000114"] = "124";
//            feeMappings["80000116"] = "121";
//            feeMappings["80000117"] = "218";
//            feeMappings["80000118"] = "123";
//            feeMappings["80000119"] = "125";
//            feeMappings["80000120"] = "131";
//            feeMappings["80000122"] = "161";
//            feeMappings["80000123"] = "165";
//            feeMappings["80000124"] = "162";
//            feeMappings["80000125"] = "167";
//            feeMappings["80000126"] = "168";
//            feeMappings["80000127"] = "163";
//            feeMappings["80000128"] = "164";
//            feeMappings["80000129"] = "136";
//            feeMappings["80000131"] = "113";
//            feeMappings["80000132"] = "127";
//            feeMappings["80000133"] = "114";
//            feeMappings["80000134"] = "116";
//            feeMappings["80000136"] = "115";
//            feeMappings["80000137"] = "132";
//            feeMappings["80000138"] = "122";
//            feeMappings["80000139"] = "136";
//            feeMappings["80000149"] = "169";
//            feeMappings["80000201"] = "219";
//            feeMappings["80000211"] = "211";
//            feeMappings["80000212"] = "212";
//            feeMappings["80000213"] = "213";
//            feeMappings["80000214"] = "214";
//            feeMappings["80000215"] = "215";
//            feeMappings["80000216"] = "241";
//            feeMappings["80000217"] = "231";
//            feeMappings["80000219"] = "216";
//            feeMappings["80000220"] = "217";
//            feeMappings["80000227"] = "242";
//            feeMappings["80000228"] = "136";
//            feeMappings["80000229"] = "136";
//            feeMappings["80000239"] = "169";

//            feeMappings["80000221"] = "141";//特别交通费

//            //非业务付款
//            feeMappings["80100321"] = "344"; //	通信费
//            feeMappings["80100322"] = "345"; //	办公费
//            feeMappings["80100323"] = "346"; //	房租费
//            feeMappings["80100324"] = "385"; //	挂靠费
//            feeMappings["80100325"] = "347"; //	差旅费
//            feeMappings["80100327"] = "349"; //	事故处理费
//            feeMappings["80100328"] = "348"; //	业务招待费
//            feeMappings["80100329"] = "350"; //	其它公司经费
//            feeMappings["80100331"] = "352"; //	坏帐损失
//            feeMappings["80100332"] = "371"; //	税金
//            feeMappings["80100339"] = "351"; //	其它管理费用
//            feeMappings["80100341"] = "335"; //	开发票税
//            feeMappings["80100342"] = "372"; //	营业税金其它费
//            feeMappings["80100343"] = "371"; //	教育费附加费
//            feeMappings["80100349"] = "372"; //	附加费其它费
//            feeMappings["80100352"] = "331"; //	利息费
//            feeMappings["80100359"] = "332"; //	银行手续费
//            feeMappings["80100361"] = "375"; //	赔偿金
//            feeMappings["80100362"] = "376"; //	违约金
//            feeMappings["80100369"] = "377"; //	营业外支出其它
//            feeMappings["80100376"] = "383"; //	固定资产修理费
//            feeMappings["80100379"] = "384"; //	固定资产其它费
//            feeMappings["80100401"] = "321"; //	个人投资
//            feeMappings["80100402"] = "321"; //	单位投资
//            feeMappings["80100404"] = "321"; //	其它投资
//            feeMappings["80100411"] = "391"; //	分红
//            feeMappings["80100500"] = "389"; //	其它业务

//            feeMappings["80100383"] = "011"; //	运输车辆资产
//            feeMappings["80100384"] = "011"; //	房产资产
//            feeMappings["80100385"] = "011"; //	办公设备资产
//            feeMappings["80100386"] = "011"; //	生产设备资产
//            feeMappings["80100387"] = "011"; //	固定资产其它
//            feeMappings["80100403"] = "012"; //	银行贷款
//            feeMappings["80100405"] = "012"; //	其它借贷

//            //非业务收款
//            feeMappings["80000352"] = "331";//	利息费
//            feeMappings["80000353"] = "389";//	其它收入
//            feeMappings["80000381"] = "381";//	固定资产出租
//            feeMappings["80000383"] = "386";//	运输车辆资产
//            feeMappings["80000384"] = "386";//	房产资产
//            feeMappings["80000385"] = "386";//	办公设备资产
//            feeMappings["80000386"] = "386";//	生产设备资产
//            feeMappings["80000387"] = "386";//	固定资产其它
//            feeMappings["80000391"] = "377";//	营业外收入其它
//            feeMappings["80000401"] = "321";//	个人投资
//            feeMappings["80000402"] = "321";//	单位投资
//            feeMappings["80000404"] = "321";//	其它投资
//            feeMappings["80000500"] = "389";//	其它业务
//            feeMappings["80100351"] = "333";//贴息

//            feeMappings["80000403"] = "002";//	银行贷款
//            feeMappings["80000405"] = "002";//	其它借贷
//            //feeMappings["80000511"] = "";//	应收款坏账
//            //feeMappings["80000521"] = "";//	应收款调节

//            feeMappings["80000202"] = "201";
//        }

//        public static void ProcessYsyf()
//        {
//            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository(typeof(对账单)))
//            {
//                IList<对账单> list = rep.List<对账单>();

//                foreach (对账单 dzd in list)
//                {
//                    try
//                    {
//                        rep.BeginTransaction();
//                        (new 财务对账单Dao()).Submit(rep, dzd);
//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();
//                        Console.WriteLine(ex.Message);
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }

//            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//            {
//                IList<凭证> list = rep.List<凭证>();

//                foreach (凭证 pz in list)
//                {
//                    try
//                    {
//                        rep.BeginTransaction();

//                        (new 凭证Dao()).Submit(rep, pz);

//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();
//                        Console.WriteLine(ex.Message);
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }

//            }
//        }

//        public static void ConvertNmcg费用()
//        {
//            System.Data.DataTable dtFees = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_视图业务费用付款_内贸出港_保险公司");

//            Console.WriteLine("all is " + dtFees.Rows.Count.ToString());

//            int cnt = 0;
//            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//            {
//                try
//                {
//                    rep.BeginTransaction();
                   
//                    foreach (System.Data.DataRow feeRow in dtFees.Rows)
//                    {
//                        cnt++;

//                        业务费用 ywfy = new 业务费用();

//                        ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                        ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                        ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                        ywfy.费用项编号 = feeMappingsNmcg[feeRow["费用项"].ToString()];
//                        ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                        ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                        ywfy.费用信息 = null;
//                        ywfy.金额 = (decimal)feeRow["金额"];

//                        ywfy.相关人编号 = feeRow["相关人"].ToString();
//                        ywfy.箱Id = feeRow["内贸出港箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["内贸出港箱Id"];

//                        (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                        Console.WriteLine(cnt.ToString());
//                    }
                    

//                    rep.CommitTransaction();
//                }
//                catch (Exception ex)
//                {
//                    rep.RollbackTransaction();
//                    Console.WriteLine(ex.Message);
//                    //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                }
//            }

//        }

//        public static void Convert费用()
//        {
//            System.Data.DataTable dtFees = DbHelper.Instance.ExecuteDataTable("select * from HdTest.dbo.B版_视图业务费用凭证核销 where 凭证号 in ('P10906100023')");
//            Console.WriteLine("all is " + dtFees.Rows.Count.ToString());

//            int cnt = 0;
//            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//            {
//                try
//                {
//                    rep.BeginTransaction();
                    
//                    foreach (System.Data.DataRow feeRow in dtFees.Rows)
//                    {
//                        cnt++;

//                        业务费用 ywfy = new 业务费用();

//                        ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                        ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                        ywfy.收付标志 = 收付标志.收; // feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                        ywfy.费用项编号 = "160";// feeMappings[feeRow["费用项"].ToString()];
//                        ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                        ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                        ywfy.费用信息 = null;
//                        ywfy.金额 = -(decimal)feeRow["金额"];

//                        ywfy.相关人编号 = feeRow["相关人"].ToString();
//                        ywfy.箱Id = feeRow["进口箱ID"] == System.DBNull.Value ? null : (Guid?)feeRow["进口箱ID"];

//                        if (ywfy.相关人编号 == "990800")
//                        { ywfy.相关人编号 = "900001"; }

//                        (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                        Console.WriteLine(cnt.ToString());
//                    }
                    

//                    rep.CommitTransaction();
//                }
//                catch (Exception ex)
//                {
//                    rep.RollbackTransaction();
//                    Console.WriteLine(ex.Message);
//                    //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                }
//            }

//        }

//        public static void Convert应付对帐单(string dzdhs)
//        {
//            string[] dzdhss = dzdhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string dzdh in dzdhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM Hdtest.dbo.B版_视图业务费用应付对帐单 WHERE 对帐单号 = '" + dzdh + "'");

//                using (Feng.NH.Repository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用>() as Feng.NH.Repository)
//                {
//                    try
//                    {
//                        rep.BeginTransaction();

//                        for (int i = 0; i < dt.Rows.Count; ++i)
//                        {
//                            System.Data.DataRow feeRow = dt.Rows[i];
//                            业务费用 ywfy = new 业务费用();
//                            ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                            ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                            ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                            ywfy.费用项编号 = feeMappings[feeRow["费用项"].ToString()];
//                            ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                            ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                            ywfy.费用信息 = null;
//                            ywfy.金额 = (decimal)feeRow["金额"];
//                            ywfy.凭证费用明细 = null;

//                            ywfy.相关人编号 = feeRow["相关人"].ToString();
//                            ywfy.箱Id = feeRow["箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["箱Id"];

//                            if (ywfy.相关人编号 == "990800")
//                            { ywfy.相关人编号 = "900001"; }

//                            ywfy.对账单 = rep.Session.CreateCriteria<对账单>()
//                                .Add(NHibernate.Criterion.Expression.Eq("备注", feeRow["对帐单号"]))
//                                .UniqueResult<对账单>(); ;

//                            (new HdBaseDao<业务费用>()).Save(rep, ywfy);
//                        }
//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();

//                        ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }
//        }

//        public static void Convert应收对帐单(string dzdhs)
//        {
//            string[] dzdhss = dzdhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string dzdh in dzdhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM Hdtest.dbo.B版_视图业务费用对帐单 WHERE 对帐单号 = '" + dzdh + "'");

//                using (Feng.NH.Repository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用>() as Feng.NH.Repository)
//                {
//                    try
//                    {
//                        rep.BeginTransaction();

//                        for (int i = 0; i < dt.Rows.Count; ++i)
//                        {
//                            System.Data.DataRow feeRow = dt.Rows[i];
//                            业务费用 ywfy = new 业务费用();
//                            ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                            ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                            ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                            ywfy.费用项编号 = feeMappings[feeRow["费用项"].ToString()];
//                            ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                            ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                            ywfy.费用信息 = null;
//                            ywfy.金额 = (decimal)feeRow["金额"];
//                            ywfy.凭证费用明细 = null;

//                            ywfy.相关人编号 = feeRow["相关人"].ToString();
//                            ywfy.箱Id = feeRow["箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["箱Id"];

//                            ywfy.对账单 = rep.Session.CreateCriteria<对账单>()
//                                .Add(NHibernate.Criterion.Expression.Eq("备注", feeRow["对帐单号"]))
//                                .UniqueResult<对账单>(); ;

//                            (new HdBaseDao<业务费用>()).Save(rep, ywfy);
//                        }
//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();

//                        ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }
//        }

//        public static void Convert收付款凭证应付应收款(string pzhs, bool is付款)
//        {
//            string[] pzhss = pzhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string pzh in pzhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.财务_会计凭证表 WHERE 凭证号 = '" + pzh + "'");
//                foreach (System.Data.DataRow row in dt.Rows)
//                {
//                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//                    {
//                        try
//                        {
//                            rep.BeginTransaction();

//                            凭证 pz = new 凭证();
//                            pz.Submitted = true;
//                            pz.备注 = "原凭证号: " + pzh + System.Environment.NewLine + row["备注"].ToString();
//                            pz.出纳编号 = "100000";
//                            pz.审核状态 = true;
//                            pz.收支状态 = true;
//                            pz.会计编号 = "100000";
//                            pz.会计金额 = (decimal)row["金额"];
//                            pz.金额.币制编号 = "CNY";
//                            pz.金额.数额 = pz.会计金额.Value;
//                            pz.凭证类别 = is付款 ? 凭证类别.付款凭证 : 凭证类别.收款凭证;
//                            pz.日期 = (DateTime)row["出纳时间"];
//                            pz.审核人编号 = row["审核人"] == System.DBNull.Value ? null : (string)row["审核人"];
//                            pz.相关人编号 = row["相关人"] == System.DBNull.Value ? null : (string)row["相关人"];
//                            if (pz.相关人编号 == "990800")
//                            { pz.相关人编号 = "900001"; }
//                            pz.自动手工标志 = 自动手工标志.手工;
//                            //pz.凭证号 = PrimaryMaxIdGenerator.GetMaxId("财务_凭证", "凭证号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(pz.日期)).ToString();

//                            (new HdBaseDao<凭证>()).Save(rep, pz);
//                            //rep.Save(pz);


//                            凭证费用明细 pzs1 = new 凭证费用明细();
//                            pzs1.费用项编号 = "000";
//                            pzs1.金额 = pz.会计金额;
//                            pzs1.凭证 = pz;
//                            pzs1.收付标志 = is付款 ? 收付标志.付 : 收付标志.收;
//                            pzs1.相关人编号 = pz.相关人编号;
//                            pzs1.业务类型编号 = JudgeYwlxByWtr(pzs1.相关人编号);
//                            (new HdBaseDao<凭证费用明细>()).Save(rep, pzs1);

//                            凭证收支明细 pzs2 = new 凭证收支明细();
//                            pzs2.凭证 = pz;
//                            pzs2.金额 = pz.会计金额;
//                            pzs2.收付标志 = is付款 ? 收付标志.付 : 收付标志.收;
//                            switch (row["收付方式"].ToString())
//                            {
//                                case "1":
//                                    pzs2.收付款方式 = 收付款方式.现金;
//                                    break;
//                                case "2":
//                                    pzs2.收付款方式 = 收付款方式.现金支票;
//                                    pzs2.票据号码 = row["票号"].ToString();
//                                    break;
//                                case "3":
//                                    pzs2.收付款方式 = 收付款方式.转账支票;
//                                    pzs2.票据号码 = row["票号"].ToString();
//                                    pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                                    break;
//                                case "4":
//                                    pzs2.收付款方式 = 收付款方式.银行本票汇票;
//                                    pzs2.票据号码 = row["票号"].ToString();
//                                    pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                                    break;
//                                case "5":
//                                    pzs2.收付款方式 = 收付款方式.银行承兑汇票;
//                                    pzs2.票据号码 = row["票号"].ToString();
//                                    pzs2.出票银行 = Get出票银行From票号(row["票号"].ToString());
//                                    pzs2.承兑期限 = Get承兑期限From票号(row["票号"].ToString());
//                                    break;
//                                case "6":
//                                    pzs2.收付款方式 = 收付款方式.电汇;
//                                    pzs2.票据号码 = row["票号"].ToString();
//                                    pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                                    break;
//                                case "7":
//                                    pzs2.收付款方式 = 收付款方式.银行收付;
//                                    pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                                    break;
//                                case "8":
//                                    pzs2.收付款方式 = 收付款方式.银行收付;
//                                    pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                                    break;
//                            }

//                            (new HdBaseDao<凭证收支明细>()).Save(rep, pzs2);

//                            rep.CommitTransaction();
//                        }
//                        catch (Exception ex)
//                        {
//                            rep.RollbackTransaction();
//                            Feng.Windows.Forms.MessageForm.ShowError("凭证号 " + pzh + "转换出现问题");
//                            ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                        }
//                    }
//                }
//            }
//        }

//        private static int JudgeYwlxByWtr(string wtr)
//        {
//            string s = "900163,900164,900210,900218,900231,900232,900238,900240,900155,900245,900260,900267,900284,900285,900297";
//            if (s.Contains(wtr))
//                return 15;
//            else
//                return 11;
//        }

//        public static void Convert收付款凭证核销(string pzhs, bool is付款)
//        {
//            string[] pzhss = pzhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string pzh in pzhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.财务_会计凭证表 WHERE 凭证号 = '" + pzh + "'");
//                foreach (System.Data.DataRow row in dt.Rows)
//                {
//                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//                    {
//                        try
//                        {
//                            rep.BeginTransaction();

//                            凭证 pz = new 凭证();
//                            pz.Submitted = false;
//                            pz.备注 = "原凭证号: " + pzh + System.Environment.NewLine + row["备注"].ToString();
//                            pz.出纳编号 = "100000";
//                            pz.审核状态 = false;
//                            pz.收支状态 = false;
//                            pz.会计编号 = "100000";
//                            pz.会计金额 = (decimal)row["金额"];
//                            pz.金额.币制编号 = "CNY";
//                            pz.金额.数额 = pz.会计金额.Value;
//                            pz.凭证类别 = is付款 ? 凭证类别.付款凭证 : 凭证类别.收款凭证;
//                            pz.日期 = (DateTime)row["出纳时间"];
//                            pz.审核人编号 = row["审核人"] == System.DBNull.Value ? null : (string)row["审核人"];
//                            pz.相关人编号 = row["相关人"] == System.DBNull.Value ? null : (string)row["相关人"];
//                            if (pz.相关人编号 == "990800")
//                            { pz.相关人编号 = "900001"; }
//                            pz.自动手工标志 = 自动手工标志.手工;
//                            //pz.凭证号 = PrimaryMaxIdGenerator.GetMaxId("财务_凭证", "凭证号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(pz.日期)).ToString();

//                            (new HdBaseDao<凭证>()).Save(rep, pz);
//                            //rep.Save(pz);

//                            System.Data.DataTable dtFees = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_视图业务费用凭证核销 WHERE 凭证号 = '" + row["凭证号"].ToString() + "'");
//                            Dictionary<string, Dictionary<string, decimal>> feeMoney = new Dictionary<string, Dictionary<string, decimal>>();

//                            Dictionary<string, Dictionary<string, IList<System.Data.DataRow>>> feeRows = new Dictionary<string, Dictionary<string, IList<System.Data.DataRow>>>();

//                            foreach (System.Data.DataRow row1 in dtFees.Rows)
//                            {
//                                string a = row1["业务类型"].ToString();
//                                if (!feeMoney.ContainsKey(a))
//                                {
//                                    feeMoney[a] = new Dictionary<string, decimal>();
//                                    feeRows[a] = new Dictionary<string, IList<System.Data.DataRow>>();
//                                }

//                                string s = row1["费用项"].ToString();
//                                if (!feeMoney[a].ContainsKey(s))
//                                {
//                                    feeMoney[a][s] = 0m;
//                                    feeRows[a][s] = new List<System.Data.DataRow>();
//                                }
//                                feeMoney[a][s] += (decimal)row1["金额"];
//                                feeRows[a][s].Add(row1);
//                            }

//                            foreach (KeyValuePair<string, Dictionary<string, decimal>> kvp1 in feeMoney)
//                            {
//                                foreach (KeyValuePair<string, decimal> kvp in kvp1.Value)
//                                {
//                                    if (!feeMappings.ContainsKey(kvp.Key))
//                                    {
//                                        throw new ArgumentException(kvp.Key + "is invalid");
//                                    }

//                                    凭证费用明细 pzs1 = new 凭证费用明细();
//                                    pzs1.费用项编号 = feeMappings[kvp.Key];
//                                    pzs1.金额 = kvp.Value;
//                                    pzs1.凭证 = pz;
//                                    pzs1.收付标志 = is付款 ? 收付标志.付 : 收付标志.收;
//                                    pzs1.相关人编号 = pz.相关人编号;
//                                    pzs1.业务类型编号 = string.IsNullOrEmpty(kvp1.Key) ? null : Feng.Utils.ConvertHelper.ToInt(kvp1.Key);

//                                    pzs1.费用 = new List<费用>();

//                                    foreach (System.Data.DataRow feeRow in feeRows[kvp1.Key][kvp.Key])
//                                    {
//                                        if (Convert.ToBoolean(feeRow["是否业务费用"]))
//                                        {
//                                            业务费用 ywfy = new 业务费用();

//                                            ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                                            ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                                            ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                                            ywfy.费用项编号 = pzs1.费用项编号;
//                                            ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                                            ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                                            ywfy.费用信息 = null;
//                                            ywfy.金额 = (decimal)feeRow["金额"];
//                                            ywfy.凭证费用明细 = pzs1;

//                                            ywfy.相关人编号 = pz.相关人编号;
//                                            ywfy.箱Id = feeRow["业务类型"].ToString() == "11" ? (feeRow["进口箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["进口箱Id"])
//                                                : (feeRow["内贸出港箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["内贸出港箱Id"]);

//                                            (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                                            pzs1.费用.Add(ywfy);
//                                        }
//                                        else
//                                        {
//                                            非业务费用 fywfy = new 非业务费用();

//                                            fywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                                            fywfy.费用实体 = rep.Get<费用实体>(feeRow["票Id"]);

//                                            fywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                                            fywfy.费用项编号 = pzs1.费用项编号;
//                                            fywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(fywfy.费用项编号);
//                                            fywfy.费用类别编号 = fywfy.收付标志 == 收付标志.收 ? fywfy.费用项.收入类别 : fywfy.费用项.支出类别;

//                                            fywfy.金额 = (decimal)feeRow["金额"];
//                                            fywfy.凭证费用明细 = pzs1;

//                                            fywfy.相关人编号 = pz.相关人编号;

//                                            (new HdBaseDao<非业务费用>()).Save(rep, fywfy);

//                                            pzs1.费用.Add(fywfy);
//                                        }
//                                    }

//                                    (new HdBaseDao<凭证费用明细>()).Save(rep, pzs1);
//                                }
//                            }

//                            //凭证收支明细 pzs2 = new 凭证收支明细();
//                            //pzs2.凭证 = pz;
//                            //pzs2.金额 = pz.会计金额;
//                            //pzs2.收付标志 = is付款 ? 收付标志.付 : 收付标志.收;

//                            //switch (row["收付方式"].ToString())
//                            //{
//                            //    case "1":
//                            //        pzs2.收付款方式 = 收付款方式.现金;
//                            //        break;
//                            //    case "2":
//                            //        pzs2.收付款方式 = 收付款方式.现金支票;
//                            //        pzs2.票据号码 = row["票号"].ToString();
//                            //        break;
//                            //    case "3":
//                            //        pzs2.收付款方式 = 收付款方式.转账支票;
//                            //        pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                            //        break;
//                            //    case "4":
//                            //        pzs2.收付款方式 = 收付款方式.银行本票汇票;
//                            //        pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                            //        break;
//                            //    case "5":
//                            //        pzs2.收付款方式 = 收付款方式.银行承兑汇票;
//                            //        pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.出票银行 = Get出票银行From票号(row["票号"].ToString());
//                            //        pzs2.承兑期限 = Get承兑期限From票号(row["票号"].ToString());
//                            //        break;
//                            //    case "6":
//                            //        pzs2.收付款方式 = 收付款方式.电汇;
//                            //        pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                            //        break;
//                            //    case "7":
//                            //        pzs2.收付款方式 = 收付款方式.银行收付;
//                            //        //pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                            //        break;
//                            //    case "8":
//                            //        pzs2.收付款方式 = 收付款方式.银行收付;
//                            //        //pzs2.票据号码 = row["票号"].ToString();
//                            //        pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(row["银行账号"].ToString());
//                            //        break;
//                            //}

//                            //(new HdBaseDao<凭证收支明细>()).Save(rep, pzs2);

//                            rep.CommitTransaction();
//                        }
//                        catch (Exception ex)
//                        {
//                            rep.RollbackTransaction();
//                            Feng.Windows.Forms.MessageForm.ShowError("凭证号 " + pzh + "转换出现问题");
//                            ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                        }
//                    }
//                }
//            }
//        }

//        public static void Convert工资(string pzhs)
//        {
//            string[] pzhss = pzhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string pzh in pzhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.财务_会计凭证表 WHERE 凭证号 = '" + pzh + "'");
//                foreach (System.Data.DataRow row in dt.Rows)
//                {
//                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<工资单>())
//                    {
//                        try
//                        {
//                            rep.BeginTransaction();

//                            凭证 pz = new 凭证();
//                            pz.Submitted = true;
//                            pz.备注 = "原凭证号: " + pzh + System.Environment.NewLine + row["备注"].ToString();
//                            pz.出纳编号 = "100000";
//                            pz.审核状态 = true;
//                            pz.收支状态 = true;
//                            pz.会计编号 = "100000";
//                            pz.会计金额 = (decimal)row["金额"];
//                            pz.金额.币制编号 = "CNY";
//                            pz.金额.数额 = pz.会计金额.Value;
//                            pz.凭证类别 = 凭证类别.付款凭证;
//                            pz.日期 = (DateTime)row["出纳时间"];
//                            pz.审核人编号 = row["审核人"] == System.DBNull.Value ? null : (string)row["审核人"];
//                            pz.相关人编号 = row["相关人"] == System.DBNull.Value ? null : (string)row["相关人"];
//                            pz.自动手工标志 = 自动手工标志.手工;
//                            //pz.凭证号 = PrimaryMaxIdGenerator.GetMaxId("财务_凭证", "凭证号", 8, PrimaryMaxIdGenerator.GetIdYearMonth(pz.日期)).ToString();

//                            (new HdBaseDao<凭证>()).Save(rep, pz);
//                            //rep.Save(pz);

//                            System.Data.DataTable dtGzd = DbHelper.Instance.ExecuteDataTable(@"SELECT * FROM HdTest.dbo.货代财务_员工工资表 A WHERE 工资编号 = (SELECT TOP 1 主表编号 FROM HdTest.dbo.货代财务_费用登记表 WHERE 凭证号 = '" + pzh + "')");
//                            工资单 gzd = new 工资单();
//                            gzd.Submitted = true;
//                            gzd.登记金额 = pz.会计金额;
//                            gzd.基本工资 = pz.会计金额;
//                            gzd.员工编号 = pz.相关人编号;
//                            gzd.凭证号 = pz.凭证号;
//                            gzd.简介 = ((DateTime)dtGzd.Rows[0]["月份"]).ToString("yyyy年MM月");
//                            gzd.备注 = dtGzd.Rows[0]["备注"] == System.DBNull.Value ? null : dtGzd.Rows[0]["备注"].ToString();
//                            (new HdBaseDao<工资单>()).Save(gzd);

//                            凭证费用明细 pzs1 = new 凭证费用明细();
//                            pzs1.费用项编号 = "341";
//                            pzs1.金额 = pz.会计金额;
//                            pzs1.凭证 = pz;
//                            pzs1.收付标志 = 收付标志.付;
//                            pzs1.相关人编号 = pz.相关人编号;
//                            pzs1.费用 = new List<费用>();
//                            pzs1.业务类型编号 = null;

//                            非业务费用 fywfy = new 非业务费用();
//                            fywfy.费用实体 = gzd;
//                            fywfy.收付标志 = 收付标志.付;
//                            fywfy.费用项编号 = "341";
//                            fywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(fywfy.费用项编号);
//                            fywfy.费用类别编号 = fywfy.收付标志 == 收付标志.收 ? fywfy.费用项.收入类别 : fywfy.费用项.支出类别;
//                            fywfy.金额 = pz.会计金额;
//                            fywfy.凭证费用明细 = pzs1;
//                            fywfy.相关人编号 = pz.相关人编号;
//                            (new HdBaseDao<非业务费用>()).Save(rep, fywfy);

//                            pzs1.费用.Add(fywfy);
//                            (new HdBaseDao<凭证费用明细>()).Save(rep, pzs1);

//                            凭证收支明细 pzs2 = new 凭证收支明细();
//                            pzs2.凭证 = pz;
//                            pzs2.金额 = pz.会计金额;
//                            pzs2.收付标志 = 收付标志.付;
//                            pzs2.收付款方式 = 收付款方式.现金;
//                            (new HdBaseDao<凭证收支明细>()).Save(rep, pzs2);

//                            rep.CommitTransaction();
//                        }
//                        catch (Exception ex)
//                        {
//                            rep.RollbackTransaction();
//                            Feng.Windows.Forms.MessageForm.ShowError("凭证号 " + pzh + "转换出现问题");
//                            ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                        }
//                    }
//                }
//            }
//        }

//        public static void Convert贴息费(string pzhs)
//        {
//            string[] pzhss = pzhs.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
//            foreach (string pzh in pzhss)
//            {
//                System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.财务_会计凭证表 WHERE 凭证号 = '" + pzh + "'");
//                foreach (System.Data.DataRow row in dt.Rows)
//                {
//                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<凭证>())
//                    {
//                        try
//                        {
//                            rep.BeginTransaction();

//                            凭证 pz = new 凭证();
//                            pz.Submitted = true;
//                            pz.备注 = "原凭证号: " + pzh + System.Environment.NewLine + row["备注"].ToString();
//                            pz.出纳编号 = "100000";
//                            pz.审核状态 = true;
//                            pz.收支状态 = true;
//                            pz.会计编号 = "100000";
//                            pz.会计金额 = (decimal)row["金额"];
//                            pz.金额.币制编号 = "CNY";
//                            pz.金额.数额 = pz.会计金额.Value;
//                            pz.凭证类别 = 凭证类别.付款凭证;
//                            pz.日期 = (DateTime)row["出纳时间"];
//                            pz.审核人编号 = row["审核人"] == System.DBNull.Value ? null : (string)row["审核人"];
//                            pz.相关人编号 = row["相关人"] == System.DBNull.Value ? null : (string)row["相关人"];
//                            if (pz.相关人编号 == "990800")
//                            { pz.相关人编号 = "900001"; }
//                            pz.自动手工标志 = 自动手工标志.承兑汇票;

//                            (new HdBaseDao<凭证>()).Save(rep, pz);

//                            System.Data.DataTable dtFees = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_视图业务费用凭证核销 WHERE 凭证号 = '" + row["凭证号"].ToString() + "'");
//                            Dictionary<string, decimal> feeMoney = new Dictionary<string, decimal>();
//                            Dictionary<string, IList<System.Data.DataRow>> feeRows = new Dictionary<string, IList<System.Data.DataRow>>();
//                            foreach (System.Data.DataRow row1 in dtFees.Rows)
//                            {
//                                string s = row1["费用项"].ToString();
//                                if (!feeMoney.ContainsKey(s))
//                                {
//                                    feeMoney[s] = 0m;
//                                    feeRows[s] = new List<System.Data.DataRow>();
//                                }
//                                feeMoney[s] += (decimal)row1["金额"];
//                                feeRows[s].Add(row1);
//                            }

//                            foreach (KeyValuePair<string, decimal> kvp in feeMoney)
//                            {
//                                if (!feeMappings.ContainsKey(kvp.Key))
//                                {
//                                    throw new ArgumentException(kvp.Key + "is invalid");
//                                }
//                                凭证费用明细 pzs1 = new 凭证费用明细();
//                                pzs1.费用项编号 = feeMappings[kvp.Key];
//                                pzs1.金额 = kvp.Value;
//                                pzs1.凭证 = pz;
//                                pzs1.收付标志 = 收付标志.付;
//                                pzs1.相关人编号 = pz.相关人编号;
//                                pzs1.业务类型编号 = null;

//                                pzs1.费用 = new List<费用>();

//                                foreach (System.Data.DataRow feeRow in feeRows[kvp.Key])
//                                {
//                                    if (Convert.ToBoolean(feeRow["是否业务费用"]))
//                                    {
//                                        业务费用 ywfy = new 业务费用();

//                                        ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                                        ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                                        ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                                        ywfy.费用项编号 = pzs1.费用项编号;
//                                        ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                                        ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                                        ywfy.费用信息 = null;
//                                        ywfy.金额 = (decimal)feeRow["金额"];
//                                        ywfy.凭证费用明细 = pzs1;

//                                        ywfy.相关人编号 = pz.相关人编号;
//                                        ywfy.箱Id = feeRow["箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["箱Id"];

//                                        (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                                        pzs1.费用.Add(ywfy);
//                                    }
//                                    else
//                                    {
//                                        非业务费用 fywfy = new 非业务费用();

//                                        fywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                                        fywfy.费用实体 = rep.Get<费用实体>(feeRow["票Id"]);

//                                        fywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                                        fywfy.费用项编号 = pzs1.费用项编号;
//                                        fywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(fywfy.费用项编号);
//                                        fywfy.费用类别编号 = fywfy.收付标志 == 收付标志.收 ? fywfy.费用项.收入类别 : fywfy.费用项.支出类别;

//                                        fywfy.金额 = (decimal)feeRow["金额"];
//                                        fywfy.凭证费用明细 = pzs1;

//                                        fywfy.相关人编号 = pz.相关人编号;

//                                        (new HdBaseDao<非业务费用>()).Save(rep, fywfy);

//                                        pzs1.费用.Add(fywfy);
//                                    }
//                                }

//                                (new HdBaseDao<凭证费用明细>()).Save(rep, pzs1);
//                            }

//                            // 承兑汇票
//                            System.Diagnostics.Debug.Assert(dt.Rows.Count == 1, "");
//                            System.Data.DataTable dtCdhp = DbHelper.Instance.ExecuteDataTable(@"SELECT * FROM HdTest.dbo.财务_承兑汇票进出表 A WHERE ID = (SELECT 主表编号 FROM HdTest.dbo.货代财务_费用登记表 WHERE 凭证号 = '" + pzh + "')");
//                            凭证收支明细 pzs2 = new 凭证收支明细();
//                            pzs2.凭证 = pz;
//                            pzs2.金额 = Convert.ToDecimal(dtCdhp.Rows[0]["金额"]);
//                            pzs2.收付标志 = 收付标志.付;
//                            pzs2.收付款方式 = 收付款方式.银行承兑汇票;
//                            pzs2.票据号码 = dtCdhp.Rows[0]["承兑汇票号码"].ToString();
//                            pzs2.出票银行 = Get出票银行From票号(dtCdhp.Rows[0]["承兑汇票号码"].ToString());
//                            pzs2.承兑期限 = Get承兑期限From票号(dtCdhp.Rows[0]["承兑汇票号码"].ToString());
//                            (new HdBaseDao<凭证收支明细>()).Save(rep, pzs2);

//                            pzs2 = new 凭证收支明细();
//                            pzs2.凭证 = pz;
//                            pzs2.金额 = Convert.ToDecimal(dtCdhp.Rows[0]["金额"]) - Convert.ToDecimal(dt.Rows[0]["金额"]);
//                            pzs2.收付标志 = 收付标志.收;

//                            if (dtCdhp.Rows[0]["返回方式"].ToString() == "1")
//                            {
//                                pzs2.收付款方式 = 收付款方式.现金;
//                            }
//                            else
//                            {
//                                pzs2.收付款方式 = 收付款方式.银行收付;
//                                //pzs2.票据号码 = row["票号"].ToString();
//                                pzs2.银行账户编号 = Get银行账户IdFrom银行账户编号(dtCdhp.Rows[0]["返回银行账号"].ToString());
//                            }
//                            (new HdBaseDao<凭证收支明细>()).Save(rep, pzs2);

//                            rep.CommitTransaction();
//                        }
//                        catch (Exception ex)
//                        {
//                            rep.RollbackTransaction();
//                            Feng.Windows.Forms.MessageForm.ShowError("凭证号 " + pzh + "转换出现问题");
//                            ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                        }
//                    }
//                }
//            }
//        }



//        public static void Convert票号To银行账户()
//        {
//            using (Feng.NH.Repository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用>() as Feng.NH.Repository)
//            {
//                IList<凭证收支明细> list = rep.Session.CreateCriteria<凭证收支明细>()
//                    .Add(NHibernate.Criterion.Expression.Eq("收付款方式", 收付款方式.银行收付))
//                    .Add(NHibernate.Criterion.Expression.IsNull("银行账户编号"))
//                    .List<凭证收支明细>();

//                try
//                {
//                    rep.BeginTransaction();

//                    foreach (凭证收支明细 item in list)
//                    {
//                        rep.Initialize(item.凭证, item);
//                        System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT 银行账号 FROM HdTest.dbo.财务_会计凭证表 WHERE 凭证号 = '" + item.凭证.备注.Substring(6, 12) + "'");

//                        item.银行账户编号 = Get银行账户IdFrom银行账户编号(dt.Rows[0][0].ToString());

//                        rep.Update(item);
//                    }
//                    rep.CommitTransaction();
//                }
//                catch (Exception ex)
//                {
//                    rep.RollbackTransaction();
//                    ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                }
//            }
//        }

//        private static string Get出票银行From票号(string ph)
//        {
//            string s = "SELECT 名称 FROM HdParameters.dbo.信息_银行 WHERE 缩写 = (SELECT 出票银行 FROM HdTest.dbo.财务_承兑汇票进出表 WHERE 承兑汇票号码 = '" + ph + "')";
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable(s);
//            if (dt.Rows.Count > 0)
//                return (string)dt.Rows[0][0];
//            else
//                return null;
//        }
//        private static DateTime Get承兑期限From票号(string ph)
//        {
//            string s = "SELECT 承兑期限 FROM HdTest.dbo.财务_承兑汇票进出表 WHERE 承兑汇票号码 = '" + ph + "'";
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable(s);
//            if (dt.Rows.Count > 0)
//                return (DateTime)dt.Rows[0][0];
//            else
//                return DateTime.MinValue;
//        }

//        private static Guid? Get银行账户IdFrom银行账户编号(string bh)
//        {
//            string s = "SELECT Id FROM jkhd2.dbo.参数备案_银行账户 A WHERE 账号 = (SELECT 账号 FROM HdTest.dbo.财务_银行账号 WHERE 编号 = '" + bh + "')";
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable(s);
//            if (dt.Rows.Count > 0)
//                return (Guid)dt.Rows[0][0];
//            else
//                return null;
//        }

//        //private static string GetFeeNameFromFeeCode(string feeCode)
//        //{
//        //    static System.Data.DataTable tbFyx = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.信息_费用参数");
//        //    System.Data.DataRow[] rows = tbFyx.Select("代码 = '" + feeCode.Substring(5, 3);
//        //    return rows[0]["名称"].ToString();
//        //}

//        public static void GenerateFyxx()
//        {
//            BaseDao<费用信息> m_dao = new HdBaseDao<费用信息>();
//            while (true)
//            {
//                using (Feng.NH.Repository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用>() as Feng.NH.Repository)
//                {
//                    IList<业务费用> listSrc = rep.Session.CreateCriteria<业务费用>()
//                        .Add(NHibernate.Criterion.Expression.IsNull("费用信息"))
//                        .SetMaxResults(1000)
//                        .List<业务费用>();

//                    if (listSrc.Count == 0)
//                        break;

//                    try
//                    {
//                        rep.BeginTransaction();

//                        int cnt = 0;
//                        foreach (业务费用 entity in listSrc)
//                        {
//                            cnt++;

//                            entity.费用项 = EntityBufferCollection.Instance.Get<费用项>(entity.费用项编号);
//                            entity.费用类别编号 = entity.收付标志 == 收付标志.收 ? entity.费用项.收入类别 : entity.费用项.支出类别;
//                            if (!entity.费用类别编号.HasValue)
//                            {
//                                throw new InvalidUserOperationException("您选择的费用项和收付有误，请重新选择！");
//                            }

//                            entity.费用类别 = EntityBufferCollection.Instance.Get<费用类别>(entity.费用类别编号) as 费用类别;

//                            费用信息 fyxx = null;
//                            if (entity.费用类别.大类 == "业务额外"
//                                || entity.费用类别.大类 == "业务常规")
//                            {
//                                IList<费用信息> list = rep.Session.CreateCriteria(typeof(费用信息))
//                                    .Add(NHibernate.Criterion.Expression.Eq("费用项编号", entity.费用项编号))
//                                    .Add(NHibernate.Criterion.Expression.Eq("票.Id", entity.票.Id))
//                                    .List<费用信息>();
//                                if (list.Count == 0)
//                                {
//                                    费用信息 item = new 费用信息();
//                                    item.票Id = entity.票.Id;
//                                    item.费用项编号 = entity.费用项编号;
//                                    item.业务类型编号 = entity.票.费用实体类型编号;

//                                    m_dao.Save(item);

//                                    fyxx = item;
//                                }
//                                else if (list.Count == 1)
//                                {
//                                    fyxx = list[0];
//                                }
//                                else
//                                {
//                                    System.Diagnostics.Debug.Assert(false, "费用信息对同一费用主体同一费用项有多条！");
//                                }
//                            }

//                            if (entity.费用信息 == null)
//                            {
//                                entity.费用信息 = fyxx;
//                                rep.Update(entity);
//                            }
//                        }
//                        rep.CommitTransaction();
//                    }
//                    catch (Exception)
//                    {
//                        rep.RollbackTransaction();
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }


//                }
//            }
//        }

//        public static void Convert调节款()
//        {
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_调节款");
//            foreach (System.Data.DataRow row in dt.Rows)
//            {
//                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<调节款>())
//                {
//                    try
//                    {
//                        rep.BeginTransaction();

//                        //调节款 tjk = new 调节款();
//                        //(new HdBaseDao<调节款>()).Save(rep, tjk);

//                        调节款 mx = new 调节款();
//                        // mx.调节款 = tjk;
//                        mx.费用项编号 = (string)row["费用项"];
//                        mx.结算期限 = (DateTime)row["结算期限"];
//                        mx.金额 = (decimal)row["金额"];
//                        mx.日期 = (DateTime)row["日期"];
//                        mx.收付标志 = row["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                        mx.相关人编号 = (string)row["相关人"];
//                        mx.业务类型编号 = (int)row["业务类型"];

//                        if (mx.相关人编号 == "990800")
//                        { mx.相关人编号 = "900001"; }

//                        (new HdBaseDao<调节款>()).Save(rep, mx);

//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();

//                        Console.WriteLine(ex.Message);
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }
//        }


//        public static void Convert业务费用收款未处理()
//        {
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_视图业务费用收款未处理");
//            foreach (System.Data.DataRow feeRow in dt.Rows)
//            {
//                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
//                {
//                    try
//                    {
//                        rep.BeginTransaction();


//                        业务费用 ywfy = new 业务费用();

//                        ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                        ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                        ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                        ywfy.费用项编号 = feeMappings[feeRow["费用项"].ToString()];
//                        ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                        ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                        ywfy.费用信息 = null;
//                        ywfy.金额 = (decimal)feeRow["金额"];
//                        ywfy.凭证费用明细 = null;

//                        ywfy.相关人编号 = feeRow["相关人"].ToString();
//                        ywfy.箱Id = feeRow["业务类型"].ToString() == "11" ? (feeRow["进口箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["进口箱Id"])
//                            : (feeRow["内贸出港箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["内贸出港箱Id"]);

//                        if (ywfy.相关人编号 == "990800")
//                        { ywfy.相关人编号 = "900001"; }

//                        (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();
//                        Console.WriteLine(ex.Message);
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }
//        }

//        public static void Convert业务费用付款未处理()
//        {
//            System.Data.DataTable dt = DbHelper.Instance.ExecuteDataTable("SELECT * FROM HdTest.dbo.B版_视图业务费用付款未处理");
//            foreach (System.Data.DataRow feeRow in dt.Rows)
//            {
//                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<业务费用>())
//                {
//                    try
//                    {
//                        rep.BeginTransaction();


//                        业务费用 ywfy = new 业务费用();

//                        ywfy.备注 = feeRow["备注"] == System.DBNull.Value ? null : (string)feeRow["备注"];

//                        ywfy.费用实体 = rep.Get<普通票>(feeRow["票Id"]);

//                        ywfy.收付标志 = feeRow["收付标志"].ToString() == "1" ? 收付标志.付 : 收付标志.收;
//                        ywfy.费用项编号 = feeMappings[feeRow["费用项"].ToString()];
//                        ywfy.费用项 = EntityBufferCollection.Instance.Get<费用项>(ywfy.费用项编号);
//                        ywfy.费用类别编号 = ywfy.收付标志 == 收付标志.收 ? ywfy.费用项.收入类别 : ywfy.费用项.支出类别;

//                        ywfy.费用信息 = null;
//                        ywfy.金额 = (decimal)feeRow["金额"];
//                        ywfy.凭证费用明细 = null;

//                        ywfy.相关人编号 = feeRow["相关人"].ToString();
//                        ywfy.箱Id = feeRow["业务类型"].ToString() == "11" ? (feeRow["进口箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["进口箱Id"])
//                            : (feeRow["内贸出港箱Id"] == System.DBNull.Value ? null : (Guid?)feeRow["内贸出港箱Id"]);

//                        if (ywfy.相关人编号 == "990800")
//                        { ywfy.相关人编号 = "900001"; }

//                        (new HdBaseDao<业务费用>()).Save(rep, ywfy);

//                        rep.CommitTransaction();
//                    }
//                    catch (Exception ex)
//                    {
//                        rep.RollbackTransaction();
//                        Console.WriteLine(ex.Message);
//                        //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
//                    }
//                }
//            }
//        }

//        //public static void Convert费用信息2To费用信息()
//        //{
//        //    StringBuilder sb = new StringBuilder();
//        //    int idx = 0;
//        //    while (true)
//        //    {
//        //        using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用信息>())
//        //        {
//        //            try
//        //            {
//        //                rep.BeginTransaction();

//        //                IList<费用信息2> list = rep.Session.CreateCriteria<费用信息2>()
//        //                    .SetFirstResult(idx)
//        //                    .SetMaxResults(100)
//        //                    .List<费用信息2>();

//        //                if (list.Count == 0)
//        //                    break;

//        //                idx += 100;

//        //                foreach (费用信息2 i in list)
//        //                {
//        //                    IList<费用项> fees = rep.Session.CreateCriteria<费用项>()
//        //                        .Add(NHibernate.Criterion.Expression.Or(
//        //                            NHibernate.Criterion.Expression.Eq("收入类别", i.费用类别编号),
//        //                            NHibernate.Criterion.Expression.Eq("支出类别", i.费用类别编号)))
//        //                        .List<费用项>();

//        //                    foreach (费用项 f in fees)
//        //                    {
//        //                        费用信息 j = new 费用信息();
//        //                        j.IsActive = i.IsActive;
//        //                        j.Submitted = i.Submitted;
//        //                        j.备注 = i.备注;
//        //                        j.费用项编号 = f.编号;
//        //                        j.票Id = i.票Id;
//        //                        j.业务类型编号 = i.业务类型编号;
//        //                        (new HdBaseDao<费用信息>()).Save(rep, j);
//        //                    }
//        //                }

//        //                rep.CommitTransaction();
//        //            }
//        //            catch (Exception ex)
//        //            {
//        //                rep.RollbackTransaction();
//        //                sb.Append(ex.Message);
//        //                sb.Append("  From" + idx);
//        //                sb.Append(System.Environment.NewLine);
//        //            }
//        //        }
//        //    }
//        //    Feng.Windows.Forms.MessageForm.ShowError(sb.ToString());
//        //}
//    }
//}
