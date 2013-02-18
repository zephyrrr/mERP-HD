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
                self.companyName_zh_cn = "挪威天熙箱运";#船公司中文名
                self.companyName_us_en = "TCC";#船公司英文名
                self.searchMode = "get";#查询方式get、post
                self.search_url = "http://server2.vilden.net/cgi-tc/wcs001.pgm?SCTYPE=BL&SCREFN=#提单号#&SCPRIN=TC";#根据提单号、箱号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/br[1]/table[1]/tbody[1]/tr[2]/td[10]/font[1]";#预计到港时间节点
                self.vessel_Node_Xpath = "/html[1]/body[1]/table[2]/tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[2]/td[3]/font[1]/b[1]";#船名节点
                self.voyage_Node_Xpath = "/html[1]/body[1]/table[2]/tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[2]/td[3]/font[1]/b[2]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点

        def GetHtmlInfo(self, tdh):#为了处理提单号前缀TCUG
                try:                        
                        if (tdh == None):
                                return None;
                        if (tdh.Contains("TCUG")):
                                tdh = tdh.Replace("TCUG", "");
                        htmlInfo = None;
                        if (self.searchMode == "get"):
                                m_search_url = self.search_url.Replace("#提单号#", tdh);
                                htmlInfo = Feng.Net.WebProxy().GetToString(m_search_url);
                        else:
                                m_postString = self.postString.Replace("#提单号#", tdh);
                                htmlInfo = Feng.Net.WebProxy().PostToString(self.search_url, m_postString);
                        return htmlInfo;
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;
        

