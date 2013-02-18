# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("System")
clr.AddReference("Feng.Base")
clr.AddReference("Feng.Windows.Model")
clr.AddReference("Feng.Windows.Application")
clr.AddReference("System.Windows.Forms")

import System;
import System.Text.RegularExpressions;
import System.Windows.Forms;
import Feng;

def execute(mdiForm):
	return CompareDifferentBoxs();
        
class CompareDifferentBoxs(Feng.IPlugin):
        def OnLoad(self):
                c = Feng.ServiceProvider.GetService[Feng.IWindowFactory]();
                c.WindowCreated += self.onWindowCreatedCreated;
                #c.WindowCreated -= self.onWindowCreatedCreated;
        def OnUnload(self):
                Feng.ServiceProvider.GetService[Feng.IWindowFactory]().WindowCreated -= self.onWindowCreatedCreated;
                
        def onWindowCreatedCreated(self, sender, e):
                window = sender;
                #Feng.ServiceProvider.GetService[Feng.IMessageBox]().ShowWarning(window.Name, "test");
                nameList = ["业务财务_批量费用登记", "进口_箱过程", "内贸出港_箱过程", "进口_箱信息汇总", "内贸出港_箱信息汇总", "选择_应付对账单_费用", "选择_会计凭证_业务费用", "进口_批量费用登记"];
                for name in nameList:
                        if (window.Name == name):
                                window.ToolStrip.Items.Add("比较不同_箱号").Click += self.onToolStripClick;
                                break;
        def onToolStripClick(self, sender, e):
                sb = System.Text.StringBuilder(); 
                parentWindow = sender.Owner.FindForm();
                his = parentWindow.DisplayManager.SearchManager.GetHistory(0);
                if (not System.String.IsNullOrEmpty(his.Expression)):
                        seValues = set();
                        gridValues = set();
                        vs = None;
                        if (not his.Expression.Contains("箱号") or his.Expression == None):
                                return;
                        ses = Feng.SearchExpression.GetSimpleExpressions(Feng.SearchExpression.Parse(his.Expression));
                        for se in ses:
                                if (se.FullPropertyName.Contains("箱号")):
                                        if isinstance(se.Values, System.Collections.IList):
                                                for i in se.Values:
                                                        seValues.add(i);
                                        else:
                                                seValues.add(se.Values);
                                        break;

                        gridValues = self.GetGridCellValue(parentWindow.MasterGrid);
                        diff = seValues.difference(gridValues);
                        for i in diff:
                                sb.Append(i);
                                sb.Append(System.Environment.NewLine);
                        Feng.ServiceProvider.GetService[Feng.IMessageBox]().ShowWarning(sb.ToString(), "不同箱号");
                        if (sb != None and str(sb) != ""):
                                System.Windows.Forms.Clipboard.SetDataObject(sb.ToString());
        #Feng.MessageForm.ShowWarning(__name__, "__name__");

        def GetGridCellValue(self, grid):
                getGridValues = set();
                for row in grid.DataRows:
                        if (row.Cells["箱号"] != None):
                                if (row.Cells["箱号"].Value != None):
                                        getGridValues.add(row.Cells["箱号"].Value.ToString());
                return getGridValues;

if __name__ == "<module>" or __name__ == "__builtin__":
	result = execute(mdiForm);
	

