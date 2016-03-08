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
    public partial class LoginF : Form
    {
        public LoginF()
        {
            InitializeComponent();
        }
        
        DataTable _userList;
        public DataTable UserList
        {
            set
            {
                _userList = value;
                if (_userList != null)
                {
                    loginIdEdt.DataSource = _userList;
                    loginIdEdt.DisplayMember = "userName";
                    loginIdEdt.ValueMember = "userid";
                }
            }
        }

        private void loginCmd_Click(object sender, EventArgs e)
        {
            if (_userList != null) 
            {
                foreach (DataRow _row in _userList.Rows)
                {
                    int _userId = _row.Field<int>("userid");
                    String _password = _row.Field<String>("password");
                    if (_userId == Convert.ToInt32(loginIdEdt.SelectedValue))
                    {
                        if (passwordEdt.Text == _password)
                        {
                            DialogResult = DialogResult.OK;
                            return;
                        }
                    }
                }
            }
            if (loginIdEdt.SelectedValue.ToString() == "0")
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cancelCmd_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
