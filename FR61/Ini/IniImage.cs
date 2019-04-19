using System;
using System.Collections.Specialized;
using System.Text;

namespace RiggVar.FR
{
    public class TIniImage : TBaseIniImage
    {
        public const bool UseResource = false;
        public const bool UseIni = true;
        public const string DefaultEventName= "FR";
        public const int DefaultEventType = LookupKatID.FR; //TypFREvent;
        public const int DefaultScoringProviderID = 2; //see const definitions in CalcEV

        public TIniImage()
        {
            TMain.IniImage = this;

            EventType = DefaultEventType;

            LoadDefaults();

            if (FolderInfo == null)
            {
                FolderInfo = new TFolderInfo();
            }

            Modifier = new IniModifier();
            DB = new DBConfig();

            IniLoader = new IniLoaderAppSettings();
            LoadConfiguration();

            //IniLoaderNameValue:
            //AppName is WebDev.WebServer when started via FR64
            //so there is no file <riggvar workspace>\Settings VS2003\WebDev.WebServer.xml ?
            //it is not written because SaveConfiguration is not called from Web-Application
            if (TMain.IsWinGUI)
            {
                IniLoader = new IniLoaderNameValue();
                LoadConfiguration();
            }
        }

        private void LoadDefaults()
        {
            WantAdapter = true;
            DBInterface = "WEB";
        }

