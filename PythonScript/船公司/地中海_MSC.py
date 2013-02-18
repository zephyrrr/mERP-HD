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
                self.companyName_zh_cn = "地中海";#船公司中文名
                self.companyName_us_en = "MSC";#船公司英文名
                self.searchMode = "get";
                self.search_url = None;#根据提单号查询地址
                self.postString = None;#Post查询
                self.yjdgTime_Node_Xpath = "/div[1]/table[1]/tr[1]/td[1]/table[1]/tr[6]/td[2]";#预计到港时间节点
                self.vessel_Node_Xpath = "/div[1]/table[1]/tr[1]/td[2]/table[1]/tr[{0}]/td[4]";#船名节点
                self.voyage_Node_Xpath = "/div[1]/table[1]/tr[1]/td[2]/table[1]/tr[{0}]/td[5]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点 "/div[1]/table[1]/tr[7]/td[2]"
                self.dataTable_Node_Xpath = "/div[1]/table[1]/tr[1]/td[2]/table[1]";#船名航次的<table>节点

        def GetHtmlInfo(self, tdh):#因分别在2个地址查询预计到港时间、船名航次，所以直接return提单号，由ParseHtmlInfo执行所有操作
                try:                        
                        if (tdh == None):
                                return None;
                        return tdh;
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;                

        def ParseHtmlInfo(self, htmlInfo):
                try:
                        if (htmlInfo == None):
                                return None;
                        webProxy = Feng.Net.WebProxy();                        
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        #根据提单号查询船名航次
                        #tdh_search_url = "http://tracking.mscgva.ch/MscTracking_bl_details.php?SearchStr=#提单号#&Applic=";
                        #tdh_HtmlInfo = webProxy.GetToString(tdh_search_url.Replace("#提单号#", htmlInfo));
                        #hap_HtmlInfo.LoadHtml(tdh_HtmlInfo);
                        #node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_voyage_Node_Xpath);
                        #print node_Vessel_Voyage.InnerHtml;
                        
                        #根据箱号查询预计到港时间
                        xh_search_url = "http://tracking.mscgva.ch/MscTracking_ct_details.php?SearchStr=#箱号#&BR=&Applic=";
                        xiangHao = Feng.Data.DbHelper.Instance.ExecuteScalar("select top 1 箱号 from 业务备案_普通箱 P inner join 业务备案_进口箱 J on P.id = J.id inner join 业务备案_普通票 A on J.票 = A.id where A.提单号 = '" + htmlInfo + "'");
                        xh_HtmlInfo = webProxy.GetToString(xh_search_url.Replace("#箱号#", xiangHao));
                        hap_HtmlInfo.LoadHtml(xh_HtmlInfo);
                        node_yjdgTime = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.yjdgTime_Node_Xpath);
                        m_yjdgTime = System.DateTime.ParseExact(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml),"dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture);
                        #不知道船名的节点行数，循环查询
                        rowCount = self.GetRowCount(xh_HtmlInfo);
                        m_ywcm = None;
                        for i in range(2, rowCount + 1):
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(System.String.Format(self.vessel_Node_Xpath, i.ToString()));
                                if (node_Vessel == None or Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml).Replace(" ", "") == ""):
                                        continue;
                                else:
                                        m_ywcm = Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml);
                                        break;
                        return ShipCompany.ShipResult(m_yjdgTime, m_ywcm); 
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
        
        def GetVessel(self, vesselAndVoyage):#停止使用 NORTHERN JUPITER 1051R
                try:
                        if (vesselAndVoyage == None):
                                return vesselAndVoyage;
                        if (vesselAndVoyage.Contains(" ")):
                                vesselAndVoyage = vesselAndVoyage[:vesselAndVoyage.rfind(" ")];
                        return vesselAndVoyage.strip();
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetVessel Error:" + ex.Message;
                        return None;

