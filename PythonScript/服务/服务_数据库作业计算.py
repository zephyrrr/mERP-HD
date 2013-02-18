# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Data")

import sys;
import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Data;
import time;

def execute():
    db = Feng.Data.DbHelper.Instance.CreateDatabase("DataConnectionString");
    sqlList = ["exec 过程更新_查询_现金日记帐_行数", "exec 过程更新_查询_银行日记帐_行数", "exec 过程更新_查询_资金日记帐_行数", "exec 过程更新_进口_滞箱费减免", \
               "exec 过程更新_财务_费用信息", "exec 过程更新_财务_费用信息_费用承担", "exec 过程更新_财务_对账单", "exec 过程更新_业务备案_普通票", "exec 过程更新_业务备案_进口票", \
               "exec 过程更新_业务备案_进口箱", "exec 过程更新_业务备案_内贸出港票", "exec 过程更新_业务备案_出口票", "exec 过程更新_进口转关_白卡排车时间", "update 查询_业务费用小计_票汇总 set 更新时间 = null", \
               "exec 过程更新_查询_业务费用小计_票汇总"];
    while (True):
        for sql in sqlList:
            try:
                print sql;
                executeTime = System.DateTime.Now;
                db.ExecuteNonQuery(sql);
                print "执行时间：" + str(executeTime) + "\t用时：" + (System.DateTime.Now - executeTime).ToString() + "\n";
            except System.Exception, ex:
                #如执行过程中出错，忽略错误，继续执行
                print ex.Message + "\n";
                continue;
                #Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
                #System.Windows.Forms.MessageBox.Show(ex.Message);
        time.sleep(3600);
            
if __name__ == "__main__":
    execute();
if __name__ == "<module>" or __name__ == "__builtin__":
    execute();

