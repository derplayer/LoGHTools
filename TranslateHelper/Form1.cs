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
                string result = op.FileName;
                TranslateableREader r = new TranslateableREader();
                int x = 0;
                foreach(List<byte> item in r.read(result))
                {
                    dataGridView1.Rows.Add(x++, Encoding.Default.GetString(item.ToArray()));
                }
            }


        }

    }
}
