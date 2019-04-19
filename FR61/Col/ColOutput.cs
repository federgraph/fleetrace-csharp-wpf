using System;
using System.Text;
using System.Windows.Controls;
using System.Collections.Generic;

namespace RiggVar.FR
{
    public enum OutputField 
    {
        Host,
        Port
    }

    public class TOutputItem
    {
        public int ID;
        public string InfoName;
        public string Host;
        public int Port;
        public string DisplayName;

        public TOutputItem()
        {
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
            Host = o.OutputHost;
            Port = o.OutputPort;
        }

        public void Show(StringBuilder sb)
        {
            string crlf = Environment.NewLine;
            sb.Append("OutputHost=" + Host);
            sb.Append(crlf);
            sb.Append("OutputPort=" + Port.ToString());
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

    public class TOutputItems
    {
        private List<TOutputItem> FItems;
        public TOutputItems()
        {
            FItems = new List<TOutputItem>();
            Current = new TOutputItem();
            Init();
        }
        public TOutputItem Add()
        {        
            TOutputItem result = new TOutputItem();
            FItems.Add(result);
            result.ID = FItems.Count - 1;
            return result;
        }

        public int Count => FItems.Count;

        public TOutputItem Current { get; }
        public TOutputItem this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return FItems[index] as TOutputItem;
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
        public TOutputItem FindItem(string ComboEntry)
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
            TOutputItem cr;

            cr = Add();
            cr.InfoName = "gsup3-Delphi";
            cr.Host = "gsup3";
            cr.Port = 3428;
            cr.DisplayName = "gsup3 delphi";

            cr = Add();
            cr.InfoName = "gsup3-VS2005";
            cr.Host = "gsup3";
            cr.Port = 6228;
            cr.DisplayName = "gsup3 VS2005";

            cr = Add();
            cr.InfoName = "gsup3-Java";
            cr.Host = "gsup3";
            cr.Port = 3028;
            cr.DisplayName = "gsup3 Java";
        }

        public void InitOutputInfo(ComboBox Combo)
        {
            foreach (TOutputItem cr in FItems) 
            {
                Combo.Items.Add(cr.ComboEntry);
            }
        }

    }

}
