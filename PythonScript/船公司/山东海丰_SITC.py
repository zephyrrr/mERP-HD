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
                self.companyName_zh_cn = "山东海丰";#船公司中文名
                self.companyName_us_en = "SITC";#船公司英文名
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www.sitc.com/hangyun/blresult.asp";#根据提单号、箱号查询地址
                self.postString = "blno=#提单号#&search2.x=1&search2.y=5";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[6]";#预计到港时间节点
                self.vessel_Node_Xpath = "/html[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[1]";#船名节点
                self.voyage_Node_Xpath = "/html[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[2]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点
        

