using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Feng;
using Feng.Windows.Utils;
using Feng.Net;
using Feng.Windows;
using Feng.Windows.Forms;
using Feng.Grid;
using Hd.Model;
using Hd.Model.Jk;
using Hd.Model.Ck;
using Hd.Model.Px;
using Hd.Model.Nmcg;
using Hd.NetRead;

namespace Hd.Service
{
    public class process_readticketfromnet
    {
        private static nbeportRead m_nbeportGrab = new nbeportRead();
        private static nbediRead m_nbediRead = new nbediRead();
        private static npediRead m_npediGrab = new npediRead();
        private static string m_nbedi_H2000EPORT_ID = ServiceProvider.GetService<IDefinition>().TryGetValue("Nbedi_H2000EPORT_ID");

        /// <summary>
        /// 分割多个账号密码
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> Get_nbediRead_ID()
        {
            try
            {
                Dictionary<string, string> uid_pwd = new Dictionary<string, string>();
                string[] ids = m_nbedi_H2000EPORT_ID.Split(';');
                foreach (string id in ids)
                {
                    uid_pwd.Add(id.Split(',')[0].Trim(), id.Split(',')[1].Trim());
                }
                return uid_pwd;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message, ex);
            }
        }

        public static void ReadJk(ArchiveOperationForm masterForm)
        {
            IControlManager masterCm = masterForm.ControlManager;

            if (masterCm.DisplayManager.DataControls["提单号"].SelectedDataValue == null)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入提单号！");
                return;
            }

            string tdh = masterCm.DisplayManager.DataControls["提单号"].SelectedDataValue.ToString().Trim();

            #region 从"npedi进口卸箱查询"获取箱信息
            IList<进口卸箱查询结果> boxList = m_npediGrab.进口卸箱查询(tdh);
            if (boxList.Count <= 0)
            {
                return;
            }

            AskToReplace(masterCm, "船名航次", boxList[0].船名航次);
            AskToReplace(masterCm, "卸箱地编号", NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", boxList[0].码头));
            AskToReplace(masterCm, "到港时间", boxList[0].卸船时间);
            AskToReplace(masterCm, "提单号", boxList[0].提单号);
            AskToReplace(masterCm, "箱量", boxList.Count);

            IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

            int? xxbh = null;
            string pm = null;
            foreach (进口卸箱查询结果 data in boxList)
            {
                bool have = false;
                foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
                {
                    if (row.Cells["箱型编号"].Value != null)
                    {
                        xxbh = (int)row.Cells["箱型编号"].Value;
                    }

                    if (row.Cells["品名"].Value != null)
                    {
                        pm = row.Cells["品名"].Value.ToString();
                    }

                    if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
                    {
                        have = true;
                        break;
                    }
                }
                if (!have)
                {
                    if (xxbh == null)
                    {
                        xxbh = Convert.ToInt32(NameValueMappingCollection.Instance.FindIdFromName("备案_箱型_全部", boxList[0].尺寸类型));
                    }
                    进口箱 newItem = new 进口箱 { 箱号 = data.集装箱号, 箱型编号 = xxbh, 重量 = Convert.ToInt32(data.箱毛重), 品名 = pm };
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
                    detailCm.EndEdit();
                }
            }
            #endregion

            #region 从"nbeport集装箱进场信息"获取箱信息
            //m_nbeportGrab.SetLoginInfo(SystemDirectory.DefaultUserProfile.GetValue("Hd.Options", "NetReadUserName", ""),
            //    SystemDirectory.DefaultUserProfile.GetValue("Hd.Options", "NetReadPassword", ""));
            //IList<集装箱数据> boxList = m_nbeportGrab.查询集装箱数据(ImportExportType.进口集装箱, tdh);

            //if (boxList.Count <= 0)
            //{
            //    return;
            //}

            //AskToReplace(masterCm, "船名航次", boxList[0].船舶英文名称 + "/" + boxList[0].航次);
            //AskToReplace(masterCm, "卸箱地编号", NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", boxList[0].堆场区));
            //AskToReplace(masterCm, "到港时间", boxList[0].进场时间);
            //AskToReplace(masterCm, "提单号", boxList[0].提单号);
            //AskToReplace(masterCm, "箱量", boxList.Count);

            //IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

            //int? xxbh = null;
            //string pm = null;
            //foreach (集装箱数据 data in boxList)
            //{
            //    bool have = false;
            //    foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
            //    {
            //        if (row.Cells["箱型编号"].Value != null)
            //        {
            //            xxbh = (int)row.Cells["箱型编号"].Value;
            //        }

            //        if (row.Cells["品名"].Value != null)
            //        {
            //            pm = row.Cells["品名"].Value.ToString();
            //        }

            //        if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
            //        {
            //            have = true;
            //            break;
            //        }
            //    }
            //    if (!have)
            //    {
            //        //进口箱 newItem = new 进口箱 { 箱号 = data.集装箱号, 箱型 = ConvertHelper.ChangeType(data.箱型, typeof(箱型)) as 箱型 };
            //        进口箱 newItem = new 进口箱 { 箱号 = data.集装箱号, 箱型编号 = xxbh, 品名 = pm };
            //        detailCm.AddNew();
            //        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
            //        detailCm.EndEdit();
            //    }
            //}
            #endregion
        }

