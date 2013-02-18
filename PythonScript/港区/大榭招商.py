# -*- coding: UTF-8 -*- 
import clr
import sys
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Web")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Net")
clr.AddReferenceByPartialName("HtmlAgilityPack")

import System;
import System.Web;
import Feng;
import Feng.Net;
import HtmlAgilityPack;
import GangQu;

class 港区(GangQu.BaseGangQu):#网页源码，分析不一致
        def __init__(self):
                self.gangQuName = "大榭招商";#港区名称
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://www.cmict.com.cn/business/public/Sailing.aspx";#根据时间查询地址
                self.postString = "__VIEWSTATE=#ViewState#&__EVENTVALIDATION=#EventVaildation#&pState=D&Button1=+%E6%9F%A5+%E8%AF%A2+";#Post查询
                self.ywcm_Node_Xpath = "/html[1]/body[1]/div[1]/table[2]/tr[{0}]/td[3]";#英文船名节点
                self.yjkbTime_Node_Xpath = "/html[1]/body[1]/div[1]/table[2]/tr[{0}]/td[7]";#预计靠泊时间节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/div[1]/table[2]";#数据的<table>节点
                self.beginRow = 2;#HtmlAgilityPack解析后<table>的开始行数

        def GetHtmlInfo(self, beginTime, endTime):
                try:                        
                        if (beginTime == None or endTime == None):
                                return None;
                        webProxy = Feng.Net.WebProxy();
                        htmlInfo = webProxy.GetToString(self.search_url);
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);

                        node_viewState = hap_HtmlInfo.DocumentNode.SelectSingleNode("/html[1]/body[1]/input[1]/@value[1]");
                        viewState = node_viewState.Attributes["value"].Value;
                        m_postString = self.postString.Replace("#ViewState#", System.Web.HttpUtility.UrlEncode(viewState));

                        node_eventValidation = hap_HtmlInfo.DocumentNode.SelectSingleNode("/html[1]/body[1]/input[2]/@value[1]");
                        eventValidation = node_eventValidation.Attributes["value"].Value;
                        m_postString = m_postString.Replace("#EventVaildation#", System.Web.HttpUtility.UrlEncode(eventValidation));
                        return webProxy.PostToString(self.search_url, m_postString);
                except System.Exception, ex:
                        print self.gangQuName + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;

