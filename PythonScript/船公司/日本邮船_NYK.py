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
                self.companyName_zh_cn = "日本邮船";#船公司中文名
                self.companyName_us_en = "NYK";#船公司英文名
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www2.nykline.com/ct/containerSearch.nyk?lang=zh&country=CHN";#根据提单号、箱号查询地址
                self.postString = "searchText=#提单号#&ctReset=true&fromCT=true";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/div[3]/div[1]/table[3]/tr[6]/td[1]";#预计到港时间节点
                self.vessel_Node_Xpath = None;#船名节点
                self.voyage_Node_Xpath = None;#航次节点
                self.vessel_voyage_Node_Xpath = "/html[1]/body[1]/div[3]/div[1]/table[3]/tr[6]/td[4]/a[1]";#船名航次节点

        def GetHtmlInfo(self, tdh):#为了忽略错误HTTP/1.1  100  Continue  初始的请求已经接受，客户应当继续发送请求的其余部分。ex.Message中含有htmlInfo
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
                        #print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return ex.Message;
                
        def ParseHtmlInfo(self, htmlInfo):#为了处理预计到港时间后面的(Estimated)<tt class="blue">*</tt>
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
                                if (node_yjdgTime.InnerHtml.Contains("(")):
                                        m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml.Substring(0,node_yjdgTime.InnerHtml.IndexOf('('))));
                                else:
                                        m_yjdgTime = System.DateTime.Parse(Feng.Net.WebProxy.RemoveSpaces(node_yjdgTime.InnerHtml));
                        if (self.vessel_voyage_Node_Xpath == None):                                
                                node_Vessel = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_Node_Xpath);
                                #node_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml.Replace(" ", "")) + "/" + Feng.Net.WebProxy.RemoveSpaces(node_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel.InnerHtml));
                        else:
                                node_Vessel_Voyage = hap_HtmlInfo.DocumentNode.SelectSingleNode(self.vessel_voyage_Node_Xpath);
                                #return ShipCompany.ShipResult(m_yjdgTime, Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml.Replace(" ", "")));
                                return ShipCompany.ShipResult(m_yjdgTime, self.GetVessel(Feng.Net.WebProxy.RemoveSpaces(node_Vessel_Voyage.InnerHtml)));
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tParseHtmlInfo Error:" + ex.Message;
                        return None;
        

