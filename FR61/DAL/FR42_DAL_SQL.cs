using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace RiggVar.FR42.DAL
{
    public class FR42SQL : FR42DBCommon, IFR42DB
    {
        public string SqlConnectionString;

        public FR42SQL(string cs)
        {
            SqlConnectionString = cs;
        }

        public override StringCollection GetEventList()
        {
            StringCollection sc = new StringCollection();
            try
            {                
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();

                SqlCommand c;
                //SqlParameter p;

                c = new SqlCommand
                {
                    Connection = con,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT EventName FROM EventData ORDER BY EventID"
                };
                using (SqlDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
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
            int result;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlParameter p;
                    SqlCommand c;

                    //new Event
                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_NewEvent"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    result = c.ExecuteNonQuery();

                    //update Data
                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_UpdateFRData"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    p = c.Parameters.Add("@FRData", SqlDbType.NText);
                    p.Value = FRData;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_UpdateFRData"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    p = c.Parameters.Add("@FRData", SqlDbType.NText);
                    p.Value = FRData;
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
            string s;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_GetParams"
                    };
                    p = c.Parameters.Add("@EventID", SqlDbType.Int);
                    p.Value = EventID;
                    p = c.Parameters.Add("@DeviceID", SqlDbType.Int);
                    p.Value = DeviceID;
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

        public override string GetFRData(string EventName)
        {
            string s;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_GetFRData"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_GetDataString"
                    };
                    p = c.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50);
                    p.Value = FieldName;
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text
                    };
                    string f = @"Update EventData set {0} = @DataString where EventName = @EventName";
                    c.CommandText = string.Format(f, FieldName);
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    p = c.Parameters.Add("@DataString", SqlDbType.NText);
                    p.Value = DataString;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_BackupEvent"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_RestoreEvent"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_DeleteEvent"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT Count(*) FROM EventData WHERE EventName = @EventName"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    result = (int) c.ExecuteScalar();
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text
                    };
                    if (KeyExists(KatID, EventName))
                    {
                        c.CommandText = "UPDATE EventData SET FRData = @Data WHERE EventName = @EventName";
                    }
                    else
                    {
                        c.CommandText = "INSERT INTO EventData (EventName, FRData) VALUES (@EventName, @Data)";
                    }
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    p = c.Parameters.Add("@Data", SqlDbType.NText);
                    p.Value = Data;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_AddMsg"
                    };
                    p = c.Parameters.Add("@EventID", SqlDbType.Int);
                    p.Value = EventID;
                    p = c.Parameters.Add("@DeviceID", SqlDbType.Int);
                    p.Value = DeviceID;
                    p = c.Parameters.Add("@Msg", SqlDbType.NVarChar, 50);
                    p.Value = Msg;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_CleanupMessages"
                    };
                    p = c.Parameters.Add("@EventID", SqlDbType.Int);
                    p.Value = EventID;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();

                SqlCommand c;
                SqlParameter p;

                c = new SqlCommand
                {
                    Connection = con,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "FR42_ProcessNewMessages"
                };
                p = c.Parameters.Add("@EventID", SqlDbType.Int);
                p.Value = EventID;
                using (SqlDataReader dr = c.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    string msg;
                    while (dr.Read())
                    {
                        msg = dr["Msg"] as string;
                        sc.Add(msg);
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
        public int NewEvent(string EventName, FR42EventInfo EventInfo)
        {
            if (EventInfo == null)
            {
                EventInfo = new FR42EventInfo();
            }

            int result;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_NewEvent"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
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
            string result;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_GetDataLen"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    result = c.ExecuteScalar() as string;
                }
                finally
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                result = "";
            }
            return result;
        }

        public override int UpdateInfo(string EventName, FR42EventInfo info)
        {
            int result;
            try
            {
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "FR42_UpdateInfo"
                    };
                    p = c.Parameters.Add("@EventName", SqlDbType.NVarChar, 50);
                    p.Value = EventName;
                    p = c.Parameters.Add("@Division", SqlDbType.NVarChar, 50);
                    p.Value = info.Division;
                    p = c.Parameters.Add("@RaceCount", SqlDbType.Int);
                    p.Value = info.RaceCount;
                    p = c.Parameters.Add("@ITCount", SqlDbType.Int);
                    p.Value = info.ITCount;
                    p = c.Parameters.Add("@StartlistCount", SqlDbType.Int);
                    p.Value = info.StartlistCount;
                    p = c.Parameters.Add("@ShowPoints", SqlDbType.Bit);
                    p.Value = info.ShowPoints;
                    p = c.Parameters.Add("@StrictMode", SqlDbType.Bit);
                    p.Value = info.StrictMode;
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
                SqlConnection con = new SqlConnection(SqlConnectionString);
                con.Open();
                try
                {
                    SqlCommand c;
                    //SqlParameter p;

                    c = new SqlCommand
                    {
                        Connection = con,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT ID, EventName, Division, RaceCount, ITCount, StartlistCount, ShowPoints, StrictMode FROM EventData"
                    };
                    SqlDataAdapter da = new SqlDataAdapter();
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
