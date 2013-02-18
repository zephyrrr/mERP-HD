# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
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
            if (dt.Columns.Count != 17):
                raise Feng.InvalidUserOperationException("文件栏目格式不符，请查证！");
            errorDt = dt.Clone();
            db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
            tran = db.BeginTransaction();
            count = 0;
            try:
                for row in dt.Rows:
                    fyxs = ('161');                    
                    for i in range(0, 1, 1):
                        fyx = '';je = 0;
                        je = Feng.Utils.ConvertHelper.ToDecimal(row[13]);
                        if (je == None):
                            continue;
                        fyx = fyxs[i];
                        db.ExecuteNonQuery("insert Temp_对账单费用查验(相关人,提单号,箱号,费用项,金额) values ('900003','" + str(row[4]) + "','" \
                                           + str(row[1]) + "','" + str(fyx) + "'," + str(je) + ")");
                        count = count + 1;
                db.CommitTransaction(tran);
                Feng.MessageForm.ShowInfo("已导入" + str(count) + "条数据！");
            except System.Exception, ex:
                db.RollbackTransaction(tran);
                errorDt.ImportRow(row);
                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(System.Exception('提单号' + row[4] + '数据有问题，请查证!', ex));
            if (errorDt.Rows.Count > 0):
                errorFileName =  System.IO.Path.GetDirectoryName(ofg.FileName) + '\\' + \
                                System.IO.Path.GetFileName(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".xml"));
                Feng.Utils.ExcelXmlHelper.WriteExcelXml(errorDt, errorFileName);
                Feng.MessageForm.ShowInfo("请查看错误文件" + errorFileName);
        except System.Exception, ex:
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);
                

