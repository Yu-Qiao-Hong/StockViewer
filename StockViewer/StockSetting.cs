using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockViewer
{
    public partial class StockSetting : Form
    {
        SqliteHelper sql = null;

        public StockSetting(SqliteHelper sql)
        {
            InitializeComponent();

            this.sql = sql;
            CreateInfo();
        }

        private void CreateInfo()
        {
            listView1.Items.Clear();
            List<int> stockIdList = sql.QueryMyStock();
            foreach (var id in stockIdList)
            {
                listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), "" }));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int stockId = int.Parse(textBox1.Text);
            // TODO: vaild id

            bool result = sql.InsertStockId(stockId);
            if (!result)
            {
                MessageBox.Show(SqliteHelper.GetLastError());
                return;
            }

            textBox1.Text = "";
            CreateInfo();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || (int)e.KeyChar == (int)Keys.Back)
            {
            }
            else if ((int)e.KeyChar == (int)Keys.Enter)
            {
                button1.PerformClick();
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
