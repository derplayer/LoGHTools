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
using System.Net;
using System.Web;
using System.Threading;

namespace TranslateHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        XMLExportImport xmlExportImport = new XMLExportImport();

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //load translateable game file
        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            if(op.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                int x = 0;
                FileStream f = File.Open(op.FileName, FileMode.Open, FileAccess.Read);
                int fsize = (int)f.Length;
                byte[] content = new byte[fsize];
                f.Read(content, 0, fsize);
                f.Close();


                TranslateableREader r = new TranslateableREader();
                dataGridView1.DataSource = new BindingList<Translateable>(r.read(content));
                /*
                foreach (Translateable item in r.read(content))
                {
                    string baseString = Encoding.Default.GetString(item.baseString.ToArray());
                    dataGridView1.Rows.Add(item.sort, baseString, baseString);
                }
                */


            }


        }

       

        //export translated for game
        private void button2_Click(object sender, EventArgs e)
        {

            SaveFileDialog s = new SaveFileDialog();
            if(s.ShowDialog() == DialogResult.OK)
            {
                FileStream f = File.Open(s.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                TranslateableREader r = new TranslateableREader();
                List<Translateable> data = new List<Translateable>();

                //generate translateables from datagrid
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    Translateable newT = new Translateable();
                    newT.targetString = Convert.ToString(item.Cells[2].Value);
                    newT.sort = Convert.ToInt32(item.Cells[0].Value.ToString());
                    newT.baseString = Convert.ToString(item.Cells[1].Value);
                    data.Add(newT);
                }

                //convert translateables to bytes which can be write to file directly
                List<byte> buffer = r.translateableToByte(data);

                foreach (byte x in buffer)
                    f.WriteByte(x);

                f.Close();
            }


        }


        //serialize button
        private void button3_Click(object sender, EventArgs e)
        {

            SaveFileDialog s = new SaveFileDialog();
            s.DefaultExt = "xml";
            s.AddExtension = true;
            if(s.ShowDialog() == DialogResult.OK)
            {
                xmlExportImport.export(s.FileName,dataGridView1);
            }

        }


        //load serialized xml to datagridview
        private void button4_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.DefaultExt = "xml";
            if(op.ShowDialog() == DialogResult.OK)
            {
                xmlExportImport.import(op.FileName,dataGridView1);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                string target = Convert.ToString(item.Cells[1].Value);
                if(target!=null)
                {
                    item.Cells[2].Value = hackyTranslate(target);
                    dataGridView1.Update();
                    dataGridView1.CurrentCell = item.Cells[0];
                }
            }
        }


        public string hackyTranslate(string word)
        {
            var toLanguage = "en";
            var fromLanguage = "ja";
            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={word}";
            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };

            try
            {
                Thread.Sleep(6000);
                var result = webClient.DownloadString(url);
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                return result;
            }
            catch
            {

                MessageBox.Show("got you 429, will be sleep 5 sec ");
                return hackyTranslate(word);
            }
        }


        //fill target transation with translate file which translations should be separated by lines
        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();

            if(op.ShowDialog() == DialogResult.OK)
            {
                string fileName = op.FileName;
                StreamReader file = new StreamReader(fileName);
                string line;
                int counter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    dataGridView1.Rows[counter].Cells[2].Value = line;
                    counter++;    
                }

            }
        }
    }

}
