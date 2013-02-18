using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Utils;
using Feng.Windows.Forms;

namespace Hd.Report
{
    //public sealed class ReportProcess
    //{
    //    private ReportProcess()
    //    {
    //    }

    //    public static void ProcessPz(System.Data.DataSet ds, Dictionary<string, IEnumerable> data)
    //    {
    //        int i = 0;
    //        foreach (object entity in data["凭证"])
    //        {
    //            Hd.Model.凭证 pz = entity as Hd.Model.凭证;

    //            ds.Tables["凭证"].Rows[i]["大写金额"] = ChineseHelper.ConvertToChinese(pz.金额.数额.Value);
    //            ds.Tables["凭证"].Rows[i]["金额.币制编号"] = NameValueMappingCollection.Instance.FindColumn2FromColumn1("财务_币制", "代码", "符号", pz.金额.币制编号);

    //            i++;
    //        }
    //    }
    //}
}
