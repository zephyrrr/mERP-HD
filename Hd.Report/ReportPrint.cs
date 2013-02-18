using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Feng;
using Feng.Data;
using Feng.Windows.Forms;

namespace Hd.Report
{
    public class ReportPrint
    {
        public static void 打印凭证(string 凭证号)
        {
            MyReportForm form = new MyReportForm("报表_凭证");
            form.FillDataSet(0, DbHelper.Instance.ExecuteDataTable("SELECT * FROM 报表_凭证 WHERE 凭证号 = '" + 凭证号 + "'").DefaultView);
            form.FillDataSet(1, DbHelper.Instance.ExecuteDataTable("SELECT * FROM 报表_凭证费用明细 WHERE 凭证号 = '" + 凭证号 + "'").DefaultView);
            form.Show();
        }
        
        public static void 打印对账单(string 对账单号,int iFyxs)
        {
            if (iFyxs == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("不能打印费用项数为0的对账单！");
                return;
            }
            string sql = "exec 过程查询_交叉表_对账单 '" + 对账单号 + "'";
            DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
            int count = tempTB.Columns.Count;
            DataTable dt = dtx(tempTB,iFyxs);
            MyReportForm form = null;
            if (iFyxs <= 5)
            {
                form = new MyReportForm("报表_对账单_" + iFyxs);
            }
            else if (iFyxs > 5 && iFyxs < 10)
            {
                form = new MyReportForm("报表_对账单_5");
            }
            else
            {
                form = new MyReportForm("报表_对账单_10");
            }
           
            form.FillDataSet(0, dt.DefaultView);

            //设置参数，即表头
            if (iFyxs > 5 && iFyxs < 10)
            {
                for (int i = 1; i <= 5 ; i++)
                {
                    if (i <= tempTB.Columns.Count - 20)
                        form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[15 + i].ColumnName);
                    //else
                    //    //给没用到的参数一个空值
                    //    form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
                }
            }
            else
            {
                for (int i = 1; i <= (iFyxs <= 5 ? iFyxs : 10) ; i++)
                {
                    if (i <= tempTB.Columns.Count - 20)
                        form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[15 + i].ColumnName);
                    //else
                    //    //给没用到的参数一个空值
                    //    form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
                }
            }
            
