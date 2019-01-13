using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            string mode = comboBox1.SelectedItem.ToString();
            if (mode.Equals("4 x 4"))
            {
                ClientForm4x4 clientForm4X4 = new ClientForm4x4();
                this.Hide();
                clientForm4X4.Show();
            }
            else if (mode.Equals("9 x 9"))
            {

                ClientForm9x9 clientForm9x9 = new ClientForm9x9();
                this.Hide();
                clientForm9x9.Show();
            }

        }
    }
}
