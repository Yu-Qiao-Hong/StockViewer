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
        public StockSetting()
        {
            InitializeComponent();

            CreateInfo();
        }

        private void CreateInfo()
        {
            DBHelper.QueryMyStock();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int stockId = int.Parse(textBox1.Text);
            // TODO: vaild id

            bool result = DBHelper.InsertStockId(stockId);
            if (!result)
            {
                MessageBox.Show(DBHelper.GetLastError());
                return;
            }

            textBox1.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (int)e.KeyChar != (int)Keys.Back)
                e.Handled = true;
        }
    }
}
