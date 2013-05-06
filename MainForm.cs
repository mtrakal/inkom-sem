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
        Logger logger = Logger.GetInstance();
        Scanner scanner;
        Parser parser;
        Compiler compiler;

        public MainForm()
        {
            InitializeComponent();
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
            saveFileDialog1.FileName = null;
            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog1.Filter = "eMTe files (*.mt)|*.mt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, textBoxEditor.Text);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBoxEditor.Text = "# eMTe\r\n" + textBoxEditor.Text;
            textBoxEditor.SelectionStart = textBoxEditor.TextLength;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = "# ";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = "\r\nvypis PROM;";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = "\r\npromenna PROM=0;\r\nnactiInt PROM;";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = "\r\npromenna PROM=\"TEXT\";";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = "\r\npromenna PROM=0;";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBoxEditor.SelectionLength = 0;
            textBoxEditor.SelectedText = @"
promenna pocet=5;
promenna i = 0;
pro i=0 do pocet delej
 # tělo cyklu
konec;";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            textBoxScanner.Text = "";
            using (TextBoxStreamWriter tbsw = new TextBoxStreamWriter(textBoxScanner))
            {
                Console.SetOut(tbsw);
                using (TextReader reader = new StringReader(textBoxEditor.Text))
                {
                    try
                    {
                        scanner = new Scanner(reader);
                    }
                    catch (ScannerException ex)
                    {
                        scanner = null;
                        MessageBox.Show(ex.Message, "Chyba scannerovani", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// http://saezndaree.wordpress.com/2009/03/29/how-to-redirect-the-consoles-output-to-a-textbox-in-c/
        /// </summary>
        public class TextBoxStreamWriter : TextWriter, IDisposable
        {
            TextBox output = null;
            public TextBoxStreamWriter(TextBox textbox)
            {
                output = textbox;

            }
            public override void Write(char value)
            {
                base.Write(value);
                if (output != null)
                {
                    output.AppendText(value.ToString());
                }
            }

            public override Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }

            protected override void Dispose(bool disposing)
            {
                output = null;
                StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                base.Dispose(disposing);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            textBoxParser.Text = "";
            using (TextBoxStreamWriter tbsw = new TextBoxStreamWriter(textBoxParser))
            {
                Console.SetOut(tbsw);
                if (scanner == null)
                {
                    logger.Log("Nebylo provedeno skenování", Logger.Type.ERROR);
                }
                else
                {
                    try
                    {
                        parser = new Parser(scanner.Tokens);
                    }
                    catch (ParserException ex)
                    {
                        MessageBox.Show(ex.Message, "Chyba v parsovani", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            textBoxCompiler.Text = "";
            using (TextBoxStreamWriter tbsw = new TextBoxStreamWriter(textBoxCompiler))
            {
                Console.SetOut(tbsw);
                if (parser == null)
                {
                    logger.Log("Nebylo provedeno parsování", Logger.Type.ERROR);
                }
                else
                {
                    try
                    {
                        saveFileDialog1.FileName = null;
                        saveFileDialog1.Filter = "Executable file (*.exe)|*.exe";
                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            compiler = new Compiler(parser.Statement, saveFileDialog1.FileName);
                        }
                    } catch (CompilerException ex) {
                        MessageBox.Show(ex.Message, "Chyba kompilce", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

}
