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
                self.companyName_zh_cn = "马士基";#船公司中文名
                self.companyName_us_en = "Maersk";#船公司英文名
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www.maerskline.com/appmanager/maerskline/public?_nfpb=true&portlet_quickentries_2_actionOverride=%2Fportlets%2Fquickentries%2FtrackCargo&_windowLabel=portlet_quickentries_2&_pageLabel=home";#根据提单号、箱号查询地址
                self.postString = "portlet_quickentries_2wlw-select_key%3A%7BactionForm.trackType%7DOldValue=true&portlet_quickentries_2wlw-select_key%3A%7BactionForm.trackType%7D=CONTAINERNUMBER&portlet_quickentries_2%7BactionForm.trackNo%7D=#箱号#";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[2]/table[3]/tr[{0}]/td[3]";#预计到港时间节点
                self.vessel_Node_Xpath = "/html[1]/body[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[2]/table[3]/tr[{0}]/td[4]/span[1]";#船名节点
                self.voyage_Node_Xpath = "/html[1]/body[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[2]/table[3]/tr[{0}]/td[5]/span[1]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[2]/table[3]";#数据<table>的节点
        
        def GetHtmlInfo(self, tdh):#先post查询整票信息，根据获得的url再get查询箱
                try:                        
                        if (tdh == None):
                                return None;
                        m_webProxy = Feng.Net.WebProxy();
                        xiangHao = Feng.Data.DbHelper.Instance.ExecuteScalar("select top 1 箱号 from 业务备案_普通箱 P inner join 业务备案_进口箱 J on P.id = J.id inner join 业务备案_普通票 A on J.票 = A.id where A.提单号 = '" + tdh + "'");
                        m_postString = self.postString.Replace("#箱号#", xiangHao);
                        p_htmlInfo = m_webProxy.PostToString(self.search_url, m_postString);
                        m_search_url = self.ParseUrl(p_htmlInfo);
                        return m_webProxy.GetToString(m_search_url);
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
                                m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml));
                        if (self.vessel_voyage_Node_Xpath == None):                                
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.vessel_Node_Xpath, rowCount.ToString()));
                                #node_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.voyage_Node_Xpath, rowCount.ToString()));
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml.Replace(" ", "")) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(m_vessel_voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml)));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;

        def ParseUrl(self, htmlInfo):#因不知根据箱号查询url中的某个参数来源，所以通过解析htmlInfo获取url
                try:
                        if (htmlInfo == None):
                                return None;
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        node_search_url = hap_HtmlInfo.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[2]/tr[1]/td[1]/table[1]/tr[1]/td[2]/table[2]/tr[2]/td[4]/a[1]/@href[1]");
                        return node_search_url.Attributes["href"].Value;
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseUrl Error:" + ex.Message;
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

