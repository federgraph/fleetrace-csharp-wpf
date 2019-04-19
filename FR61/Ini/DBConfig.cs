namespace RiggVar.FR
{
    /// <summary>
    /// Comstomization of IniDB for project FR
    /// </summary>
    public class DBConfig : IniDB
    {
        public DBConfig()
        {
        }
        public override bool SectionEnabled(IniSection value)
        {
            switch (value)
            {
                case IniSection.Options: return true;
                case IniSection.Connections: return true;
                case IniSection.Switch: return true;
                case IniSection.Bridge: return true;
                case IniSection.Output: return true;
                case IniSection.Provider: return true;
                default: return false;
            }
        }
        public override bool KeyEnabled(IniKey value)
        {
            switch (value)
            {
                //case IniKey.AppTitle: return true;
                //case IniKey.AutoConnect: return true;
                //case IniKey.AutoLoadAdminConfig: return true;
                case IniKey.AutoSave: return true;
                //case IniKey.BridgeHomePage: return true;
                case IniKey.BridgeHost: return true;
                case IniKey.BridgePort: return true;
                //case IniKey.BridgePortHTTP: return true;
                case IniKey.BridgeProvider: return true;
                case IniKey.BridgeProxy: return true;
                case IniKey.BridgeUrl: return true;
                case IniKey.BrowserHomePage: return true;
                case IniKey.CalcHost: return true;
                case IniKey.CalcPort: return true;
                case IniKey.CopyRankEnabled: return true;
                case IniKey.DBInterface: return true;
                //case IniKey.FeedbackEnabled: return true;
                //case IniKey.FeedbackHost: return true;
                //case IniKey.FeedbackPort: return true;
                case IniKey.HomeUrl: return true;
                case IniKey.Host: return true;
                case IniKey.IsMaster: return true;
                //case IniKey.JSDataDir: return true;
                case IniKey.LogProxyXML: return true;
                //case IniKey.MaxConnections: return true;
                case IniKey.NoAutoSave: return true;
                case IniKey.OutputHost: return true;
                case IniKey.OutputPort: return true;
                case IniKey.PortIn: return true;
                case IniKey.PortOut: return true;
                case IniKey.RouterHost: return true;
                //case IniKey.SafeStartupMode: return true;
                case IniKey.ScoringProvider: return true;
                //case IniKey.SearchForUsablePorts: return true;
                case IniKey.SwitchHost: return true;
                case IniKey.SwitchPort: return true;
                case IniKey.SwitchPortHTTP: return true;
                case IniKey.TaktIn: return true;
                case IniKey.TaktOut: return true;
                case IniKey.UseRouterHost: return true;
                //case IniKey.WantAdapter: return true;
                //case IniKey.WantLocalAccess: return true;
                case IniKey.WantSockets: return true;
                case IniKey.WebApplicationUrl: return true;
                //case IniKey.WebServerPort: return true;
                default: return false;
            }
        }

    }

}