using System;
using System.Xml;
using System.Configuration;
using System.IO;

namespace RiggVar.FR
{
    /// <summary>
    /// reads ConfigurationSettings.Appsettings and uses a hack when writing
    /// </summary>
    public class IniLoaderAppSettings : IIniLoader
    {

        private void SetValue(XmlNode n, string value)
        {
            n.Attributes["value"].Value = value;
        }

        /// <summary>
        /// is a hack, used to update the application.exe.config
        /// </summary>
        /// <param name="nl">the nodelist of the appsettings section</param>
        private void WriteToNodeList(TBaseIniImage ini, XmlNodeList nl)
        {
            foreach (XmlNode n in nl)
            {
                string s = n.Attributes["key"].Value;
                switch (s)
                {
                    case "WantSockets": SetValue(n, Utils.BoolStr[ini.WantSockets]); break;
                    case "IsMaster": SetValue(n, Utils.BoolStr[ini.IsMaster]); break;

                    case "AutoSave": SetValue(n, Utils.BoolStr[ini.AutoSave]); break;
                    case "NoAutoSave": SetValue(n, Utils.BoolStr[ini.NoAutoSave]); break;

                    case "CopyRankEnabled": SetValue(n, Utils.BoolStr[ini.CopyRankEnabled]); break;

                    case "CalcHost": SetValue(n, ini.CalcHost); break;
                    case "CalcPort": SetValue(n, ini.CalcPort.ToString()); break;

                    case "WebServerPort": SetValue(n, ini.WebServerPort.ToString()); break;
                    case "WebApplicationUrl": SetValue(n, ini.WebApplicationUrl); break;

                    case "SwitchHost": SetValue(n, ini.SwitchHost); break;
                    case "SwitchPort": SetValue(n, ini.SwitchPort.ToString()); break;
                    case "SwitchPortHTTP": SetValue(n, ini.SwitchPortHTTP.ToString()); break;
                    case "RouterHost": SetValue(n, ini.RouterHost); break;
                    case "UseRouterHost": SetValue(n, Utils.BoolStr[ini.UseRouterHost]); break;

                    case "BridgeProxy": SetValue(n, ini.BridgeProxyID); break;
                    case "BridgeHost": SetValue(n, ini.BridgeHost); break;
                    case "BridgePort": SetValue(n, ini.BridgePort.ToString()); break;
                    case "BridgePortHTTP": SetValue(n, ini.BridgePortHTTP.ToString()); break;
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

        #region IniLoader Member

        public void LoadSettings(TBaseIniImage ini)
        {
            ini.ReadFromNameValueCollection(PlatformDiff.GetAppSettings());
        }

        public void SaveSettings(TBaseIniImage ini)
        {
            string fn = PlatformDiff.GetConfigFileName();
            if (File.Exists(fn))
            {
                try
                {
                    XmlDocument d =new XmlDocument();
                    d.Load(fn);
                    XmlNodeList nl = d.SelectNodes("//configuration/appSettings/add");
                    WriteToNodeList(ini, nl);
                    d.Save(fn);        
                }
                catch(Exception ex)
                {
                    throw new IniLoaderException("cannot write to AppSettings file " + fn, ex);
                }

            }
            else
            {
                throw new IniLoaderException(fn + "does not exist");
            }
        }

        #endregion

    }

}
