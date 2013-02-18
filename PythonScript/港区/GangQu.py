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

class BaseGangQu:
        def __init__(self):                
                self.gangQuName = None;#港区名称
                self.searchMode = "get";#查询方式get、post
                self.search_url = None;#根据时间查询地址
                self.postString = None;#Post查询
                self.ywcm_Node_Xpath = None;#英文船名节点
                self.zwcm_Node_Xpath = None;#中文船名节点
                self.yjkbTime_Node_Xpath = None;#预计靠泊时间节点
                self.dataTable_Node_Xpath = None;#数据的<table>节点
                self.beginRow = None;#HtmlAgilityPack解析后<table>的开始行数
        
        def GetHtmlInfo(self, beginTime, endTime):
                try:                        
                        if (beginTime == None or endTime == None):
                                return None;
                        htmlInfo = None;
                        if (self.searchMode == "get"):
                                m_search_url = self.search_url.Replace("#开始时间#", beginTime);
                                m_search_url = m_search_url.Replace("#结束时间#", endTime);
                                htmlInfo = Feng.Net.WebProxy().GetToString(m_search_url);
                        else:
                                m_postString = self.postString.Replace("#开始时间#", beginTime);
                                m_postString = m_postString.Replace("#结束时间#", endTime);
                                htmlInfo = Feng.Net.WebProxy().PostToString(self.search_url, m_postString);
                        return htmlInfo;
                except System.Exception, ex:
                        print self.gangQuName + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;

        def ParseHtmlInfo(self, htmlInfo):
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
                                results.append(GangQuResult(Feng.Net.WebProxy.RemoveSpaces(node_ywcm.InnerHtml.strip()), Feng.Net.WebProxy.RemoveSpaces(node_zwcm.InnerHtml.strip()), m_yjkbTime));
                        return results;
                except System.Exception, ex:
                        print self.gangQuName + "\tParseHtmlInfo Error:" + ex.Message;
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
                        print self.gangQuName + "\tGetRowCount Error:" + ex.Message;
                        return None;

#港区查询单行结果
class GangQuResult:
        def __init__(self, ywcm, zwcm, yjkbTime):
                self.ywcm = ywcm;#英文船名
                self.zwcm = zwcm;#中文船名
                self.yjkbTime = yjkbTime;#预计靠泊时间

        def Show(self):
                return "英文船名:" + str(self.ywcm) + "中文船名:" + str(self.zwcm) + "\t预计靠泊时间:" + str(self.yjkbTime);

