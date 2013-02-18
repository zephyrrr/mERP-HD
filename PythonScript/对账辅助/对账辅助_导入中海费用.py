# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("System.Data")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;

def execute(masterForm):
    ofg = System.Windows.Forms.OpenFileDialog();
    ofg.RestoreDirectory = True;
    ofg.Filter = "所有 Excel 文件|*.xls;*.xlsx;*.xml|Excel 97-2003 文件|*.xls|Excel 2007 文件|*.xlsx|XML 文件|*.xml";
    if (ofg.ShowDialog() == System.Windows.Forms.DialogResult.OK):
        try:
            dts = Feng.Utils.ExcelHelper.ReadExcel(ofg.FileName);
            if (dts.Count == 0):
                raise Feng.InvalidUserOperationException("Excel中无数据！");
            dt = dts[0];
            if (not dt.Columns.Contains("提单编号") or not dt.Columns.Contains("箱号") or not dt.Columns.Contains("费用类型") \
                or not dt.Columns.Contains("免费天数") or not dt.Columns.Contains("币种") or not dt.Columns.Contains("实际金额")):
                raise Feng.InvalidUserOperationException("文件中必须要有提单编号、箱号、费用类型、币种、实际金额栏，请查证！");
            errorDt = dt.Clone();
            db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
            tran = db.BeginTransaction();
            count = 0;
            try:
                for row in dt.Rows:
                    fyx = '';je = 0;
                    if (row["费用类型"].ToString() == "XXF"): fyx = "165";
                    elif (row["费用类型"].ToString() == "ZX"): fyx = "167";
                    else: raise Feng.InvalidUserOperationException("费用类型不能是" + row["费用类型"].ToString() + "!");
                    if (row["币种"].ToString() == "RMB"): je = Feng.Utils.ConvertHelper.ToDecimal(row["实际金额"]);
                    elif (row["币种"].ToString() == "USD"): je = 7 * Feng.Utils.ConvertHelper.ToDecimal(row["实际金额"]);
                    else: raise Feng.InvalidUserOperationException("币种不能是" + row["币种"].ToString() + "!");
                    db.ExecuteNonQuery("insert Temp_对账单费用查验(相关人,提单号,箱号,费用项,金额) values ('900010','" + str(row["提单编号"]) + "','" \
                                       + str(row["箱号"]) + "','" + str(fyx) + "'," + str(je) + ")");
                    count = count + 1;
                db.CommitTransaction(tran);
                Feng.MessageForm.ShowInfo("已导入" + str(count) + "条数据！");
            except System.Exception, ex:
                db.RollbackTransaction(tran);
                errorDt.ImportRow(row);
                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(System.Exception('提单号' + row["提单编号"] + '数据有问题，请查证!', ex));
            if (errorDt.Rows.Count > 0):
                errorFileName =  System.IO.Path.GetDirectoryName(ofg.FileName) + '\\' + \
                                System.IO.Path.GetFileName(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".xml"));
                Feng.Utils.ExcelXmlHelper.WriteExcelXml(errorDt, errorFileName);
                Feng.MessageForm.ShowInfo("请查看错误文件" + errorFileName);
        except System.Exception, ex:
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);
                

