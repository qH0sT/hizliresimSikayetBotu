using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hizliresimSikayet
{
    public partial class Form2 : Form
    {
        Form1 frm = null;
        public Form2(Form1 anaForm)
        {
            InitializeComponent();
            frm = anaForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
            {
                ListViewItem lvi = new ListViewItem(textBox1.Text);
                lvi.SubItems.Add(textBox2.Text);
                frm.listView1.Items.Add(lvi);
            }
        }
    }
}
