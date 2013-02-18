using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Kj;

namespace Hd.Service
{
    public class process_fy_gdzc
    {        
        public static void 固定资产_生产全部费用(GeneratedArchiveOperationForm masterForm)
        {
            if (MessageForm.ShowYesNo("是否生产当前所有费用？", "提示"))
            {
                IList list = masterForm.DisplayManager.Items;
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<固定资产>())
                {
                    rep.BeginTransaction();
                    foreach (object i in list)
                    {
                        非业务费用 fy_dw = new 非业务费用();
                        if (i is 固定资产)
                        {
                            固定资产 item = i as 固定资产;
                            if (item.Submitted)
                            {
                                continue;
                            }                            
                            fy_dw.费用实体 = item;
                            fy_dw.收付标志 = Hd.Model.收付标志.付;
                            fy_dw.相关人编号 = "900031";
                            fy_dw.费用项编号 = "387";
                            fy_dw.金额 = item.月折旧额;
                            fy_dw.费用类别编号 = 186;
                            new 非业务费用Dao().Save(rep, fy_dw);
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "费用实体类型不是要求类型，而是" + i.GetType().ToString());
                        }
                    } 
                    rep.CommitTransaction();
                }
                masterForm.DisplayManager.DisplayCurrent();
            }
        }
    }
}
