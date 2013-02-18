# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Net")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("Hd.Model.Jk")
clr.AddReferenceByPartialName("Hd.Model.Yw")

import System;
import NHibernate;
import Feng;
import Feng.Net;
import Feng.Data;
import Hd.Model.Jk;
import Hd.Model;
import time;
import ShipCompany;
import re;

def execute():#进口票.船名航次为中文时，str()字符编码不支持，用System.Convert.ToString()，但当等于None时无法拼入字符串
        while(True):
                db = Feng.Data.DbHelper.Instance;
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Jk.进口票]() as rep:
                        try:                                
                                jkps = rep.Session.CreateCriteria[Hd.Model.Jk.进口票]()      \
                                        .Add(NHibernate.Criterion.Expression.IsNull("到港时间")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("提单号")) \
                                        .Add(NHibernate.Criterion.Expression.IsNotNull("船公司编号")) \
                                        .AddOrder(NHibernate.Criterion.Order.Desc("委托时间")) \
                                        .List[Hd.Model.Jk.进口票]();
                                if (jkps == None or jkps.Count == 0):
                                        time.sleep(1800);
                                        continue;
                                print "服务_船公司 进口票票数：" + str(jkps.Count);
                                for jkp in jkps:
                                        if (System.String.IsNullOrEmpty(jkp.提单号) or System.String.IsNullOrEmpty(jkp.船公司编号)):
                                                continue;
                                        try:
                                                pyFileName = Feng.ServiceProvider.GetService[Feng.IDefinition]().TryGetValue("服务_船公司_" + jkp.船公司编号);
                                                if (System.String.IsNullOrEmpty(pyFileName)):
                                                        continue;
                                                exec 'import ' + pyFileName[:-3];
                                                exec 'shipCompany = ' + pyFileName[:-3] + '.船公司()';
                                                exec 'result = shipCompany.ParseHtmlInfo(shipCompany.GetHtmlInfo("' + str(jkp.提单号) + '"))';
                                                if (result == None):
                                                        continue;
                                                m_船名航次 = System.Convert.ToString(jkp.船名航次);
                                                dataRow = None;
                                                if (m_船名航次 != None):
                                                        dataRow = db.ExecuteDataRow("select * from Temp_港区_船舶停靠 where 中文船名 = '" + m_船名航次 + "' order by 港区 desc,预计靠泊时间");
                                                if (m_船名航次 != None and dataRow == None):
                                                        dataRow = db.ExecuteDataRow("select * from Temp_港区_船舶停靠 where replace(英文船名,' ','') = '" + m_船名航次.Replace(" ","") + "' order by 港区 desc,预计靠泊时间");
                                                ship_ywcm = result.vesselAndVoyage;                                                
                                                if (ship_ywcm != None):
                                                        ship_ywcm = str(ship_ywcm).Replace(" ", "");                                                
                                                if (dataRow == None):
                                                        print "港区无数据:(港区)" + str(None) + "\t(船公司)" + str(ship_ywcm);
                                                        if (jkp.船名航次 != None and jkp.船名航次.Replace(" ", "") == ship_ywcm and jkp.预计到港时间 == result.yjdgTime):
                                                                continue;
                                                        if (jkp.船名航次 != None):
                                                                r = re.match("[^\000-\177]", jkp.船名航次);#只验证中文打头
                                                                if (r <> None):
                                                                        continue;
                                                        jkp.预计到港时间 = result.yjdgTime;
                                                        jkp.船名航次 = str(result.vesselAndVoyage);
                                                        jkp.船名 = str(result.vesselAndVoyage);
                                                else:
                                                        gq_ywcm = dataRow["英文船名"];
                                                        if (gq_ywcm != None):
                                                                gq_ywcm = str(gq_ywcm).Replace(" ", "");
                                                        print "港区有数据:(港区)" + str(gq_ywcm) + "\t(船公司)" + str(ship_ywcm);
                                                        if (gq_ywcm.Replace(" ", "") == ship_ywcm and jkp.卸箱地编号 == str(dataRow["港区编号"]) and jkp.预计到港时间 == dataRow["预计靠泊时间"]):
                                                                continue;
                                                        if (gq_ywcm.Replace(" ", "") == ship_ywcm and jkp.卸箱地编号 == str(dataRow["港区编号"]) and jkp.预计到港时间 == result.yjdgTime and dataRow["预计靠泊时间"] == None):
                                                                continue;
                                                        jkp.卸箱地编号 = str(dataRow["港区编号"]);
                                                        jkp.船名航次 = System.Convert.ToString(dataRow["中文船名"]);
                                                        jkp.船名 = str(result.vesselAndVoyage);
                                                        if (dataRow["预计靠泊时间"] == None):
                                                                jkp.预计到港时间 = result.yjdgTime;
                                                        else:
                                                                jkp.预计到港时间 = System.Convert.ToDateTime(str(dataRow["预计靠泊时间"]));
                                                rep.BeginTransaction();
                                                rep.Update(jkp);
                                                rep.CommitTransaction();
                                                print pyFileName[:-3] + "\t提单号：" + str(jkp.提单号) + "\t预计到港时间：" + str(jkp.预计到港时间);
                                        except System.Exception, ex:
                                                rep.RollbackTransaction();
                                                print "服务_船公司Update Error:" + ex.Message;
                                                continue;
                        except System.Exception, ex:
                                print "服务_船公司Error:" + ex.Message;
                                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                #time.sleep(1800);

def GetVessel(vesselAndVoyage):#停止使用
        if (vesselAndVoyage == None):
                return vesselAndVoyage;
        if (vesselAndVoyage.Contains("-")):
                vesselAndVoyage = vesselAndVoyage[vesselAndVoyage.find("-") + 1 : vesselAndVoyage.find("/")];
        else:
                vesselAndVoyage = vesselAndVoyage[: vesselAndVoyage.find("/")];
        if (vesselAndVoyage.find(" ") <> -1):
                vesselAndVoyage = vesselAndVoyage[vessel.find(" ") + 1:];
        return vesselAndVoyage;

def GetVoyage(vesselAndVoyage):#停止使用
        if (vesselAndVoyage == None):
                return None;
        strlist = vesselAndVoyage.split('/');
        if (strlist.Count < 2):
                return None;
        else:
                return strlist[1];

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

