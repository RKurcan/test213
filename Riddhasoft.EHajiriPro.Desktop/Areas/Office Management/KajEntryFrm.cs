﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Riddhasoft.EHajiriPro.Desktop.Areas.Office_Management
{
    public partial class KajEntryFrm : Form
    {
        private bool forApprove;

        public KajEntryFrm()
        {
            InitializeComponent();
        }

        public KajEntryFrm(bool kajApprove)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.forApprove = kajApprove;
        }
    }
}
