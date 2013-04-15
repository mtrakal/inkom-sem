using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void oAplikaciToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void oAplikaciToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();


        }

        private void nápovědaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void konecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void načtiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = Directory.GetCurrentDirectory();
            openFileDialog1.InitialDirectory = dir;
            openFileDialog1.Filter = "eMTe files (*.mt)|*.mt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string text = System.IO.File.ReadAllText(openFileDialog1.FileName);
                textBoxEditor.Text = text;
                tabControl1.SelectedIndex = 0;
            }
        }

        private void uložToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dir = Directory.GetCurrentDirectory();
            saveFileDialog1.InitialDirectory = dir;
            saveFileDialog1.Filter = "eMTe files (*.mt)|*.mt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, textBoxEditor.Text);
            }
        }
    }
}
