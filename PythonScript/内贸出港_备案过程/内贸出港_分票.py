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
clr.AddReferenceByPartialName("Hd.Model.Nmcg")

import sys;
import System;
import System.Windows.Forms;
import NHibernate;
import Feng;
import Feng.Windows.Forms;
import Hd.Model;
import Hd.Model.Nmcg;

def execute(masterForm):
        if (System.Windows.Forms.MessageBox.Show("是否分票？", "确认", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No):
                return;

        cmMaster = masterForm.ControlManager;
        with Feng.ServiceProvider.GetService[Feng.IRepositoryFactory]().GenerateRepository[Hd.Model.Nmcg.内贸出港票]() as rep:
                try:
                        rep.BeginTransaction();
                        piao = cmMaster.DisplayManager.CurrentItem;
                        piao2 = Hd.Model.Nmcg.内贸出港票();
                        piao2.箱 = System.Collections.Generic.List[Hd.Model.Nmcg.内贸出港箱]();
                        piao2.委托时间 = piao.委托时间;
                        piao2.预配提单号 = piao.预配提单号;
                        piao2.预配船名航次 = piao.预配船名航次;
                        piao2.船名航次 = piao.船名航次;
                        piao2.船公司编号 = piao.船公司编号;
                        piao2.倒箱仓库编号 = piao.倒箱仓库编号;
                        piao2.目的港编号 = piao.目的港编号;
                        piao2.免箱天数 = piao.免箱天数;
                        piao2.预计开航日期 = piao.预计开航日期;
                        piao2.开航日期 = piao.开航日期;
                        piao2.进港地编号 = piao.进港地编号;
                        piao2.预计到港时间 = piao.预计到港时间;
                        piao2.到港时间 = piao.到港时间;
                        piao2.承运人编号 = piao.承运人编号;
                        piao2.内部备注 = piao.内部备注;
                        piao2.对上备注 = piao.对上备注;
                        piao2.对下备注 = piao.对下备注;
                        piao2.条款 = piao.条款;
                        cnt = 0;
                        for i in masterForm.ArchiveDetailForm.DetailGrids[0].SelectedRows:
                                i.Tag.票 = piao2;
                                rep.Update(i.Tag);
                                cnt += 1;
                                piao.箱.Remove(i.Tag);
                                piao2.箱.Add(i.Tag);
                        piao2.箱量 = cnt;
                        (Hd.Model.HdBaseDao[Hd.Model.Nmcg.内贸出港票]()).Save(rep, piao2);
                        rep.CommitTransaction();
                        cmMaster.OnCurrentItemChanged();
                        Feng.MessageForm.ShowInfo("已生成新票，请刷新后查看！");
                except System.Exception, ex:
                        rep.RollbackTransaction();
                        Feng.ServiceProvider.GetService[Feng.IExceptionProcess]().ProcessWithNotify(ex);

if __name__ == "<module>" or __name__ == "__builtin__":
        execute(masterForm);

