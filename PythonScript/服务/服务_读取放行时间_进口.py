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
        time_s = System.DateTime.Now;
        time_z = System.DateTime.Now;
        i_num = 0;
        #sys.stdin.readline();        
        read = Hd.NetRead.npediRead();
        print "读取放行时间;开始时间：" +str(time_s);
        while(True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("放行时间")) \
                                        .List[Hd.Model.Jk.进口票]();
                                print "未放行票数：" + str(tasks.Count);
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        time.sleep(180);
                                        continue;
                                for task in tasks:
                                        if (task.箱 == None or task.箱.Count == 0):
                                                continue;
                                        #查询海关放行时间报关单短号，返回Dictionary<DateTime?, string>  放行时间，提单号
                                        task.放行时间 = read.查询海关放行时间(task.箱[0].箱号, task.提单号);
                                        print "货代自编号：" + str(task.货代自编号) + "\t放行时间：" + str(task.放行时间);
                                        time_z = System.DateTime.Now;
                                        time_sz = (time_z - time_s).TotalSeconds;
                                        if (time_sz > 600 and i_num > 0):
                                                break;
                                        if (task.放行时间 == None):
                                                continue;
                                        rep.Update(task);
                                i_num = i_num + 1;
                                print("放行时间循环了" + str(i_num) +"次，运行时间：" + str(time_sz) +"秒");
                                rep.CommitTransaction();                                                       
                                if (time_sz > 600  and i_num > 1):
                                        break;  
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                #time.sleep(1800);
        print "读取放行时间;结束时间：" +str(System.DateTime.Now);              

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

