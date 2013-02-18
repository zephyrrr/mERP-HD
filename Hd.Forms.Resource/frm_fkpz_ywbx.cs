using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Feng.Windows.Forms;
using Feng.Grid;

namespace Hd.Forms
{
    public partial class frm_fkpz_ywbx : ArchiveDetailFormWithDetailGrids
    {
        public frm_fkpz_ywbx()
        {
            InitializeComponent();

            this.AddDetailGrid(gridControl1);
        }

    }
}
