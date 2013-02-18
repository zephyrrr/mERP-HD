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

def execute(detailForm):
    masterForm = detailForm.ParentArchiveForm;
    result = masterForm.DoDefaultAdd();
    if (result):
        if (masterForm.MasterGrid.CurrentRow != None):
            detailForm.ControlManager.DisplayManager.DataControls["相关人编号"].SelectedDataValue = masterForm.MasterGrid.CurrentRow.Cells["委托人"].Value;
    return result;
if __name__ == "<module>" or __name__ == "__builtin__":
    result = execute(detailForm);

