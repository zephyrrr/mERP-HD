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
            if (dt.Columns.Count != 17):
                raise Feng.InvalidUserOperationException("文件栏目格式不符，请查证！");
            dzd = masterForm.DisplayManager.CurrentItem;
            dao = Hd.Model.业务费用Dao();
            errorDt = dt.Clone(); 
            dataStart = False;
            for row in dt.Rows:
                tdh = row[4].ToString();
                System.Windows.Forms.MessageBox.Show(tdh);
                if (tdh == '提单号'):
                    dataStart = True;
                    continue;
                if (not dataStart):
                    continue;
                System.Windows.Forms.MessageBox.Show('start');
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                    try:
                        rep.BeginTransaction();
                        item = Hd.Model.业务费用();
                        item.票 = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                .Add(NHibernate.Criterion.Expression.Eq("提单号", tdh))    \
                                .UniqueResult[Hd.Model.Jk.进口票]();
                        item.费用实体 = item.票;
                        if (item.票 == None):
                            raise eng.InvalidUserOperationException("提单号" + tdh + "不存在！");
                        xh = row[1];
                        item.箱 = rep.Session.CreateCriteria[Hd.Model.Jk.进口箱]()      \
                                .Add(NHibernate.Criterion.Expression.Eq("箱号", xh))   \
                                .Add(NHibernate.Criterion.Expression.Eq("票", item.票))   \
                                .UniqueResult[Hd.Model.Jk.进口箱]();
                        if (item.箱 == None):
                            raise eng.InvalidUserOperationException("箱号" + xh + "不存在！");
                        item.箱Id = item.箱.ID;

                        fyxs = ('161');
                        for i in range(0, 1, 1):
                            item2 = Hd.Model.业务费用();
                            item2.金额 = Feng.Utils.ConvertHelper.ToDecimal(row[13]);
                            if (item2.金额 == None):
                                continue;
                            item2.票 = item.票;
                            item2.费用实体 = item.费用实体;
                            item2.箱 = item.箱;
                            item2.箱Id = item.箱Id;
                            item2.费用项编号 = fyxs[i];
                            item2.收付标志 = Hd.Model.收付标志.付;
                            item2.相关人编号 = "900003";
                            item2.对账单 = dzd;
                            dao.Save(rep, item2);
                            dzd.费用.Add(item2);
                        rep.CommitTransaction();
                    except System.Exception, ex:
                        rep.RollbackTransaction();
                        errorDt.ImportRow(row);
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(System.Exception('提单号' + tdh + '数据有问题，请查证!', ex));
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
                