        public void LoadConfiguration() 
        {
            try
            {    
                if (IniLoader != null)
                {
                    IniLoader.LoadSettings(this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }                
        }
        public void SaveConfiguration()
        {
            try
            {                
                if (IniLoader != null)
                {
                    IniLoader.SaveSettings(this);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private bool ReadBool(string val, bool def)
        {
            if (val == null)
            {
                return def;
            }
            else
            {
                return Utils.IsTrue(val);
            }
        }

        internal override void ReadFromNameValueCollection(NameValueCollection c)
        {
            string s;

            //--Options

            SafeStartupMode = ReadBool(c["SafeStartupMode"], SafeStartupMode);
            AutoLoadAdminConfig = ReadBool(c["AutoLoadAdminConfig"], AutoLoadAdminConfig);

            WantSockets = ReadBool(c["WantSockets"], WantSockets);
            SearchForUsablePorts = ReadBool(c["SearchForUsablePorts"], SearchForUsablePorts);

            s = c["IsMaster"];
            IsMaster = ReadBool(s, IsMaster);

            AutoSave = ReadBool(c["AutoSave"], AutoSave);
            NoAutoSave = ReadBool(c["NoAutoSave"], NoAutoSave);

            WantLocalAccess = ReadBool(c["WantLocalAccess"], WantLocalAccess);

            CopyRankEnabled = ReadBool(c["CopyRankEnabled"], CopyRankEnabled);
            LogProxyXML = ReadBool(c["LogProxyXML"], LogProxyXML);

            //--Connections

            s = c["WantAdapter"];
            WantAdapter = ReadBool(s, WantAdapter);

            s = c["AutoConnect"];
            AutoConnect = ReadBool(s, AutoConnect);

            s = c["Host"];
            if (s != null && s != "")
            {
                Host = s;
            }

            s = c["PortIn"];
            PortIn = Utils.StrToIntDef(s, PortIn);

            s = c["PortOut"];
            PortOut = Utils.StrToIntDef(s, PortOut);

            s = c["CalcHost"];
            if (s != null && s != "")
            {
                CalcHost = s;
            }

            s = c["CalcPort"];
            CalcPort = Utils.StrToIntDef(s, CalcPort);

            s = c["FeedbackEnabled"];
            FeedbackEnabled = ReadBool(s, FeedbackEnabled);

            s = c["FeedbackHost"];
            if (s != null && s != "")
            {
                FeedbackHost = s;
            }

            s = c["FeedbackPort"];
            CalcPort = Utils.StrToIntDef(s, FeedbackPort);

            s = c["WebServerPort"];
            WebServerPort = Utils.StrToIntDef(s, WebServerPort);

            s = c["HomeUrl"];
            if (s != null && s != "")
            {
                HomeUrl = s;
            }

            s = c["BrowserHomePage"];
            if (s != null && s != "")
            {
                BrowserHomePage = s;
            }

            s = c["WebApplicationUrl"];
            if (s != null && s != "")
            {
                WebApplicationUrl = s;
            }

            s = c["AppTitle"];
            if (s != null && s != "")
            {
                AppTitle = s;
            }

            s = c["JSDataDir"];
            if (s != null && s != "")
            {
                JSDataDir = s;
            }

            //--Switch

            s = c["SwitchHost"];
            if (s != null && s != "")
            {
                SwitchHost = s;
            }

            s = c["SwitchPort"];
            SwitchPort = Utils.StrToIntDef(s, SwitchPort);

            s = c["SwitchPortHTTP"];
            SwitchPortHTTP = Utils.StrToIntDef(s, SwitchPortHTTP);

            s = c["RouterHost"];
            if (s != null && s != "")
            {
                RouterHost = s;
            }

            UseRouterHost = ReadBool(c["UseRouterHost"], UseRouterHost);
            UseAddress = ReadBool(c["UseAddress"], UseAddress);

            s = c["MaxConnections"];
            MaxConnections = Utils.StrToIntDef(s, MaxConnections);

            //--Bridge

            s = c["BridgeProxy"];
            if (s != null && s != "")
            {
                BridgeProxyID = s;
            }

            s = c["BridgeHost"];
            if (s != null && s != "")
            {
                BridgeHost = s;
            }

            s = c["BridgePort"];
            BridgePort = Utils.StrToIntDef(s, BridgePort);

            s = c["BridgePortHTTP"];
            BridgePortHTTP = Utils.StrToIntDef(s, BridgePortHTTP);

            s = c["BridgeUrl"];
            if (s != null && s != "")
            {
                BridgeUrl = s;
            }

            s = c["BridgeHomePage"];
            if (s != null && s != "")
            {
                BridgeHomePage = s;
            }

            s = c["TaktIn"];
            TaktIn = Utils.StrToIntDef(s, TaktIn);

            s = c["TaktOut"];
            TaktOut = Utils.StrToIntDef(s, TaktOut);

            //--Output

            s = c["OutputHost"];
            if (s != null && s != "")
            {
                OutputHost = s;
            }

            s = c["OutputPort"];
            OutputPort = Utils.StrToIntDef(s, OutputPort);


            //--Provider

            s = c["DBInterface"];
            if (s != null && s != "")
            {
                if (s.ToUpper() == "TXT")
                {
                    DBInterface = "TXT";
                }
                else if (s.ToUpper() == "MDB")
                {
                    DBInterface = "MDB";
                }
                else if (s.ToUpper() == "WEB")
                {
                    DBInterface = "WEB";
                }
                else if (s.ToUpper() == "REST")
                {
                    DBInterface = "REST";
                }
            }

            s = c["ScoringProvider"];
            ScoringProvider = Utils.StrToIntDef(s, ScoringProvider);

            s = c["BridgeProvider"];
            BridgeProvider = Utils.StrToIntDef(s, BridgeProvider);

            //--WorkspaceInfo

            //<add key="WorkspaceType" value="5"/>
            //<add key="WorkspaceID" value="5"/>
            //<add key="WorkspaceUrl" value="http://gshsm/FR64/WorkspaceFiles.asmx"/>
            //<add key="WorkspaceRoot" value="D:\Test\Workspace\FR64"/>
            //<add key="AutoSaveIni" value="false"/>                                                

            if (TMain.ReadWorkspaceInfo)
            {
                TWorkspaceInfo wi = TMain.WorkspaceInfo;
                if (wi == null)
                {
                    return;
                }

                s = c["WorkspaceType"];
                wi.WorkspaceType = Utils.StrToIntDef(s, wi.WorkspaceType);

                s = c["WorkspaceID"];
                wi.WorkspaceID = Utils.StrToIntDef(s, wi.WorkspaceID);

                s = c["WorkspaceUrl"];
                if (s != null && s != "")
                {
                    wi.WorkspaceUrl = s;
                }

                s = c["WorkspaceRoot"];
                if (s != null && s != "")
                {
                    wi.WorkspaceRoot = s;
                }

                s = c["AutoSaveIni"];
                wi.AutoSaveIni = ReadBool(s, wi.AutoSaveIni);
            }

        }

        internal override void WriteToNameValueCollection(NameValueCollection c)
        {
            //--Options
            c.Set("SafeStartupMode", Utils.BoolStr[SafeStartupMode]);  
            c.Set("AutoLoadAdminConfig", Utils.BoolStr[AutoLoadAdminConfig]);  

            c.Set("WantSockets", Utils.BoolStr[WantSockets]);  
            c.Set("SearchForUsablePorts", Utils.BoolStr[SearchForUsablePorts]);  
            c.Set("IsMaster", Utils.BoolStr[IsMaster]);  

            c.Set("WantLocalAccess", Utils.BoolStr[WantLocalAccess]);  
            c.Set("AutoSave", Utils.BoolStr[AutoSave]);  
            c.Set("NoAutoSave", Utils.BoolStr[NoAutoSave]);  

            c.Set("CopyRankEnabled", Utils.BoolStr[CopyRankEnabled]);  
            c.Set("LogProxyXML", Utils.BoolStr[LogProxyXML]);  

            //--Connections
            c.Set("WantAdapter", Utils.BoolStr[WantAdapter]);  
            c.Set("AutoConnect", Utils.BoolStr[AutoConnect]);  

            c.Set("Host", Host);  
            c.Set("PortIn", PortIn.ToString());  
            c.Set("PortOut", PortOut.ToString());  

            c.Set("CalcHost", CalcHost);  
            c.Set("CalcPort", CalcPort.ToString());  

            c.Set("FeedbackEnabled", Utils.BoolStr[FeedbackEnabled]);  
            c.Set("FeedbackHost", FeedbackHost);  
            c.Set("FeedbackPort", FeedbackPort.ToString());  

            c.Set("WebServerPort", WebServerPort.ToString());  
            c.Set("HomeUrl" , HomeUrl);  
            c.Set("WebApplicationUrl" , WebApplicationUrl);  
            c.Set("BrowserHomePage" , BrowserHomePage);  
            c.Set("AppTitle" , AppTitle);  

            c.Set("JSDataDir", JSDataDir);

            //--Switch
            c.Set("SwitchHost", SwitchHost);  
            c.Set("SwitchPort", SwitchPort.ToString());  
            c.Set("SwitchPortHTTP", SwitchPortHTTP.ToString());  
            c.Set("RouterHost", RouterHost);  
            c.Set("UseRouterHost", Utils.BoolStr[UseRouterHost]);  
            c.Set("UseAddress", Utils.BoolStr[UseAddress]);  
            c.Set("MaxConnections", MaxConnections.ToString());

            //--Bridge
            c.Set("BridgeProxy", BridgeProxyID);  
            c.Set("BridgeHost", BridgeHost);  
            c.Set("BridgePort", BridgePort.ToString());  
            c.Set("BridgePortHTTP", BridgePortHTTP.ToString());  
            c.Set("BridgeUrl", BridgeUrl);  
            c.Set("BridgeHomePage", BridgeHomePage);  
            c.Set("TaktIn", TaktIn.ToString());  
            c.Set("TaktOut", TaktOut.ToString());  

            //--Output
            c.Set("OutputHost", OutputHost);  
            c.Set("OutputPort", OutputPort.ToString());  

            //--Provider
            c.Set("DBInterface", DBInterface);  
            c.Set("ScoringProvider", ScoringProvider.ToString());  
            c.Set("BridgeProvider", BridgeProvider.ToString());  
        }

        public override string ToString()
        {
            string crlf = Environment.NewLine;
            StringBuilder sb = new StringBuilder();

            sb.Append("[Options]");
            sb.Append(crlf);

            sb.Append("SafeStartupMode=");
            sb.Append(SafeStartupMode);
            sb.Append(crlf);

            sb.Append("AutoLoadAdminConfig=");
            sb.Append(AutoLoadAdminConfig);
            sb.Append(crlf);

            sb.Append("WantSockets=");
            sb.Append(WantSockets);
            sb.Append(crlf);

            sb.Append("SearchForUsablePorts=");
            sb.Append(SearchForUsablePorts);
            sb.Append(crlf);

            sb.Append("IsMaster=");
            sb.Append(IsMaster);
            sb.Append(crlf);

            sb.Append("AutoSave=");
            sb.Append(AutoSave);
            sb.Append(crlf);

            sb.Append("NoAutoSave=");
            sb.Append(NoAutoSave);
            sb.Append(crlf);

            sb.Append("CopyRankEnabled=");
            sb.Append(CopyRankEnabled);
            sb.Append(crlf);

            sb.Append("LogProxyXML=");
            sb.Append(LogProxyXML);
            sb.Append(crlf);

            sb.Append(crlf);
            sb.Append("[Connections]");
            sb.Append(crlf);

            sb.Append("WantAdapter=");
            sb.Append(WantAdapter);
            sb.Append(crlf);

            sb.Append("AutoConnect=");
            sb.Append(AutoConnect);
            sb.Append(crlf);

            sb.Append("Host=");
            sb.Append(Host);
            sb.Append(crlf);

            sb.Append("PortIn=");
            sb.Append(PortIn);
            sb.Append(crlf);

            sb.Append("PortOut=");
            sb.Append(PortOut);
            sb.Append(crlf);

            sb.Append("CalcHost=");
            sb.Append(CalcHost);
            sb.Append(crlf);

            sb.Append("CalcPort=");
            sb.Append(CalcPort);
            sb.Append(crlf);

            sb.Append("FeedbackEnabled=");
            sb.Append(FeedbackEnabled);
            sb.Append(crlf);

            sb.Append("FeedbackHost=");
            sb.Append(FeedbackHost);
            sb.Append(crlf);

            sb.Append("FeedbackPort=");
            sb.Append(FeedbackPort);
            sb.Append(crlf);

            sb.Append("WebServerPort=");
            sb.Append(WebServerPort);
            sb.Append(crlf);

            sb.Append("WebApplicationUrl=");
            sb.Append(WebApplicationUrl);
            sb.Append(crlf);

            sb.Append("HomeUrl=");
            sb.Append(HomeUrl);
            sb.Append(crlf);

            sb.Append("BrowserHomePage=");
            sb.Append(BrowserHomePage);
            sb.Append(crlf);

            sb.Append("AppTitle=");
            sb.Append(AppTitle);
            sb.Append(crlf);

            sb.Append("JSDataDir=");
            sb.Append(JSDataDir);
            sb.Append(crlf);

            sb.Append(crlf);
            sb.Append("[Switch]");
            sb.Append(crlf);

            sb.Append("SwitchHost=");
            sb.Append(SwitchHost);
            sb.Append(crlf);

            sb.Append("SwitchPort=");
            sb.Append(SwitchPort);
            sb.Append(crlf);

            sb.Append("SwitchPortHTTP=");
            sb.Append(SwitchPortHTTP);
            sb.Append(crlf);

            sb.Append("RouterHost=");
            sb.Append(RouterHost);
            sb.Append(crlf);

            sb.Append("UseRouterHost=");
            sb.Append(UseRouterHost);
            sb.Append(crlf);

            sb.Append("UseAddress=");
            sb.Append(UseAddress);
            sb.Append(crlf);

            sb.Append("MaxConnections=");
            sb.Append(MaxConnections);
            sb.Append(crlf);

            sb.Append(crlf);
            sb.Append("[Bridge]");
            sb.Append(crlf);

            sb.Append("BridgeProxyType=");
            sb.Append(BridgeProxyID);
            sb.Append(crlf);

            sb.Append("BridgeHost=");
            sb.Append(BridgeHost);
            sb.Append(crlf);

            sb.Append("BridgePort=");
            sb.Append(BridgePort);
            sb.Append(crlf);

            sb.Append("BridgePortHTTP=");
            sb.Append(BridgePortHTTP);
            sb.Append(crlf);

            sb.Append("BridgeUrl=");
            sb.Append(BridgeUrl);
            sb.Append(crlf);

            sb.Append("BridgeHomePage=");
            sb.Append(BridgeHomePage);
            sb.Append(crlf);

            sb.Append("TaktIn=");
            sb.Append(TaktIn);
            sb.Append(crlf);

            sb.Append("TaktOut=");
            sb.Append(TaktOut);
            sb.Append(crlf);

            sb.Append(crlf);
            sb.Append("[Output]");
            sb.Append(crlf);

            sb.Append("OutputHost=");
            sb.Append(OutputHost);
            sb.Append(crlf);

            sb.Append("OutputPort=");
            sb.Append(OutputPort);
            sb.Append(crlf);

            sb.Append(crlf);
            sb.Append("[Provider]");
            sb.Append(crlf);

            sb.Append("DataProvider=");
            sb.Append(DBInterface);
            sb.Append(crlf);

            sb.Append("ScoringProvider=");
            sb.Append(ScoringProvider);
            sb.Append(crlf);

            sb.Append("BridgeProvider=");
            sb.Append(BridgeProvider);
            sb.Append(crlf);

            sb.Append(crlf);

            return sb.ToString();
        }

    }

}
