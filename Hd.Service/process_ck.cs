using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Feng;
using Feng.Data;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Ck;
using Hd.NetRead;

namespace Hd.Service
{
    public class process_ck
    {
        public static void AutoExecute(ArchiveSeeForm masterForm)
        {
            (masterForm.ArchiveDetailForm as IArchiveDetailFormAuto).DataControlsCreated += new EventHandler(ArchiveDetailForm_DataControlsCreated);
        }

        static void ArchiveDetailForm_DataControlsCreated(object sender, EventArgs e)
        {
            ArchiveDetailForm form = sender as ArchiveDetailForm;
            ((form.DisplayManager.DataControls["货代自编号"] as IWindowControl).Control as MyTextBox).DoubleClick += new EventHandler(Ck_货代自编号_DoubleClick);
        }

        public static void Ck_货代自编号_DoubleClick(object sender, EventArgs e)
        {
            if (sender is MyTextBox)
            {
                MyTextBox box_hdzbh = sender as MyTextBox;
                if (box_hdzbh.SelectedDataValue == null)
                {
                    ArchiveDetailForm form = box_hdzbh.FindForm() as ArchiveDetailForm;

                    if (((form.DisplayManager.DataControls["委托人编号"] as IWindowControl).Control as MyComboBox).SelectedDataValue == null
                        || ((form.DisplayManager.DataControls["委托时间"] as IWindowControl).Control as MyDatePicker).SelectedDataValue == null)
                    {
                        return;
                    }

                    string wtr = ((form.DisplayManager.DataControls["委托人编号"] as IWindowControl).Control as MyComboBox).SelectedDataValue.ToString();
                    DateTime wtsj = DateTime.Parse(((form.DisplayManager.DataControls["委托时间"] as IWindowControl).Control as MyDatePicker).SelectedDataValue.ToString());

                    box_hdzbh.SelectedDataValue = Get货代自编号(wtr, wtsj, GetSimpleParamValue(form.GridName + "_货代自编号"));
                }
            }
        }

        private static string GetSimpleParamValue(string simpleParamName)
        {
            return ServiceProvider.GetService<IDefinition>().TryGetValue(simpleParamName);
        }

        public static string Get货代自编号(string 委托人编号, DateTime 委托时间, string yearMonthFormat)
        {
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<人员>())
            {
                IList<人员> list = (rep as Feng.NH.INHibernateRepository).List<人员>(NHibernate.Criterion.DetachedCriteria.For<人员>()
                    .Add(NHibernate.Criterion.Expression.Eq("编号", 委托人编号)).SetMaxResults(1));

                if (list == null || list.Count == 0)
                {
                    MessageForm.ShowWarning("找不到有效的人员！");
                    return null;
                }

                if (yearMonthFormat == "yyMM")
                {
                    return list[0].字母简称 + PrimaryMaxIdGenerator.GetIdYearMonth(委托时间) +
                        (PrimaryMaxIdGenerator.GetMaxInt("业务备案_普通票", "货代自编号", list[0].字母简称 + PrimaryMaxIdGenerator.GetIdYearMonth(委托时间) + "?-") + 1).ToString();
                }
                else if (yearMonthFormat == "yy")
                {
                    return list[0].字母简称 + PrimaryMaxIdGenerator.GetIdYearMonth(委托时间).Substring(0, 2) +
                        (PrimaryMaxIdGenerator.GetMaxInt("业务备案_普通票", "货代自编号", list[0].字母简称 + PrimaryMaxIdGenerator.GetIdYearMonth(委托时间).Substring(0, 2) + "?-") + 1).ToString();
                }
                else
                {
                    throw new ArgumentNullException("未在简单参数中设置Grid.货代自编号的生成规则，或规则格式错误");
                }
            }
        }

        #region 用于服务

        /// <summary>
        /// 用于服务_填写委托人货代自编号_出口，默认为年月
        /// </summary>
        /// <param name="委托人编号"></param>
        /// <param name="委托时间"></param>
        /// <returns></returns>
        public static string Get货代自编号(string 委托人编号, DateTime 委托时间)
        {
            return Get货代自编号(委托人编号, 委托时间, "yyMM");
        }

        #endregion

        #region 用于AppHelper

        /// <summary>
        /// 补填没有进港地的出口票
        /// </summary>
        public static void Helper_出口票_进港地()
        {
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<出口票>())
            {
                IList<出口票> list = (rep as Feng.NH.INHibernateRepository)
                    .List<出口票>(NHibernate.Criterion.DetachedCriteria.For<出口票>()
                    .Add(NHibernate.Criterion.Expression.IsNull("进港地编号")));

                if (list != null && list.Count > 0)
                {
                    nbediRead nbedi = new nbediRead();
                    string mt;
                    foreach (出口票 ckp in list)
                    {
                        try
                        {
                            mt = nbedi.查询进场码头(ckp.船名航次.Split('/')[0], ckp.船名航次.Split('/')[1], ckp.提单号);
                            if (mt == "BLCTMS") mt = "梅山码头";
                        }
                        catch
                        {
                            // 查询进场码头的网站可能会发生错误，将被忽略继续执行
                            continue;
                        }
                        ckp.进港地编号 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", mt);
                        rep.BeginTransaction();
                        rep.Update(ckp);
                        rep.CommitTransaction();
                    }
                }
            }
        }

        #endregion
    }
}
