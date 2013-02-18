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
import ShipCompany;

class 船公司(ShipCompany.BaseShipCompany):#HtmlInfo无法正确解析
        def __init__(self):
                self.companyName_zh_cn = "法国达飞";#船公司中文名
                self.companyName_us_en = "CMA CGM";#船公司英文名
                self.searchMode = "post";
                self.search_url = "http://www.cma-cgm.com/eBusiness/Tracking/Default.aspx";#根据提单号查询地址
                self.postString = "__WPPS=s#&__LASTFOCUS=&ctl00_MainLeftMenu_LMenu_ExpandState=eunnnunnnnnunn&ctl00_MainLeftMenu_LMenu_SelectedNode=ctl00_MainLeftMenu_LMenun4&__EVENTTARGET=&__EVENTARGUMENT=&ctl00_MainLeftMenu_LMenu_PopulateLog=&__VSTATE=#VState#&__VIEWSTATE=&ctl00%24ContentPlaceBody%24DListSearch=BL&ctl00%24ContentPlaceBody%24TextSearch=#提单号#";#Post查询
                self.yjdgTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/div[3]/div[1]/div[2]/div[3]/table[1]/tr[3]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/table[1]/tr[1]/td[1]/table[1]/td[3]/span[1]";#预计到港时间节点
                self.vessel_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/div[3]/div[1]/div[2]/div[3]/table[1]/tr[3]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/table[1]/tr[3]/td[1]/div[1]/table[1]/tr[3]/td[4]";#船名节点
                self.voyage_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[1]/div[3]/div[1]/div[2]/div[3]/table[1]/tr[3]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[1]/td[1]/div[1]/div[1]/table[1]/tr[3]/td[1]/div[1]/table[1]/tr[3]/td[5]/span[1]/a[1]";#航次节点
                self.vessel_voyage_Node_Xpath = None;#船名航次节点

        def GetHtmlInfo(self, tdh):
                try:                        
                        if (tdh == None):
                                return None;
                        webProxy = Feng.Net.WebProxy();
                        htmlInfo = webProxy.GetToString(self.search_url);
                        hap_HtmlInfo = HtmlAgilityPack.HtmlDocument();
                        hap_HtmlInfo.LoadHtml(htmlInfo);

                        node_viewState = hap_HtmlInfo.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/input[8]/@value[1]");
                        viewState = node_viewState.Attributes["value"].Value;
                        m_postString = self.postString.Replace("#VState#", System.Web.HttpUtility.UrlEncode(viewState));
                        m_postString = m_postString.Replace("#提单号#", tdh);

                        #node_eventValidation = hap_HtmlInfo.DocumentNode.SelectSingleNode("/html[1]/body[1]/input[2]/@value[1]");
                        #eventValidation = node_eventValidation.Attributes["value"].Value;
                        #m_postString = m_postString.Replace("#EventVaildation#", System.Web.HttpUtility.UrlEncode(eventValidation));
                        return webProxy.PostToString(self.search_url, m_postString);
                except System.Exception, ex:
                        print self.companyName_zh_cn + self.companyName_us_en + "\tGetHtmlInfo Error:" + ex.Message;
                        return None;        

