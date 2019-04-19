using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using RiggVar.FR;

namespace RiggVar.FR42.DAL
{
    public class FR42XML : FR42DBCommon, IFR42DB
    {
        public string XmlConnectionString;

        private readonly string emptyFile = "";
        private readonly string emptyTxtFile = "empty";
        private readonly string emptyXmlFile = @"<?xml version=""1.0"" ?><root>empty</root>";
        private readonly string emptyHtmFile = "<html><body>empty</body></html>";

        //private string newEventFile = "DP.RaceCount=1\r\nDP.ITCount=0\r\nDP.StartlistCount=2\r\n";

        public FR42XML(string cs)
        {
            XmlConnectionString = cs;
        }

        public string MsgListFileName => string.Format("{0}MsgList.txt", XmlConnectionString);
        public string EventListFileName => XmlConnectionString + "EventList.xml";
        public string ParamsFileName => XmlConnectionString + "Params.xml";

        public override StringCollection GetEventList()
        {
            StringCollection sc = new StringCollection();
            string fn = EventListFileName;
            if (File.Exists(fn))
            {
                try
                {
                    XPathDocument xpdoc = new XPathDocument(fn);
                    if (xpdoc != null)
                    {
                        XPathNavigator nav = xpdoc.CreateNavigator();
                        XPathNodeIterator nit = nav.Select("/EventList/EventInfo/EventName");
                        while (nit.MoveNext())
                        {
                            string s = nit.Current.Value;
                            if (s != null && s != "")
                            {
                                sc.Add(s);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
            return sc;
        }

        public int NewEvent(string EventName, FR42EventInfo EventInfo)
        {
            if (EventInfo == null)
            {
                EventInfo = new FR42EventInfo();
            }

            try
            {
                string fn = GetFileName("FRData", EventName);
                if (File.Exists(fn))
                {
                    UpdateInfo(EventName, EventInfo);
                    return 0;
                }
                else
                {
                    TStringList SL = new TStringList();
                    SL.Text = EventInfo.NewEventData();
                    SL.SaveToFile(fn);
                    UpdateInfo(EventName, EventInfo);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return -1;
            }
        }

        public override string GetFRData(string EventName)
        {
            return GetDataString("FRData", EventName);
        }

        public override string GetDataString(string FieldName, string EventName)
        {
            string fn = GetFileName(FieldName, EventName);
            if (File.Exists(fn))
            {
                TStringList SL = new TStringList();
                SL.LoadFromFile(fn);
                return SL.Text;
            }
            return emptyFile;
        }

        public override int SetDataString(string FieldName, string EventName, string DataString)
        {
            try
            {
                string fn = GetFileName(FieldName, EventName);
                TStringList SL = new TStringList();
                SL.Text = DataString;
                SL.SaveToFile(fn);
                return 1;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return 0;
        }

        public bool BackupEvent(string EventName)
        {
            string fn1 = GetFileName("FRData", EventName);
            string fn2 = GetFileName("FRBackup", EventName);
            if (File.Exists(fn1))
            {
                File.Copy(fn1, fn2, true);
                return true;
            }
            return false;
        }

        public bool RestoreEvent(string EventName)
        {
            string fn1 = GetFileName("FRData", EventName);
            string fn2 = GetFileName("FRBackup", EventName);
            if (File.Exists(fn2))
            {
                File.Copy(fn2, fn1, true);
                return true;
            }
            return false;
        }

        public bool DeleteEvent(string EventName)
        {
            string fn;

            fn = GetFileName("FRData", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FRTXT", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FRHTM", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FRBackup", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FRXML", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("JSXML", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FREvent", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            fn = GetFileName("FRRace", EventName);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            return true;
        }

        public bool KeyExists(int KatID, string EventName)
        {
            string fn = GetFileName("FRData", EventName);
            return File.Exists(fn);
        }

        public override bool CreateRow(string EventName, string FRData)
        {
            //Copy: if (KeyExists) CreateRow() else UpdateRow();
            int i;
            i = this.NewEvent(EventName, null);
            if (i < 0)
            {
                return false;
            }

            return UpdateRow(EventName, FRData);
        }

        public override bool UpdateRow(string EventName, string FRData)
        {
            int i = SetDataString("FRData", EventName, FRData);
            FR42EventInfo info = this.ExtractInfo(FRData);
            UpdateInfo(EventName, info);
            return i == 1;
        }

        public bool SaveEvent(int KatID, string EventName, string Data)
        {
            return SetDataString("FRData", EventName, Data) == 1;
        }

        public bool AddMsg(int EventID, int DeviceID, string Msg)
        {
            try
            {
                string fn = MsgListFileName;
                StreamWriter sw;
                if (File.Exists(fn))
                {
                    sw = File.AppendText(fn);
                }
                else
                {
                    sw = File.CreateText(fn);
                }
                sw.WriteLine(Msg);
                sw.Flush();
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return false;
        }

        public int CleanupMessages(int EventID)
        {
            string fn = MsgListFileName;
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }

            return -1;
        }

        /// <summary>
        /// Status field and EventID will be ignored.
        /// All Messages are target the Current event.
        /// </summary>
        /// <param name="EventID">ignored</param>
        /// <returns>Collection of new messages</returns>
        public StringCollection ProcessNewMessages(int EventID)
        {
            /* select Msg from TimingMsg
             * where EventID=@EventID and Status > 0
             * 
             * update TimingMsg set Status = 0
             * where EventID=@EventID
             * */
            StringCollection sc = new StringCollection();
            string fn = MsgListFileName;
            if (File.Exists(fn))
            {
                TStringList SL = new TStringList();
                SL.LoadFromFile(fn);
                foreach (string s in SL)
                {
                    sc.Add(s);
                }
                SL.Text = "empty";
                SL.SaveToFile(fn);
            }
            return sc;
        }

        public string GetDataLen(string EventName)
        {
            long FRData = this.GetFileSize("FRData", EventName);
            long FRXML = this.GetFileSize("FRXML", EventName);
            long FRXSD = this.GetFileSize("FRXSD", EventName);
            long FRTXT = this.GetFileSize("FRTXT", EventName);
            long FRBackup = this.GetFileSize("FRBackup", EventName);
            long FRHTM = this.GetFileSize("FRHTM", EventName);
            long JSXML = this.GetFileSize("JSXML", EventName);
            long FRRace = this.GetFileSize("FRRace", EventName);
            long FREvent = this.GetFileSize("FREvent", EventName);

            string s;
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.WriteStartDocument();
            xw.WriteStartElement("EventData");
            xw.WriteElementString("EventName", EventName);
            s = string.Format("{0}", FRData);
            xw.WriteElementString("FRData", s);
            s = string.Format("{0}", FRXML);
            xw.WriteElementString("FRXML", s);
            s = string.Format("{0}", FRXSD);
            xw.WriteElementString("FRXSD", s);
            s = string.Format("{0}", FRTXT);
            xw.WriteElementString("FRTXT", s);
            s = string.Format("{0}", FRBackup);
            xw.WriteElementString("FRBackup", s);
            s = string.Format("{0}", FRHTM);
            xw.WriteElementString("FRHTM", s);
            s = string.Format("{0}", JSXML);
            xw.WriteElementString("JSXML", s);
            s = string.Format("{0}", FRRace);
            xw.WriteElementString("FRRace", s);
            s = string.Format("{0}", FREvent);
            xw.WriteElementString("FREvent", s);
            xw.WriteEndElement();
            xw.WriteEndDocument();
            return sw.ToString();
        }

        public string GetParams(int EventID, int DeviceID)
        {
            string s = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(this.ParamsFileName);
                s = doc.SelectSingleNode("/Params").InnerXml;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return s;
        }

        public bool FillEventInfoDataSet(System.Data.DataSet ds)
        {
            try
            {
                ds.ReadXml(this.EventListFileName);
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return false;
        }

        public override int UpdateInfo(string EventName, FR42EventInfo info)
        {
            if (info == null)
            {
                return 0;
            }

            try
            {
                string fn = EventListFileName;
                XmlDocument doc = new XmlDocument();
                doc.Load(fn);
                string x = string.Format("/EventList/EventInfo[EventName='{0}']", EventName);
                XmlNode n = doc.SelectSingleNode(x);
                if (n == null)
                {
                    n = doc.CreateElement("EventInfo");
                    doc.SelectSingleNode("/EventList").AppendChild(n);
                    XmlNode m = doc.CreateElement("EventName");
                    n.AppendChild(m);
                }
                if (n != null)
                {
                    info.UpdateInfoNode(EventName, n);
                }
                doc.Save(fn);
                return 1;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            return 0;
        }

        private string GetFileName(string FieldName, string EventName)
        {
            string fn = "";
            string emptyFile = "";

            switch (FieldName)
            {
                case "FRData":
                    fn = string.Format("{0}FR_{1}_{2}.txt", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyTxtFile;
                    break;
                case "FRTXT":
                    fn = string.Format("{0}FR_{1}_{2}.txt", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyTxtFile;
                    break;
                case "FRHTM":
                    fn = string.Format("{0}FR_{1}_{2}.htm", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyHtmFile;
                    break;
                case "FRBackup":
                    fn = string.Format("{0}FR_{1}_{2}.txt", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyTxtFile;
                    break;
                case "FRXML":
                    fn = string.Format("{0}FR_{1}_{2}.xml", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyXmlFile;
                    break;
                case "JSXML":
                    fn = string.Format("{0}FR_{1}_{2}.xml", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyXmlFile;
                    break;
                case "FREvent":
                    fn = string.Format("{0}FR_{1}_{2}.htm", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyHtmFile;
                    break;
                case "FRRace":
                    fn = string.Format("{0}FR_{1}_{2}.htm", XmlConnectionString, EventName, FieldName);
                    emptyFile = this.emptyHtmFile;
                    break;
            }
            return fn;
        }

        private long GetFileSize(string FieldName, string EventName)
        {
            string fn = GetFileName(FieldName, EventName);
            return File.Exists(fn) ? new FileInfo(fn).Length : -1;
        }
    }
}