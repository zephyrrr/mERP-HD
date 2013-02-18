# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Net")
clr.AddReferenceByPartialName("Feng.Data")
clr.AddReferenceByPartialName("HtmlAgilityPack")

import System;
import Feng;
import Feng.Net;
import Feng.Data;
import HtmlAgilityPack;
import ShipCompany;

class 船公司(ShipCompany.BaseShipCompany):
        def __init__(self):
                self.companyName_zh_cn = "韩进";#船公司中文名
                self.companyName_us_en = "HANJIN";#船公司英文名
                self.searchMode = "post";
                self.search_url = "http://www.hanjin.com/eservice/alps/cn/cargo/Cargo.do";#根据提单号查询地址
                self.postString = "forward=bkgSingleBInfo&cop=cop&numbertype=B&number=#提单号#";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[3]/table[2]/tr[1]/td[1]/table[6]/tr[2]/td[2]/table[3]/tr[{0}]/td[5]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[3]/table[2]/tr[1]/td[1]/table[6]/tr[2]/td[2]/table[3]/tr[{0}]/td[1]/a[1]";#船名航次节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/table[2]/tr[1]/td[3]/table[2]/tr[1]/td[1]/table[6]/tr[2]/td[2]/table[3]";#数据<table>的节点

        def GetHtmlInfo(self, tdh):#为了处理提单号前缀HJSC
                try:                        
                        if (tdh == None):
                                return None;
                        if (tdh.Contains("HJSC")):
                                tdh = tdh.Replace("HJSC", "");
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

        def ParseHtmlInfo(self, htmlInfo):#为了处理行数不同，节点不同
                try:
                        if (htmlInfo == None):
                                return None;
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        rowCount = self.GetRowCount(htmlInfo);
                        m_yjdgTime_Node_Xpath = System.String.Format(self.yjdgTime_Node_Xpath, rowCount.ToString());
                        m_vessel_voyage_Node_Xpath = System.String.Format(self.vessel_voyage_Node_Xpath, rowCount.ToString());
                        node_yjdgTime = hap_HtmlInfo.DocumentNode.SelectSingleNode(m_yjdgTime_Node_Xpath);
                        m_yjdgTime = System.DateTime.Now;
                        if (Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml) == ""):
                                m_yjdgTime = None;
                        else:
                                m_yjdgTime = System.DateTime.ParseExact(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml),"ddMMMyy",System.Globalization.CultureInfo.InvariantCulture);
                        if (self.vessel_voyage_Node_Xpath == None):                                
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_Node_Xpath);
                                #node_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml.Replace(" ", "")) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(m_vessel_voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml)));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;

        def GetRowCount(self, htmlInfo):
                try:
                        rowCount = 0;
                        if (htmlInfo == None):
                                return rowCount;                        
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        node_dataTable = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.dataTable_Node_Xpath);
                        if (node_dataTable == None or node_dataTable.ChildNodes.Count == 0):
                                return rowCount;
                        for childNode in node_dataTable.ChildNodes:
                                if (childNode.Name == "tr"):
                                        rowCount = rowCount + 1;
                        return rowCount;
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetRowCount Error:" + ex.Message;
                        return None;
        
        def GetVessel(self, vesselAndVoyage):#HANJIN VALENCIA(HJVL0090W) 
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        if (vesselAndVoyage.Contains("(")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.find("(")];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;

