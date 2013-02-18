# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Feng.Data")

import System;
import Feng.Data;
import time;
import GangQu;

def execute():
        while(True):
                gqList = ["北仑二期900002", "北仑三期900003", "北仑四期900004", "北仑五期900098", "EDI_北仑二期900002", "EDI_北仑三期900003", "EDI_北仑四期900004", "EDI_北仑五期900098", "EDI_大榭招商900005"];
                db = Feng.Data.DbHelper.Instance;
                for gq in gqList:
                        try:
                                if (gq.Contains("EDI")):
                                        exec 'import EDI';
                                        exec 'gangQu = EDI.港区("' + gq + '")';
                                        gqValue = "";
                                        if (gq == "EDI_北仑二期900002"):
                                                gqValue = "BLCT";#NBCT(二期)
                                        elif (gq == "EDI_北仑三期900003"):
                                                gqValue = "BLCT2";#北二集司(三期)
                                        elif (gq == "EDI_北仑四期900004"):
                                                gqValue = "BLCT3";#港吉(四期)
                                        elif (gq == "EDI_北仑五期900098"):
                                                gqValue = "BLCTYD";#远东(五期)
                                        elif (gq == "EDI_大榭招商900005"):
                                                gqValue = "BLCTZS";#大榭招商(CMICT)
                                        exec 'results = gangQu.ParseHtmlInfo(gangQu.GetHtmlInfo("' + gqValue + '"))';
                                else:
                                        exec 'import ' + gq[:-6];
                                        exec 'gangQu = ' + gq[:-6] + '.港区()';
                                        exec 'results = gangQu.ParseHtmlInfo(gangQu.GetHtmlInfo("' + System.DateTime.Today.ToString("yyyy-MM-dd") + '", "2020-12-31"))';
                                if (results == None or results.Count == 0):
                                        continue;                                
                                db.ExecuteNonQuery("delete Temp_港区_船舶停靠 where 港区 = '" + gq[:-6] + "'");
                                print gq + "\tResultsCount:" + str(results.Count);
                                for result in results:
                                        cmdText = "insert Temp_港区_船舶停靠 (港区,港区编号,英文船名,中文船名,Created,预计靠泊时间) values ('" + gq[:-6] + "','" + gq[-6:] + "','"+ result.ywcm + "','"+ result.zwcm + "','" + System.DateTime.Now.ToString() + "',";
                                        if (result.yjkbTime == None or result.yjkbTime <= System.DateTime.Today):
                                                cmdText = cmdText + "NULL)";
                                        else:
                                                cmdText = cmdText + "'" + str(result.yjkbTime) + "')";
                                        db.ExecuteNonQuery(cmdText);
                        except System.Exception, ex:
                                print gq[:-6] + "\tError:" + ex.Message;
                                Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                                continue;
                time.sleep(1800);

if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

