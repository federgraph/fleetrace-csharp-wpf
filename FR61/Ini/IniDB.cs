using System;
using System.Xml;

namespace RiggVar.FR
{
    public enum IniType
    {
        I,
        S,
        B        
    }
    public enum IniSection
    {
        Options,
        Connections,
        Switch,
        Bridge,
        Output,
        Provider,
    }

    public enum IniKey
    {
        EventType = 0,

        DBInterface = 1,
        ScoringProvider,
        BridgeProvider,

        SafeStartupMode = 20,
        AutoLoadAdminConfig,

        WantSockets = 30,
        SearchForUsablePorts,
        IsMaster,

        WantLocalAccess = 40,
        AutoSave,
        NoAutoSave,

        WantAdapter = 50,
        AutoConnect,

        Host = 60,
        PortIn,
        PortOut,

        CalcHost = 70,
        CalcPort,

        FeedbackEnabled = 80,
        FeedbackHost,
        FeedbackPort,

        WebServerPort = 90,
        HomeUrl,
        WebApplicationUrl,
        BrowserHomePage,
        AppTitle,
        JSDataDir,

        SwitchHost = 100,
        SwitchPort,
        SwitchPortHTTP,
        RouterHost,
        UseRouterHost,
        UseAddress,
        MaxConnections,

        BridgeProxy = 110,
        BridgeHost,
        BridgePort,
        BridgePortHTTP,
        BridgeUrl,
        BridgeHomePage,
        TaktIn,
        TaktOut,

        OutputHost = 120,
        OutputPort,

        CopyRankEnabled = 200,
        LogProxyXML,
    
    }

    public class IniDB
    {
        public IniDB()
        {

        }

        public string Name(IniKey value)
        {
            return value.ToString();
        }

        public IniType Type(IniKey value)
        {
            switch (value)
            {
                case IniKey.AppTitle: return IniType.S;
                case IniKey.AutoConnect: return IniType.B;
                case IniKey.AutoLoadAdminConfig: return IniType.B;
                case IniKey.AutoSave: return IniType.B;
                case IniKey.BridgeHomePage: return IniType.S;
                case IniKey.BridgeHost: return IniType.S;
                case IniKey.BridgePort: return IniType.I;
                case IniKey.BridgePortHTTP: return IniType.I;
                case IniKey.BridgeProvider: return IniType.I;
                case IniKey.BridgeProxy: return IniType.I;
                case IniKey.BridgeUrl: return IniType.S;
                case IniKey.BrowserHomePage: return IniType.S;
                case IniKey.CalcHost: return IniType.S;
                case IniKey.CalcPort: return IniType.I;
                case IniKey.CopyRankEnabled: return IniType.B;
                case IniKey.DBInterface: return IniType.I;
                case IniKey.EventType: return IniType.I;
                case IniKey.FeedbackEnabled: return IniType.B;
                case IniKey.FeedbackHost: return IniType.S;
                case IniKey.FeedbackPort: return IniType.I;
                case IniKey.HomeUrl: return IniType.S;
                case IniKey.Host: return IniType.S;
                case IniKey.IsMaster: return IniType.B;
                case IniKey.JSDataDir: return IniType.S;
                case IniKey.LogProxyXML: return IniType.B;
                case IniKey.MaxConnections: return IniType.I;
                case IniKey.NoAutoSave: return IniType.B;
                case IniKey.OutputHost: return IniType.S;
                case IniKey.OutputPort: return IniType.I;
                case IniKey.PortIn: return IniType.I;
                case IniKey.PortOut: return IniType.I;
                case IniKey.RouterHost: return IniType.S;
                case IniKey.SafeStartupMode: return IniType.B;
                case IniKey.ScoringProvider: return IniType.I;
                case IniKey.SearchForUsablePorts: return IniType.B;
                case IniKey.SwitchHost: return IniType.S;
                case IniKey.SwitchPort: return IniType.I;
                case IniKey.SwitchPortHTTP: return IniType.I;
                case IniKey.TaktIn: return IniType.I;
                case IniKey.TaktOut: return IniType.I;
                case IniKey.UseRouterHost: return IniType.B;
                case IniKey.UseAddress: return IniType.B;
                case IniKey.WantAdapter: return IniType.B;
                case IniKey.WantLocalAccess: return IniType.B;
                case IniKey.WantSockets: return IniType.B;
                case IniKey.WebApplicationUrl: return IniType.S;
                case IniKey.WebServerPort: return IniType.B;
            }
            return IniType.S;
        }

