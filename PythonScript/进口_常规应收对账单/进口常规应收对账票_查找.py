# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Jk")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Jk;

def execute(masterForm):
    cmMaster = masterForm.ArchiveDetailForm.ControlManager;
    cmMaster.SaveCurrent();
    if (cmMaster.DisplayManager.CurrentEntity.相关人编号 == None):
        Feng.MessageForm.ShowWarning("请输入相关人编号！");
        return;
    if (cmMaster.DisplayManager.CurrentEntity.关账日期 == None):
        Feng.MessageForm.ShowWarning("请输入关账日期！");
        return;
    masterForm.ArchiveDetailForm.DetailGrids[0].DisplayManager.SearchManager.ReloadData();
    
if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);

