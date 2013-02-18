# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Model")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Model")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Service")

import System;
import System.Windows.Forms;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Service;

def execute(masterForm):
        entity = masterForm.DisplayManager.CurrentItem;
        detailCm = masterForm.ArchiveDetailForm.DetailGrids[0].ControlManager;
        checkForm = Feng.ServiceProvider.GetService[Feng.IWindowFactory]().CreateWindow(Feng.ADInfoBll.Instance.GetWindowInfo("选择_凭证_已确认对账单"));
        if (checkForm.ShowDialog() == System.Windows.Forms.DialogResult.OK):
                with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.对账单]() as rep:
                        for i in checkForm.SelectedEntites:
                                rep.Initialize(i.费用, i);
                                Hd.Service.process_pz.AddFees(masterForm.DisplayManager.CurrentItem, i.费用, detailCm, True, i.收付标志);
if __name__ == "<module>" or __name__ == "__builtin__":
	execute(masterForm);
	