        private static void AskToReplace(IControlManager cm, string propertyName, object destValue)
        {
            if (propertyName == null || destValue == null)
            {
                return;
            }

            bool replace = true;

            if (//cm.DisplayManager.DataControls[propertyName].SelectedDataValue == null && destValue != null || 
                cm.DisplayManager.DataControls[propertyName].SelectedDataValue != null && destValue == null
                || (cm.DisplayManager.DataControls[propertyName].SelectedDataValue != null && destValue != null &&
                    cm.DisplayManager.DataControls[propertyName].SelectedDataValue.ToString() != destValue.ToString()) && propertyName != "报关单快照")
            {
                if (!MessageForm.ShowYesNo(propertyName + "是否要替换成" + (destValue == null ? "空" : destValue.ToString()), "网上读取"))
                {
                    replace = false;
                }
            }
            if (replace)
            {
                cm.DisplayManager.DataControls[propertyName].SelectedDataValue = destValue;
            }
        }

        public static void ReadNmcg(ArchiveOperationForm masterForm)
        {
            IControlManager masterCm = masterForm.ControlManager;

            if (masterCm.DisplayManager.DataControls["预配提单号"].SelectedDataValue == null)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入预配提单号！");
                return;
            }

            string tdh = masterCm.DisplayManager.DataControls["预配提单号"].SelectedDataValue.ToString().Trim();

            IList<集装箱进门查询结果> boxList = m_npediGrab.集装箱进门查询(tdh);
            if (boxList.Count <= 0)
            {
                //boxList = m_npediGrab.集装箱进门查询(tdh);
                return;
            }

            AskToReplace(masterCm, "预配船名航次", boxList[0].船名航次);
            //AskToReplace(masterCm, "进港地编号", NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", boxList[0].码头));
            //AskToReplace(masterCm, "到港时间", boxList[0].进场时间);
            //AskToReplace(masterCm, "提单号", boxList[0].提单号);
            AskToReplace(masterCm, "箱量", boxList.Count);

            IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

