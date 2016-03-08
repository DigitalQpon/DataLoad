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
    public partial class BaseEditorF : Form
    {
        public BaseEditorF()
        {
            InitializeComponent();
        }

        public virtual void DoSave() { }
        public virtual void DoCancel(){ }
        public virtual void SetupUpdated() { }
        public virtual bool CanClose() { return true; }
        public virtual void CloseForm() { }

    }
}
