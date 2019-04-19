using System;
using RiggVar.FR;
using System.Windows.Controls;

namespace FR62.Tabs
{

    public enum UrlScheme { Http, File, App };

    public struct TWorkspaceUrl
    {
        public string Value;
        public UrlScheme GetScheme()
        {
            if (IsHttpScheme())
            {
                return UrlScheme.Http;
            }

            if (IsAppScheme())
            {
                return UrlScheme.App;
            }

            return UrlScheme.File;
        }
        public bool IsAppScheme()
        {
            return Value.StartsWith("app");
        }
        public bool IsHttpScheme()
        {
            return Value.StartsWith("http");
        }
    }

    interface IWorkspaceList
    {
        void Init();
        void Load(ItemCollection Combo);
        string GetName(int i);
        string GetUrl(int i);
        bool IsWritable(int i);
    }

    public class TWorkspaceListBase : IWorkspaceList
    {
        //string u0 = "app://DocManager/EventMenu.xml";

        private const string u1 = "http://www.fleetrace.org/EventMenu.txt";
        private const string u2 = "http://data.riggvar.de/EventMenu.txt";
        private const string u3 = @"data.riggvar.de/xml=http://data.riggvar.de/EventMenu.xml";
        private const string u4 = @"www.riggvar.de/xml=http://www.riggvar.de/results/EventMenu.xml";
        private const string u5 = @"www.fleetrace.org/demo=http://www.fleetrace.org/DemoIndex.xml";
        private const string u6 = @"www.riggvar.de/html=http://www.riggvar.de/results/EventMenuHtml.xml";

        protected TStringList VL;
        protected void InitDefault()
        {
            bool WantXml = true; //CheckWin32Version(5,1);

            if (WantXml)
            {
                AddUrl(u3);
                AddUrl(u4);
                AddUrl(u5);
                AddUrl(u6);
            }
            else
            {
                AddUrl(u1);
                AddUrl(u2);
            }
        }

        protected virtual void AddUrl(string s)
        {
            int i;
            string u;

            i = s.IndexOf("=", StringComparison.OrdinalIgnoreCase);
            if (i > 0)
            {
                u = s.Substring(i+1).Trim(); // Trim(Copy(s, i+1, Length(s)))
            }
            else
            {
                u = s.Trim();
            }

            VL.Add(u);
        }

        public TWorkspaceListBase()
        {
            VL = new TStringList();
        }

        public virtual void Init()
        {
            if (VL.Count == 0)
            {
                InitDefault();
            }
        }

        public virtual void Load(ItemCollection Combo)
        {
            Combo.Clear();
            for (int i = 0; i < VL.Count; i++)
            {
                Combo.Add(VL[i]);
            }
        }

        public virtual string GetName(int i)
        {
            return "";
        }

        public virtual string GetUrl(int i)
        {
            if (i >= 0 && i < VL.Count)
            {
                return VL[i];
            }

            return "";
        }

        public virtual bool IsWritable(int i)
        {
            return false;
        }

    }
}

