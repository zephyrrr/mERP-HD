# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Jk")
clr.AddReferenceByPartialName("Hd.NetRead")

import sys;
import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Jk;
import Hd.NetRead;

def execute(masterForm):
    entity = masterForm.DisplayManager.CurrentItem;
    read = Hd.NetRead.npediRead();
    boxList = read.集装箱出门查询(entity.票.提单号);
    if (boxList.Count <= 0):
        System.Windows.Forms.MessageBox.Show("未读得任何数据，请稍后再读。");
        return;

    detailCm = masterForm.ArchiveDetailForm.DetailGrids[0].ControlManager;

    for data in boxList:
        for row in masterForm.ArchiveDetailForm.DetailGrids[0].DataRows:
            if (row.Cells["箱号"].Value != None and row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim()):
             		#为了改变ControlManager的Position
              	masterForm.ArchiveDetailForm.DetailGrids[0].CurrentRow = row;
                row.BeginEdit();
                #row.Cells["提箱时间"].EnterEdit();
                #row.Cells["提箱时间"].LeaveEdit(True);
                row.Cells["提箱时间"].Value = data.出门时间;
                row.EndEdit();
                break;
            
if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);

