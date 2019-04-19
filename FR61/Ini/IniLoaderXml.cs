using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace RiggVar.FR
{
    /// <summary>
    ///  uses XmlSerializer to read/write user configuration;
    ///  writes everything, including those properties not used;
    ///  does not make use of sections
    /// </summary>
    public class IniLoaderXml : IIniLoader
    {

        private void Assign(TBaseIniImage s, TBaseIniImage d)
        {
            d.WantLocalAccess = s.WantLocalAccess;
            d.WantSockets = s.WantSockets;

            d.AutoSave = s.AutoSave;
            d.NoAutoSave = s.AutoSave;
            
            d.CopyRankEnabled = s.CopyRankEnabled;
            d.LogProxyXML = s.LogProxyXML;

            d.AutoConnect = s.AutoConnect;

            d.CalcHost = s.CalcHost;
            d.CalcPort = s.CalcPort;
            d.FeedbackEnabled = s.FeedbackEnabled;

            d.ScoringProvider = s.ScoringProvider;

        }

        #region IniLoader Member

        public void LoadSettings(TBaseIniImage ini)
        {
            string fn = ini.FolderInfo.ConfigFileName;
            if (File.Exists(fn))
            {
                try
                {
                
                    XmlSerializer xs = new XmlSerializer(typeof(TBaseIniImage));
                    StreamReader sw = new StreamReader(fn);
                    TBaseIniImage source = xs.Deserialize(sw) as TBaseIniImage;    
                    Assign(source, ini);
                    sw.Close();
                }
                catch(Exception ex)
                {
                    throw new IniLoaderException("cannot read deserialize IniImage from " + fn, ex);
                }
            }
        }


        public void SaveSettings(TBaseIniImage ini)
        {
            string fn = ini.FolderInfo.ConfigFileName;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TBaseIniImage));

                // Create an XmlSerializerNamespaces object.
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                // Add a namespace with prefix
                ns.Add("iniimage", "http://www.riggvar.net");

                // Create an XmlTextWriter using a FileStream.
                Stream fs = new FileStream(fn, FileMode.Create);
                XmlTextWriter writer = new XmlTextWriter(fs, System.Text.Encoding.UTF8);
                writer.Formatting = Formatting.Indented;

                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, ini, ns);
                writer.Close();
            }
            catch(Exception ex)
            {                
                throw new IniLoaderException("cannot serialize IniImage to " + fn, ex);
            }
        }

        #endregion
    }
}
