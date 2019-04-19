using System;
using System.Text;
using System.Windows.Controls;
using System.Collections.Generic;

namespace RiggVar.FR
{
    public enum SwitchField 
    {
        ProxyType,
        Host,
        Port,
        PortHTTP,
        RouterHost,
        UseRouterHost
    }

    public class TSwitchItem
    {
        public int ID;
        public string InfoName;
        public string Host;
        public int Port;
        public int PortHTTP;
        public string RouterHost;
        public bool UseRouterHost;
        public string DisplayName;

        public TSwitchItem()
        {
        }

        public string ComboEntry => ID.ToString() + "=" + DisplayName;

        public void InitFromCurrent()
        {
            TIniImage o = TMain.IniImage;
            Host = o.SwitchHost;
            Port = o.SwitchPort;
            PortHTTP = o.SwitchPortHTTP;
            RouterHost = o.RouterHost;
            UseRouterHost = o.UseRouterHost;
        }

        public void Show(StringBuilder sb)
        {
            string crlf = Environment.NewLine;
            sb.Append("SwitchHost=" + Host);
            sb.Append(crlf);
            sb.Append("SwitchPort=" + Port.ToString());
            sb.Append(crlf);
            sb.Append("SwitchPortHTTP=" + PortHTTP.ToString());
            sb.Append(crlf);
            sb.Append("RouterHost=" + RouterHost);
            sb.Append(crlf);
            sb.Append("UseRouterHost=" + Utils.BoolStr[UseRouterHost]);
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

    public class TSwitchItems
    {
        private List<TSwitchItem> FItems;
        public TSwitchItems()
        {
            FItems = new List<TSwitchItem>();
            Current = new TSwitchItem();
            Init();
        }
        public TSwitchItem Add()
        {        
            TSwitchItem result = new TSwitchItem();
            FItems.Add(result);
            result.ID = FItems.Count - 1;
            return result;
        }
        public int Count => FItems.Count;
        public TSwitchItem Current { get; }
        public TSwitchItem this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return FItems[index] as TSwitchItem;
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
        public TSwitchItem FindItem(string ComboEntry)
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

        public void Init()
        {
            TSwitchItem cr;

            //[SwitchInfo-gsup3-intern]
            //SwitchHost=gsup3
            //SwitchPort=4029
            //SwitchPortHTTP=8085
            //RouterHost=
            //UseRouterHost=0
            //Description=intranet gui switch app

            cr = Add();
            cr.InfoName = "gsup3-intern";
            cr.Host = "gsup3";
            cr.Port = 4029;
            cr.PortHTTP = 8085;
            cr.RouterHost = "";
            cr.UseRouterHost = false;
            cr.DisplayName = "intranet gui switch app";

            //[SwitchInfo-HE-extern]
            //SwitchHost=riggvar.de
            //SwitchPort=4029
            //SwitchPortHTTP=8085
            //RouterHost=rvinfo.dynalias.net
            //UseRouterHost=1
            //Description=extranet switch-service

            cr = Add();
            cr.InfoName = "HE-extern";
            cr.Host = "riggvar.de";
            cr.Port = 4029;
            cr.PortHTTP = 8085;
            cr.RouterHost = "rvinfo.dynalias.net";
            cr.UseRouterHost = true;
            cr.DisplayName = "extranet switch-service";

        }

        public void InitSwitchInfo(ComboBox Combo)
        {
            foreach (TSwitchItem cr in FItems)
            {
                Combo.Items.Add(cr.ComboEntry);
            }
        }

    }

}
