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
                self.companyName_zh_cn = "金星";#船公司中文名
                self.companyName_us_en = "GOLD STAR";#船公司英文名
                self.searchMode = "get";#查询方式get、post
                self.search_url = "http://www.goldstarline.com/Tracing.aspx?hidSearch=true&hidFromHomePage=false&hidSearchType=1&id=166&l=4&textContainerNumber=&rb=BLNum&textBLNumber=#提单号#";#根据提单号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[6]/td[1]/table[1]/tr[1]/td[2]/table[1]/tr[7]/td[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[3]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tr[6]/td[1]/table[1]/tr[1]/td[2]/table[1]/tr[7]/td[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[4]";#船名航次节点

                self.search_url_Container = "http://www.goldstarline.com/Tracing.aspx?hidSearch=true&hidFromHomePage=false&hidSearchType=2&id=166&l=4&rb=ConNum&textContainerNumber=#箱号#&textBLNumber=";#根据箱号查询地址
                self.container_Node_Xpath = "/html[1]/body[1]/table[1]/tr[6]/td[1]/table[1]/tr[1]/td[2]/table[1]/tr[7]/td[1]/table[1]/tr[2]/td[1]/table[1]/tr[2]/td[1]/a[1]";#根据提单号获取第一个箱号节点

        def GetHtmlInfo(self, tdh):#根据提单号获取箱号，再用箱号查询
                try:                        
                        if (tdh == None):
                                return None;
                        webProxy = Feng.Net.WebProxy();
                        xiangHao = self.GetContainerNum(webProxy.GetToString(self.search_url.Replace("#提单号#", tdh)));
                        m_search_url_Container = self.search_url_Container.Replace("#箱号#", xiangHao);
                        return webProxy.GetToString(m_search_url_Container);
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;

        def GetContainerNum(self, htmlInfo):
                try:
                        if (htmlInfo == None):
                                return None;
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        node_container = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.container_Node_Xpath);
                        return Feng.Net.WebProxy.RemoveSpaces(node_container.InnerHtml);
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetContainerNum Error:" + ex.Message;
                        return None;
                        