        public IniSection Section(IniKey value)
        {
            switch (value)
            {
                case IniKey.SafeStartupMode: return IniSection.Options;
                case IniKey.AutoLoadAdminConfig: return IniSection.Options;

                case IniKey.WantSockets: return IniSection.Options;
                case IniKey.SearchForUsablePorts: return IniSection.Options;
                case IniKey.IsMaster: return IniSection.Options;

                case IniKey.WantLocalAccess: return IniSection.Options;
                case IniKey.AutoSave: return IniSection.Options;
                case IniKey.NoAutoSave: return IniSection.Options;

                case IniKey.CopyRankEnabled: return IniSection.Options;
                case IniKey.LogProxyXML: return IniSection.Options;

                case IniKey.WantAdapter: return IniSection.Connections;
                case IniKey.AutoConnect: return IniSection.Connections;
                case IniKey.Host: return IniSection.Connections;
                case IniKey.PortIn: return IniSection.Connections;
                case IniKey.PortOut: return IniSection.Connections;

                case IniKey.CalcHost: return IniSection.Connections;
                case IniKey.CalcPort: return IniSection.Connections;

                case IniKey.FeedbackEnabled: return IniSection.Connections;
                case IniKey.FeedbackHost: return IniSection.Connections;
                case IniKey.FeedbackPort: return IniSection.Connections;

                case IniKey.WebServerPort: return IniSection.Connections;            
                case IniKey.WebApplicationUrl: return IniSection.Connections;
                case IniKey.HomeUrl: return IniSection.Connections;
                case IniKey.BrowserHomePage: return IniSection.Connections;
                case IniKey.AppTitle: return IniSection.Connections;

                case IniKey.JSDataDir: return IniSection.Connections;

                case IniKey.SwitchHost: return IniSection.Switch;
                case IniKey.SwitchPort: return IniSection.Switch;
                case IniKey.SwitchPortHTTP: return IniSection.Switch;
                case IniKey.RouterHost: return IniSection.Switch;
                case IniKey.UseRouterHost: return IniSection.Switch;
                case IniKey.UseAddress: return IniSection.Switch;
                case IniKey.MaxConnections: return IniSection.Switch;

                case IniKey.BridgeHost: return IniSection.Bridge;
                case IniKey.BridgePort: return IniSection.Bridge;
                case IniKey.BridgePortHTTP: return IniSection.Bridge;
                case IniKey.BridgeProxy: return IniSection.Bridge;
                case IniKey.BridgeUrl: return IniSection.Bridge;
                case IniKey.BridgeHomePage: return IniSection.Bridge;
                case IniKey.TaktIn: return IniSection.Bridge;
                case IniKey.TaktOut: return IniSection.Bridge;

                case IniKey.OutputHost: return IniSection.Output;
                case IniKey.OutputPort: return IniSection.Output;

                case IniKey.DBInterface: return IniSection.Provider;
                case IniKey.ScoringProvider: return IniSection.Provider;
                case IniKey.BridgeProvider: return IniSection.Provider;

            }
            return IniSection.Options;
        }

        public string SectionString(IniSection value)
        {
            return value.ToString();
        }
        public string TypeString(IniType value)
        {
            switch (value)
            {
                case IniType.B: return "bool";
                case IniType.I: return "int";
                case IniType.S: return "string";
            }
            return "error";
        }
        public int ID(IniKey value)
        {
            return (int) value;
        }
        public string IDString(IniKey value)
        {
            return ((int) value).ToString();
        }
        public virtual bool SectionEnabled(IniSection value)
        {
            return false;
        }
        public virtual bool KeyEnabled(IniKey value)
        {
            return false;
        }

        public void BuildXml(XmlDocument d)
        {            
            XmlElement eSection;
            XmlElement e;

            foreach (int i in Enum.GetValues(typeof(IniSection)))
            {
                IniSection s = (IniSection) Enum.Parse(typeof(IniSection), i.ToString());

                if (this.SectionEnabled(s))
                {
                    eSection = d.CreateElement(this.SectionString(s));
                    d.DocumentElement.AppendChild(eSection);
                    foreach (int j in Enum.GetValues(typeof(IniKey)))
                    {
                        IniKey k = (IniKey) Enum.Parse(typeof(IniKey), j.ToString());
                        
                        if (Section(k) == s && this.KeyEnabled(k))
                        {
                            e = d.CreateElement("add");
                            e.SetAttribute("key", this.Name(k));
                            e.SetAttribute("value", "");
                            eSection.AppendChild(e);            
                        }
                    }
                }

            }
        }

    }

}
