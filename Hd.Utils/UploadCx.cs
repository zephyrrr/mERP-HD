using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web.Services;
using System.Web.Services.Protocols;
using Feng;
using Feng.Windows.Utils;

namespace Hd.Utils
{
    /// <summary>
    /// 提交查询数据
    /// </summary>
    public class UploadCx
    {
        public UploadCx(string address)
            : this()
        {
            m_serverAddress = address;
        }

        public UploadCx()
        {
            //m_webProxy.Credentials = new System.Net.NetworkCredential("administrator", "");
            //m_webProxy.Encoding = System.Text.Encoding.Unicode;
        }

        private string m_serverAddress;
        public string ServerAddress
        {
            get { return m_serverAddress; }
            set { m_serverAddress = value; }
        }
        private Feng.Net.WebProxy m_webProxy = new Feng.Net.WebProxy();

        private int clientId = 0;
        public DateTime? GetLastUpdateDate()
        {
            string s = m_webProxy.GetToString("http://" + m_serverAddress + "/UploadCx/GetLastUploadDate.ashx?clientid=" + clientId);
            DateTime? d = Feng.Utils.ConvertHelper.ToDateTime(s);
            return d;
        }

        public void UpdateData()
        {
            UpdateData(null, null, null);
        }

        public void UpdateData(DateTime? dateBegin, DateTime? dateEnd, int? type)
        {
            //System.Windows.Forms.MessageBox.Show("fd");

            if (!type.HasValue)
            {
                type = 1;
            }
            if (!dateBegin.HasValue)
            {
                dateBegin = GetLastUpdateDate();
            }
            if (!dateEnd.HasValue)
            {
                dateEnd = System.DateTime.Today;
            }
            if (dateBegin.HasValue)
            {
                string fileName = GenerateDate(dateBegin.Value, dateEnd.Value, type.Value, clientId);
                byte[] data;
                using (FileStream sr = new FileStream(fileName, FileMode.Open))
                {
                    data = new byte[sr.Length];
                    sr.Read(data, 0, (int)sr.Length);
                }

                //SoapHttpClientProtocol client = new SoapHttpClientProtocol();
                //client.Url = "http://" + m_serverAddress + "/UploadCx/UploadCxFile.asmx";
                //client.Invoke("UploadFile", new object[] { fileName, data });

                try
                {
                    UploadCxService.UploadCxFileSoapClient client = new Hd.Utils.UploadCxService.UploadCxFileSoapClient(
                        new System.ServiceModel.BasicHttpBinding(), new System.ServiceModel.EndpointAddress("http://" + m_serverAddress + "/UploadCx/UploadCxFile.asmx"));

                    client.UploadFile(fileName, data);
                }
                catch (Exception)
                {
                    //ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                }
            }
        }

        public void UpdateDataUsePut(DateTime? dateBegin, DateTime? dateEnd, int? type)
        {
            //m_webProxy.PutToString("http://" + m_serverAddress + "/UploadCx/a.txt", data);

            // 在Neokernel中不可用此种方式
            if (!type.HasValue)
            {
                type = 1;
            }
            if (!dateBegin.HasValue)
            {
                dateBegin = GetLastUpdateDate();
            }
            if (!dateEnd.HasValue)
            {
                dateEnd = System.DateTime.Today;
            }
            if (dateBegin.HasValue)
            {
                string fileName = GenerateDate(dateBegin.Value, dateEnd.Value, type.Value, clientId);
                byte[] data;
                using (FileStream sr = new FileStream(fileName, FileMode.Open))
                {
                    data = new byte[sr.Length];
                    sr.Read(data, 0, (int)sr.Length);
                }
                m_webProxy.PutToString("http://" + m_serverAddress + "/UploadCx/UploadCxFile.ashx?fileName=" + fileName, data);
            }
        }

        public string GenerateDate(DateTime dateBegin, DateTime dateEnd, int type, int cliendId)
        {
            string d1 = dateBegin.ToString("yyyy-MM-dd");
            string d2 = dateEnd.AddDays(1).ToString("yyyy-MM-dd");
            string whereSql = "((A.Created >= '" + d1 + "' OR A.Updated >= '" + d1 + "') AND (A.Created <= '" + d2 + "' OR A.Updated <= '" + d2 + "') AND ClientId = " + cliendId + ")";
            string whereSql2 = "(A.Created >= '" + d1 + "' AND A.Created <= '" + d2 + "')";

            if (type == 1)
            {
                string[] sqls = new string[] {
                "SELECT A.ID, A.Version, A.编号, A.费用实体类型, A.Submitted, A.CreatedBy, A.Created, A.UpdatedBy, A.Updated, A.ClientId, A.OrgId, A.IsActive FROM 财务_费用实体 A WHERE (A.费用实体类型 = 11 OR A.费用实体类型 = 15 OR A.费用实体类型 = 45) AND " + whereSql,
                "SELECT B.ID, B.货代自编号, B.委托时间, B.委托人, B.提单号, B.代表性箱号, B.合同号, B.报检号, B.报关单号, B.船公司, B.船名航次, B.箱量, B.标箱量, B.货物类别, B.件数, B.单价, B.总重量, B.内部备注, B.对上备注, B.对下备注, B.允许应收对账 FROM 业务备案_普通票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_内贸出港票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口其他业务票 B INNER JOIN 财务_费用实体 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 业务备案_普通箱 A WHERE " + whereSql,
                "SELECT B.* FROM 业务备案_进口箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_内贸出港箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务备案_进口其他业务箱 B INNER JOIN 业务备案_普通箱 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 业务过程_进口票_转关标志 A WHERE " + whereSql,
                "SELECT B.* FROM 业务过程_进口票_转关 B INNER JOIN 业务过程_进口票_转关标志 A ON A.ID = B.ID AND " + whereSql,
                "SELECT B.* FROM 业务过程_进口票_清关 B INNER JOIN 业务过程_进口票_转关标志 A ON A.ID = B.ID AND " + whereSql,
                "SELECT A.* FROM 参数备案_币制 A WHERE " + whereSql,
                "SELECT A.* FROM 参数备案_人员单位 A WHERE " + whereSql,
                "SELECT A.* FROM 参数备案_箱型 A WHERE " + whereSql,
                "SELECT A.* FROM 信息_角色用途 A WHERE " + whereSql,
                "SELECT A.* FROM 信息_费用类别 A WHERE " + whereSql/*,
                "SELECT A.* FROM SD_AuditLogRecord A WHERE " + whereSql2*/};

                string[] dbTables = new string[] {
                "财务_费用实体", 
                "业务备案_普通票",
                "业务备案_进口票", 
                "业务备案_内贸出港票", 
                "业务备案_进口其他业务票", 
                "业务备案_普通箱", 
                "业务备案_进口箱", 
                "业务备案_内贸出港箱",
                "业务备案_进口其他业务箱", 
                "业务过程_进口票_转关标志",
                "业务过程_进口票_转关",
                "业务过程_进口票_清关",
                "参数备案_币制",
                "参数备案_人员单位", 
                "参数备案_箱型", 
                "信息_角色用途", 
                "信息_费用类别",
                /*"SD_AuditLogRecord"*/};

                string fileName = cliendId + "-" + type + "-" + d1 + "-" + d2 + ".xml";
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8);
                ExcelXmlHelper.WriteExcelXmlHead(sw);

                if (dbTables.Length != sqls.Length)
                {
                    throw new ArgumentException("length should be equal!");
                }
                for (int i = 0; i < sqls.Length; ++i)
                {
                    ExcelXmlHelper.WriteExcelXml(Feng.Data.DbHelper.Instance.ExecuteDataTable(sqls[i]), sw, dbTables[i]);
                }
                ExcelXmlHelper.WriteExcelXmlTail(sw);
                sw.Close();

                string zipFileName = System.IO.Path.ChangeExtension(fileName, ".zip");

                CompressionHelper.ZipFromFile(fileName, zipFileName);

                return zipFileName;
            }
            else
            {
                throw new ArgumentException("Invalid type!");
            }
        }
    }
}
