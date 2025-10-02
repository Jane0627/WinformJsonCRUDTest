using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTest
{
    public partial class UserEditorForm : Form
    {
        public User User { get; set; }
        public UserEditorForm(User user)
        {
            InitializeComponent();
            this.User = user;
            txtName.Text = user.Name;
            txtEmail.Text = user.Email;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            User.Name = txtName.Text;
            User.Email = txtEmail.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
