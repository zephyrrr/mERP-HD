# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Net")
clr.AddReferenceByPartialName("HtmlAgilityPack")

import System;
import Feng;
import Feng.Net;
import HtmlAgilityPack;
import ShipCompany;

class 船公司(ShipCompany.BaseShipCompany):
        def __init__(self):
                self.companyName_zh_cn = "北欧亚";#船公司中文名
                self.companyName_us_en = "CSAV NORASIA";#船公司英文名
                self.searchMode = "get";#查询方式get、post
                self.search_url = "http://www.csavnorasia.com/rastreo/rastreo.nsf/yourtrace?openagent&#提单号#";#根据提单号、箱号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[3]/tr[2]/td[1]/table[1]/tr[4]/td[4]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[3]/tr[2]/td[1]/table[1]/tr[3]/td[4]";#船名航次节点

        def GetVessel(self, vesselAndVoyage):#EOK-PUELCHE/00835/N
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        if (vesselAndVoyage.Contains("/")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.find("/")];
                        if (vesselAndVoyage.Contains("-")):
                                vesselAndVoyage = vesselAndVoyage[vesselAndVoyage.find("-") + 1:];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;
        

