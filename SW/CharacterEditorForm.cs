using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsTestv4
{
    public partial class CharacterEditorForm : Form
    {

        public Character character { get; set; }
        public CharacterEditorForm(Character character)
        {
            InitializeComponent();
            this.character = character;
            txtName.Text = character.name;
            txtHeight.Text = character.height;
            txtMass.Text = character.mass;
            txtHair.Text = character.hair_color;
            txtSkin.Text = character.skin_color;
            txtEye.Text = character.eye_color;
            txtYear.Text = character.birth_year;
            txtGender.Text = character.gender;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            character.name = txtName.Text;
            character.height = txtHeight.Text;
            character.mass = txtMass.Text;
            character.hair_color = txtHair.Text;
            character.skin_color = txtSkin.Text;
            character.eye_color = txtEye.Text;
            character.birth_year = txtYear.Text;
            character.gender = txtGender.Text;

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
