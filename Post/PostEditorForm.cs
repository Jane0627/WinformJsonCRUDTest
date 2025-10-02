using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTestv3
{
    public partial class PostEditorForm : Form
    {
        public Post currentPost { get; set; }
        public PostEditorForm(Post post)
        {
            InitializeComponent();
            this.currentPost = post;
            txtTitle.Text = post.title;
            txtBody.Text = post.body;
        }

        private void PostEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            currentPost.title = txtTitle.Text;
            currentPost.body = txtBody.Text;
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
