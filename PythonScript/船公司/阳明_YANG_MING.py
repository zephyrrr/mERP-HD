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
                self.companyName_zh_cn = "阳明";#船公司中文名
                self.companyName_us_en = "YANG MING";#船公司英文名
                self.searchMode = "get";
                self.search_url = "http://www.yangming.com/simplified_version/track_trace/blconnect.asp?searchno=&saveflag=&num1=#提单号#&num2=&num3=&num4=&num5=&num6=&num7=&num8=&num9=&num10=&num11=&num12=";#根据提单号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/table[7]/tr[2]/td[2]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/table[5]/tr[1]/td[1]/strong[1]/a[1]";#船名航次节点

        def ParseHtmlInfo(self, htmlInfo):#为了处理预计到港时间节点的<br>(actual)或<br>(estimated)
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
                                m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml.Replace("<br>(Actual)", "").Replace("<br>(estimated)", "").Replace("<br>(planned)", "")));
                        if (self.vessel_voyage_Node_Xpath == None):                                
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_Node_Xpath);
                                #node_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml));
                                return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml)));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;             
        
        def GetVessel(self, vesselAndVoyage):#HANJIN MARSEILLES - 0145W (45W)
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        if (vesselAndVoyage.Contains("-")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.rfind("-")];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;

