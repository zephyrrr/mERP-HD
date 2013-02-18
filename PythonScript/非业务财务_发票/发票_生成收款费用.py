# -*- coding: UTF-8 -*- 
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
clr.AddReferenceByPartialName("Hd.Model.Fp")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Fp;

def execute(masterForm):
    fp = masterForm.DisplayManager.CurrentItem;
    dao = Hd.Model.非业务费用Dao();
    item = Hd.Model.非业务费用();
    with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Fp.发票]() as rep:
        try:
            rep.BeginTransaction();
            item.费用实体 = fp;
            item.收付标志 = Hd.Model.收付标志.收;
            item.相关人编号 = fp.相关人编号;
            item.费用项编号 = "335";
            item.金额 = fp.金额 * 0.04;
            dao.Save(rep, item);
            rep.CommitTransaction();
            fp.费用.Add(item);
            masterForm.ControlManager.OnCurrentItemChanged();
            Feng.MessageForm.ShowInfo("已生成收款费用！");
        except System.Exception, ex:
            rep.RollbackTransaction();
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);

