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
    if (not detailForm.ControlManager.CheckControlValue()):
        return False;
    detailForm.ControlManager.SaveCurrent();
    dzd = detailForm.DisplayManager.CurrentEntity;
    dzd.对账单类型 = Hd.Model.对账单类型.应收对账单;
    dzd.业务类型编号 = 11;
    dzd.收付标志 = Hd.Model.收付标志.收;
    dzd.费用项编号 = "000";
    dzd.Submitted = True;
    dzd.费用 = System.Collections.Generic.List[Hd.Model.费用]();
    dao = Hd.Model.对账单Dao();
    with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.对账单]() as rep:
        try:
            cnt = 0;
            rep.BeginTransaction();
            for row in masterForm.ArchiveDetailForm.DetailGrids[0].DataRows:
                if (Feng.Utils.ConvertHelper.ToBoolean(row.Cells["选定"].Value)):
                    fees = rep.Session.CreateCriteria[Hd.Model.费用]()    \
                                .Add(NHibernate.Criterion.Expression.Eq("费用实体.ID", row.Cells["Id"].Value))   \
                                .Add(NHibernate.Criterion.Expression.Eq("收付标志", Hd.Model.收付标志.收))   \
                                .Add(NHibernate.Criterion.Expression.IsNull("对账单"))   \
                                .CreateCriteria("费用类别") \
                                .Add(NHibernate.Criterion.Expression.Eq("大类", "业务常规"))   \
                                .List[Hd.Model.费用]();
                    cnt += fees.Count;
                    for fee in fees:
                        dzd.费用.Add(fee);
            if (cnt == 0):
                raise System.ArgumentException("费用数量为0，不能生成对账单！");
            
            dao.Save(rep, dzd);
            for fee in dzd.费用:
                fee.对账单 = dzd;
                rep.Update(fee);
            rep.CommitTransaction();
            detailForm.ControlManager.State = Feng.StateType.View;
            if (detailForm.ControlManager.ControlCheckExceptionProcess != None):
                detailForm.ControlManager.ControlCheckExceptionProcess.ClearError();
            detailForm.ControlManager.EndEdit(False);
            return True;
        except System.Exception, ex:
            rep.RollbackTransaction();
            Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);
            return False;
if __name__ == "<module>" or __name__ == "__builtin__":
    result = execute(detailForm);

