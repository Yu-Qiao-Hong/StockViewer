using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockViewer
{
    public partial class MainForm : Form
    {
        List<int> stockIdList = null;
        WebClient web = new WebClient();

        public MainForm()
        {
            InitializeComponent();

            Config.Parse();

            DBHelper.OpenDB();
            stockIdList = DBHelper.QueryMyStock();
            //stockIdList = new List<int>();

            //stockIdList.Add(2317);
            //stockIdList.Add(1101);
            //stockIdList.Add(2308);

            for (int i = 0; i < stockIdList.Count; i++)
            {
                listView.Items.Add(new ListViewItem());
            }

            SetProxy(web);
        }

        private void Start()
        {
            Debug.WriteLine("Start start");
            for (int i = 0; i < stockIdList.Count; i++)
            {
                GetStockInformation(i, stockIdList[i].ToString());
            }
            Thread.Sleep(5);
            Debug.WriteLine("Start finish");
        }

        private bool GetStockInformation(int rowIdx, string stockId)
        {
            string url = @"https://tw.stock.yahoo.com/q/q?s=" + stockId;

            string str = "";
            try
            {
                str = web.DownloadString(url);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception : " + ex.Message);
                return true;
            }

            string startStr = "<td align=\"center\" bgcolor=\"#FFFfff\" nowrap>";
            //string endStr = "<td align=center width=137 class=\"tt\">";
            string endStr = "<tr bgcolor=\"#fdedef\">";

            int startPos = str.IndexOf(startStr);
            int endPos = str.IndexOf(endStr);

            if (startPos == -1 || endPos == -1)
            {
                InsertData(rowIdx, new Stock());
                return false;
            }

            string realStr = str.Substring(startPos, endPos - startPos);

            Stock stock = ParseStockData(stockId, realStr);
            InsertData(rowIdx, stock);

            return true;
        }

        private void InsertData(int rowIdx, Stock stock)
        {
            if (string.IsNullOrEmpty(stock.UpDown))
                stock.UpDown = "-";

            listView.Items.RemoveAt(rowIdx);
            listView.Items.Insert(rowIdx, new ListViewItem(new string[] { stock.Name, stock.DealPrice, stock.UpDown }));
        }

        private Stock ParseStockData(string stockId, string realStr)
        {
            Stock stock = new Stock();
            stock.ID = stockId;

            Match match = Regex.Match(realStr, "<td align=\"center\" bgcolor=\"#FFFfff\" nowrap>(<b>)?([0-9.,:]+)(</b>)?</td>");
            stock.Time = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.DealPrice = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.BuyPrice = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.SellPrice = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.TotalCount = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.Yesterday = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.Opening = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.High = match.Groups[2].ToString();

            match = match.NextMatch();
            stock.Low = match.Groups[2].ToString();

            //<td align="center" bgcolor="#FFFfff" nowrap><font color=#ff0000>△1.1
            //<td align="center" bgcolor="#FFFfff" nowrap><font color=#009900>▽0.70
            match = Regex.Match(realStr, "<td align=\"center\" bgcolor=\"#FFFfff\" nowrap><font color=#(ff0000)?(009900)?>(.+)");
            stock.UpDown = match.Groups[3].ToString();

            //<input type=hidden name="stkname" value="鴻海">
            match = Regex.Match(realStr, "<input type=hidden name=\"stkname\" value=\"(.+)\">");
            stock.Name = match.Groups[1].ToString();

            return stock;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Opacity = (double)trackBar1.Value / trackBar1.Maximum;
        }


        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stopToolStripMenuItem.Text == "Stop")
            {
                Debug.WriteLine("Run");
                stopToolStripMenuItem.Text = "Run";
                timer1.Start();
            }
            else
            {
                Debug.WriteLine("Stop");
                stopToolStripMenuItem.Text = "Stop";
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Start();
        }

        private void proxySettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProxySetting proxyForm = new ProxySetting();
            if (proxyForm.ShowDialog() == DialogResult.OK)
            {
                SetProxy(web);
            }
        }

        private void SetProxy(WebClient web)
         {
             if (Config.Proxy.Enable)
             {
                 WebProxy proxyObj = new WebProxy(Config.Proxy.Host, Config.Proxy.Port);
                 proxyObj.UseDefaultCredentials = true;
                 web.Proxy = proxyObj;
             }
             else
             {
                 WebProxy proxyObj = new WebProxy();
                 proxyObj.UseDefaultCredentials = false;
                 web.Proxy = proxyObj;
             }
         }

        private void editStockFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StockSetting ss = new StockSetting();
            ss.ShowDialog();
        }
    }

    class Stock
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string DealPrice { get; set; }
        public string BuyPrice { get; set; }
        public string SellPrice { get; set; }
        public string UpDown { get; set; }
        public string TotalCount { get; set; }
        public string Yesterday { get; set; }
        public string Opening { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
    }
}
