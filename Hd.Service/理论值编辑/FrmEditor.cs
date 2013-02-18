using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Feng;

namespace Hd.Service.理论值编辑
{
    public partial class FrmEditor : Form
    {
        private FrmEditor()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> m_cobs;

        public FrmEditor(Dictionary<string, string> cobs, IList<string> initLists)
        {
            InitializeComponent();

            m_cobs = cobs;
            foreach (KeyValuePair<string, string> kvp in cobs)
            {
                cobNames.Items.Add(kvp.Key);
            }
            if (cobNames.Items.Count > 0)
            {
                cobValues.SelectedIndexChanged += new EventHandler(cobValues_SelectedIndexChanged);
            }

            for (int i = 0; i < initLists.Count; i += 2)
            {
                int rows = lstVew.Items.Count;
                string num = (rows + 1).ToString();
                ListViewItem item = new ListViewItem(new string[] { num, initLists[i], initLists[i + 1] });
                lstVew.Items.Add(item);
            }

            lstVew.ItemActivate += new EventHandler(lstVew_ItemActivate);
        }

        void lstVew_ItemActivate(object sender, EventArgs e)
        {
            btnModify_Click(btnModify, System.EventArgs.Empty);
        }

        void cobValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cobValues.SelectedIndex != -1)
            {
                string s = cobValues.SelectedValue.ToString();
                EdtHelper.InsertChr(tbxCon, s.Trim(), 0);
            }
        }

        private void cobNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cobNames.SelectedIndex != -1)
            {
                EdtHelper.InsertChr(tbxCon, "entity." + cobNames.Text.Trim(), 0);
                Feng.Windows.Utils.ControlDataLoad.InitDataControl(cobValues as INameValueMappingBindingControl, m_cobs[cobNames.Text]);
            }
            else
            {
                cobValues.Items.Clear();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbxCon.Text.Trim()))
            {
                if (index == -1)
                    EdtHelper.AddItems(lstVew, tbxCon, tbxRef);
                else
                {
                    EdtHelper.EditItem(lstVew, index, tbxCon.Text.Trim(),tbxRef.Text.Trim());
                    index = -1;
                }
            }
            EdtHelper.TextBoxRefresh(tbxCon, tbxRef);
            tbxCon.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            EdtHelper.TextBoxRefresh(tbxCon, tbxRef);
            cobNames.SelectedIndex = -1;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            EdtHelper.SetHeader(lstVew);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            EdtHelper.MoveUp(lstVew);
            lstVew.Focus();
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            EdtHelper.MoveDown(lstVew);
            lstVew.Focus();
        }

        protected void Btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Text)
            {
                case "and":
                    EdtHelper.InsertChr(tbxCon, " and ", 0);
                    break;
                case "or":
                    EdtHelper.InsertChr(tbxCon, " or ", 0);
                    break;
                case "not":
                    EdtHelper.InsertChr(tbxCon, " not ", 0);
                    break;
                case "( )":
                    EdtHelper.InsertChr(tbxCon, " ()", 1);
                    break;
                case "' '":
                    EdtHelper.InsertChr(tbxCon, " \'\'", 1);
                    break;
                case "==":
                    EdtHelper.InsertChr(tbxCon, " == ", 0);
                    break;
                case ">":
                    EdtHelper.InsertChr(tbxCon, " > ", 0);
                    break;
                case "<":
                    EdtHelper.InsertChr(tbxCon, " < ", 0);
                    break;
                default:
                    break;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstVew.SelectedItems.Count < 1)
                return;
            DialogResult rsp;
            string msg = "确定删除该项？";
            rsp = MessageBox.Show(msg, "删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rsp == DialogResult.Yes)
            {
                EdtHelper.DeleteItem(lstVew);
            }
        }

        int index=-1;
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (this.lstVew.FocusedItem != null)
            {
                this.tbxCon.Text = this.lstVew.FocusedItem.SubItems[1].Text;//获得的listView的值显示在文本框里  
                this.tbxCon.Focus();
                this.index = Int32.Parse(this.lstVew.FocusedItem.SubItems[0].Text);

                this.tbxRef.Text = this.lstVew.FocusedItem.SubItems[2].Text;
            }
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            EdtHelper.TextBoxRefresh(tbxCon, tbxRef);
            tbxCon.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
        }

        public IList<string> GetResult()
        {
            return EdtHelper.GenResult(lstVew);
        }
    }
}
