using System;
using System.Collections.Generic;
using System.Text;
using Feng;
using Feng.Windows.Forms;
using Hd.Model;

namespace Hd.Service.理论值编辑
{
    public class process_llzbj
    {
        public static void EditLlz(ArchiveSeeForm masterForm)
        {
            Xceed.Grid.Row row = (masterForm.ArchiveDetailForm as IArchiveDetailFormWithDetailGrids).DetailGrids[0].CurrentRow;
            if (row == null)
            {
                throw new InvalidUserOperationException("请选择要编辑理论值的合同费用项！");
            }
            合同费用项 htfyx = (row as Xceed.Grid.DataRow).Tag as 合同费用项;
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用理论值信息>())
            {
                rep.Initialize(htfyx.费用理论值, htfyx);
            }
            
            IList<string> llzs = new List<string>();
            if (htfyx.费用理论值 != null)
            {
                foreach (费用理论值信息 i in htfyx.费用理论值)
                {
                    llzs.Add(i.条件);
                    llzs.Add(i.结果);
                }
            }

            FrmEditor form = new FrmEditor(
                new Dictionary<string, string> { {"委托人编号", "人员单位_委托人"}, {"船公司编号", "人员单位_船公司"},
                {"箱型编号", "备案_箱型_全部"}, {"卸箱地编号", "人员单位_港区堆场"}, {"费用项编号", "费用项_全部"}},
                llzs);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                IList<string> ret = form.GetResult();
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<费用理论值信息>())
                {
                    try
                    {
                        rep.BeginTransaction();
                        foreach (费用理论值信息 i in htfyx.费用理论值)
                        {
                            rep.Delete(i);
                        }
                        htfyx.费用理论值.Clear();
                        for (int i = 0; i < ret.Count; i += 2)
                        {
                            费用理论值信息 item = new 费用理论值信息();
                            item.合同费用项 = htfyx;
                            item.结果 = ret[i + 1];
                            item.条件 = ret[i];
                            item.序号 = i / 2;

                            (new HdBaseDao<费用理论值信息>()).Save(rep, item);
                            htfyx.费用理论值.Add(item);
                        }

                        rep.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        rep.RollbackTransaction();
                        ServiceProvider.GetService<IExceptionProcess>().ProcessWithNotify(ex);
                    }
                }
            }

        }
    }
}
