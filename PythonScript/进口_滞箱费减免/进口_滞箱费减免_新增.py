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
clr.AddReferenceByPartialName("Hd.Model.Jk")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Jk;

def execute(masterForm):
        form = Feng.ProcessSelect.Execute(masterForm.ArchiveDetailForm.ControlManager.DisplayManager, "选择_滞箱费减免_进口票", None);
        cmMaster = masterForm.ControlManager;

        if (form != None) :
                cnt = 0;
                ids = '';
                dao = Hd.Model.HdBaseDao[Hd.Model.Jk.进口费用信息]();
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口费用信息]() as rep:
                        for i in form.SelectedEntites :
                                try:
                                        rep.BeginTransaction();
                                        list = rep.Session.CreateCriteria[Hd.Model.费用信息]()      \
                                                .Add(NHibernate.Criterion.Expression.Eq("票Id", i.ID))   \
                                                .Add(NHibernate.Criterion.Expression.Eq("费用项编号", "167"))   \
                                                .Add(NHibernate.Criterion.Expression.Eq("业务类型编号", int(i.费用实体类型)))   \
                                                .List[Hd.Model.费用信息]();
                                        if (list.Count == 0):
                                                item = Hd.Model.Jk.进口费用信息();
                                                item.票Id = i.ID;
                                                item.票 = i;
                                                item.费用项编号 = "167";
                                                item.业务类型编号 = int(i.费用实体类型);
                                                dao.Save(rep, item);
                                        if (not i.滞箱费减免标志):
                                                i.滞箱费减免标志 = True;
                                                rep.Update(i);
                                        rep.CommitTransaction();
                                        if (list.Count == 0):
                                                cnt = cnt + 1;
                                        ids = ids + str(i.ID) + ',';
                                except System.Exception, ex:
                                        rep.RollbackTransaction();
                                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                if (ids.Length > 0):
                        exp = '票Id InG [' + ids + ']' + ' AND 费用项编号 = 167 AND 业务类型编号 = ' + str(int(i.费用实体类型));
                        cmMaster.DisplayManager.SearchManager.LoadData(Feng.SearchExpression.Parse(exp), None);
                Feng.MessageForm.ShowInfo("已生成" + str(cnt) + "条费用信息！");

if __name__ == "<module>" or __name__ == "__builtin__":
	execute(masterForm);