            foreach (集装箱进门查询结果 data in boxList)
            {
                bool have = false;
                foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
                {
                    if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
                    {
                        have = true;
                        break;
                    }
                }
                if (!have)
                {
                    内贸出港箱 newItem = new 内贸出港箱 { 箱号 = data.集装箱号, 封志号 = data.铅封号 };
                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
                    detailCm.EndEdit();
                }
            }
        }

        public static void ReadCk(ArchiveOperationForm masterForm)
        {
            IControlManager masterCm = masterForm.ControlManager;

            if (masterCm.DisplayManager.DataControls["提单号"].SelectedDataValue == null)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入提单号！");
                return;
            }

            string tdh = masterCm.DisplayManager.DataControls["提单号"].SelectedDataValue.ToString().Trim();
            m_nbeportGrab.SetLoginInfo(SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadUserName", ""),
                SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadPassword", ""));
            IList<集装箱数据> boxList = m_nbeportGrab.查询集装箱数据(ImportExportType.出口集装箱, tdh);

            if (boxList.Count <= 0)
            {
                return;
            }

            AskToReplace(masterCm, "船名航次", boxList[0].船舶英文名称 + "/" + boxList[0].航次);
            AskToReplace(masterCm, "进港地编号", NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", boxList[0].堆场区));
            AskToReplace(masterCm, "离港时间", boxList[0].进场时间);
            AskToReplace(masterCm, "提单号", boxList[0].提单号);
            AskToReplace(masterCm, "箱量", boxList.Count);

            IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

            foreach (集装箱数据 data in boxList)
            {
                bool have = false;
                foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
                {
                    if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
                    {
                        have = true;
                        break;
                    }
                }
                if (!have)
                {
                    出口箱 newItem = new 出口箱
                    {
                        箱号 = data.集装箱号,
                        箱型 = Feng.Utils.ConvertHelper.ChangeType(data.箱型, typeof(箱型)) as 箱型,
                        装货地编号 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", data.堆场区),
                        进港时间 = data.Real进场时间,
                        提箱时间 = data.Real提箱时间
                    };

                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
                    detailCm.EndEdit();
                }
            }
        }

        public static void ReadCk_报关单号(ArchiveOperationForm masterForm)
        {
            IControlManager masterCm = masterForm.ControlManager;

            if (masterCm.DisplayManager.DataControls["报关单号"].SelectedDataValue == null)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入报关单号！");
                return;
            }

            string bgdh = masterCm.DisplayManager.DataControls["报关单号"].SelectedDataValue.ToString().Trim();

            报关单数据 bgdInfo = null;
            foreach (KeyValuePair<string, string> id in Get_nbediRead_ID())
            {
                m_nbediRead = new nbediRead();
                m_nbediRead.SetLoginInfo(id.Key, id.Value);
                bgdInfo = m_nbediRead.长短号查询报关单数据(bgdh);
                if (bgdInfo != null && !string.IsNullOrEmpty(bgdInfo.报关单号))
                {
                    break;
                }
            }

            if (bgdInfo == null)
            {
                ReadCk(masterForm);
            }
            else
            {
                AskToReplace(masterCm, "报关单号", bgdInfo.报关单长号);
                AskToReplace(masterCm, "抬头", bgdInfo.经营单位);
                AskToReplace(masterCm, "提单号", bgdInfo.提运单号);
                AskToReplace(masterCm, "核销单号", bgdInfo.批准文号);
                AskToReplace(masterCm, "通关单号", bgdInfo.通关单号);
                AskToReplace(masterCm, "箱号", bgdInfo.箱号);
                AskToReplace(masterCm, "箱量", bgdInfo.箱量);
                AskToReplace(masterCm, "委托时间", bgdInfo.申报日期);
                AskToReplace(masterCm, "船名航次", bgdInfo.船名航次);
                //AskToReplace(masterCm, "报关单快照", bgdInfo.网页快照);
                AskToReplace(masterCm, "报关员编号", bgdInfo.报关员);
                AskToReplace(masterCm, "报关公司", bgdInfo.报关公司);

                if (string.IsNullOrEmpty(bgdInfo.提运单号))
                {
                    return;
                }

                IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

                IList<集装箱数据> boxList = m_nbediRead.查询集装箱数据(bgdInfo.提运单号.Trim(), bgdInfo.船名航次.Split('/')[0], bgdInfo.船名航次.Split('/')[1]);
                //IList<集装箱数据> boxList = 查询出口集装箱数据By提单号航次(bgdInfo.提运单号.Trim(), bgdInfo.船名航次.Split('/')[1]);
                string 进港地 = null;
                //bool is查验 = true; // 海关查验是否正常，异常将不查询查验结果

                foreach (集装箱数据 data in boxList)
                {
                    bool have = false;
                    foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
                    {
                        if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
                        {
                            have = true;
                            break;
                        }

                        进港地 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", data.堆场区);
                    }
                    if (!have)
                    {
                        int xx = 0;
                        if (int.TryParse(data.箱型, out xx))
                        {
                            if (xx < 40)
                            {
                                xx = 20;
                            }

                            if (xx >= 45)
                            {
                                xx = 41;
                            }
                            else
                            {
                                xx = 40;
                            }
                        }

                        int? 箱型编号 = null;
                        if (xx != 0)
                        {
                            箱型编号 = xx;
                        }

                        出口箱 newItem = new 出口箱
                        {
                            箱号 = data.集装箱号,
                            箱型编号 = 箱型编号,
                            装货地编号 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", data.堆场区),
                            进港时间 = data.Real进场时间,
                            //提箱时间 = data.Real提箱时间
                        };

                        //if (is查验)
                        //{
                        //    try
                        //    {
                        //        查询海关查验结果(newItem);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        is查验 = false;
                        //        MessageForm.ShowError(ex.Message, "海关查验");
                        //    }
                        //}

                        detailCm.AddNew();
                        detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
                        detailCm.EndEdit();
                    }
                }

                AskToReplace(masterCm, "进港地编号", 进港地);

                保存报关单快照(bgdInfo.报关单长号, bgdInfo.网页快照);
            }
        }

        public static void ReadPx_报关单号(ArchiveOperationForm masterForm)
        {
            IControlManager masterCm = masterForm.ControlManager;

            if (masterCm.DisplayManager.DataControls["报关单号"].SelectedDataValue == null)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入报关单号！");
                return;
            }

            string bgdh = masterCm.DisplayManager.DataControls["报关单号"].SelectedDataValue.ToString().Trim();

            报关单数据 bgdInfo = null;
            foreach (KeyValuePair<string, string> id in Get_nbediRead_ID())
            {
                nbediRead m_nbediRead = new nbediRead();
                m_nbediRead.SetLoginInfo(id.Key, id.Value);
                bgdInfo = m_nbediRead.长短号查询报关单数据(bgdh);
                if (bgdInfo != null && !string.IsNullOrEmpty(bgdInfo.报关单号))
                {
                    break;
                }
            }

            if (bgdInfo == null)
            {
                return;
            }

            AskToReplace(masterCm, "报关单号", bgdInfo.报关单长号);
            AskToReplace(masterCm, "抬头", bgdInfo.经营单位);
            AskToReplace(masterCm, "提单号", bgdInfo.提运单号);
            AskToReplace(masterCm, "核销单号", bgdInfo.批准文号);
            AskToReplace(masterCm, "通关单号", bgdInfo.通关单号);
            AskToReplace(masterCm, "箱号", bgdInfo.箱号);
            AskToReplace(masterCm, "箱量", bgdInfo.箱量);
            AskToReplace(masterCm, "委托时间", bgdInfo.申报日期);
            AskToReplace(masterCm, "船名航次", bgdInfo.船名航次);
            AskToReplace(masterCm, "报关单快照", bgdInfo.网页快照);
            AskToReplace(masterCm, "报关员编号", bgdInfo.报关员);
            AskToReplace(masterCm, "报关公司", bgdInfo.报关公司);

            if (string.IsNullOrEmpty(bgdInfo.提运单号))
            {
                return;
            }

            IControlManager detailCm = (((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0] as IArchiveGrid).ControlManager;

            IList<集装箱数据> boxList = 查询出口集装箱数据By提单号航次(bgdInfo.提运单号.Trim(), bgdInfo.船名航次.Split('/')[1]);
            string 进港地 = null;
            DateTime? 离港时间 = null;

            foreach (集装箱数据 data in boxList)
            {
                bool have = false;
                foreach (Xceed.Grid.DataRow row in ((IArchiveDetailFormWithDetailGrids)masterForm.ArchiveDetailForm).DetailGrids[0].DataRows)
                {
                    if (row.Cells["箱号"].Value != null && row.Cells["箱号"].Value.ToString().Trim() == data.集装箱号.Trim())
                    {
                        have = true;
                        break;
                    }

                    进港地 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", data.堆场区);
                    离港时间 = data.Real进场时间;
                }
                if (!have)
                {
                    拼箱箱 newItem = new 拼箱箱
                    {
                        箱号 = data.集装箱号,
                        箱型 = Feng.Utils.ConvertHelper.ChangeType(data.箱型, typeof(箱型)) as 箱型,
                        装货地编号 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", data.堆场区),
                        进港时间 = data.Real进场时间,
                        提箱时间 = data.Real提箱时间
                    };

                    detailCm.AddNew();
                    detailCm.DisplayManager.Items[detailCm.DisplayManager.Position] = newItem;
                    detailCm.EndEdit();
                }
            }

            AskToReplace(masterCm, "进港地编号", 进港地);
            AskToReplace(masterCm, "离港时间", 离港时间);
        }

        private static IList<集装箱数据> 查询出口集装箱数据By提单号(string tdh)
        {
            m_nbeportGrab.SetLoginInfo(SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadUserName", ""),
                SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadPassword", ""));
            return m_nbeportGrab.查询集装箱数据(ImportExportType.出口集装箱, tdh);
        }

        private static IList<集装箱数据> 查询出口集装箱数据By提单号航次(string tdh, string 航次)
        {
            m_nbeportGrab.SetLoginInfo(SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadUserName", ""),
                SystemProfileFile.DefaultUserProfile.GetValue("Hd.Options", "NetReadPassword", ""));
            return m_nbeportGrab.查询集装箱数据(ImportExportType.出口集装箱, tdh, 航次);
        }

        public static void 载入出口报关单数据(ArchiveOperationForm masterForm)
        {
            GeneratedArchiveDataControlForm dataControlForm = ServiceProvider.GetService<IWindowFactory>().CreateWindow(ADInfoBll.Instance.GetWindowInfo("出口_备案_报关单导入")) as GeneratedArchiveDataControlForm;

            if (dataControlForm.ShowDialog() == DialogResult.OK)
            {
                if (dataControlForm.DataControls["委托时间"].SelectedDataValue == null)
                {
                    ServiceProvider.GetService<IMessageBox>().ShowWarning("请输入委托时间！");
                    return;
                }

                if (DateTime.Parse(dataControlForm.DataControls["委托时间"].SelectedDataValue.ToString()) > DateTime.Today)
                {
                    ServiceProvider.GetService<IMessageBox>().ShowWarning("不能载入大于今天的报关单数据！");
                    return;
                }

                DateTime wtsj = DateTime.Parse(dataControlForm.DataControls["委托时间"].SelectedDataValue.ToString());

                int count = 0;
                ProgressAsyncHelper pah = new ProgressAsyncHelper(new Feng.Async.AsyncHelper.DoWork(
                    delegate()
                    {
                        foreach (KeyValuePair<string, string> id in Get_nbediRead_ID())
                        {
                            m_nbediRead = new nbediRead();
                            m_nbediRead.SetLoginInfo(id.Key, id.Value);

                            //List<string> bgdhList = m_nbediRead.查询报关单号(wtsj);
                            Dictionary<string, string> bgdhList = m_nbediRead.查询报关单号2(wtsj);

                            if (bgdhList == null || bgdhList.Count == 0)
                            {
                                continue;
                            }

                            // 查询数据库已有提单号，避免重复数据
                            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<出口票>())
                            {
                                try
                                {
                                    //IList<string> bgdh_List = (rep as Feng.NH.INHibernateRepository).Session.CreateCriteria(typeof(出口票))
                                    //    .SetProjection(NHibernate.Criterion.Projections.Distinct(NHibernate.Criterion.Projections.ProjectionList()
                                    //    .Add(NHibernate.Criterion.Projections.Property("报关单号")))).List<string>();

                                    //bool is查验 = true; // 海关查验是否正常，异常将不查询查验结果

                                    //foreach (string bgdh in bgdhList)   // 网上的报关单号
                                    //{
                                    foreach (KeyValuePair<string, string> bgdh in bgdhList)
                                    {
                                        if (bgdh.Key.Substring(9, 1) == "0" || bgdh.Key.Substring(9, 1) == "1")    // 0 = 进口，5 = 出口
                                        {
                                            continue;
                                        }

                                        IList<出口票> ckp_List = (rep as Feng.NH.INHibernateRepository).Session.CreateCriteria(typeof(出口票))
                                            .Add(NHibernate.Criterion.Expression.Eq("报关单号", bgdh.Key)).List<出口票>();

                                        if (ckp_List != null && ckp_List.Count > 0)
                                        {
                                            continue;
                                        }

                                        rep.BeginTransaction();

                                        报关单数据 bgdsj = m_nbediRead.查询报关单数据2(bgdh.Key, bgdh.Value);
                                        if (bgdsj != null)
                                        {
                                            出口票 ckp = new 出口票
                                            {
                                                报关单号 = bgdsj.报关单长号,
                                                抬头 = bgdsj.经营单位,
                                                提单号 = bgdsj.提运单号,
                                                核销单号 = bgdsj.批准文号,
                                                通关单号 = bgdsj.通关单号,
                                                //箱号 = bgdsj.箱号,// Formula = Distinct 出口箱.箱号
                                                箱量 = bgdsj.箱量,
                                                委托时间 = bgdsj.申报日期,
                                                船名航次 = bgdsj.船名航次,
                                                //报关单快照 = bgdsj.网页快照,
                                                报关员编号 = bgdsj.报关员,
                                                报关公司 = bgdsj.报关公司
                                            };

                                            if (!string.IsNullOrEmpty(ckp.提单号))
                                            {
                                                IList<集装箱数据> jzxList = null;
                                                try
                                                {
                                                    jzxList = m_nbediRead.查询集装箱数据(bgdsj.提运单号.Trim(), bgdsj.船名航次.Split('/')[0], bgdsj.船名航次.Split('/')[1]);
                                                    //IList<集装箱数据> jzxList = 查询出口集装箱数据By提单号航次(bgdsj.提运单号.Trim(), bgdsj.船名航次.Split('/')[1]);
                                                }
                                                catch
                                                {
                                                }

                                                if (jzxList != null && jzxList.Count > 0)
                                                {
                                                    List<string> success箱号 = new List<string>();

                                                    foreach (集装箱数据 jzx in jzxList)
                                                    {
                                                        // 避免重复箱保存
                                                        bool isSame箱号 = false;
                                                        foreach (string 箱号 in success箱号)
                                                        {
                                                            if (jzx.集装箱号 == 箱号)
                                                            {
                                                                isSame箱号 = true;
                                                                break;
                                                            }
                                                        }

                                                        if (isSame箱号)
                                                        {
                                                            continue;
                                                        }

                                                        int xx = 0;
                                                        if (int.TryParse(jzx.箱型, out xx))
                                                        {
                                                            if (xx < 40)
                                                            {
                                                                xx = 20;
                                                            }

                                                            if (xx >= 45)
                                                            {
                                                                xx = 41;
                                                            }
                                                            else
                                                            {
                                                                xx = 40;
                                                            }
                                                        }

                                                        int? 箱型编号 = null;
                                                        if (xx != 0)
                                                        {
                                                            箱型编号 = xx;
                                                        }

                                                        出口箱 newCkx = new 出口箱
                                                        {
                                                            箱号 = jzx.集装箱号,
                                                            箱型编号 = 箱型编号,
                                                            装货地编号 = (string)NameValueMappingCollection.Instance.FindColumn2FromColumn1("人员单位_港区堆场", "全称", "编号", jzx.堆场区),
                                                            进港时间 = jzx.Real进场时间,
                                                            //提箱时间 = jzx.Real提箱时间
                                                        };

                                                        //if (is查验)
                                                        //{
                                                        //    try
                                                        //    {
                                                        //        查询海关查验结果(newCkx);
                                                        //    }
                                                        //    catch (Exception ex)
                                                        //    {
                                                        //        is查验 = false;
                                                        //        MessageForm.ShowError(ex.Message, "海关查验");
                                                        //    }
                                                        //}

                                                        success箱号.Add(newCkx.箱号);

                                                        if (ckp.进港地编号 == null && ckp.离港时间 == null)
                                                        {
                                                            ckp.进港地编号 = newCkx.装货地编号;
                                                            //ckp.离港时间 = newCkx.进港时间;

                                                            new 出口票Dao().Save(rep, ckp);

                                                            保存报关单快照(bgdsj.报关单长号, bgdsj.网页快照);
                                                        }

                                                        newCkx.票 = ckp;
                                                        new HdBaseDao<出口箱>().Save(rep, newCkx);
                                                    }
                                                }
                                                else
                                                {
                                                    new 出口票Dao().Save(rep, ckp);
                                                }
                                            }
                                        }
                                        rep.CommitTransaction();
                                        count++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    rep.RollbackTransaction();
                                    throw new NullReferenceException(ex.Message, ex);
                                }
                            }
                        }
                        return null;
                    }), new Feng.Async.AsyncHelper.WorkDone(
                        delegate(object result)
                        {
                            MessageForm.ShowInfo("成功载入报关单数据" + count + "条");
                        }), masterForm, "载入报关单");
            }
        }

        private static void 保存报关单快照(string 报关单号, string 网页快照)
        {
            Feng.Data.DbHelper db = Feng.Data.DbHelper.Instance;
            db.ExecuteNonQuery("delete Temp_报关单网页快照 where 报关单号 = '" + 报关单号 + "'");
            db.ExecuteNonQuery(string.Format("insert Temp_报关单网页快照 (报关单号,网页快照) values ('{0}','{1}')", 报关单号, 网页快照.Replace("'", "''")));
        }

        //public static void 读取报关单网页快照(string 报关单号)
        //{
        //    foreach (KeyValuePair<string, string> id in Get_nbediRead_ID())
        //    {
        //        m_nbediRead = new nbediRead();
        //        m_nbediRead.SetLoginInfo(id.Key, id.Value);
        //        报关单数据 bgdsj = m_nbediRead.查询报关单数据(报关单号);
        //        if (bgdsj == null)
        //        {
        //            continue;
        //        }
        //        保存报关单快照(bgdsj.报关单长号, bgdsj.网页快照);
        //    }
        //}

        /// <summary>
        /// 生成并预览报关单
        /// 在Html文件夹下，生成以报关单号为文件名的Html文件
        /// </summary>
        /// <param name="masterForm"></param>
        public static void 预览报关单快照(ArchiveOperationForm masterForm)
        {
            if (!System.IO.Directory.Exists("Html"))
            {
                System.IO.Directory.CreateDirectory("Html");
            }

            bool isNull = false;
            string warnMessages = "没有以下报关单的预览代码，请重新网上读取：";

            foreach (Xceed.Grid.DataRow row in masterForm.MasterGrid.GridControl.SelectedRows)
            {
                if (row.Cells["报关单号"].Value == null)
                {
                    continue;
                }

                object wykz = Feng.Data.DbHelper.Instance.ExecuteScalar("select 网页快照 from Temp_报关单网页快照 where 报关单号 = '" + row.Cells["报关单号"].Value + "'");
                if (wykz == null || string.IsNullOrEmpty(wykz.ToString()))
                {
                    warnMessages += Environment.NewLine + row.Cells["报关单号"].Value;
                    isNull = true;
                    continue;
                }

                System.IO.File.WriteAllText("Html\\" + row.Cells["报关单号"].Value + ".htm", wykz.ToString(), Encoding.UTF8);

                System.Diagnostics.Process.Start("Html\\" + row.Cells["报关单号"].Value + ".htm");
            }

            if (isNull)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning(warnMessages);
            }
        }

        #region 供服务使用

        // 服务_读取出口海关查验结果
        public static 出口箱 查询海关查验结果(出口箱 ckx)
        {
            if (string.IsNullOrEmpty(ckx.箱号))
            {
                throw new ArgumentNullException("出口箱箱号 is null");
            }

            海关查验查询结果 hgcxjg = m_npediGrab.查询海关查验结果(ckx.箱号);


            if (hgcxjg == null)
            {
                ckx.查验标志 = false;
                //ckx.海关查验 = Hd.Model.查验标志.不查验;
            }
            else
            {
                if (ckx.票.船名航次 == hgcxjg.船名 + "/" + hgcxjg.航次)
                {
                    using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<出口票>())
                    {
                        rep.BeginTransaction();
                        出口票 ckp = rep.Get<出口票>(ckx.票.ID);
                        ckp.海关查验时间 = hgcxjg.处理时间;
                        new 出口票Dao().Update(rep, ckp);
                        rep.CommitTransaction();
                    }

                    ckx.查验标志 = true;
                    if (hgcxjg.H986 == "Y")
                    {
                        ckx.海关查验 = Hd.Model.查验标志.不开箱门;
                    }
                    else if (hgcxjg.H986 == "N")
                    {
                        ckx.海关查验 = Hd.Model.查验标志.开箱门;
                    }
                }
            }
            return ckx;
        }

        #endregion
    }
}
