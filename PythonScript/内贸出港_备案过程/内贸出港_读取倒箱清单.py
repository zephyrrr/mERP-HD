# -*- coding: UTF-8 -*- 
import clr
clr.AddReferenceByPartialName("System")
clr.AddReferenceByPartialName("System.Windows.Forms")
clr.AddReferenceByPartialName("System.Drawing")
clr.AddReferenceByPartialName("NHibernate")
clr.AddReferenceByPartialName("Feng.Base")
clr.AddReferenceByPartialName("Feng.Windows")
clr.AddReferenceByPartialName("Feng.Windows.Forms")
clr.AddReferenceByPartialName("Feng.Windows.Application")
clr.AddReferenceByPartialName("Hd.Model.Base")
clr.AddReferenceByPartialName("Hd.Model.Dao")
clr.AddReferenceByPartialName("Hd.Model.Nmcg")

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Nmcg;

def execute(masterForm):
    fileName = "\\\\192.168.0.10\\业务数据\\2版内贸出港\\倒箱清单\\" + System.DateTime.Today.ToString("yyyy-MM") + ".xls";
    try:
        dts = Feng.Utils.ExcelHelper.ReadExcel(fileName);
        if (dts.Count == 0):
            raise Feng.InvalidUserOperationException("Excel中无Sheet数据！");
        dt = dts[0];
        if (dt.Rows.Count == 0 or dt.Columns.Count == 0):
            raise Feng.InvalidUserOperationException("Excel中无数据！");
        checkList = (u"预配提单号",u"箱号",u"车号",u"回货箱号",u"重量",u"件数",u"装货时间",u"破损记录");
        for check in checkList:
            if (not dt.Columns.Contains(check)):
                raise Feng.InvalidUserOperationException("文件中必须要有" + check + "，请查证！");
        detailGrid = masterForm.ArchiveDetailForm.DetailGrids[0];
        detailCm = detailGrid.ControlManager;
        piao = masterForm.DisplayManager.CurrentItem;
        yptdh = piao.预配提单号;
        for i in range(detailGrid.DataRows.Count):
            gridRow = detailGrid.DataRows[i];
            for row in dt.Rows:
                if (gridRow.Cells["箱号"].Value != None and row["箱号"].ToString() == gridRow.Cells["箱号"].Value.ToString().Trim() and row["预配提单号"].ToString() == yptdh):
                    gridRow.BeginEdit();
                    #必须要EnterCell，否则不会保存（Core中以CellEnter作为开始修改条件）
                    #Cell.Value必须在LeaveEdit以后，否则不会保存
                    gridRow.Cells["车号"].EnterEdit();
                    gridRow.Cells["车号"].LeaveEdit(True);
                    gridRow.Cells["车号"].Value = Feng.Utils.ConvertHelper.ToString(row["车号"]);
                    gridRow.Cells["回货箱号"].Value = Feng.Utils.ConvertHelper.ToString(row["回货箱号"]);
                    gridRow.Cells["重量"].Value = Feng.Utils.ConvertHelper.ToInt(row["重量"]);
                    gridRow.Cells["件数"].Value = Feng.Utils.ConvertHelper.ToInt(row["件数"]);
                    gridRow.Cells["装货时间"].Value = Feng.Utils.ConvertHelper.ToDateTime(row["装货时间"]);
                    gridRow.Cells["破损记录"].Value = Feng.Utils.ConvertHelper.ToString(row["破损记录"]);
                    gridRow.EndEdit();
                    break;
        #masterForm.ControlManager.OnCurrentItemChanged();
    except System.Exception, ex:
        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
    execute(masterForm);
                

