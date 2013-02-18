# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Jk")
clr.AddReferenceByPartialName("Hd.Service")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Data;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Jk;
import Hd.Service;

def execute(masterForm):
        db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
        count = db.ExecuteScalar("SELECT COUNT(Id) FROM 财务_费用 WHERE 费用实体 = '" + str(masterForm.DisplayManager.CurrentItem.ID) + "' AND 收付标志 = 1");
        if (count < 1):
                if (Feng.MessageForm.ShowYesNo("没有收款费用，是否继续？", "提示") == System.Windows.Forms.DialogResult.No):
                        return;                      

        masterForm.DoEdit();
        cmMaster = masterForm.ControlManager;
        with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                try:
                        rep.BeginTransaction();
                        for row in masterForm.ArchiveDetailForm.DetailGrids[0].DataRows:                                
                                masterForm.ArchiveDetailForm.DetailGrids[0].CurrentRow = row;                                
                                row.BeginEdit();
                                fylb = row.Cells["费用类别"].Value.ToString();
                                if (fylb == "311" or fylb == "312" or fylb == "313"):
                                        if (row.Cells["Submitted"].Value == None or row.Cells["Submitted"].Value.ToString() != "True"):
                                                row.Cells["Submitted"].Value = True;
                                row.EndEdit();
                        Hd.Service.process_fy_piao.票费用保存(masterForm);
                        cmMaster.DisplayManager.CurrentItem.允许应收对账 = True;
                        rep.Update(cmMaster.DisplayManager.CurrentItem);
                        rep.CommitTransaction();
                        cmMaster.OnCurrentItemChanged();
                except System.Exception, ex:
                        rep.RollbackTransaction();
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute(masterForm);

