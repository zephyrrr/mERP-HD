# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.NetRead")
clr.AddReferenceByPartialName("Hd.Model.Ck")

import System;
import NHibernate;
import Feng;
import Hd.NetRead;
import Hd.Model.Ck;
import time;

def execute():
        #sys.stdin.readline();        
        read = Hd.NetRead.npediRead();
        while(True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Ck.出口票]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Ck.出口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("放行时间")) \
                                        .Add(NHibernate.Criterion.Expression.Ge("委托时间", System.DateTime.Today.AddDays(-30))) \
                                        .List[Hd.Model.Ck.出口票]();
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        time.sleep(1800);
                                        continue;
                                for task in tasks:
                                        if (task.报关单号 == None):
                                                continue;
                                        task.放行时间 = read.查询海关放行时间(task.报关单号);
                                        print "报关单号：" + task.报关单号 + "\t放行时间：" + str(task.放行时间);
                                        if (task.放行时间 == None):
                                                continue;
                                        rep.Update(task);                                        
                                rep.CommitTransaction();
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

