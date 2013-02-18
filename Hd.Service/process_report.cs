using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Hd.Model;
using Hd.Model.Jk;

namespace Hd.Service
{
    public class process_report
    {
        public static void 打印货代报关部委托运输联系单(ArchiveOperationForm masterForm)
        {
            进口票过程转关标志 pz = masterForm.DisplayManager.CurrentItem as 进口票过程转关标志;
            进口票 entity = pz.票;

            if (entity == null)
            {
                MessageForm.ShowError("请选择要打印的票！");
                return;
            }

            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<进口票>())
            {
                rep.Initialize(entity.箱, entity);
            }
            MyReportForm form = new MyReportForm("报表_货代报关部委托运输联系单_进口");
            form.FillDataSet(0, new List<进口票> { entity });
            form.FillDataSet(1, entity.箱);

            form.Show();
        }
    }
}
