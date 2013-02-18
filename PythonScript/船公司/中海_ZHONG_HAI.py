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
                self.companyName_zh_cn = "中海";#船公司中文名
                self.companyName_us_en = "ZHONG HAI";#船公司英文名
                self.searchMode = "post";
                self.search_url = "http://222.66.158.204/cargo_track/cargo_track_rst.jsp";#根据提单号查询地址
                self.postString = "tr_num=bl_no&tf_bl_no=#提单号#&submit%222=++O+K++";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/center[1]/table[1]/tr[1]/td[1]/table[2]/tr[9]/td[1]/table[1]/tr[2]/td[5]/div[1]/font[1]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/center[1]/table[1]/tr[1]/td[1]/table[2]/tr[9]/td[1]/table[1]/tr[2]/td[1]/div[1]/font[1]/a[1]";#船名航次节点

        def ParseHtmlInfo(self, htmlInfo):#<br>替换成/
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
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml));
                                return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace("<br>", "/").Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace("<br>", "/"))));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None
        