            if (iFyxs > 5 && count > 24)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c5", "其他费用");
            }

            if (iFyxs > 10 && count > 27)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c10", "其他费用");
            }

            form.Show();
        }

        public static void 打印对账单_额外(string 对账单号, int iFyxs)
        {
            if (iFyxs == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("不能打印费用项数为0的对账单！");
                return;
            }
            string sql = "exec 过程查询_交叉表_对账单_额外 '" + 对账单号 + "'";
            DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
            int count = tempTB.Columns.Count;
            DataTable dt = dtx_ew(tempTB, iFyxs);
            MyReportForm form = null;
            if (iFyxs <= 5)
            {
                form = new MyReportForm("报表_对账单_额外_" + iFyxs);
            }
            else if (iFyxs > 5 && iFyxs < 10)
            {
                form = new MyReportForm("报表_对账单_额外_5");
            }
            else
            {
                form = new MyReportForm("报表_对账单_额外_10");
            }

            form.FillDataSet(0, dt.DefaultView);

            //设置参数，即表头
            if (iFyxs > 5 && iFyxs < 10)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (i <= tempTB.Columns.Count - 19)
                        form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[14 + i].ColumnName);
                    //else
                    //    //给没用到的参数一个空值
                    //    form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
                }
            }
            else
            {
                for (int i = 1; i <= (iFyxs <= 5 ? iFyxs : 10); i++)
                {
                    if (i <= tempTB.Columns.Count - 19)
                        form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[14 + i].ColumnName);
                    //else
                    //    //给没用到的参数一个空值
                    //    form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
                }
            }

            if (iFyxs > 5 && count > 23)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c5", "其他费用");
            }

            if (iFyxs > 10 && count > 26)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c10", "其他费用");
            }

            form.Show();
        }

        //YYH对账单  
        public static void 打印原始对账单(string 对账单号, int iFyxs)
        {
            if (iFyxs == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("不能打印费用项数为0的对账单！");
                return;
            }
            string sql = "exec 过程查询_交叉表_对账单 '" + 对账单号 + "'";
            DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
            int count = tempTB.Columns.Count;
            DataTable dt = dtx_ys(tempTB, iFyxs);
            MyReportForm form = new MyReportForm("报表_对账单_原始");
            form.FillDataSet(0, dt.DefaultView);
            form.Show();
        }

        public static void 打印内贸结算明细单(string 对账单号, int iFyxs)
        {
            if (iFyxs == 0)
            {
                ServiceProvider.GetService<IMessageBox>().ShowWarning("不能打印费用项数为0的对账单！");
                return;
            }
            string sql = "exec 过程查询_交叉表_对账单_内贸结算明细'" + 对账单号 +"'";
            DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
            DataTable dt = dtx2(tempTB,iFyxs);

            MyReportForm form = new MyReportForm(iFyxs > 1 ? "报表_内贸结算明细_2" : "报表_内贸结算明细");
            form.FillDataSet(0, dt.DefaultView);

            for (int i = 1; i < iFyxs + 1; i++)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[16 + i].ColumnName);
                if (tempTB.Columns[16 + i].ColumnName.Equals("单箱代理费"))
                {
                    form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), "包干费");
                }
            }
            if (iFyxs > 2)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c2", "其他费用");
            }
            form.Show();
        }

        //进口其他业务应收对账单
        public static void 打印进口其他业务对账单(string 对账单号, int iFyxs)
        {
            if (iFyxs == 0)
            {
                
                return;
            }
            string sql = "exec 过程查询_交叉表_对账单_进口其他业务'" + 对账单号 + "'";
            DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
            DataTable dt = dtx_jkqt(tempTB, iFyxs);
            string reportName = null;
            switch (iFyxs)
            {
                case 1:
                    reportName = "报表_进口其他业务_应收对账单_1";
                    break;
                case 2:
                    reportName = "报表_进口其他业务_应收对账单_2";
                    break;
                case 3:
                    reportName = "报表_进口其他业务_应收对账单_3";
                    break;
                case 4:
                    reportName = "报表_进口其他业务_应收对账单_4";
                    break;
                case 5:
                    reportName = "报表_进口其他业务_应收对账单_5";
                    break;
                case 6:
                    reportName = "报表_进口其他业务_应收对账单_6";
                    break;
                case 7:
                    reportName = "报表_进口其他业务_应收对账单_7";
                    break;
                default:
                    MessageForm.ShowError("报表创建失败！");
                    return;
            }

            MyReportForm form = new MyReportForm(reportName);
            form.FillDataSet(0, dt.DefaultView);

            for (int i = 1; i < iFyxs + 1; i++)
            {
                form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[10 + i].ColumnName);
            }
            form.Show();
        }

        //public static void 打印对账单05(string 对账单号)
        //{
        //    string sql = "exec 过程查询_交叉表_对账单 '" + 对账单号 + "'";
        //    DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
        //    int count = tempTB.Columns.Count;
        //    DataTable dt = dtx5(tempTB);

        //    MyReportForm form = new MyReportForm("报表_对账单_5");
        //    form.FillDataSet(0, dt.DefaultView);

        //    //设置参数，即表头
        //    for (int i = 1; i <= 6; i++)
        //    {
        //        if (i <= tempTB.Columns.Count - 19)
        //            form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[14 + i].ColumnName);
        //        else
        //            //给没用到的参数一个空值
        //            form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
        //    }
        //    if (count > 23)
        //    {
        //        form.CrystalReportViewer.CrystalHelper.SetParameter("c6", "其他费用");
        //    }

        //    form.Show();
        //}

        //public static void 打印对账单10(string 对账单号)
        //{
        //    string sql = "exec 过程查询_交叉表_对账单 '" + 对账单号 + "'";
        //    DataTable tempTB = DbHelper.Instance.ExecuteDataTable(sql);
        //    int count = tempTB.Columns.Count;
        //    DataTable dt = dtx10(tempTB);
            
        //    MyReportForm form = new MyReportForm("报表_对账单_10");
        //    form.FillDataSet(0, dt.DefaultView);

        //    //设置参数，即表头
        //    for (int i = 1; i <= 11; i++)
        //    {
        //        if (i <= tempTB.Columns.Count - 19)
        //            form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), tempTB.Columns[14 + i].ColumnName);
        //        else
        //            //给没用到的参数一个空值
        //            form.CrystalReportViewer.CrystalHelper.SetParameter("c" + i.ToString(), " ");
        //    }
        //    if (count > 26)
        //    {
        //        form.CrystalReportViewer.CrystalHelper.SetParameter("c11", "其他费用");
        //    }

        //    form.Show();
        //}

        /// <summary>
        /// 将传入的datatable转换成报表模板所需要的datatable
        /// </summary>
        /// <param name="dt">来源表</param>
        /// <returns>报表模板所需要的datatable</returns>
        public static DataTable dtx(DataTable dt, int iFyxs)
        {
            宁波易可报关有限公司业务费用核对确认单_10Ds.宁波易可报关有限公司业务费用核对确认单_10DataTable dtx1 = new 宁波易可报关有限公司业务费用核对确认单_10Ds.宁波易可报关有限公司业务费用核对确认单_10DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtx1.Rows.Add(dtx1.NewRow());
                decimal moneySum = 0;
                //从第一列循环至所有费用项完
                for (int j = 0; j < dt.Columns.Count - 5; j++)
                {
                    //增加减少基本信息列，修改一下3个j的值和2个dtx1.Rows[i][?]
                    //大于10个费用项，其余费用项金额累计=其他费用
                    if ((iFyxs > 10 && j > 24) || (iFyxs > 5 && iFyxs < 10 && j > 19)) //15基本信息列+10列费用项
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                        {
                            moneySum = moneySum + Convert.ToDecimal(dt.Rows[i][j]);
                        }
                        else
                        {
                            moneySum = moneySum + 0;
                        }
                        if (iFyxs > 10)
                        {
                            dtx1.Rows[i][25] = moneySum;//修改处
                        }
                        if (iFyxs > 5 && iFyxs < 10)
                        {
                            dtx1.Rows[i][20] = moneySum;//修改处
                        }
                    }
                    else
                    {
                        dtx1.Rows[i][j] = dt.Rows[i][j];
                        if (j > 15 && string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                        {
                            dtx1.Rows[i][j] = 0;
                        }
                    }
                }
                //拼写备注
                string beiZhu = "";
                //最终免箱标志 = true and 免箱联系货主时间 is not null
                if (dt.Rows[i]["最终免箱标志"].ToString().Equals("True") && !string.IsNullOrEmpty(dt.Rows[i]["免箱联系货主时间"].ToString()))
                {
                    beiZhu = "船公司已确认滞箱费减免；";
                }

                //单证晚到 is not null
                if (!string.IsNullOrEmpty(dt.Rows[i]["单证晚到"].ToString()))
                {
                    if (Convert.ToInt32(dt.Rows[i]["单证晚到"].ToString()) > 0)
                    {
                        beiZhu += "单证晚到" + dt.Rows[i]["单证晚到"].ToString() + "天；";
                    }
                }
                //拼写备注 + Columns["备注"]    对账单_页脚
                dtx1.Rows[i]["备注"] = beiZhu + dt.Rows[i]["备注"].ToString();
                dtx1.Rows[i]["对账单_页脚"] = dt.Rows[i]["对账单_页脚"].ToString();
            }
            return dtx1;
        }

        public static DataTable dtx_ew(DataTable dt, int iFyxs)
        {
            宁波易可报关有限公司业务费用核对确认单_额外_10Ds.宁波易可报关有限公司业务费用核对确认单_10DataTable dtx1 = new 宁波易可报关有限公司业务费用核对确认单_额外_10Ds.宁波易可报关有限公司业务费用核对确认单_10DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtx1.Rows.Add(dtx1.NewRow());
                decimal moneySum = 0;
                //从第一列循环至所有费用项完
                for (int j = 0; j < dt.Columns.Count - 5; j++)
                {
                    //大于10个费用项，其余费用项金额累计=其他费用
                    if ((iFyxs > 10 && j > 23) || (iFyxs > 5 && iFyxs < 10 && j > 18)) //15基本信息列+10列费用项
                    {
                        if (!dt.Rows[i][j].ToString().Equals("未确定"))
                        {
                            moneySum = moneySum + Convert.ToDecimal(dt.Rows[i][j]);
                        }
                        else
                        {
                            moneySum = moneySum + 0;
                        }
                        if (iFyxs > 10)
                        {
                            dtx1.Rows[i][24] = moneySum.ToString();
                        }
                        if (iFyxs > 5 && iFyxs < 10)
                        {
                            dtx1.Rows[i][19] = moneySum.ToString();
                        }
                    }
                    else
                    {
                        dtx1.Rows[i][j] = dt.Rows[i][j].ToString();
                    }
                }
                //拼写备注
                string beiZhu = "";
                //最终免箱标志 = true and 免箱联系货主时间 is not null
                if (dt.Rows[i]["最终免箱标志"].ToString().Equals("True") && !string.IsNullOrEmpty(dt.Rows[i]["免箱联系货主时间"].ToString()))
                {
                    beiZhu = "船公司已确认滞箱费减免；";
                }

                //单证晚到 is not null
                if (!string.IsNullOrEmpty(dt.Rows[i]["单证晚到"].ToString()))
                {
                    if (Convert.ToInt32(dt.Rows[i]["单证晚到"].ToString()) > 0)
                    {
                        beiZhu += "单证晚到" + dt.Rows[i]["单证晚到"].ToString() + "天；";
                    }
                }
                //拼写备注 + Columns["备注"]    对账单_页脚
                dtx1.Rows[i]["备注"] = beiZhu + dt.Rows[i]["备注"].ToString();
                dtx1.Rows[i]["对账单_页脚"] = dt.Rows[i]["对账单_页脚"].ToString();
            }
            return dtx1;
        }

        //YYH对账单
        public static DataTable dtx_ys(DataTable dt, int iFyxs)
        {
            宁波易可报关有限公司业务费用核对确认单Ds.宁波易可报关有限公司业务费用核对确认单DataTable dtx1 = new 宁波易可报关有限公司业务费用核对确认单Ds.宁波易可报关有限公司业务费用核对确认单DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtx1.Rows.Add(dtx1.NewRow());
                decimal 换单费 = 0;
                decimal 输单费 = 0;  //商检输单费 + 海关输单费
                decimal 港务港建费 = 0;  //港务费 + 港建费
                decimal 查验费 = 0;  //商检查验费 + 海关查验费
                decimal 卫生箱检费 = 0;  //卫生处理费 + 箱检费
                decimal 开箱单理货费 = 0; //开箱单代理费 + 外理费
                decimal 箱单费 = 0;
                decimal 检验检疫费 = 0;
                decimal 倒箱费 = 0;  //商检开倒箱费 + 海关开倒箱费 + 指运地开倒箱费
                decimal 施封费 = 0;  //转关施封费_现金 + 转关施封费_支票
                decimal 单箱代理费 = 0;
                decimal 其它 = 0;
                //decimal 小计 = 0;
                //合并费用项      16基本信息列                     
                for (int j = 16; j < dt.Columns.Count - 5; j++) //修改处
                { 
                    switch (dt.Columns[j].ColumnName)
                    {
                        case "换单费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                换单费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "商检输单费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                输单费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "海关输单费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                输单费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "港务费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                港务港建费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "港建费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                港务港建费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "商检查验费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                查验费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "海关查验费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                查验费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "卫生处理费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                卫生箱检费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "箱检费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                卫生箱检费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "开箱单代理费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                开箱单理货费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "外理费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                开箱单理货费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "箱单费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                箱单费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "检验检疫费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                检验检疫费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "商检开倒箱费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                倒箱费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "海关开倒箱费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                倒箱费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "指运地开倒箱费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                倒箱费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "转关施封费_现金":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                施封费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "转关施封费_支票":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                施封费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        case "单箱代理费":
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                单箱代理费 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                        default:
                            if (!String.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                其它 += Convert.ToDecimal(dt.Rows[i][j]);
                            }
                            break;
                    }
                }
                decimal[] moneyList = new decimal[] { 换单费, 输单费, 港务港建费, 查验费, 卫生箱检费, 开箱单理货费, 箱单费, 检验检疫费, 倒箱费, 施封费, 单箱代理费, 其它 };

                for (int k = 0; k <= dtx1.Columns.Count - 3; k++)
                {
                    if (k < 16)//修改处
                    {
                        dtx1.Rows[i][k] = dt.Rows[i][k];
                    }
                    else
                    {
                        dtx1.Rows[i][k] = moneyList[moneyList.Length - dtx1.Columns.Count + 2 + k];
                    }
                }

                //拼写备注
                string beiZhu = "";
                //最终免箱标志 = true and 免箱联系货主时间 is not null
                if (dt.Rows[i]["最终免箱标志"].ToString().Equals("True") && !string.IsNullOrEmpty(dt.Rows[i]["免箱联系货主时间"].ToString()))
                {
                    beiZhu = "船公司已确认滞箱费减免；";
                }

                //单证晚到 is not null
                if (!string.IsNullOrEmpty(dt.Rows[i]["单证晚到"].ToString()))
                {
                    if (Convert.ToInt32(dt.Rows[i]["单证晚到"].ToString()) > 0)
                    {
                        beiZhu += "单证晚到" + dt.Rows[i]["单证晚到"].ToString() + "天；";
                    }
                }
                //拼写备注 + Columns["备注"]    对账单_页脚
                dtx1.Rows[i]["备注"] = beiZhu + dt.Rows[i]["备注"].ToString();
                dtx1.Rows[i]["对账单_页脚"] = dt.Rows[i]["对账单_页脚"].ToString();
            }
            return dtx1;
        }

        //进口其他业务应收对账单
        public static DataTable dtx_jkqt(DataTable dt, int iFyxs)
        {
            进口其他业务应收对账单Ds.进口其他业务DataTable dtx1 = new 进口其他业务应收对账单Ds.进口其他业务DataTable();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtx1.Rows.Add(dtx1.NewRow());
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    dtx1.Rows[i][j] = dt.Rows[i][j];
                }
            }
            return dtx1;
        }

        //内贸结算明细单
        public static DataTable dtx2(DataTable dt, int iFyxs)
        {
            内贸结算明细Ds.内贸结算明细DataTable dtx1 = new 内贸结算明细Ds.内贸结算明细DataTable();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtx1.Rows.Add(dtx1.NewRow());
                decimal moneySum = 0;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //计算其他费用
                    if (j > 17) //基本信息列    修改处
                    {
                        if (iFyxs < 3)
                        {
                            if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                            {
                                dtx1.Rows[i][j] = 0;
                            }
                            else
                            {
                                dtx1.Rows[i][j] = Convert.ToDecimal(dt.Rows[i][j]);
                            }
                        }
                        else
                        {
                            if (dt.Columns[j].ColumnName.Equals("单箱代理费"))
                            {
                                if (string.IsNullOrEmpty(dt.Rows[i]["单箱代理费"].ToString()))
                                {
                                    dtx1.Rows[i][17] = 0;//修改处
                                }
                                else
                                {
                                    dtx1.Rows[i][17] = Convert.ToDecimal(dt.Rows[i]["单箱代理费"]);//修改处
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(dt.Rows[i][j].ToString()))
                                {
                                    moneySum = moneySum + 0;
                                }
                                else
                                {
                                    moneySum = moneySum + Convert.ToDecimal(dt.Rows[i][j]);
                                }
                                dtx1.Rows[i][18] = moneySum;//修改处
                            }
                        }
                        
                    }
                    else
                    {
                        dtx1.Rows[i][j] = dt.Rows[i][j];
                    }
                }
            }
            return dtx1;
        }        
    }
}
