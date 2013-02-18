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
clr.AddReferenceByPartialName("Hd.Service")
clr.AddReferenceByPartialName("Hd.Model.Ck")

import System;
import NHibernate;
import Feng;
import Hd.NetRead;
import Hd.Service;
import Hd.Model.Ck;
import time;

def execute():
        #sys.stdin.readline();
        #read = Hd.NetRead.npediRead();
        while(True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Ck.出口箱]() as rep:
                        try:
                                tasks = rep.Session.CreateCriteria[Hd.Model.Ck.出口箱]()\
                                        .Add(NHibernate.Criterion.Expression.Eq("查验标志", False))\
                                        .CreateCriteria("票")\
                                        .Add(NHibernate.Criterion.Expression.Ge("委托时间", System.DateTime.Today.AddDays(-30)))\
                                        .List[Hd.Model.Ck.出口箱]();                                
                                if (tasks.Count == 0):
                                        time.sleep(1800);
                                        continue;
                                for task in tasks:
                                        try:                                                
                                                task = Hd.Service.process_readticketfromnet.查询海关查验结果(task);
                                                print "箱号：" + str(task.箱号) + "\t海关查验：" + str(task.海关查验) + "\t查验标志:" + str(task.查验标志);
                                                if (task.海关查验 == None):
                                                        continue;
                                                rep.BeginTransaction();
                                                rep.Update(task);
                                                rep.CommitTransaction();
                                        except System.Exception, ex:
                                                rep.RollbackTransaction();
                                                print "海关查验结果\tError:" + ex.Message;
                                                continue;                                
                        except System.Exception, ex:
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                print ex.Message;
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

