# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Hd.Model.Base")


import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;

def execute(masterForm):
        def DoWork():
                try:
                        count = 0;
                        db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
                        cd_boxList = db.ExecuteDataView("select * from 视图查询交互_对账辅助_车队 where 对账单号 = '" + dzd + "'");
                        if (cd_boxList.Count == 0):
                                Feng.MessageForm.ShowInfo("没有费用记录！");
                                return -1;
                        cd_dataReader = cd_boxList.Table.CreateDataReader();                       
                        tran = db.BeginTransaction();
                        while (cd_dataReader.Read()):
                                db.ExecuteNonQuery("insert Temp_对账单费用查验(相关人,提单号,箱号,费用项,金额) values ('900001','" + str(cd_dataReader["提单号"]) + "','" \
                                                   + str(cd_dataReader["箱号"]) + "','" + str(cd_dataReader["费用项"]) + "'," + str(cd_dataReader["金额"]) + ")");
                                count = count + 1;
                        db.CommitTransaction(tran);
                        return count;
                except System.Exception, ex:
                        db.RollbackTransaction(tran);
                        #Feng.MessageForm.ShowInfo(ex.Message);
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
        def WorkDown(result):
                if (result >= 0):
                        Feng.MessageForm.ShowInfo("已导入" + str(result) + "条数据！");                        
        
        checkForm = Feng.ServiceProvider.GetService[Feng.IWindowFactory]().CreateWindow(Feng.ADInfoBll.Instance.GetWindowInfo("业务财务_对账单费用查验_车队导入"));
        if (checkForm.ShowDialog() == System.Windows.Forms.DialogResult.OK):
                dzd = checkForm.DataControls["对账单号"].SelectedDataValue;
                if (dzd == None or str(dzd) == ''):
                        return;                
                Feng.Utils.ProgressAsyncHelper(     \
                        Feng.Async.AsyncHelper.DoWork(DoWork), \
                        Feng.Async.AsyncHelper.WorkDone(WorkDown), \
                        masterForm, "导入");

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);
                

