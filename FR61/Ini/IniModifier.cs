namespace RiggVar.FR
{
    public class IniModifier
    {
        //--Options
        public bool S_Options;

        public bool W_SafeStartupMode;
        public bool W_AutoLoadAdminConfig;

        public bool W_WantSockets;
        public bool W_SearchForUsablePorts;
        public bool W_IsMaster;

        public bool W_AutoSave;
        public bool W_NoAutoSave;

        public bool W_CopyRankEnabled;
        public bool W_LogProxyXML;
    
        //--Connections
        public bool S_Connections;

        public bool W_WantAdapter;
        public bool W_AutoConnect;
        public bool W_Host;
        public bool W_PortIn;
        public bool W_PortOut;

        public bool W_CalcHost;
        public bool W_CalcPort;

        public bool W_FeedbackEnabled;
        public bool W_FeedbackHost;
        public bool W_FeedbackPort;

        public bool W_WebServerPort;
        public bool W_WebApplicationUrl;
        public bool W_HomeUrl;
        public bool W_BrowserHomePage;
        public bool W_AppTitle;

        public bool W_JSDataDir;

        //--Switch
        public bool S_Switch;
        public bool W_SwitchHost; 
        public bool W_SwitchPort;
        public bool W_SwitchPortHTTP;
        public bool W_RouterHost;
        public bool W_UseRouterHost;
        public bool W_UseAddress;
        public bool W_MaxConnections;

        //--Bridge
        public bool S_Bridge;
        public bool W_BridgeProxyType;
        public bool W_BridgeHost;
        public bool W_BridgePort;
        public bool W_BridgePortHTTP;
        public bool W_BridgeUrl;
        public bool W_BridgeHomePage;
        public bool W_TaktIn;
        public bool W_TaktOut;

        //--Output
        public bool S_Output;
        public bool W_OutputHost; 
        public bool W_OutputPort;

        //--Provider
        public bool S_Provider;
        public bool W_BridgeProvider;
        public bool W_ScoringProvider;
        public bool W_DataProvider;

        //--Info Sections
        public bool S_ScoringProviderHelp;
        public bool S_BridgeProviderHelp;
        public bool S_DataProviderHelp;
        public bool S_BridgeProxyHelp;

    }

}
