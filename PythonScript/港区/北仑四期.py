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
                self.gangQuName = "北仑四期";#港区名称
                self.searchMode = "post";#查询方式get、post
                self.search_url = "http://csct.nbport.com.cn/csct/business/cqcx.jsp";#根据时间查询地址
                self.postString = "check_mode=0&start_date=#开始时间#&end_date=#结束时间#&e_voyage=&vRef=&holdFlag=display&imageField32.x=33&imageField32.y=11";#Post查询
                self.ywcm_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[1]/tr[2]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[3]/div[1]";#英文船名节点
                self.zwcm_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[1]/tr[2]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[2]/div[1]";#中文船名节点
                self.yjkbTime_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[1]/tr[2]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[1]/table[1]/tr[{0}]/td[7]/div[1]";#预计靠泊时间节点
                self.dataTable_Node_Xpath = "/html[1]/body[1]/table[1]/tr[1]/td[2]/table[1]/tr[2]/td[1]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/table[1]/tr[4]/td[1]/table[1]";#数据的<table>节点
                self.beginRow = 3;#HtmlAgilityPack解析后<table>的开始行数

