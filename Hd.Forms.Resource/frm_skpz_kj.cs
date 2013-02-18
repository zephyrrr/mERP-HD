using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Feng;
using Feng.Windows.Forms;
using Feng.Grid;
using Feng.Windows.Utils;
using Hd.Model;

namespace Hd.Forms
{
    public partial class frm_skpz_kj : ArchiveDetailFormWithDetailGrids
    {
        public frm_skpz_kj()
        {
            InitializeComponent();

            this.AddDetailGrid(grdFymx);
        }

        protected override void Form_Load(object sender, EventArgs e)
        {
            this.ControlManager.StateControls.Add(grdFymx);

            base.Form_Load(sender, e);
        }

        private void myCurrencyTextBox1_TextChanged(object sender, EventArgs e)
        {
            decimal? d = Feng.Utils.ConvertHelper.ToDecimal(myCurrencyTextBox1.Text);
            if (d.HasValue)
            {
                大写金额.SelectedDataValue = ChineseHelper.ConvertToChinese(d.Value);
            }
            else
            {
                大写金额.SelectedDataValue = null;
            }
        }
    }
}
