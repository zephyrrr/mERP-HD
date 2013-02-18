# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("IronPython")
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
    def DoWork():
        Feng.Utils.ReportGenerator.GenerateMonthReport(entity, reportListArray);
        return None;
    def WorkDown(result):
        masterForm.ControlManager.OnCurrentItemChanged();
    entity = masterForm.DisplayManager.CurrentItem;
    reportList = [u'报表_财务月报表', u'报表_进口报关业务月报表',u'报表_业务盈亏预估表',u'报表_财务费用开支明细',u'报表_管理费用开支明细',u'报表_处理坏账损失明细',u'报表_营业外收入明细',u'报表_借出款明细',u'报表_贷款明细',u'报表_固定资产明细',u'报表_进口报关业务明细',u'报表_其它开支明细',u'报表_实付佣金明细',u'报表_业务应收款月报表_总表',u'报表_业务应收款月报表_进口报关',u'报表_业务应收款月报表_内贸出港',u'报表_业务应收款月报表_其它',u'报表_业务应付款月报表_总表',u'报表_业务应付款月报表_进口报关',u'报表_业务应付款月报表_内贸出港',u'报表_业务应付款月报表_其它',u'报表_他人投资公司明细',u'报表_公司投资他人明细',u'报表_资金流水',u'报表_资金明细',u'报表_业务盈亏预估表',u'报表_进口报关毛利逐票明细表(预估)',u'报表_进口报关盈亏简明表(预估)'];
    reportListArray = System.Array[str](reportList);
    Feng.Utils.ProgressAsyncHelper(     \
                Feng.Async.AsyncHelper.DoWork(DoWork), \
                Feng.Async.AsyncHelper.WorkDone(WorkDown), \
                masterForm, "生成");
    
    

if __name__ == "<module>" or __name__ == "__builtin__":
	execute(masterForm);

