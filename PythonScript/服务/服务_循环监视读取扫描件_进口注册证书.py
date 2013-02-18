# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Hd.Service")

import System;
import Hd.Service;

def execute():
    path = "\\\\192.168.0.10\\2版电子文档\\2版注册证书";
    wat = Hd.Service.ForWatcher(path, "Hd.Model.Jk.进口票", "货代自编号");
    while(True):
        try:
            #print "开始监视一遍:\t" + path;
            wat.Run();            
        except System.Exception, ex:
            print "服务_循环监视读取扫描件_进口注册证书 Error:" + ex.Message;
            continue;

if __name__ == "__main__":
    execute();
if __name__ == "<module>" or __name__ == "__builtin__":
    execute();
                

