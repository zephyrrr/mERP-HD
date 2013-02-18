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
                self.companyName_zh_cn = "阿拉伯";#船公司中文名
                self.companyName_us_en = "UASC";#船公司英文名
                self.searchMode = "get";
                self.search_url = "http://uasconline.net/uascOnlineWeb/EcomApplicationCtrlServlet?iId=100044&ContainerNo=#箱号#&BookNo=#提单号#&nameList=&sequence=#箱号#";#根据提单号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/body[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/div[1]/div[2]/table[1]/tr[2]/td[6]";#预计到港时间节点
                self.vessel_Node_Xpath = "/body[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/div[1]/div[2]/table[1]/tr[2]/td[4]";#船名节点
                self.voyage_Node_Xpath = "//body[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/div[1]/div[2]/table[1]/tr[2]/td[5]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点

        def GetHtmlInfo(self, tdh):#需要根据提单号、箱号查询，由ParseHtmlInfo执行所有操作
                try:                        
                        if (tdh == None):
                                return None;
                        return tdh;
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;                

        def ParseHtmlInfo(self, htmlInfo):#参数htmlInfo为提单号
                try:
                        if (htmlInfo == None):
                                return None;
                        #根据箱号查询预计到港时间
                        xiangHao = Feng.Data.DbHelper.Instance.ExecuteScalar("select top 1 箱号 from 业务备案_普通箱 P inner join 业务备案_进口箱 J on P.id = J.id inner join 业务备案_普通票 A on J.票 = A.id where A.提单号 = '" + htmlInfo + "'");
                        m_search_url = self.search_url.Replace("#提单号#", htmlInfo).Replace("#箱号#", xiangHao);                    
                        htmlInfo = Feng.Net.WebProxy().GetToString(m_search_url);
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        node_yjdgTime = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.yjdgTime_Node_Xpath);
                        m_yjdgTime = System.DateTime.Now;
                        if (Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml) == ""):
                                m_yjdgTime = None;
                        else:
                                m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml));
                        node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_Node_Xpath);
                        return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml)); 
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;

