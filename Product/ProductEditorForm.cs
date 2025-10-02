using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTestv2
{
    public partial class ProductEditorForm : Form
    {
        public Product currentProduct { get; private set; }
        public ProductEditorForm(Product product)
        {
            InitializeComponent();
            this.currentProduct = product;
            txtTitle.Text = product.title;
            txtCategory.Text = product.category;
            numPrice.Value = (decimal)product.price;
            numStock.Value = product.stock;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            currentProduct.title = txtTitle.Text;
            currentProduct.category = txtCategory.Text;
            currentProduct.brand = txtBrand.Text;
            currentProduct.price = (float)numPrice.Value;
            currentProduct.stock = (int)numStock.Value;
            

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult= DialogResult.Cancel;
            this.Close();
        }
    }
}
