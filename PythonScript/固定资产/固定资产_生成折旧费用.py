# -*- coding: utf-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Kj")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Kj;

def execute(masterForm):
    lt = masterForm.DisplayManager.CurrentItem;
    if (lt == None):
        return;
    dao = Hd.Model.HdBaseDao[Hd.Model.非业务费用]();
    item = Hd.Model.非业务费用();
    with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Kj.固定资产]() as rep:
        try:
            rep.BeginTransaction();
            item.费用实体 = lt;
            item.收付标志 = Hd.Model.收付标志.付;
            item.相关人编号 = "900031";
            item.费用项编号 = "387";
            item.金额 = lt.月折旧额;
            item.IsActive = "True";
            item.费用类别编号 = 86;
            dao.Save(rep, item);
            rep.CommitTransaction();
            lt.费用.Add(item);
            masterForm.ControlManager.OnCurrentItemChanged();
            Feng.MessageForm.ShowInfo("已生成费用！");
        except System.Exception, ex:
            rep.RollbackTransaction();
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);

