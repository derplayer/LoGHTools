using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslateHelper
{
    class XMLExportImport
    {


        public void export(string fileName,DataGridView dataGridView1)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            dt.Columns.Add("sort");
            dt.Columns.Add("baseString");
            dt.Columns.Add("target");
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                dt.Rows.Add(item.Cells[0].Value, item.Cells[1].Value, item.Cells[2].Value);
            }

            ds.Tables.Add(dt);
            ds.WriteXml(fileName, System.Data.XmlWriteMode.IgnoreSchema);
        }

        public void import (string fileName, DataGridView dataGridView1)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(fileName);
            dataGridView1.DataSource = dataSet.Tables[0];
        }

    }
}
