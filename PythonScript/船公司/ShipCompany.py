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

class BaseShipCompany:
        def __init__(self):                
                self.companyName_zh_cn = None;#船公司中文名
                self.companyName_us_en = None;#船公司英文名
                self.searchMode = "get";#查询方式get、post
                self.search_url = None;#根据提单号、箱号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = None;#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点 优先
        
        def GetHtmlInfo(self, tdh):
                try:                        
                        if (tdh == None):
                                return None;
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

        def ParseHtmlInfo(self, htmlInfo):
                try:
                        if (htmlInfo == None):
                                return None;
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        node_yjdgTime = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.yjdgTime_Node_Xpath);
                        m_yjdgTime = System.DateTime.Now;
                        if (Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml) == ""):
                                m_yjdgTime = None;
                        else:
                                m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml));
                        if (self.vessel_voyage_Node_Xpath == None):                                
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_Node_Xpath);
                                #node_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.voyage_Node_Xpath);
                                #return ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml.Replace(" ", "")) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_voyage_Node_Xpath);
                                #return ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml)));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;

        def GetVessel(self, vesselAndVoyage):#解析船名航次，获取船名
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        #if (vesselAndVoyage.find(" ") <> -1):
                        #        vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.find(" ")];
                        if (vesselAndVoyage.Contains("-")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.rfind("-")];
                        if (vesselAndVoyage.Contains("/")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.find("/")];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;

#船公司查询结果
class ShipResult:
        def __init__(self, yjdgTime, vesselAndVoyage):
                self.yjdgTime = yjdgTime;#预计到港时间
                self.vesselAndVoyage = vesselAndVoyage;#船名航次

        def Show(self):
                return "预计到港时间:" + str(self.yjdgTime) + "\t船名航次:" + str(self.vesselAndVoyage);

