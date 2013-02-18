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
                self.companyName_zh_cn = "中远";#船公司中文名
                self.companyName_us_en = "COSCO";#船公司英文名
                self.searchMode = "post";
                self.search_url = "http://www.coscon.com/ebusiness/service/cargo/trackbybl.do";#根据提单号查询地址
                self.postString = "locale=zh&blNum=#提单号#&type=bybl";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/div[3]/div[2]/div[2]/table[2]/tr[4]/td[2]";#预计到港时间节点
                self.vessel_Node_Xpath = "/html[1]/body[1]/div[3]/div[2]/div[2]/table[2]/tr[5]/td[2]";#船名节点
                self.voyage_Node_Xpath = "/html[1]/body[1]/div[3]/div[2]/div[2]/table[2]/tr[6]/td[2]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点
        

