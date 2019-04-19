using System;
using System.Text;
using System.Windows.Controls;
using System.Collections.Generic;

namespace RiggVar.FR
{
    public enum BridgeField 
    {
        ProxyType,
        Host,
        Port,
        PortHTTP,
        Url,
        HomePage,
        TaktIn,
        TaktOut
    }

    public class TBridgeItem
    {
        public int ID;
        public string InfoName;
        public int ProxyType;
        public string Host;
        public int Port;
        public int PortHTTP;
        public string Url;
        public string HomePage;
        public int TaktIn;
        public int TaktOut;
        public string DisplayName;

        public TBridgeItem()
        {
        }

        public static bool Enabled(BridgeProxyType t, BridgeField f)
        {
            switch (t)
            {
                case BridgeProxyType.Asmx:
                case BridgeProxyType.Php: 
                case BridgeProxyType.REST: 
                {
                    switch (f)
                    {
                        case BridgeField.Url: return true;
                        case BridgeField.HomePage: return true;
                        case BridgeField.TaktIn: return true;
                        case BridgeField.TaktOut: return true;
                    }
                    break;
                }    
                case BridgeProxyType.Client:
                case BridgeProxyType.Server:
                case BridgeProxyType.Synchron:
                case BridgeProxyType.Proxy:
                case BridgeProxyType.Combi:
                {
                    switch (f)
                    {
                        case BridgeField.Host: return true;
                        case BridgeField.Port: return true;
                        case BridgeField.PortHTTP: return true;
                        case BridgeField.Url: return true;
                    }
                    break;
                }
            }
            return false;
        }

        public string ComboEntry
        {
            get 
            {
                return ID.ToString() + "=" + DisplayName;
            }
        }

        public void InitFromCurrent()
        {
            TIniImage o = TMain.IniImage;

            BridgeProxyType pt;
            try
            {
                pt = (BridgeProxyType)Enum.Parse(typeof(BridgeProxyType), o.BridgeProxyID);
            }
            catch (Exception)
            {
                pt = BridgeProxyType.Mock;
            }

            ProxyType = (int)pt;
            Host = o.BridgeHost;
            Port = o.BridgePort;
            PortHTTP = o.BridgePortHTTP;
            Url = o.BridgeUrl;
            HomePage = o.BridgeHomePage;
            TaktIn = o.TaktIn;
            TaktOut = o.TaktOut;
        }

        public void Show(StringBuilder sb)
        {
            string crlf = Environment.NewLine;
            sb.Append("ProxyType=" + ProxyType);
            sb.Append(crlf);
            sb.Append("Host=" + Host);
            sb.Append(crlf);
            sb.Append("Port=" + Port);
            sb.Append(crlf);
            sb.Append("PortHTTP=" + PortHTTP);
            sb.Append(crlf);
            sb.Append("Url=" + Url);
            sb.Append(crlf);
            sb.Append("HomePage=" + HomePage);
            sb.Append(crlf);
            sb.Append("TaktIn=" + TaktIn);
            sb.Append(crlf);
            sb.Append("TaktOut=" + TaktOut);
            sb.Append(crlf);
            sb.Append("DisplayName=" + DisplayName);
            sb.Append(crlf);
        }
        public void Show(TextBox tb)
        {
            StringBuilder sb = new StringBuilder();
            Show(sb);
            tb.Text = sb.ToString();
        }

    }

    public class TBridgeItems
    {
        private List<TBridgeItem> FItems;

        public TBridgeItems()
        {
            FItems = new List<TBridgeItem>();
            Current = new TBridgeItem();
            Init();
        }

