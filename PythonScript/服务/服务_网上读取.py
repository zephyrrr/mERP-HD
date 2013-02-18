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
clr.AddReferenceByPartialName("Cd.Model.Yw")

import System;
import NHibernate;
import Feng;
import Hd.NetRead;
import Cd.Model;
import time;

def execute():
        #sys.stdin.readline();
        read = Hd.NetRead.npediRead();
        while(True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Cd.Model.任务]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Cd.Model.任务]()      \
                                        .Add(NHibernate.Criterion.Expression.Eq("任务类别", Cd.Model.任务类别.拆)) \
                                        .Add(NHibernate.Criterion.Expression.IsNull("提箱时间")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("提单号")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("箱号")) \
                                        .CreateCriteria("车辆产值") \
                                        .Add(NHibernate.Criterion.Expression.Ge("日期", System.DateTime.Today.AddMonths(-2))) \
                                        .List[Cd.Model.任务]();
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        return;
                                readTdhs = {};
                                readBoxs = {};
                                for task in tasks:
                                        
                                        if (System.String.IsNullOrEmpty(task.提单号)):
                                                continue;
                                        if (System.String.IsNullOrEmpty(task.箱号)):
                                                continue;
                                        if (not readTdhs.has_key(task.提单号)):
                                                readTdhs[task.提单号] = True;
                                                boxList = read.集装箱出门查询(task.提单号);
                                                for data in boxList:
                                                        readBoxs[data.集装箱号.Trim()] = data;
                                        if (readBoxs.has_key(task.箱号)):
                                                task.提箱时间 = readBoxs[task.箱号].出门时间;
                                                rep.Update(task);
                                rep.CommitTransaction();
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

