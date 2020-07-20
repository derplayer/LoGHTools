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
                foreach (Translateable item in r.read(content))
                {
                    dataGridView1.Rows.Add(x++, Encoding.Default.GetString(item.baseString.ToArray()), Encoding.Default.GetString(item.baseString.ToArray()));
                }


            }


        }

        private void button2_Click(object sender, EventArgs e)
        {

            SaveFileDialog s = new SaveFileDialog();

            if(s.ShowDialog() == DialogResult.OK)
            {
                FileStream f = File.Open(s.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                TranslateableREader r = new TranslateableREader();
                List<byte> data = new List<byte>();

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    byte[] targetString = Encoding.Default.GetBytes(item.Cells["target"].Value.ToString());
                    string sort = item.Cells["sort"].Value.ToString();
                    int sortAsInt = Convert.ToInt32(sort);
                    int offset = sortAsInt * r.getBlockSize();


                    List<byte> buffer = new List<byte>();

                    buffer.Add((byte)(sortAsInt & 0XFF));
                    buffer.Add((byte)(sortAsInt >> 8));

                    foreach (byte x in targetString)
                        buffer.Add(x);

                    //+2 is comes from 16bit sort which is determines the sort of the string and it might important for internal
                    //game structures
                    for (int i = targetString.Length + 1; i <= r.getBlockSize(); i++)
                        buffer.Add(0);


                    foreach (byte x in buffer)
                        f.WriteByte(x);

                }

                f.Close();
            }


        }
    }
}