        public TBridgeItem Add()
        {        
            TBridgeItem result = new TBridgeItem();
            FItems.Add(result);
            result.ID = FItems.Count - 1;
            return result;
        }
        public int Count => FItems.Count;
        public TBridgeItem Current { get; }
        public TBridgeItem this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return FItems[index] as TBridgeItem;
                }
                return null;
            }
            set
            {
                if (index >= 0 && index < Count)
                {
                    FItems[index] = value;
                }
            }
        }

        public TBridgeItem FindItem(string ComboEntry)
        {
            char [] ca = new char[1] {'='};
            string [] sc = ComboEntry.Split(ca, 2);
            if (sc.Length == 2)
            {
                string s = sc[0];
                int i = Utils.StrToIntDef(s, 0);
                return this[i];
            }
            return null;
        }

        public BridgeProxyType FindProxyType(string ProxyTypeComboEntry)
        {
            char[] ca = new char[1] { '=' };
            string[] sc = ProxyTypeComboEntry.Split(ca, 2);
            if (sc.Length == 2)
            {
                string s = sc[0];
                int i = Utils.StrToIntDef(s, 0);
                return this.Parse(i);
            }
            return BridgeProxyType.Mock;
        }

        public void Init()
        {
            TBridgeItem cr;

            cr = Add();
            cr.InfoName = "gsup3-asmx";
            cr.ProxyType = 1;
            cr.Host = "gsup3";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://gsup3/FR88/";
            cr.HomePage = "FRBridge.aspx";
            cr.TaktIn = 2;
            cr.TaktOut = 2;
            cr.DisplayName = "gsup3 - asp.net 2.0";
        
            cr = Add();
            cr.InfoName = "riggvar.net-asmx";
            cr.ProxyType = 1;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://riggvar.net/cgi-bin/RiggVar15/";
            cr.HomePage = "FRBridge.aspx";
            cr.TaktIn = 30;
            cr.TaktOut = 30;
            cr.DisplayName = "riggvar.net - asp.net 2.0";

            cr = Add();
            cr.InfoName = "riggvar.de-asmx";
            cr.ProxyType = 1;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://riggvar.de/Sound/";
            cr.HomePage = "FRBridgePage.aspx";
            cr.TaktIn = 30;
            cr.TaktOut = 30;
            cr.DisplayName = "riggvar.de - asp.net 1.1";

            cr = Add();
            cr.InfoName = "gsup3-php";
            cr.ProxyType = 2;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://gsup3:3569/FR88/";
            cr.HomePage = "FR88_FRPage.php";
            cr.TaktIn = 10;
            cr.TaktOut = 10;
            cr.DisplayName = "gsup3 - D4PHP";

            cr = Add();
            cr.InfoName = "riggvar.net-php";
            cr.ProxyType = 2;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://riggvar.net/cgi-bin/FR88PHP/";
            cr.HomePage = "FR88_FRPage.php";
            cr.TaktIn = 60;
            cr.TaktOut = 60;
            cr.DisplayName = "riggvar.net - php";

            cr = Add();
            cr.InfoName = "riggvar.de-php";
            cr.ProxyType = 2;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://riggvar.de/projectindex/FR88PHP/";
            cr.HomePage = "FR88_FRPage.php";
            cr.TaktIn = 60;
            cr.TaktOut = 60;
            cr.DisplayName = "riggvar.de - php";

            cr = Add();
            cr.InfoName = "ClientBridge";
            cr.ProxyType = 3;
            cr.Url = "http://gsup3:8037";
            cr.Host = "riggvar.de";
            cr.Port = 4037;
            cr.PortHTTP = 8037;
            cr.Url = "";
            cr.HomePage = "";
            cr.TaktIn = 0;
            cr.TaktOut = 0;
            cr.DisplayName = "ClientBridge";

            cr = Add();
            cr.InfoName = "ServerBridge";
            cr.ProxyType = 4;
            cr.Host = "gsup3";
            cr.Port = 4030;
            cr.PortHTTP = 8087;
            cr.Url = "http://gsup3:8087";
            cr.HomePage = "";
            cr.TaktIn = 0;
            cr.TaktOut = 0;
            cr.DisplayName = "ServerBridge";

            cr = Add();
            cr.InfoName = "ProxyBridge";
            cr.ProxyType = 7;
            cr.Host = "gsup3";
            cr.Port = 4030;
            cr.PortHTTP = 8087;
            cr.Url = "http://gsup3:8087";
            cr.HomePage = "";
            cr.TaktIn = -1;
            cr.TaktOut = -1;
            cr.DisplayName = "ProxyBridge";

            cr = Add();
            cr.InfoName = "CombiBridge";
            cr.ProxyType = 8;
            cr.Host = "gsup3";
            cr.Port = 4030;
            cr.PortHTTP = 8087;
            cr.Url = "http://gsup3:8087";
            cr.HomePage = "";
            cr.TaktIn = 0;
            cr.TaktOut = 0;
            cr.DisplayName = "CombiBridge";    

            cr = Add();
            cr.InfoName = "gsVista-REST.net";
            cr.ProxyType = 6;
            cr.Host = "";
            cr.Port = 0;
            cr.PortHTTP = 0;
            cr.Url = "http://gsvista/FR88/";
            cr.HomePage = "FRBridge.aspx";
            cr.TaktIn = 5;
            cr.TaktOut = 5;
            cr.DisplayName = "gsVista - REST asp.net 3.5";

        }

        public void InitBridgeInfo(ComboBox Combo)
        {
            Combo.Items.Clear();
            foreach (TBridgeItem cr in FItems)
            {
                Combo.Items.Add(cr.ComboEntry);
            }
        }

        public void InitBridgeInfoFiltered(ComboBox Combo, BridgeProxyType pt)
        {
            Combo.Items.Clear();
            foreach (TBridgeItem cr in FItems)
            {
                if (cr.ProxyType == (int) pt)
                {
                    Combo.Items.Add(cr.ComboEntry);
                }
            }
        }

        public void InitProxyType(ComboBox Combo)
        {
            ItemCollection o = Combo.Items;
            o.Clear();
            o.Add("0=MockBridge");
            o.Add("1=ASP.NET Web Service");
            o.Add("2=PHP Web Service");
            o.Add("3=ClientBridge");
            o.Add("4=ServerBridge");
            o.Add("5=SynchronBridge");
            o.Add("6=RESTBridge");
            o.Add("7=ProxyBridge");
            o.Add("8=CombiBridge");
        }

        public BridgeProxyType Parse(int i)
        {
            switch (i)
            {
                case 0: return BridgeProxyType.Mock;
                case 1: return BridgeProxyType.Asmx;
                case 2: return BridgeProxyType.Php;
                case 3: return BridgeProxyType.Client;
                case 4: return BridgeProxyType.Server;
                case 5: return BridgeProxyType.Synchron;
                case 6: return BridgeProxyType.REST;
                case 7: return BridgeProxyType.Proxy;
                case 8: return BridgeProxyType.Combi;
                default: return BridgeProxyType.Mock;
            }
 
        }

    }

}
