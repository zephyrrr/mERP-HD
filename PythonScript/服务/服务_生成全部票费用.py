# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Service")
clr.AddReferenceByPartialName("Hd.Model.Jk")

import System;
import NHibernate;
import Feng;
import Hd.Service;
import Hd.Model.Jk;
import time;

def execute():
        while (True):                
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                        try:
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                        .Add(NHibernate.Criterion.Expression.Eq("货物类别", "废纸")) \
                                        .Add(NHibernate.Criterion.Expression.Eq("Submitted", True)) \
                                        .Add(NHibernate.Criterion.Expression.Eq("允许应收对账", False)) \
                                        .List[Hd.Model.Jk.进口票]();
                                if (tasks.Count == 0):
                                        return;
                                print "正在为 " + str(tasks.Count) + " 票进口票生成票费用..."
                                count = 0;
                                for task in tasks:
                                        Hd.Service.process_fy_piao.生成票费用(task, True);
                                        count = count + 1;
                                        print str(count) + "\t提单号:" + str(task.提单号) + "\t已完成";
                                print "服务_生成全部票费用\t" + str(System.DateTime.Now.ToString());
                        except System.Exception, ex:
                                #Feng.MessageForm.ShowInfo(ex.Message);
                                rep.RollbackTransaction();
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                print ex.Message;
                time.sleep(1800);
                      

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

