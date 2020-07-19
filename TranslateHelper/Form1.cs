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

namespace TranslateHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            if(op.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                int x = 0;
                FileStream f = File.Open(op.FileName, FileMode.Open, FileAccess.Read);
                int fsize = (int)f.Length;
                byte[] content = new byte[fsize];
                f.Read(content, 0, fsize);
                f.Close();

                TranslateableREader r = new TranslateableREader();
                foreach (List<byte> item in r.read(content))
                {
                    dataGridView1.Rows.Add(x++, Encoding.Default.GetString(item.ToArray()));
                }
            }


        }

    }
}
