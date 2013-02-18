# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Report")

import Feng;
import Feng.Windows.Forms;
import System;
import System.Windows.Forms;
import Hd.Report;

def execute(masterForm):
    entity = masterForm.DisplayManager.CurrentItem;
    if (entity == None):
        Feng.MessageForm.ShowInfo("请选择要查看的报表");
    else:
        Feng.Utils.ReportGenerator.OpenReports(entity);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);


