using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Feng;
using Feng.Utils;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Nmcg;
using Hd.Model.Jk;
using Hd.Model.Jk2;

namespace Hd.Service
{
    public class process_fy_generate
    {
        public static void 批量生成费用收款(IRepository rep, int 费用实体类型, 普通票 票, IEnumerable 箱s, string 费用项编号, 收付标志? 收付标志, IList<业务费用理论值> llzs)
        {
            if (!收付标志.HasValue || 收付标志.Value == Hd.Model.收付标志.收)
            {
                // 生成委托人合同费用
                委托人合同 wtrht = HdDataBuffer.Instance.Get委托人合同(rep, 费用实体类型, 票.委托人编号);
                if (wtrht != null)
                {
                    foreach (委托人合同费用项 htfyx in wtrht.合同费用项)
                    {
                        // 如果指定费用项，则只生成此费用项下的费用
                        if (!string.IsNullOrEmpty(费用项编号)
                            && htfyx.费用项编号 != 费用项编号)
                        {
                            continue;
                        }

                        批量生成费用(rep, 票, 箱s, Hd.Model.收付标志.收, htfyx, llzs, !string.IsNullOrEmpty(费用项编号));
                    }
                }
            }
        }

        public static void 批量生成费用付款(IRepository rep, int 费用实体类型, 普通票 票, IEnumerable 箱s, string 费用项编号, 收付标志? 收付标志, IList<业务费用理论值> llzs, bool service)
        {
            if (!收付标志.HasValue || 收付标志.Value == Hd.Model.收付标志.付)
            {
                // 生成付款费用
                付款合同 fkht = HdDataBuffer.Instance.Get付款合同(rep, 费用实体类型);
                if (fkht != null)
                {
                    foreach (付款合同费用项 htfyx in fkht.合同费用项)
                    {
                        // 如果指定费用项，则只生成此费用项下的费用
                        if (!string.IsNullOrEmpty(费用项编号)
                            && htfyx.费用项编号 != 费用项编号)
                        {
                            continue;
                        }

                        // 不是服务运行时（即界面上点按钮），付款合同费用项.是否生成实际费用 = false 不生产费用
                        if (!service && !htfyx.是否生成实际费用)
                        {
                            continue;
                        }

                        批量生成费用(rep, 票, 箱s, Hd.Model.收付标志.付, htfyx, llzs, !string.IsNullOrEmpty(费用项编号));
                    }
                }
                if (费用实体类型 == 11)
                {
                    bool? cybz = ConvertHelper.ToBoolean(EntityScript.GetPropertyValue(票, "承运标志"));
                    if (cybz.HasValue && cybz.Value)
                    {
                        // 受托人合同
                        string str = ConvertHelper.ToString(EntityScript.GetPropertyValue(票, "承运人编号"));
                        if (!string.IsNullOrEmpty(str))
                        {
                            // 生成付款费用
                            受托人合同 strht = HdDataBuffer.Instance.Get受托人合同(rep, 费用实体类型, str);
                            if (strht != null)
                            {
                                foreach (受托人合同费用项 htfyx in strht.合同费用项)
                                {
                                    // 如果指定费用项，则只生成此费用项下的费用
                                    if (!string.IsNullOrEmpty(费用项编号)
                                        && htfyx.费用项编号 != 费用项编号)
                                    {
                                        continue;
                                    }

                                    批量生成费用(rep, 票, 箱s, Hd.Model.收付标志.付, htfyx, llzs, !string.IsNullOrEmpty(费用项编号));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void 批量生成费用(IRepository rep, 普通票 票, IEnumerable 箱s, 收付标志 收付标志, 合同费用项 htfyx, IList<业务费用理论值> llzs, bool mustGenerateFy)
        {
            费用项 fyx = EntityBufferCollection.Instance.Get<费用项>(htfyx.费用项编号);
            if (fyx.箱)
            {
                foreach (普通箱 箱 in 箱s)
                {
                    GenerateFy(rep, 票, 箱, 收付标志, htfyx, llzs, mustGenerateFy);
                }
            }
            else if (fyx.票)
            {
                GenerateFy(rep, 票, null, 收付标志, htfyx, llzs, mustGenerateFy);
            }
        }

        private static void GenerateFy(IRepository rep, 普通票 票, 普通箱 箱, 收付标志 收付标志, 合同费用项 htfyx, IList<业务费用理论值> llzs, bool mustGenerateFy)
        {
            string 相关人 = Get相关人(rep, 票, 箱, htfyx);
            decimal? 金额 = Get理论值(rep, 票, 箱, htfyx);
            if (金额.HasValue && 金额.Value == decimal.MinValue)
            {
                return;
            }

            bool llrHaveGenerated = false;
            foreach (业务费用理论值 i in llzs)
            {
                bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志;                  ;
                if (htfyx.是否判断相关人)
                {
                    b &= i.相关人编号 == 相关人;
                }
                if (b && 箱 != null)
                {
                    b &= i.箱Id == 箱.ID;
                }
                if (b)
                {
                    llrHaveGenerated = true;
                    break;
                }
            }

            业务费用理论值 ywfylrz = null;
            if (!llrHaveGenerated)
            {
                if (金额.HasValue)
                {
                    ywfylrz = new 业务费用理论值();
                    ywfylrz.费用实体 = 票; // new 普通票 { Id = 票.ID, Version = 票.Version, 费用实体类型编号 = 票.费用实体类型编号 };
                    ywfylrz.费用项编号 = htfyx.费用项编号;
                    ywfylrz.金额 = 金额.Value;
                    ywfylrz.票 = 票;
                    ywfylrz.收付标志 = 收付标志;
                    ywfylrz.相关人编号 = Get相关人(rep, 票, 箱, htfyx);
                    if (箱 != null)
                    {
                        ywfylrz.箱 = 箱;
                        ywfylrz.箱Id = 箱.ID;
                    }

                    (new HdBaseDao<业务费用理论值>()).Save(rep, ywfylrz);
                    llzs.Add(ywfylrz);
                }
            }

            if (htfyx.是否生成实际费用)
            {
                bool generateFy = false;
                // 在外层，判断理论值是否生成过
                if (!mustGenerateFy)
                {
                    if (htfyx.是否空值全部生成)
                    {
                        // 金额为Null的时候判断时候生成过，没生成过也要生成
                        if (!金额.HasValue)
                        {
                            bool fyHaveGenerated = false;
                            foreach (业务费用 i in 票.费用)
                            {
                                bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志;
                                if (htfyx.是否判断相关人)
                                {
                                    b &= i.相关人编号 == 相关人;
                                }
                                if (b && 箱 != null)
                                {
                                    b &= i.箱Id == 箱.ID;
                                }
                                if (b)
                                {
                                    fyHaveGenerated = true;
                                    break;
                                }
                            }
                            generateFy = !fyHaveGenerated;
                        }
                    }

                    if (!generateFy)
                    {
                        // 只有理论值未生成过，且有理论值的情况下，准备生成费用
                        if (!llrHaveGenerated && ywfylrz != null)
                        {
                            // 如果理论值未生成过，要检查是否要生成费用
                            bool fyHaveGenerated = false;
                            foreach (业务费用 i in 票.费用)
                            {
                                bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志;
                                if (htfyx.是否判断相关人)
                                {
                                    b &= i.相关人编号 == 相关人;
                                }
                                if (b && 箱 != null)
                                {
                                    b &= i.箱Id == 箱.ID;
                                }
                                if (b)
                                {
                                    fyHaveGenerated = true;
                                    break;
                                }
                            }
                            generateFy = !fyHaveGenerated;
                        }
                        else
                        {
                            generateFy = false;
                        }
                    }
                }
                else
                {
                    generateFy = true;
                }

                if (generateFy)
                {
                    bool fylbSubmitted = false;
                    费用项 fyx = EntityBufferCollection.Instance.Get<费用项>(htfyx.费用项编号);
                    //int fylbbh = fyx.收入类别.Value;

                    //费用类别 fylb = EntityBufferCollection.Instance["费用类别"].Get(fylbbh) as 费用类别;
                    //System.Diagnostics.Debug.Assert(fylb.大类 == "业务额外" || fylb.大类 == "业务常规" || fylb.大类 == "业务其他");
                    IList<费用信息> list = (rep as Feng.NH.INHibernateRepository).List<费用信息>(NHibernate.Criterion.DetachedCriteria.For<费用信息>()
                        .Add(NHibernate.Criterion.Expression.Eq("费用项编号", htfyx.费用项编号))
                        .Add(NHibernate.Criterion.Expression.Eq("票.ID", 票.ID)));

                    System.Diagnostics.Debug.Assert(list.Count <= 1);
                    if (list.Count == 1)
                    {
                        if (收付标志 == 收付标志.收)
                        {
                            fylbSubmitted = list[0].Submitted;
                        }
                        else
                        {
                            fylbSubmitted = list[0].完全标志付;
                        }
                    }

                    // 完全标志还未打
                    if (!fylbSubmitted)
                    {
                        // 不生成理论值为0的
                        if (!金额.HasValue || (金额.HasValue && 金额.Value != 0))
                        {
                            业务费用 item = new 业务费用();
                            item.费用实体 = 票; // new 普通票 { Id = 票.ID, Version = 票.Version, 费用实体类型编号 = 票.费用实体类型编号 };
                            item.费用项编号 = htfyx.费用项编号;
                            item.金额 = 金额;
                            item.票 = 票;
                            item.收付标志 = 收付标志;
                            item.相关人编号 = 相关人;
                            if (箱 != null)
                            {
                                item.箱 = 箱;
                                item.箱Id = 箱.ID;
                            }

                            (new 业务费用Dao()).Save(rep, item);

                            票.费用.Add(item);
                        }
                    }
                }
            }
        }

        private static decimal? Get理论值(IRepository rep, 普通票 票, 普通箱 箱, 合同费用项 htfyx)
        {
            if (htfyx is 受托人合同费用项)
            {
                return Get受托人理论值(rep, 票, 箱, htfyx as 受托人合同费用项);
            }
            else if (htfyx is 付款合同费用项)
            {
                return Get付款理论值(rep, 票, 箱, htfyx as 付款合同费用项);
            }
            else if (htfyx is 委托人合同费用项)
            {
                return Get委托人理论值(rep, 票, 箱, htfyx as 委托人合同费用项);
            }
            else
            {
                throw new ArgumentException("Invalid htfyx Type!");
            }
        }


        private static decimal? Get受托人理论值(IRepository rep, 普通票 票, 普通箱 箱, 受托人合同费用项 htfyx)
        {
            decimal? d = null;
            switch (htfyx.付款合同费用项类型)
            {
                case 付款合同费用项类型.理论值计算:
                    d = (箱 != null) ? Get理论值金额(rep, htfyx, 箱) : Get理论值金额(rep, htfyx, 票);
                    break;
                case 付款合同费用项类型.报销:
                    decimal? sum = null;
                    foreach (业务费用 i in 票.费用)
                    {
                        bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志.付
                            && i.金额.HasValue;
                        if (b && 箱 != null)
                        {
                            b &= i.箱Id == 箱.ID;
                        }
                        if (b && i.凭证费用明细 != null)
                        {
                            if (!sum.HasValue)
                            {
                                sum = 0;
                            }
                            sum += i.金额.Value;
                        }
                    }
                    if (sum.HasValue)
                    {
                        d = sum;
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid " + htfyx.付款合同费用项类型);
            }
            return d;
        }

        private static decimal? Get付款理论值(IRepository rep, 普通票 票, 普通箱 箱, 付款合同费用项 htfyx)
        {
            decimal? d = null;
            switch (htfyx.付款合同费用项类型)
            {
                case 付款合同费用项类型.理论值计算:
                    d = (箱 != null) ? Get理论值金额(rep, htfyx, 箱) : Get理论值金额(rep, htfyx, 票);
                    break;
                case 付款合同费用项类型.报销:
                    decimal? sum = null;
                    foreach (业务费用 i in 票.费用)
                    {
                        bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志.付
                            && i.金额.HasValue;
                        if (b && 箱 != null)
                        {
                            b &= i.箱Id == 箱.ID;
                        }
                        if (b && i.凭证费用明细 != null)
                        {
                            if (!sum.HasValue)
                            {
                                sum = 0;
                            }
                            sum += i.金额.Value;
                        }
                    }
                    if (sum.HasValue)
                    {
                        d = sum;
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid " + htfyx.付款合同费用项类型);
            }
            return d;
        }

        private static decimal? Get委托人理论值(IRepository rep, 普通票 票, 普通箱 箱, 委托人合同费用项 htfyx)
        {
            decimal? d = null;
            switch (htfyx.委托人合同费用项类型)
            {
                case 委托人合同费用项类型.理论值计算:
                    d = (箱 != null) ? Get理论值金额(rep, htfyx, 箱) : Get理论值金额(rep, htfyx, 票);
                    break;
                case 委托人合同费用项类型.代垫:
                    decimal? sum = null;
                    foreach (业务费用 i in 票.费用)
                    {
                        bool b = i.费用项编号 == htfyx.费用项编号 && i.收付标志 == 收付标志.付
                            && i.金额.HasValue;
                        if (b && 箱 != null)
                        {
                            b &= i.箱Id == 箱.ID;
                        }
                        if (b)
                        {
                            if (!sum.HasValue)
                            {
                                sum = 0;
                            }
                            sum += i.金额.Value;
                        }
                    }
                    if (sum.HasValue)
                    {
                        d = sum;
                    }
                    else
                    {
                        d = decimal.MinValue;
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid " + htfyx.委托人合同费用项类型);
            }
            return d;
        }

        private const string s_feeThoeryGenerate = "# -*- coding: UTF-8 -*- " + "\r\n" +
            "import clr" + "\r\n" +
            "import math" + "\r\n" +
            "clr.AddReferenceByPartialName('Hd.Model.Base')" + "\r\n" +
            "import Hd" + "\r\n" +
            "if (" + "%IFEXPRESSION%" + "): result = True;" + "\r\n" +
            "else: result = False;";

        private const string s_feeThoeryGenerate2 = "# -*- coding: UTF-8 -*- " + "\r\n" +
            "import clr" + "\r\n" +
            "import math" + "\r\n" +
            "clr.AddReferenceByPartialName('Hd.Model.Base')" + "\r\n" +
            "import Hd" + "\r\n" +
            "%IFEXPRESSION%";

        private const string s_feeThoeryGenerateResult = "# -*- coding: UTF-8 -*- " + "\r\n" +
            "import clr" + "\r\n" +
            "import math" + "\r\n" +
            "clr.AddReferenceByPartialName('Hd.Model.Base')" + "\r\n" +
            "import Hd" + "\r\n" +
            "result = (" + "%IFEXPRESSION%" + ")";

        private const string s_feeThoeryGenerateResult2 = "# -*- coding: UTF-8 -*- " + "\r\n" +
            "import clr" + "\r\n" +
            "import math" + "\r\n" +
            "clr.AddReferenceByPartialName('Hd.Model.Base')" + "\r\n" +
            "import Hd" + "\r\n" +
            "%IFEXPRESSION%";

        // 返回MinValue：没符合的条件，不生成理论值，不生成费用（内部外部都不生成）
        // 返回0：符合条件，生成理论值， 但不生成费用（内部外部都不生成）
        // 返回NULL：符合条件，不生成理论值，但生成空费用记录。在不在生成全部的时候生成看配置，生成单费用项的时候生成
        // 返回具体值：生成记录
        // 根据相关人不同得到理论值
        private static decimal? Get理论值金额(IRepository rep, 合同费用项 htfyx, object entity)
        {
            rep.Initialize(htfyx.费用理论值, htfyx);
            foreach (费用理论值信息 i in htfyx.费用理论值)
            {
                string exp;
                if (i.条件.Contains(System.Environment.NewLine))
                {
                    exp = s_feeThoeryGenerate2.Replace("%IFEXPRESSION%", i.条件);
                }
                else
                {
                    exp = s_feeThoeryGenerate.Replace("%IFEXPRESSION%", i.条件);
                }
                object ret = Script.ExecuteStatement(exp, new Dictionary<string, object> { { "entity", entity } });
                if (ConvertHelper.ToBoolean(ret).Value)
                {
                    if (string.IsNullOrEmpty(i.结果))
                    {
                        return null;
                    }
                    else
                    {
                        if (i.结果.Contains(System.Environment.NewLine))
                        {
                            exp = s_feeThoeryGenerateResult2.Replace("%IFEXPRESSION%", i.结果);
                        }
                        else
                        {
                            exp = s_feeThoeryGenerateResult.Replace("%IFEXPRESSION%", i.结果);
                        }
                        ret = Script.ExecuteStatement(exp, new Dictionary<string, object> { { "entity", entity }, { "rep", rep } });
                        decimal? d = Feng.Utils.ConvertHelper.ToDecimal(ret);
                        if (d.HasValue)
                            return d.Value;
                        else
                            return null;
                    }
                }
            }
            return decimal.MinValue;
        }

        public static string Get相关人(IRepository rep, 普通票 票, 普通箱 箱, 合同费用项 htfyx)
        {
            object entity;
            if (箱 != null)
            {
                entity = 箱;
            }
            else
            {
                entity = 票;
            }
            string ret = null;
            if (htfyx is 委托人合同费用项)
            {
                return 票.委托人编号;
            }
            else if (htfyx is 付款合同费用项)
            {
                string mrxgr = (htfyx as 付款合同费用项).默认相关人;
                object r = EntityScript.CalculateExpression(mrxgr, entity);
                ret = string.IsNullOrEmpty(mrxgr) || r == null ? null : r.ToString().Replace("\"", "");
                if (string.IsNullOrEmpty(ret))
                {
                    ret = null;
                }
            }
            else if (htfyx is 受托人合同费用项)
            {
                object cyr = EntityScript.GetPropertyValue(票, "承运人编号");
                if (cyr != null)
                {
                    return cyr.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new ArgumentException("Invalid htfyx Type!");
            }


            return ret;
        }

        public static void 批量生成空费用项(ArchiveOperationForm masterForm)
        {
            普通票 piao = (masterForm.ParentForm as GeneratedArchiveOperationForm).DisplayManager.CurrentItem as 普通票;

            if (piao == null)
            {
                return;
            }

            if (masterForm.DisplayManager.Items.Count > 0)
            {
                if (!MessageForm.ShowYesNo("是否确定生成空费用项", "提示"))
                {
                    return;
                }
            }
            
            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<普通票>())
            {
                IList<费用项> fyxList = (rep as Feng.NH.INHibernateRepository).Session.CreateSQLQuery(
                    "select * from 参数备案_费用项 where 现有费用实体类型 like '%" + piao.费用实体类型编号 + ",%' and 票 = 1")
                    .AddEntity(typeof(费用项)).List<费用项>();

                if (fyxList == null || fyxList.Count == 0)
                {
                    return;
                }

                foreach (费用项 fyx in fyxList)
                {
                    业务费用 fy = new 业务费用
                    {
                        费用项编号 = fyx.编号,
                        收付标志 = 收付标志.付,
                        票 = piao
                    };

                    masterForm.ControlManager.AddNew();
                    masterForm.ControlManager.DisplayManager.Items[masterForm.DisplayManager.Position] = fy;
                    masterForm.ControlManager.EndEdit();
                }
            }
        }
    }
}
