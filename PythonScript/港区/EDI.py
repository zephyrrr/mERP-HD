# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Net")
clr.AddReferenceByPartialName("HtmlAgilityPack")
clr.AddReferenceByPartialName("Hd.NetRead")

import System;
import Feng;
import Feng.Net;
import Hd.NetRead;
import HtmlAgilityPack;
import GangQu;

class 港区(GangQu.BaseGangQu):
        def __init__(self, gangQuName):
                self.gangQuName = gangQuName;#港区名称
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www.npedi.com/edi/voyageInfoAction.do?pageIndex=1";#根据港区查询地址
                self.postString = "cpcode=#港区#&ename=&voyage=&selectAll=on";#Post查询
                self.ywcm_Node_Xpath = "/html[1]/body[1]/tr[3]/td[1]/table[1]/tr[{0}]/td[3]";#英文船名节点
                self.zwcm_Node_Xpath = "/html[1]/body[1]/tr[3]/td[1]/table[1]/tr[{0}]/td[5]";#中文船名节点
                self.yjkbTime_Node_Xpath = "/html[1]/body[1]/tr[3]/td[1]/table[1]/tr[{0}]/td[12]";#预计靠泊时间节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/tr[3]/td[1]/table[1]";#数据的<table>节点
                self.beginRow = 2;#HtmlAgilityPack解析后<table>的开始行数

        def GetHtmlInfo(self, gqValue):#查询条件不同，按港区查询
                try:                        
                        if (gqValue == None):
                                return None;
                        htmlInfo = None;
                        if (self.searchMode == "get"):
                                m_search_url = self.search_url.Replace("#港区#", gqValue);
                                htmlInfo = Feng.Net.WebProxy().GetToString(m_search_url);
                        else:
                                m_postString = self.postString.Replace("#港区#", gqValue);
                                read = Hd.NetRead.npediRead();
                                htmlInfo = read.GetHtmlInfo(self.search_url, m_postString);
                        return htmlInfo;
                except System.Exception, ex:
                        print self.gangQuName + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;

        def ParseHtmlInfo(self, htmlInfo):#因查询全部数据，所以过滤今天之前的无效数据
                try:
                        if (htmlInfo == None):
                                return None;
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);
                        rowCount = self.GetRowCount(htmlInfo) - self.beginRow + 1;
                        results = [];                        
                        for rowIndex in range(self.beginRow, rowCount + self.beginRow):
                                node_ywcm = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.ywcm_Node_Xpath, rowIndex));
                                node_zwcm = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.zwcm_Node_Xpath, rowIndex));
                                node_yjkbTime = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.yjkbTime_Node_Xpath, rowIndex));
                                m_yjkbTime = System.DateTime.Now;                                
                                if (Feng.Net.WebProxy.RemoveSpaces(node_yjkbTime.InnerHtml) == ""):
                                        m_yjkbTime = None;
                                else:
                                        m_yjkbTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjkbTime.InnerHtml));
                                        if (m_yjkbTime < System.DateTime.Today):
                                                return results;
                                results.append(GangQu.GangQuResult(Feng.Net.WebProxy.RemoveSpaces(node_ywcm.InnerHtml.strip()), Feng.Net.WebProxy.RemoveSpaces(node_zwcm.InnerHtml.strip()), m_yjkbTime));
                        return results;
                except System.Exception, ex:
                        print self.gangQuName + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;

