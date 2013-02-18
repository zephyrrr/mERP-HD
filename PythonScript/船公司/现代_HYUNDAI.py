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
                self.companyName_zh_cn = "现代";#船公司中文名
                self.companyName_us_en = "HYUNDAI";#船公司英文名
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www.hmm21.com/ebiz/track_trace/track.jsp";#根据提单号、箱号查询地址
                self.postString = "blFields=undefined&cnFields=undefined&numbers=#提单号#&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=&numbers=";#Post查询
                self.yjdgTime_Node_Xpath = "/html[2]/body[1]/table[3]/tbody[1]/tr[1]/td[6]/font[1]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[2]/body[1]/table[3]/tbody[1]/tr[1]/td[1]";#船名航次节点

        def GetHtmlInfo(self, tdh):#为了处理提单号前缀HDMU
                try:                        
                        if (tdh == None):
                                return None;
                        if (tdh.Contains("HDMU")):
                                tdh = tdh.Replace("HDMU", "");
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

        def GetVessel(self, vesselAndVoyage):#HYUNDAI BRIDGE/<br>252E
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        if (vesselAndVoyage.Contains("/")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.find("/")];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;
        

