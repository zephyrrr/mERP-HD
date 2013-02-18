# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Hd.Model.Jk")
clr.AddReferenceByPartialName("Hd.Model.Dao")

import sys;
import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Data;
import Hd.Model.Jk;
import time;

def execute():
    db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
    while (True):
        try:        
            hd_boxList = db.ExecuteDataView("select 箱Id,箱号 from 视图查询_箱信息_进口 where 到港时间 >= dateadd(month,-2,getdate()) and 承运人 = '900001' and 操作完全标志 = 0 and (提箱时间 is null or 拉箱时间 is null or 还箱时间 is null or 卸货时间 is null)");
            cd_boxList = db.ExecuteDataView("select * from 视图查询交互_车队任务 where 拉箱时间 >= dateadd(month,-2,getdate())");
            print "Hd:" + str(hd_boxList.Count) + "\tCd:" + str(cd_boxList.Count);
            time.sleep(2);
            if (hd_boxList.Count == 0 or cd_boxList.Count == 0):
                return;
            hd_dataReader = hd_boxList.Table.CreateDataReader();
            count = 0;
            while (hd_dataReader.Read()):
                xianghao = Feng.Utils.ConvertHelper.ToString(hd_dataReader["箱号"]);            
                if (xianghao == None):
                    continue;
                count = count + 1;
                print "箱号:" + xianghao + "\t" + str(count);
                cd_dataReader = cd_boxList.Table.CreateDataReader();
                while (cd_dataReader.Read()):
                    if (xianghao == Feng.Utils.ConvertHelper.ToString(cd_dataReader["箱号"])):
                        with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口箱]() as rep:
                            try:
                                xiangId = Feng.Utils.ConvertHelper.ToString(hd_dataReader["箱Id"]);
                                rep.BeginTransaction();
                                jkx = rep.Get[Hd.Model.Jk.进口箱](System.Guid(xiangId));
                                isCommit = False;                                
                                if (jkx.提箱时间 == None and str(cd_dataReader["提箱时间"]) != ""):
                                    jkx.提箱时间 = Feng.Utils.ConvertHelper.ToDateTime(cd_dataReader["提箱时间"]);
                                    isCommit = True;
                                if (jkx.还箱时间 == None and str(cd_dataReader["还箱时间"]) != ""):
                                    jkx.还箱时间 = Feng.Utils.ConvertHelper.ToDateTime(cd_dataReader["还箱时间"]);
                                    isCommit = True;
                                if (jkx.拉箱时间 == None and str(cd_dataReader["拉箱时间"]) != ""):
                                    jkx.拉箱时间 = Feng.Utils.ConvertHelper.ToDateTime(cd_dataReader["拉箱时间"]);
                                    isCommit = True;
                                if (jkx.卸货时间 == None and str(cd_dataReader["装卸货时间"]) != ""):
                                    jkx.卸货时间 = Feng.Utils.ConvertHelper.ToDateTime(cd_dataReader["装卸货时间"]);
                                    isCommit = True;
                                if (isCommit):
                                    rep.Update(jkx);             
                                    print "Id:" + xiangId + "\t箱号:" + xianghao + "\t提箱时间:" + str(jkx.提箱时间) + "\t还箱时间:" + str(jkx.还箱时间) + \
                                          "\t拉箱时间:" + str(jkx.拉箱时间) + "\t卸货时间:" + str(jkx.卸货时间);
                                rep.CommitTransaction();
                                if (isCommit):
                                    break;
                            except System.Exception, ex:
                                rep.RollbackTransaction();
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                System.Windows.Forms.MessageBox.Show(ex.Message);
                cd_dataReader.Close();
        except System.Exception, ex:
            #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
            #System.Windows.Forms.MessageBox.Show(ex.Message);
            print ex.Message;
        time.sleep(1800);
            
if __name__ == "__main__":
    execute();
if __name__ == "<module>" or __name__ == "__builtin__":
    execute();

