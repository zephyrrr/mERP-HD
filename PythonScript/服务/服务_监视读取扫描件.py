# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Hd.Service")

import System;
import Hd.Service;

def execute():
    try:
        path1 = "\\\\192.168.0.10\\2版电子文档\\2版单证";
        wat = Hd.Service.Watcher(path1, "Hd.Model.Jk.进口票", "货代自编号");
        wat.Run();
        print "开始监视:\t" + path1;

        path2 = "\\\\192.168.0.10\\2版电子文档\\2版合同";
        wat = Hd.Service.Watcher(path2, "Hd.Model.Jk.进口票", "合同号");
        wat.Run();
        print "开始监视:\t" + path2;
    except System.Exception, ex:
        print "服务_监视读取扫描件 Error:" + ex.Message;

if __name__ == "__main__":
    execute();
if __name__ == "<module>" or __name__ == "__builtin__":
    execute();
                

