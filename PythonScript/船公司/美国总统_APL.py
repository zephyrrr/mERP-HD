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

class 船公司(ShipCompany.BaseShipCompany):#无法获取正确的htmlnfo
        def __init__(self):
                self.companyName_zh_cn = "美国总统";#船公司中文名
                self.companyName_us_en = "APL";#船公司英文名
                self.searchMode = "get";
                self.search_url = "http://homeport.apl.com/gentrack/trackingMain.do?trackInput01=#提单号#&trackInput02=&trackInput03=&trackInput04=&trackInput05=&trackInput06=&trackInput07=&trackInput08=";#根据提单号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tbody[1]/tr[1]/td[1]/table[1]/tr[3]/td[1]/table[1]/tbody[1]/tr[5]/td[4]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tbody[1]/tr[1]/td[1]/table[1]/tr[3]/td[1]/table[1]/tbody[1]/tr[5]/td[3]/a[1]";#船名航次节点
            
        

