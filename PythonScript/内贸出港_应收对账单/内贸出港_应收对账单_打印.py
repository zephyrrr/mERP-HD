# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("System")
clr.AddReference("System.Windows.Forms")
clr.AddReference("System.Drawing")
clr.AddReference("Feng.Base")
clr.AddReference("Feng.Data")
clr.AddReference("Feng.Windows.Forms")
clr.AddReference("Feng.Windows.Application")
clr.AddReference("Hd.Report")

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
        s = "SELECT COUNT(DISTINCT 费用项) FROM 报表_内贸结算明细 WHERE 对账单 = '" + str(entity.编号) + "'";
        cnt = Feng.Data.DbHelper.Instance.ExecuteScalar(s);
        Hd.Report.ReportPrint.打印内贸结算明细单(entity.编号,cnt);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute(masterForm);
	

