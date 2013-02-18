# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Report")
clr.AddReferenceByPartialName("Feng.Data")

import Feng;
import Feng.Windows.Forms;
import System;
import System.Windows.Forms;
import Hd.Report;

def execute(masterForm):
    entity = masterForm.DisplayManager.CurrentItem;
    if entity == None:
        MessageForm.ShowError('请选择要打印的对账单！');
    else:
        s = "SELECT COUNT(DISTINCT 费用项) FROM 财务_费用 WHERE 对账单 = '" + str(entity.ID) + "'";
        cnt = Feng.Data.DbHelper.Instance.ExecuteScalar(s);
        Hd.Report.ReportPrint.打印原始对账单(entity.编号,cnt);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute(masterForm);
	

