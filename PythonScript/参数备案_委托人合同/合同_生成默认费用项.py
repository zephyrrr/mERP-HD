# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("System")
clr.AddReference("System.Windows.Forms")
clr.AddReference("NHibernate")
clr.AddReference("Feng.Base")
clr.AddReference("Feng.Model")
clr.AddReference("Feng.Data")
clr.AddReference("Feng.Windows.Forms")
clr.AddReference("Feng.Windows.Application")
clr.AddReference("Hd.Model.Base")
clr.AddReference("Hd.Model.Yw")
clr.AddReference("Hd.Model.Dao")
clr.AddReference("Xceed.Grid");

import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;

def execute(masterForm):
        
        existFees = System.Collections.Generic.Dictionary[str, System.Boolean]()
        for row in masterForm.ArchiveDetailForm.DetailGrids[0].DataRows:
                existFees[row.Cells["费用项编号"].Value.ToString()] = True
        masterItem = masterForm.DisplayManager.CurrentItem;
        if (masterItem.业务类型编号 == 11):
                dv = Feng.NameValueMappingCollection.Instance.GetDataSource("费用项_业务_进口_常规")
        elif (masterItem.业务类型编号 == 15):
                dv = Feng.NameValueMappingCollection.Instance.GetDataSource("费用项_业务_内贸出港_常规")
        elif (masterItem.业务类型编号 == 45):
                dv = Feng.NameValueMappingCollection.Instance.GetDataSource("费用项_业务_进口其他业务")
        else:
                System.Windows.Forms.MessageBox.Show(masterItem.业务类型编号.ToString());
                return;
        #System.Windows.Forms.MessageBox.Show(dv.Count.ToString());
	with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.委托人合同费用项]() as rep:
                try:
                        rep.BeginTransaction();
                        for row in dv:
                                s = row["编号"].ToString();
                                if existFees.ContainsKey(s):
                                        continue
                                if (masterItem.GetType() == clr.GetClrType(Hd.Model.付款合同)):
                                        i = Hd.Model.付款合同费用项()
                                        i.IsActive = True
                                        i.付款合同 = masterItem
                                        i.付款合同费用项类型 = Hd.Model.付款合同费用项类型.理论值计算
                                        i.是否生成实际费用 = True
                                        i.费用项编号 = s
                                        i.默认相关人 = None
                                        (Hd.Model.HdBaseDao[Hd.Model.付款合同费用项]()).Save(i)
                                        masterItem.合同费用项.Add(i)
                                elif (masterItem.GetType() == clr.GetClrType(Hd.Model.委托人合同)):
                                        dao1 = Hd.Model.HdBaseDao[Hd.Model.委托人合同费用项]();
                                        dao2 = Hd.Model.HdBaseDao[Hd.Model.费用理论值信息]();
                                        i = Hd.Model.委托人合同费用项();
		    			i.IsActive = True;
		    			i.委托人合同 = masterItem;
		    			i.委托人合同费用项类型 = Hd.Model.委托人合同费用项类型.理论值计算;
		    			i.是否生成实际费用 = True;
		    			i.费用项编号 = s;
		    			i.是否生成实际费用 = True;
		    			dao1.Save(rep, i);
		    			masterItem.合同费用项.Add(i)
		    			j = Hd.Model.费用理论值信息();
                                        j.序号 = 0;
                                        j.条件 = "True";
                                        j.结果 = "0";
                                        j.合同费用项 = i;
                                        dao2.Save(rep, j);
                        rep.CommitTransaction()
                        masterForm.ControlManager.OnCurrentItemChanged();
		except System.Exception, ex:
                        rep.RollbackTransaction();
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute(masterForm);

