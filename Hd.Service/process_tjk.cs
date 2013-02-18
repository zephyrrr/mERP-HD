using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;

namespace Hd.Service
{
    public class process_tjk
    {
        public static void 提交(ArchiveOperationForm masterForm)
        {
            调节款 tjk = masterForm.ControlManager.DisplayManager.CurrentItem as 调节款;
            if (tjk == null)
                return;

            using (IRepository rep = RepositoryFactory.GenerateRepository<调节款>())
            {
                try
                {
                    rep.BeginTransaction();
                    tjk.Submitted = true;
                    rep.Update(tjk);

                    rep.Initialize(tjk, tjk.调节款明细);
                    foreach (调节款明细 i in tjk.调节款明细)
                    {
                        应收应付款 j = i.Clone() as 应收应付款;
                        j.应收应付源 = tjk;
                        rep.Save(j);
                    }

                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                }
            }
        }

        public static void 撤销提交(ArchiveOperationForm masterForm)
        {
            调节款 tjk = masterForm.ControlManager.DisplayManager.CurrentItem as 调节款;
            if (tjk == null)
                return;

            using (IRepository rep = RepositoryFactory.GenerateRepository<调节款>())
            {
                try
                {
                    rep.BeginTransaction();
                    tjk.Submitted = false;
                    rep.Update(tjk);

                    IList<应收应付款> list = rep.Session.CreateCriteria<应收应付款>()
                        .Add(NHibernate.Criterion.Expression.Eq("应收应付源", tjk))
                        .List<应收应付款>();
                    foreach (应收应付款 i in list)
                    {
                        rep.Delete(i);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception)
                {
                    rep.RollbackTransaction();
                }
            }
        }
    }
}
