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
        while(True):
                read = Hd.NetRead.npediRead();
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口箱]() as rep:
                        try:
                                
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口箱]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("商检查验标志")) \
                                        .CreateCriteria("票")\
                                        .Add(NHibernate.Criterion.Expression.Ge("到港时间", System.DateTime.Today.AddDays(-30))) \
                                        .List[Hd.Model.Jk.进口箱]();                               
                                if (tasks.Count == 0):
                                        time.sleep(1800);
                                        continue;
                                for task in tasks:
                                        try:                                                
                                                task.商检查验标志 = read.查询商检查验结果(task.箱号);
                                                print "箱号：" + str(task.箱号) + "\t商检查验标志：" + str(task.商检查验标志);
                                                if (task.商检查验标志 == None):
                                                        continue;
                                                rep.BeginTransaction();
                                                rep.Update(task);
                                                rep.CommitTransaction();
                                        except System.Exception, ex:
                                                rep.RollbackTransaction();
                                                print "商检查验结果_进口\tError:" + ex.Message;
                                                continue;                                
                        except System.Exception, ex:
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                print ex.Message;
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

