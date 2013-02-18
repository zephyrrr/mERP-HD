# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Data")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.NetRead")
clr.AddReferenceByPartialName("Hd.Model.Ck")
clr.AddReferenceByPartialName("Hd.Service")

import System;
import System.Data;
import NHibernate;
import Feng;
import Feng.Data;
import Hd.NetRead;
import Hd.Service;
import Hd.Model.Ck;
import time;

def execute():
        dbHelper = Feng.Data.DbHelper.Instance;
        while(True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Ck.出口票]() as rep:
                        try:
                                #tasks = rep.Session.CreateCriteria[Hd.Model.Ck.出口票]()      \
                                #        .Add(NHibernate.Criterion.Expression.IsNull("委托人编号")) \
                                #        .Add(NHibernate.Criterion.Expression.IsNull("货代自编号")) \
                                #        .Add(NHibernate.Criterion.Expression.Ge("委托时间", System.DateTime.Today.AddDays(-5))) \
                                #        .List[Hd.Model.Ck.出口票]();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Ck.出口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("委托人编号")) \
                                        .Add(NHibernate.Criterion.Expression.Ge("委托时间", System.DateTime.Today.AddDays(-5))) \
                                        .List[Hd.Model.Ck.出口票]();        
                                print str(tasks.Count);
                                if (tasks.Count == 0):
                                        return;                                
                                for task in tasks:
                                        rep.BeginTransaction();
                                        if (task.抬头 == None):
                                                continue;
                                        #ckps = rep.Session.CreateCriteria[System.String]() \
                                        #       .Add(NHibernate.Criterion.Expression.Eq("抬头", task.抬头)) \
                                        #       .Add(NHibernate.Criterion.Expression.Ge("委托时间", System.DateTime.Today.AddDays(-30))) \
                                        #       .SetProjection(NHibernate.Criterion.Projections.Distinct(NHibernate.Criterion.Projections.ProjectionList() \
                                        #                                                                .Add(NHibernate.Criterion.Projections.Property("委托人编号")))).List[System.String]();

                                        ckps = dbHelper.ExecuteDataTable("SELECT DISTINCT 委托人,委托类别 FROM 业务备案_出口票 AS A INNER JOIN 业务备案_普通票 AS B ON A.ID = B.ID WHERE 委托类别 = '1' AND 抬头 = '" + task.抬头 + "'");
                                        if (ckps == None):
                                                ontinue;

                                        for row in ckps.Rows:
                                                if (ckps.Rows.Count > 2): #可能回有Null的行+有数据的行
                                                        break;
                                                if (row["委托人"] != None and row["委托人"].ToString() != ""):
                                                        task.委托人编号 = row["委托人"].ToString();
                                                        #task.货代自编号 = Hd.Service.process_ck.Get货代自编号(task.委托人编号, task.委托时间);
                                                        if (row["委托类别"] != None and row["委托类别"].ToString() != ""):
                                                                if (row["委托类别"].ToString() == "1"):
                                                                        task.委托类别 = Hd.Model.Ck.委托类别.P;
                                                                else:
                                                                        task.委托类别 = Hd.Model.Ck.委托类别.T;                                                                        
                                                        break;

                                        rep.Initialize(task.箱, task);
                                        if (task.箱量 == task.箱.Count):
                                                task.Submitted = True;
                                        rep.Update(task);
                                        print "报关单号：" + task.报关单号 + "\t委托人编号：" + str(task.委托人编号) + "\t货代自编号：" + str(task.货代自编号) + "\t委托类别：" + str(task.委托类别) + "\t提交备案：" + str(task.Submitted);
                                        rep.CommitTransaction();
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                print ex.Message;
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

