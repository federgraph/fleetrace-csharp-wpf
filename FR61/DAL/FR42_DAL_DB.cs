using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

using RiggVar.FR;

namespace RiggVar.FR42.DAL
{
    public class FR42EventInfo
    {
        public string Division;
        public int RaceCount;
        public int ITCount;
        public int StartlistCount;
        public bool ShowPoints;
        public bool StrictMode;

        public FR42EventInfo()
        {
            Clear();
        }
        public void Clear()
        {
            Division = "*";
            RaceCount = 1;
            ITCount = 0;
            StartlistCount = 2;
            ShowPoints = false;
            StrictMode = false;
        }
        public void UpdateInfoNode(string EventName, XmlNode n)
        {
            UpdateNode(n, "EventName", EventName);
            UpdateNode(n, "Division", Division);
            UpdateNode(n, "RaceCount", RaceCount.ToString());
            UpdateNode(n, "ITCount", ITCount.ToString());
            UpdateNode(n, "StartlistCount", StartlistCount.ToString());
            UpdateNode(n, "ShowPoints", Utils.BoolStr[ShowPoints]);
            UpdateNode(n, "StrictMode", Utils.BoolStr[StrictMode]);
        }
        private void UpdateNode(XmlNode n, string ElementName, string InnerText)
        {
            XmlNode m = n.SelectSingleNode(ElementName);
            if (m != null)
            {
                m.InnerXml = InnerText;
            }
            else
            {
                m = n.OwnerDocument.CreateElement(ElementName);
                m.InnerXml = InnerText;
                n.AppendChild(m);
            }
        }
        public string NewEventData()
        {
            System.IO.StringWriter sw = new StringWriter();
            sw.WriteLine("DP.RaceCount = {0}", RaceCount);
            sw.WriteLine("DP.ITCount = {0}", ITCount);
            sw.WriteLine("DP.StartlistCount = {0}", StartlistCount);
            sw.WriteLine("EP.DivisionName = {0}", Division);

            if (ShowPoints)
            {
                sw.WriteLine("EP.RaceLayout = Points");
            }
            else
            {
                sw.WriteLine("EP.RaceLayout = Finish");
            }

            if (StrictMode)
            {
                sw.WriteLine("EP.InputMode = Strict");
            }
            else
            {
                sw.WriteLine("EP.InputMode = Relaxed");
            }

            return sw.ToString();
        }
    }

    /// <summary>
    /// Encapsulates access to the datebase
    /// </summary>
    public interface IFR42DB
    {
        bool Calc(string EventName);
        StringCollection GetEventList();
        void CopyEventsFromRggData(string sqlConnectionString);
        //int GetMaxEventID();
        //bool CreateRow(string EventName, string FRData);
        //bool UpdateRow(string EventName, string FRData);
        string GetFRData(string EventName);
        string GetDataString(string FieldName, string EventName);
        int SetDataString(string FieldName, string EventName, string DataString);
        bool BackupEvent(string EventName);
        bool RestoreEvent(string EventName);
        bool DeleteEvent(string EventName);
        bool KeyExists(int KatID, string EventName);
        bool SaveEvent(int KatID, string EventName, string Data);
        bool AddMsg(int EventID, int DeviceID, string Msg);
        int CleanupMessages(int EventID);
        StringCollection ProcessNewMessages(int EventID);
        int NewEvent(string EventName, FR42EventInfo EventInfo);
        string GetParams(int EventID, int DeviceID);
        string GetDataLen(string EventName);
        int ExpandInfo(string EventName);
        bool FillEventInfoDataSet(DataSet ds);
        FR42EventInfo ExtractInfo(string eventData);
    }

