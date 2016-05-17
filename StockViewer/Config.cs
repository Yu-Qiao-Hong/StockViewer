using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockViewer
{
    public class ProxyInfo
    {
        public bool Enable { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    static public class Config
    {
        static public ProxyInfo Proxy = new ProxyInfo();

        static public void Parse()
        {
            ParseProxy();
            //TODO: other config setting
        }

        static private void ParseProxy()
        {
            //Proxy.Enable = true;
            //Proxy.Host = "twty3tmg01.delta.corp";
            //Proxy.Port = 8080;
        }
    }
}
