using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcParser;
using LoghRepacker;

namespace PackRePackingForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string archiveFileName = textBox1.Text;
            string extractDirectory = textBox2.Text;

            if(archiveFileName == "" || extractDirectory == "")
            {
                MessageBox.Show("empty directory");
            }
            else
            {
                ArchiveParser a = new ArchiveParser();
                a.setArchiveFileName(archiveFileName);
                a.setExtractDirectory(extractDirectory);
                a.init();
                a.run();
                MessageBox.Show("OK!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabPage1.Text = "EXPORT ARC FILE";
            tabPage2.Text = "ARCHIVING";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ArcPacker a = new ArcPacker();
            string rootDirectory = textBox4.Text;
            string exportFileName = "";
            if(exportFileName == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    exportFileName = saveFileDialog.FileName;
            }


            if (rootDirectory != "" && exportFileName != "")
            {
                a.setRootDirecotry(rootDirectory);
                a.setExportFileName(exportFileName);
                a.init();
                a.packFiles();
                MessageBox.Show("OK!");

            }
            else MessageBox.Show("directory or export file name is empty");
        }

      

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
