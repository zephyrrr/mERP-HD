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
#import time;

def execute():
        read = Hd.NetRead.nbeportRead();
        read.SetLoginInfo(Feng.SystemDirectory.DefaultUserProfile.GetValue("Hd.Options", "NetReadUserName", ""), \
                          Feng.SystemDirectory.DefaultUserProfile.GetValue("Hd.Options", "NetReadPassword", ""));
        print "读取到港时间";
        while (True):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                        try:
                                rep.BeginTransaction();
                                tasks = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("到港时间")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("提单号")) \
                                        .List[Hd.Model.Jk.进口票]();
                                if (tasks.Count == 0):
                                        rep.CommitTransaction();
                                        return;
                                for task in tasks:
                                        boxList = read.查询集装箱数据(Hd.NetRead.ImportExportType.进口集装箱, task.提单号);                                        
                                        if (boxList == None or boxList.Count == 0):
                                                continue;
                                        task.船名航次 = boxList[0].船舶英文名称 + "/" + boxList[0].航次;
                                        task.卸箱地编号 = Feng.NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", boxList[0].堆场区);
                                        #task.箱量 = boxList.Count;
                                        for box in boxList:
                                                if (box.进场时间 < task.到港时间):
                                                        task.到港时间 = box.进场时间;
                                        rep.Update(task);
                                        print "提单号：" + str(task.提单号) + "\t船名航次:" + str(task.船名航次) + "\t卸箱地:" + str(task.卸箱地编号) + "\t箱量:" + str(task.箱量);
                                        
                                        箱型编号 = None;
                                        品名 = None;                                        
                                        for xiang in task.箱:
                                                if (xiang.箱型编号 != None or xiang.品名 != None):
                                                        箱型编号 = xiang.箱型编号;
                                                        品名 = xiang.品名;
                                                        break;
                                        for jzx in boxList:
                                                ishave = False;
                                                for xiangInfo in task.箱:
                                                        if (xiangInfo.箱号 == jzx.集装箱号):
                                                                ishave = True;
                                                                break;
                                                if (ishave == False):
                                                        newjkx = Hd.Model.Jk.进口箱();
                                                        newjkx.箱号 = jzx.集装箱号;
                                                        newjkx.箱型编号 = 箱型编号;
                                                        newjkx.品名 = 品名;
                                                        newjkx.Created = System.DateTime.Now;
                                                        newjkx.CreatedBy = "服务";
                                                        newjkx.票 = task;
                                                        rep.Save(newjkx);
                                                        
                                rep.CommitTransaction();
                                print str(System.DateTime.Now.ToString());
                        except System.Exception, ex:
                                rep.RollbackTransaction();
                                print ex.Message;
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                #time.sleep(1800);
                      

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

