using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hd.Service.理论值编辑
{
    class EdtHelper
    {
        /// <summary>
        /// 设置条目表头信息
        /// </summary>
        /// <param name="listview"></param>
        public static void SetHeader(ListView listview)
        {
            listview.Columns.Add("序号", 40, HorizontalAlignment.Center);
            listview.Columns.Add("条件", 330, HorizontalAlignment.Center);
            listview.Columns.Add("结果", 100, HorizontalAlignment.Center);
            listview.View = View.Details;
            listview.GridLines = true;

        }

        /// <summary>
        /// 新增条目
        /// </summary>
        /// <param name="listview"></param>
        /// <param name="tbCondition"></param>
        /// <param name="tbResult"></param>
        public static void AddItems(ListView listview, TextBox tbCondition, TextBox tbResult)
        {
            int rows = listview.Items.Count;
            string num = (rows + 1).ToString();
            ListViewItem item = new ListViewItem(new string[] { num, tbCondition.Text.Trim(), tbResult.Text.Trim() });
            listview.Items.Add(item);

        }

        /// <summary>
        /// 所选条目上移
        /// </summary>
        /// <param name="listview"></param>
        public static void MoveUp(ListView listview)
        {
            for (int i = 0; i < listview.SelectedItems.Count; i++)
            {
                System.Windows.Forms.ListViewItem listViewItem = listview.SelectedItems[i];
                string sNo = listViewItem.SubItems[0].Text;
                if (Int32.Parse(sNo) != 1)
                    listViewItem.SubItems[0].Text = (Int32.Parse(sNo) - 1).ToString();
                int index = listview.SelectedItems[i].Index - 1;
                if (index < 0) return;
                listview.Items.Remove(listview.SelectedItems[i]);
                listview.Items.Insert(index, listViewItem);
                listview.Items[index + 1].SubItems[0].Text = sNo;
                listViewItem.Selected = true;
            }

        }

        /// <summary>
        /// 所选条目下移
        /// </summary>
        /// <param name="listview"></param>
        public static void MoveDown(ListView listview)
        {
            for (int i = listview.SelectedItems.Count - 1; i > -1; i--)
            {
                System.Windows.Forms.ListViewItem listViewItem = listview.SelectedItems[i];
                string sNo = listViewItem.SubItems[0].Text;
                if (Int32.Parse(sNo) != listview.Items.Count)
                    listViewItem.SubItems[0].Text = (Int32.Parse(sNo) + 1).ToString();
                int index = listview.SelectedItems[i].Index + 1;
                if (index > listview.Items.Count - 1) return;
                listview.Items.Remove(listview.SelectedItems[i]);
                listview.Items.Insert(index, listViewItem);
                listview.Items[index - 1].SubItems[0].Text = sNo;
                listViewItem.Selected = true;
            }
        }


        /// <summary>
        /// 删除所选条目
        /// </summary>
        /// <param name="listview"></param>
        public static void DeleteItem(ListView listview)
        {
            for (int i = listview.SelectedItems.Count - 1; i > -1; i--)
            {
                System.Windows.Forms.ListViewItem listViewItem = listview.SelectedItems[i];
                string sNo = listViewItem.SubItems[0].Text;
                int index = listview.SelectedItems[i].Index + 1;
               // if (index > listview.Items.Count - 1) return;
                listview.Items.Remove(listview.SelectedItems[i]);
                for (int k = 0; k < (listview.Items.Count + 1 - Int32.Parse(sNo)); k++)
                    listview.Items[Int32.Parse(sNo) + k-1].SubItems[0].Text =
                        (Int32.Parse(listview.Items[Int32.Parse(sNo) + k-1].SubItems[0].Text)-1).ToString();
                listViewItem.Selected = true;
            }
        }

        /// <summary>
        /// 更新所选条目
        /// </summary>
        /// <param name="listview"></param>
        /// <param name="index"></param>
        /// <param name="conInfo"></param>
        /// <param name="refInfo"></param>
        public static void EditItem(ListView listview, int index,string conInfo,string refInfo)
        {
            listview.Items[index-1].SubItems[1].Text = conInfo;
            listview.Items[index-1].SubItems[2].Text = refInfo;
        }

        /// <summary>
        /// 插入所选字符
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="str"></param>
        /// <param name="m"></param>
        public static void InsertChr(TextBox tb, string str,int m)
        {
            int currentLocation = tb.SelectionStart;
            string frontText = tb.Text.Substring(0, currentLocation);
            string lastText = tb.Text.Substring(currentLocation, tb.Text.Length - currentLocation);
            frontText += str;
            tb.Text = frontText + lastText;
            tb.SelectionStart = frontText.Length - m;
            tb.Focus();
        }

        /// <summary>
        /// 获取条目结果集
        /// </summary>
        /// <param name="listview"></param>
        /// <returns></returns>
        public static IList<string> GenResult(ListView listview)
        {
            int itemsCount = listview.Items.Count;
            IList<string> list = new List<string>();
            for (int i = 0; i < itemsCount; i++)
            {
                try
                {
                    list.Add(listview.Items[i].SubItems[1].Text.Trim());
                    list.Add(listview.Items[i].SubItems[2].Text.Trim());
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("已添加具有相同条件的项目,请检查...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return list;

        }

        /// <summary>
        /// 刷新条件、结果框
        /// </summary>
        /// <param name="conTb"></param>
        /// <param name="refTb"></param>
        public static void TextBoxRefresh(TextBox conTb,TextBox refTb)
        {
            conTb.Clear();
            refTb.Clear();
        }
    }
}
