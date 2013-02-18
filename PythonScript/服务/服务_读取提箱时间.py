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
clr.AddReferenceByPartialName("Hd.Model.Jk")

import System;
import NHibernate;
import Feng;
import Hd.NetRead;
import Hd.Model.Jk;
import time;

def execute():
        #sys.stdin.readline();
        read = Hd.NetRead.npediRead();
        print "读取提箱时间";
        while(True):
                count = 0;
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口箱]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口箱]() \
                                        .Add(NHibernate.Criterion.Expression.IsNull("提箱时间")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("箱号")) \
                                        .CreateCriteria("票") \
                                        .Add(NHibernate.Criterion.Expression.Eq("Submitted", True)) \
                                        .Add(NHibernate.Criterion.Expression.Eq("操作完全标志", False)) \
                                        .Add(NHibernate.Criterion.Expression.Ge("到港时间", System.DateTime.Today.AddMonths(-2))) \
                                        .List[Hd.Model.Jk.进口箱]();
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        time.sleep(1800);
                                        continue;
                                for task in tasks:                                        
                                        if (System.String.IsNullOrEmpty(task.箱号)):
                                                continue;
                                        boxList = read.集装箱进口综合查询(Hd.NetRead.ImportExportType.进口集装箱, task.箱号);
                                        print "箱号: " + str(task.箱号) + "\t" + str(boxList.Count);
                                        if (boxList == None or boxList.Count == 0):
                                                continue;
                                        task.提箱时间 = boxList[0].提箱时间;
                                        rep.Update(task);
                                        count = count + 1;
                                        if (count == 10):
                                                rep.CommitTransaction();
                                                print str(System.DateTime.Now.ToString()) + " count:" + str(count);
                                                count = count - 10;
                                                rep.BeginTransaction();
                                rep.CommitTransaction();
                                print str(System.DateTime.Now.ToString()) + " count:" + str(count);
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                print ex.Message;
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

