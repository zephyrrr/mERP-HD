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
import GangQu;

class 港区(GangQu.BaseGangQu):
        def __init__(self):
                self.gangQuName = "北仑三期";#港区名称
                self.searchMode = "get";#查询方式get、post
                self.search_url = "http://nbsct.nbport.com.cn/b2ct/JustPort/cqxx.jsp";#根据时间查询地址
                self.postString = "query=query&begin_time=#开始时间#+12%3A00%3A00&end_time=#结束时间#+16%3A00%3A00&submit=%B2%E9+%D1%AF";#Post查询
                self.ywcm_Node_Xpath = "/html[1]/body[1]/table[4]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[4]/tbody[1]/tr[{0}]/td[2]";#英文船名节点
                self.zwcm_Node_Xpath = "/html[1]/body[1]/table[4]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[4]/tbody[1]/tr[{0}]/td[1]";#中文船名节点
                self.yjkbTime_Node_Xpath = "/html[1]/body[1]/table[4]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[4]/tbody[1]/tr[{0}]/td[7]";#预计靠泊时间节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/table[4]/tr[1]/td[1]/table[1]/tr[2]/td[1]/table[4]/tbody[1]";#数据的<table>节点
                self.beginRow = 2;#HtmlAgilityPack解析后<table>的开始行数

