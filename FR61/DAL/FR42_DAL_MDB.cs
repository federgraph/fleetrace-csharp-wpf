using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace RiggVar.FR42.DAL
{
    public class FR42MDB : FR42DBCommon, IFR42DB
    {
        public string OleDbConnectionString;

        public FR42MDB(string cs)
        {
            OleDbConnectionString = cs;
        }

        public override StringCollection GetEventList()
        {
            StringCollection sc = new StringCollection();
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();

                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandType = CommandType.Text;
                c.CommandText = "SELECT EventName FROM EventData ORDER BY EventID";
                using (OleDbDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        sc.Add(dr.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                sc.Clear();
            }
            return sc;
        }

        public override bool CreateRow(string EventName, string FRData)
        {
            int result = this.NewEvent(EventName, null); //make sure row exists
            if (result != 1)
            {
                return false;
            }

            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;

                    //update Data
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_UpdateFRData";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    c.Parameters.AddWithValue("@FRData", FRData);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public override bool UpdateRow(string EventName, string FRData)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_UpdateFRData";
                    c.Parameters.AddWithValue("@FRData", FRData);
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public string GetParams(int EventID, int DeviceID)
        {
            /* //<?xml version="1.0" ?>
             * //<root>
             * <FR42Params K="RaceCount" V="2"/>
             * <FR42Params K="ITCount" V="1"/>
             * <FR42Params K="RowCount" V="6"/>
             * <FR42Params K="Host" V="thinkpad"/>
             * <FR42Params K="PortIn" V="3027"/>
             * <FR42Params K="PortOut" V="3028"/>
             * <FR42Params K="DivisionName" V="*"/>
             * <FR42Params K="ColCount" V="8"/>
             * <FR42Params K="BibCount" V="42"/>
             * <FR42Params K="MaxRaceCount" V="6"/>
             * <FR42Params K="MaxITCount" V="12"/>
             * <FR42Params K="HomeUrl" V="http://thinkpad/RiggVar05/FR42_MsgList.aspx"/>
             * //</root>
             * */
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            //xw.WriteStartDocument(); //in aspx page
            //xw.WriteStartElement("root"); //in aspx page
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();

                OleDbCommand c;
                c = new OleDbCommand();
                c.Connection = con;
                c.CommandType = CommandType.StoredProcedure;
                c.CommandText = "FR42_GetParams";
                c.Parameters.AddWithValue("@EventID", EventID);
                c.Parameters.AddWithValue("@DeviceID", DeviceID);
                using (OleDbDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    int i = dr.RecordsAffected;
                    //if (i != 1) return "";
                    i = 0;
                    string s;
                    while (dr.Read())
                    {
                        xw.WriteStartElement("FR42Params");
                        s = (string)dr["K"];
                        xw.WriteAttributeString("K", s);
                        s = (string)dr["V"];
                        xw.WriteAttributeString("V", s);
                        xw.WriteEndElement();
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //xw.WriteEndElement();
                //xw.WriteEndDocument();
            }
            return sw.ToString();
        }

        public override string GetFRData(string EventName)
        {
            string s;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_GetFRData";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    s = c.ExecuteScalar() as string;
                }
                finally
                {
                    con.Close();
                }
                if (s == null)
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                s = "";
            }
            return s;
        }
        public override string GetDataString(string FieldName, string EventName)
        {
            string s;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.Text;
                    string f = "SELECT {0} FROM EventData WHERE EventName='{1}'";
                    c.CommandText = string.Format(f, FieldName, EventName);
                    s = c.ExecuteScalar() as string;
                }
                finally
                {
                    con.Close();
                }
                if (s == null)
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                s = "";
            }
            return s;
        }
        public override int SetDataString(string FieldName, string EventName, string DataString)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.Text;
                    string f = @"Update EventData SET {0} = ? WHERE EventName = ?";
                    c.CommandText = string.Format(f, FieldName);
                    c.Parameters.AddWithValue("@DataString", DataString);
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result;
        }
        public bool BackupEvent(string EventName)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_BackupEvent";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }
        public bool RestoreEvent(string EventName)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;

                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_RestoreEvent";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }
        public bool DeleteEvent(string EventName)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_DeleteEvent";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public bool KeyExists(int KatID, string EventName)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.Text;
                    c.CommandText = "SELECT Count(*) FROM EventData WHERE EventName = ?";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = (int)c.ExecuteScalar();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public bool SaveEvent(int KatID, string EventName, string Data)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.Text;
                    if (KeyExists(KatID, EventName))
                    {
                        c.CommandText = "Update EventData SET FRData = ? WHERE EventName = ?";
                    }
                    else
                    {
                        c.CommandText = "INSERT INTO EventData (EventName, FRData) VALUES (@EventName, @Data)";
                    }
                    c.Parameters.AddWithValue("@Data", Data);
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public bool AddMsg(int EventID, int DeviceID, string Msg)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_AddMsg";
                    c.Parameters.AddWithValue("@EventID", EventID);
                    c.Parameters.AddWithValue("@DeviceID", DeviceID);
                    c.Parameters.AddWithValue("@Msg", Msg);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result == 1;
        }

        public int CleanupMessages(int EventID)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_CleanupMessages";
                    c.Parameters.AddWithValue("@EventID", EventID);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result;
        }

        public StringCollection ProcessNewMessages(int EventID)
        {
            StringCollection sc = new StringCollection();
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;

                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_SelectNewMessages";
                    c.Parameters.AddWithValue("@EventID", EventID);
                    using (OleDbDataReader dr = c.ExecuteReader(CommandBehavior.Default))
                    {
                        string msg;
                        while (dr.Read())
                        {
                            msg = dr["Msg"] as string;
                            sc.Add(msg);
                        }
                    }

                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_ResetMsgFlag";
                    c.Parameters.AddWithValue("@EventID", EventID);
                    int i = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                sc.Clear();
            }
            return sc;
        }
        public int NewEvent(string EventName, FR42EventInfo EventInfo)
        {
            if (EventInfo == null)
            {
                EventInfo = new FR42EventInfo();
            }

            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;

                    //test if EventName exists in DB
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_NewEventExists";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    int count = (int)c.ExecuteScalar();
                    if (count > 0)
                    {
                        return 0;
                    }

                    //add new row
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_NewEvent";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();

                    //set EventID = ID
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_NewEventID";
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
                SetDataString("FRData", EventName, EventInfo.NewEventData());
                UpdateInfo(EventName, EventInfo);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result;
        }
        public string GetDataLen(string EventName)
        {
            /* SELECT EventName, 
             * LEN(FRData) AS ['FRData'], 
             * LEN(FRXML) AS ['FRXML'], 
             * LEN(FRXSD) AS ['FRXSD'], 
             * LEN(FRTXT) AS ['FRTXT'], 
             * LEN(FRBackup) AS ['FRBackup'], 
             * LEN(FRHTM) AS ['FRHTM'], 
             * LEN(JSXML) AS ['JSXML'], 
             * LEN(FRRace) AS ['FRRace'], 
             * LEN(FREvent) AS ['FREvent']
             * FROM EventData
             * WHERE EventName=[@EventName];
             * */
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xw.WriteStartDocument();
            xw.WriteStartElement("EventData");
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();

                OleDbCommand c;
                c = new OleDbCommand();
                c.Connection = con;
                c.CommandType = CommandType.StoredProcedure;
                c.CommandText = "FR42_GetDataLen";
                c.Parameters.AddWithValue("@EventName", EventName);
                using (OleDbDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    int i = dr.RecordsAffected;
                    string s;
                    while (dr.Read())
                    {
                        s = (string)dr["EventName"];
                        xw.WriteElementString("EventName", s);
                        s = string.Format("{0}", dr["oFRData"]);
                        xw.WriteElementString("FRData", s);
                        s = string.Format("{0}", dr["oFRXML"]);
                        xw.WriteElementString("FRXML", s);
                        s = string.Format("{0}", dr["oFRXSD"]);
                        xw.WriteElementString("FRXSD", s);
                        s = string.Format("{0}", dr["oFRTXT"]);
                        xw.WriteElementString("FRTXT", s);
                        s = string.Format("{0}", dr["oFRBackup"]);
                        xw.WriteElementString("FRBackup", s);
                        s = string.Format("{0}", dr["oFRHTM"]);
                        xw.WriteElementString("FRHTM", s);
                        s = string.Format("{0}", dr["oJSXML"]);
                        xw.WriteElementString("JSXML", s);
                        s = string.Format("{0}", dr["oFRRace"]);
                        xw.WriteElementString("FRRace", s);
                        s = string.Format("{0}", dr["oFREvent"]);
                        xw.WriteElementString("FREvent", s);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            return sw.ToString();
        }
        public override int UpdateInfo(string EventName, FR42EventInfo info)
        {
            int result;
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.StoredProcedure;
                    c.CommandText = "FR42_UpdateInfo";
                    c.Parameters.AddWithValue("@Division", info.Division);
                    c.Parameters.AddWithValue("@RaceCount", info.RaceCount);
                    c.Parameters.AddWithValue("@ITCount", info.ITCount);
                    c.Parameters.AddWithValue("@StartlistCount", info.StartlistCount);
                    c.Parameters.AddWithValue("@ShowPoints", info.ShowPoints);
                    c.Parameters.AddWithValue("@StrictMode", info.StrictMode);
                    c.Parameters.AddWithValue("@EventName", EventName);
                    result = c.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = -1;
            }
            return result;
        }
        public bool FillEventInfoDataSet(DataSet ds)
        {
            try
            {
                OleDbConnection con = new OleDbConnection(OleDbConnectionString);
                con.Open();
                try
                {
                    OleDbCommand c;
                    c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandType = CommandType.Text;
                    c.CommandText = "SELECT ID, EventName, Division, RaceCount, ITCount, StartlistCount, ShowPoints, StrictMode FROM EventData";
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    da.SelectCommand = c;
                    da.Fill(ds);
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
            return true;

        }
    }
}
