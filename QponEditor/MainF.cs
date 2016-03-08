using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QponEditor
{
    public partial class MainF : Form
    {
        public MainF()
        {
            InitializeComponent();
        }

        ProsperentEdtF _pEdit;
        private void MainF_Load(object sender, EventArgs e)
        {
            _pEdit = new ProsperentEdtF();
            _pEdit.MdiParent = this;
            _pEdit.Show();
        }
    }
}
