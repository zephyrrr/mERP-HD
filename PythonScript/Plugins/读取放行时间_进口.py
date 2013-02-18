# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.NetRead")
clr.AddReferenceByPartialName("Hd.Model.Jk")
clr.AddReferenceByPartialName("Hd.Model.Yw")

import System;
import NHibernate;
import Feng;
import Hd.NetRead;
import Hd.Model;
import Hd.Model.Jk;
		
class DemoTask(Feng.Windows.Forms.ExecuteTask):
	def DoWork(self):
                read = Hd.NetRead.npediRead();
                print "读取放行时间";
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("放行时间")) \
                                        .List[Hd.Model.Jk.进口票]();
                                self.Progress(System.Array[System.String]([u'待读取'+str(tasks.Count)+'票',u'']));
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        return;
                                ifangxing = 0;
                                for task in tasks:
                                        if (task.箱 == None or task.箱.Count == 0):
                                                continue;
                                        #查询海关放行时间报关单短号，返回Dictionary<DateTime?, string>  放行时间，提单号
                                        task.放行时间 = read.查询海关放行时间(task.箱[0].箱号, task.提单号);
                                        print "货代自编号：" + str(task.货代自编号) + "\t放行时间：" + str(task.放行时间);
                                        
                                        if (task.放行时间 == None):
                                                continue;
                                        rep.Update(task);
                                        ifangxing = ifangxing + 1;
                               
                                rep.CommitTransaction();                                                       

                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
 								
                self.Progress(System.Array[System.String]([u'读取放行时间完毕'+str(ifangxing)+'票', u'']));

def execute(mdiForm):
	return DemoTask("读取放行时间");

if __name__ == "<module>" or __name__ == "__builtin__":
	result = execute(mdiForm);
