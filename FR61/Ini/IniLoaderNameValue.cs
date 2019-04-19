using System;
using System.Xml;
using System.Collections.Specialized;
using System.IO;

namespace RiggVar.FR
{
    public class IniLoaderNameValue : IIniLoader
    {

        private void SetValue(XmlNode n, string value)
        {
            n.Attributes["value"].Value = value;
        }

        private void WriteValues(TBaseIniImage ini, XmlNodeList nl)
        {
            foreach (XmlNode n in nl)
            {
                string s = n.Attributes["key"].Value;
                switch (s)
                {
                    case "SafeStartupMode": SetValue(n, Utils.BoolStr[ini.SafeStartupMode]); break;
                    case "AutoLoadAdminConfig": SetValue(n, Utils.BoolStr[ini.AutoLoadAdminConfig]); break;

                    case "WantSockets": SetValue(n, Utils.BoolStr[ini.WantSockets]); break;
                    case "SearchForUsablePorts": SetValue(n, Utils.BoolStr[ini.SearchForUsablePorts]); break;
                    case "IsMaster": SetValue(n, Utils.BoolStr[ini.IsMaster]); break;

                    case "WantLocalAccess": SetValue(n, Utils.BoolStr[ini.WantLocalAccess]); break;
                    case "AutoSave": SetValue(n, Utils.BoolStr[ini.AutoSave]); break;
                    case "NoAutoSave": SetValue(n, Utils.BoolStr[ini.NoAutoSave]); break;

                    case "CopyRankEnabled": SetValue(n, Utils.BoolStr[ini.CopyRankEnabled]); break;
                    case "LogProxyXML": SetValue(n, Utils.BoolStr[ini.LogProxyXML]); break;

                    case "WantAdapter": SetValue(n, Utils.BoolStr[ini.WantAdapter]); break;
                    case "AutoConnect": SetValue(n, Utils.BoolStr[ini.AutoConnect]); break;

                    case "Host": SetValue(n, ini.Host); break;
                    case "PortIn": SetValue(n, ini.PortIn.ToString()); break;
                    case "PortOut": SetValue(n, ini.PortOut.ToString()); break;

                    case "CalcHost": SetValue(n, ini.CalcHost); break;
                    case "CalcPort": SetValue(n, ini.CalcPort.ToString()); break;

                    case "FeedbackEnabled": SetValue(n, Utils.BoolStr[ini.FeedbackEnabled]); break;
                    case "FeedbackHost": SetValue(n, ini.FeedbackHost); break;
                    case "FeedbackPort": SetValue(n, ini.FeedbackPort.ToString()); break;

                    case "WebServerPort": SetValue(n, ini.WebServerPort.ToString()); break;
                    case "HomeUrl": SetValue(n, ini.HomeUrl); break;
                    case "WebApplicationUrl": SetValue(n, ini.WebApplicationUrl); break;                    
                    case "BrowserHomePage": SetValue(n, ini.BrowserHomePage); break;
                    case "AppTitle": SetValue(n, ini.AppTitle); break;
                    case "JSDataDir": SetValue(n, ini.JSDataDir); break;

                    case "SwitchHost": SetValue(n, ini.SwitchHost); break;
                    case "SwitchPort": SetValue(n, ini.SwitchPort.ToString()); break;
                    case "SwitchPortHTTP": SetValue(n, ini.SwitchPortHTTP.ToString()); break;
                    case "RouterHost": SetValue(n, ini.RouterHost); break;
                    case "UseRouterHost": SetValue(n, Utils.BoolStr[ini.UseRouterHost]); break;
                    case "UseAddress": SetValue(n, Utils.BoolStr[ini.UseAddress]); break;
                    case "MacConnections": SetValue(n, ini.MaxConnections.ToString()); break;

                    case "BridgeProxy": SetValue(n, ini.BridgeProxyID); break;
                    case "BridgeHost": SetValue(n, ini.BridgeHost); break;
                    case "BridgePort": SetValue(n, ini.BridgePort.ToString()); break;
                    case "BridgePortHTTP": SetValue(n, ini.BridgePortHTTP.ToString()); break;
                    case "BridgeUrl": SetValue(n, ini.BridgeUrl); break;
                    case "BridgeHomePage": SetValue(n, ini.BridgeHomePage); break;
                    case "TaktIn": SetValue(n, ini.TaktIn.ToString()); break;
                    case "TaktOut": SetValue(n, ini.TaktOut.ToString()); break;

                    case "OutputHost": SetValue(n, ini.OutputHost); break;
                    case "OutputPort": SetValue(n, ini.OutputPort.ToString()); break;

                    case "DBInterface": SetValue(n, ini.DBInterface); break;
                    case "ScoringProvider": SetValue(n, ini.ScoringProvider.ToString()); break;
                    case "BridgeProvider": SetValue(n, ini.BridgeProvider.ToString()); break;

                }
            }
        }

        private XmlDocument BuildXmlDoc(TBaseIniImage ini, IniDB db)
        {
            XmlDocument d = new XmlDocument();

            string xml = "<IniImage xmlns:rc='riggvar:configuration'></IniImage>";
            d.Load(new StringReader(xml));

            XmlDeclaration xmldecl = d.CreateXmlDeclaration("1.0", null, null);
            XmlElement root = d.DocumentElement;
            d.InsertBefore(xmldecl, root);

            db.BuildXml(d);

            XmlNodeList nl;
            foreach (int i in Enum.GetValues(typeof(IniSection)))
            {
                IniSection s = (IniSection) Enum.Parse(typeof(IniSection), i.ToString());

                if (db.SectionEnabled(s))
                {
                    nl = d.SelectNodes("//IniImage/" + db.SectionString(s) + " /add");
                    WriteValues(ini, nl);
                }
            }
            return d;
        }

        private XmlNodeList GetNodeList(string fn)
        {
            TStringList SL = TMain.DBStringListFactory.CreateInstance();
            SL.LoadFromFile(fn);

            string xml =  SL.Text;

            XmlDocument d = new XmlDocument();
            d.LoadXml(xml);

            return d.SelectNodes("//IniImage/*/add");                    
        }

        #region IIniLoader Member

        public void LoadSettings(TBaseIniImage ini)
        {
            string fn = ini.FolderInfo.ConfigFileName;
            
            if (File.Exists(fn))
            {
                try
                {
                    XmlNodeList nl = this.GetNodeList(fn);
                    NameValueCollection c = new NameValueCollection();
                    foreach (XmlNode n in nl)
                    {
                        string k = n.Attributes["key"].Value;
                        string v = n.Attributes["value"].Value;
                        c.Add(k, v);
                    }
                    ini.ReadFromNameValueCollection(c);
                }
                catch(Exception ex)
                {
                    throw new IniLoaderException("cannot create configuration file " + fn, ex);
                }

            }
            else
            {
                throw new IniLoaderException(fn + " does not exist");
            }
        }

        public void SaveSettings(TBaseIniImage ini)
        {
            string fn = ini.FolderInfo.ConfigFileName;            
            try
            {
                XmlDocument d = BuildXmlDoc(ini, ini.DB);
                d.Save(fn);        
            }
            catch(Exception ex)
            {
                throw new IniLoaderException("cannot create configuration file " + fn, ex);
            }
        }

        #endregion

    }

}
