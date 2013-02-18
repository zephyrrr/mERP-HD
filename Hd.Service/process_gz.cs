using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Kj;

namespace Hd.Service
{
    public class process_gz
    {
        public static void 生成费用(ArchiveOperationForm masterForm)
        {
            foreach (Xceed.Grid.DataRow row in masterForm.ActiveGrid.SelectedRows)
            {
                工资单 gzd = row.Tag as 工资单;

                
            }
  
            //frm_cw_fkpz_kj_detail form = ArchiveFormFactory.GenerateArchiveDetailForm(ADInfoBll.Instance.GetFormInfo(201)) as frm_cw_fkpz_kj_detail;
            //form.ControlManager.AddNew();
            //凭证 pz = form.ControlManager.DisplayManager.CurrentItem as 凭证;
            //pz.日期 = System.DateTime.Today;
            //pz.相关人编号 = gzd.员工编号;
            //pz.金额.数额 = gzd.工资小计;
            //pz.金额.币制编号 = "CNY";

            //form.UpdateContent();
            //form.AddFees(new List<费用> { fy });
            //form.ShowDialog();
        }

        public static void 作废凭证(ArchiveOperationForm masterForm)
        {
            // Todo: 作废是否更新费用
            工资单 gzd = masterForm.DisplayManager.CurrentItem as 工资单;
            费用 fy = RepositoryHelper.GetByProperty<费用>("费用实体.Id", gzd.Id);
            fy.凭证费用明细 = null;
            (new 费用Bll()).Update(fy);

            凭证 pz = RepositoryHelper.GetByProperty<凭证>("凭证号", gzd.凭证号);
            pz.是否作废 = true;

            process_pz.作废凭证(pz);
        }
    }
}
