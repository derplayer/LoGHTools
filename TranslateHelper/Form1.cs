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
                FileStream f = File.Open(result, FileMode.Open, FileAccess.Read);
                int fsize = (int)f.Length;
                byte[] content = new byte[fsize];
                f.Read(content, 0, fsize);
                f.Close();

                int labelOffset = 0;
                int blockSize = 4096;
                int labelCount = fsize / blockSize;

                //every string has 4096 byte space so i can pre calculate count of total string
                //and iterate it
                for (int i = 0; i < labelCount; i++)
                {
                    //i should find the place of string in block so i check until i hit
                    //something different than 0x00
                        int limit = blockSize * (i + 1);
                        labelOffset = blockSize * i;
                
                        while (content[labelOffset] == 0x00)
                        {
                            labelOffset++;
                        }
                        //if i reach to end or exceed it that means there is no string in block
                        if (labelOffset >= limit)
                        { 
                            dataGridView1.Rows.Add(0, "might be important for data health");
                            continue;
                        }
                    //once i found the string i can extract the string for translate
                        

                        int sort = content[labelOffset + 1] >> 8 | content[labelOffset] << 8;
                        labelOffset += 2;
                        List<byte> label = new List<byte>();
                        while (content[labelOffset] != 0x00)
                        {
                            label.Add(content[labelOffset]);
                            labelOffset++;
                        }
                        dataGridView1.Rows.Add(sort,Encoding.Default.GetString(label.ToArray()));
                       

                }

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
