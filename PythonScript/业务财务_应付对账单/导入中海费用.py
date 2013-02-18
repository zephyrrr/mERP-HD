# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Yw")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Jk")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Jk;

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
            dzd = masterForm.DisplayManager.CurrentItem;
            dao = Hd.Model.业务费用Dao();
            errorDt = dt.Clone();
            for row in dt.Rows:
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                    try:
                        moeny = Feng.Utils.ConvertHelper.ToDecimal(row["实际金额"]);
                        #if (moeny == 0):
                        #    continue;
                        rep.BeginTransaction();
                        item = Hd.Model.业务费用();
                        item.票 = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                .Add(NHibernate.Criterion.Expression.Eq("提单号", row["提单编号"]))    \
                                .UniqueResult[Hd.Model.Jk.进口票]();
                        item.费用实体 = item.票;
                        if (item.票 == None):
                            raise eng.InvalidUserOperationException("提单号" + row["提单编号"].ToString() + "不存在！");
                        item.箱 = rep.Session.CreateCriteria[Hd.Model.Jk.进口箱]()      \
                                .Add(NHibernate.Criterion.Expression.Eq("箱号", row["箱号"]))   \
                                .Add(NHibernate.Criterion.Expression.Eq("票", item.票))   \
                                .UniqueResult[Hd.Model.Jk.进口箱]();
                        if (item.箱 == None):
                            raise eng.InvalidUserOperationException("箱号" + row["箱号"].ToString() + "不存在！");
                        item.箱Id = item.箱.ID;
                        if (row["费用类型"].ToString() == "XXF"): item.费用项编号 = "165";
                        elif (row["费用类型"].ToString() == "ZX"): item.费用项编号 = "167";
                        else: raise Feng.InvalidUserOperationException("费用类型不能是" + row["费用类型"].ToString() + "!");
                        #if (row["免费天数"] != System.DBNull.Value && row["免费天数"] != null):
                        #    item.箱.最终免箱天数 = int(row["免费天数"]);
                        #    rep.Update(item.箱);
                        item.收付标志 = Hd.Model.收付标志.付;
                        item.相关人编号 = "900010";
                        if (row["币种"].ToString() == "RMB"): item.金额 = Feng.Utils.ConvertHelper.ToDecimal(row["实际金额"]);
                        elif (row["币种"].ToString() == "USD"): item.金额 = 7 * Feng.Utils.ConvertHelper.ToDecimal(row["实际金额"]);
                        else: raise Feng.InvalidUserOperationException("币种不能是" + row["币种"].ToString() + "!");
                        item.对账单 = dzd;
                        dao.Save(rep, item);
                        dzd.费用.Add(item);
                        rep.CommitTransaction();
                    except System.Exception, ex:
                        rep.RollbackTransaction();
                        errorDt.ImportRow(row);
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(System.Exception('提单号' + row["提单编号"] + '数据有问题，请查证!', ex));
            masterForm.ControlManager.OnCurrentItemChanged();
            if (errorDt.Rows.Count > 0):
                errorFileName =  System.IO.Path.GetDirectoryName(ofg.FileName) + '\\' + \
                                System.IO.Path.GetFileName(System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".xml"));
                Feng.Utils.ExcelXmlHelper.WriteExcelXml(errorDt, errorFileName);
                Feng.MessageForm.ShowInfo("请查看错误文件" + errorFileName);
        except System.Exception, ex:
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);
                

