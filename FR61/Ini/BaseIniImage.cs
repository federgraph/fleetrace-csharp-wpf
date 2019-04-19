using System;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace RiggVar.FR
{
    public class TBaseIniImage
    {
        [XmlIgnore]
        public IIniLoader IniLoader = new IniLoaderMock();

        [XmlIgnore]
        public IniModifier Modifier = new IniModifier();

        [XmlIgnore]
        public IniDB DB = new IniDB();

        public TFolderInfo FolderInfo
        {
            get => TMain.FolderInfo;
            set => TMain.FolderInfo = value;
        }

        public TBaseIniImage()
        {
            IniLoader = new IniLoaderMock();
            if (FolderInfo == null)
            {
                FolderInfo = new TFolderInfo();
            }

            Modifier = new IniModifier();
            DB = new IniDB();
        }

        //--not persisted
        public int EventType = 0; //initialized later using app-specific const DefaultEventType

        //--Options
        public bool SafeStartupMode = true;
        public bool AutoLoadAdminConfig = false;

        public bool WantSockets = false;
        public bool SearchForUsablePorts = false;
        public bool IsMaster = false;

        public bool WantLocalAccess = true;
        public bool AutoSave = false;
        public bool NoAutoSave = true;

        public bool CopyRankEnabled = false;
        public bool LogProxyXML = false;

        //--Connections
        public bool WantAdapter = false;
        public bool AutoConnect = true;
        public string Host = "localhost";
        public int PortIn = 3027;
        public int PortOut = 3028;

        public string CalcHost = "gsup3";
        public int CalcPort = 3037;

        public bool FeedbackEnabled = false;
        public string FeedbackHost;
        public int FeedbackPort;

        public int WebServerPort = 8099;
        public string HomeUrl = "about:blank";
        public string WebApplicationUrl = "http://thinkpad/FR84";
        public string BrowserHomePage = "http://federgraph.de";
        public string AppTitle = "AppTitle";

        public string JSDataDir = "not specified";

        //--Switch
        public string SwitchHost = "gsamd"; 
        public int SwitchPort = 4029;
        public int SwitchPortHTTP = 8085;
        public string RouterHost = "routerip";
        public bool UseRouterHost = false;
        public bool UseAddress;
        public int MaxConnections = 32;

        //--Bridge
        public BridgeProxyType BridgeProxy = BridgeProxyType.Client;
        public string BridgeHost = "192.168.0.10";
        public int BridgePort = 4530;
        public int BridgePortHTTP = 8087;
        public string BridgeUrl = "about:blank";
        public string BridgeHomePage = "http://gsmac";
        public int TaktIn = 3;
        public int TaktOut = 3;

        //--Output
        public string OutputHost = "gsup3";
        public int OutputPort = 3428;

        //--Provider
        public string DBInterface = "TXT";
        public int ScoringProvider = 2;
        public int BridgeProvider = 2;

        
        [XmlIgnoreAttribute]
        public string BridgeProxyID
        {
            get => BridgeProxy.ToString();
            set
            {
                try
                {
                    BridgeProxy = (BridgeProxyType)Enum.Parse(typeof(BridgeProxyType), value);
                }
                catch
                {
                }
            }
        }

        internal virtual void ReadFromNameValueCollection(NameValueCollection c)
        {
        }
        internal virtual void WriteToNameValueCollection(NameValueCollection c)
        {
        }

    }

}
