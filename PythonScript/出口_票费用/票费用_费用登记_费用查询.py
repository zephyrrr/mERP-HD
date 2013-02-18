# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("Feng.Base")
clr.AddReference("Feng.View")
clr.AddReference("Feng.Windows.Application")

import Feng;

def execute():
        masterForm = Feng.ServiceProvider.GetService[Feng.IApplication]().ActiveChildMdiForm;
        if (masterForm.DisplayManager.CurrentItem == None):
                masterForm.ArchiveDetailForm.DisplayManager.SearchManager.LoadData(Feng.SearchExpression.Eq("费用实体.ID", None), None);
                return;
        masterForm.ArchiveDetailForm.ControlManager.Dao.Set费用实体(masterForm.DisplayManager.CurrentItem.ID);
        if (masterForm.ArchiveDetailForm.GridName == "票费用_票费用项费用登记_视图"):
                masterForm.ArchiveDetailForm.DisplayManager.SearchManager.LoadData(Feng.SearchExpression.Eq("费用实体", masterForm.DisplayManager.CurrentItem.ID), None);
        else:
                masterForm.ArchiveDetailForm.DisplayManager.SearchManager.LoadData(Feng.SearchExpression.Eq("费用实体.ID", masterForm.DisplayManager.CurrentItem.ID), None);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute();