    public class FR42DBCommon
    {
        public void HandleException(Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.Message);
        }
        public void CopyEventsFromRggData(string sqlConnectionString)
        {
            StringCollection sc = GetEventList();
            try
            {
                SqlConnection con = new SqlConnection(sqlConnectionString);
                con.Open();

                SqlCommand c;
                c = new SqlCommand();
                c.Connection = con;
                c.CommandType = CommandType.Text;
                c.CommandText = "SELECT K, V FROM KV WHERE KatID = 400";
                using (SqlDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    string k, v;
                    while (dr.Read())
                    {
                        k = dr["K"] as string;
                        if (k == null)
                        {
                            continue;
                        }

                        if (dr["V"] == System.DBNull.Value)
                        {
                            continue;
                        }

                        v = dr["V"] as string;
                        if (sc.IndexOf(k) > -1)
                        {
                            UpdateRow(k, v);
                        }
                        else
                        {
                            CreateRow(k, v);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
        public FR42EventInfo ExtractInfo(string eventData)
        {
            return ExtractInfoS(eventData);
        }
        public static FR42EventInfo ExtractInfoS(string eventData)
        {
            FR42EventInfo info = new FR42EventInfo();
            info.Clear();

            TStringList SL = new TStringList();
            SL.Text = eventData;

            string s, n, v;
            int paramsRead = 0;
            for (int i = 0; i < SL.Count; i++)
            {
                s = SL[i];
                n = SL.Names(i).Trim();
                v = SL.ValueFromIndex(i).Trim();
                if (n == "DP.StartlistCount" || n == "Event.StartlistCount")
                {
                    info.StartlistCount = Utils.StrToIntDef(v, info.StartlistCount);
                    paramsRead++;
                }
                else if (n == "DP.ITCount" || n == "Event.ITCount")
                {
                    info.ITCount = Utils.StrToIntDef(v, info.ITCount);
                    paramsRead++;
                }
                else if (n == "DP.RaceCount" || n == "Event.RaceCount")
                {
                    info.RaceCount = Utils.StrToIntDef(v, info.RaceCount);
                    paramsRead++;
                }
                else if (n == "EP.DivisionName" || n == "Event.Prop_DivisionName")
                {
                    info.Division = v;
                    paramsRead++;
                }
                else if (n == "EP.RaceLayout" || n == "Event.Prop_RaceLayout")
                {
                    if (v.StartsWith("F"))
                    {
                        info.ShowPoints = false;
                    }
                    else
                    {
                        info.ShowPoints = true;
                    }

                    paramsRead++;
                }
                else if (n == "EP.InputMode" || n == "EP.IM" || n == "Event.Prop_InputMode")
                {
                    if (v.StartsWith("S"))
                    {
                        info.StrictMode = true;
                    }
                    else
                    {
                        info.StrictMode = false;
                    }

                    paramsRead++;
                }

                if (paramsRead == 6)
                {
                    break;
                }
            }
            return info;
        }
        public bool Calc(string EventName)
        {
            try
            {
                string eventData = this.GetDataString("FRData", EventName);
                string xmlData = new FRXmlGenerator().GetXml(TExcelImporter.Expand(eventData));
                SetDataString("FRXML", EventName, xmlData);
                return true;
            }
            catch
            {
            }
            return false;
        }
        public int ExpandInfo(string EventName)
        {
            string eventData = this.GetFRData(EventName);
            if (eventData != "")
            {
                FR42EventInfo info = this.ExtractInfo(eventData);
                return UpdateInfo(EventName, info);
            }
            return -1;
        }
        public virtual bool CreateRow(string EventName, string FRData)
        {
            return false;
        }

        public virtual bool UpdateRow(string EventName, string FRData)
        {
            return false;
        }

        public virtual StringCollection GetEventList()
        {
            return new StringCollection();
        }
        public virtual string GetDataString(string FieldName, string EventName)
        {
            return "";
        }
        public virtual int SetDataString(string FieldName, string EventName, string DataString)
        {
            return 0;
        }
        public virtual string GetFRData(string EventName)
        {
            return "";
        }
        public virtual int UpdateInfo(string EventName, FR42EventInfo info)
        {
            return 0;
        }
    }

}
