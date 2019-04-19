using System;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using System.IO;

namespace RiggVar.FR11.DAL
{
    /// <summary>
    /// Microsoft Access (Jet) Data Access Layer
    /// </summary>
    public class FR_DAL_MDB : DALBase, IDBEvent3
    {
        public string mdbConnectionString;

        public FR_DAL_MDB(string cs) : base()
        {
            mdbConnectionString = cs;
        }

        protected bool KeyExists(int KatID, string EventName)
        {
            int result = -1;
            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);

            string mySelectQuery = "SELECT Count(*) FROM KV where KatID = ? and K = ?";
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);

            OleDbParameter myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
            myParm.Value = KatID;
            myParm = myCommand.Parameters.Add("@K", OleDbType.VarWChar, 50);
            myParm.Value = EventName;

            try
            {
                try
                {
                    myConnection.Open();
                    result = (int)myCommand.ExecuteScalar();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return result == 1 ? true : false;
        }

        public override string Load(int KatID, string EventName)
        {
            string result = "";
            string mySelectQuery = "SELECT V FROM KV where KatID = ? and K = ?";
            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);

            OleDbParameter myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
            myParm.Value = KatID;

            myParm = myCommand.Parameters.Add("@K", OleDbType.VarWChar, 50);
            myParm.Value = EventName;
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        result = dr.GetString(0);
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return result;
        }

        public override void Save(int KatID, string EventName, string Data)
        {
            string sQuery;

            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand;

            if (KeyExists(KatID, EventName))
            {
                sQuery = "Update KV Set V = '" + Data + "' " +
                    "where (KatID = " + KatID.ToString() + ") and (K = '" + EventName + "')";
                myCommand = new OleDbCommand(sQuery, myConnection);
            }
            else
            {
                sQuery = "Insert Into KV (KatID, K, V) Values (?, ?, ?)";
                myCommand = new OleDbCommand(sQuery, myConnection);
                myCommand.Parameters.Add(new OleDbParameter("KatID", KatID));
                myCommand.Parameters.Add(new OleDbParameter("K", EventName));
                myCommand.Parameters.Add(new OleDbParameter("V", EnsureCRLF(Data)));
            }
            try
            {
                myConnection.Open();
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
        }

        public override void Delete(int KatID, string EventName)
        {
            string mySelectQuery = "DELETE FROM KV where KatID = ? and K = ?";

            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbParameter myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
            myParm.Value = KatID;
            myParm = myCommand.Parameters.Add("@K", OleDbType.VarWChar, 50);
            myParm.Value = EventName;
            try
            {
                myConnection.Open();
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
        }

        public override string UpdateEventNames(int KatID, string EventFilter)
        {
            string result = "";
            string s;

            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand;
            string sQuery;
            OleDbParameter myParm;

            if (KatID == 0)
            {
                sQuery = "SELECT TOP 100 K FROM KV";
                myCommand = new OleDbCommand(sQuery, myConnection);
            }
            else if (EventFilter != "")
            {
                sQuery = "SELECT TOP 100 K FROM KV where (KatID = ?) and (K like ?)";
                myCommand = new OleDbCommand(sQuery, myConnection);

                myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
                myParm.Value = KatID;

                myParm = myCommand.Parameters.Add("@K", OleDbType.VarChar, 50);
                myParm.Value = EventFilter;
            }
            else
            {
                sQuery = "SELECT TOP 100 K FROM KV where (KatID = ?)";
                myCommand = new OleDbCommand(sQuery, myConnection);

                myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
                myParm.Value = KatID;
            }
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr;
                    dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    eventNames.Clear();
                    while (dr.Read())
                    {
                        s = dr.GetString(0);
                        eventNames.Add(s);
                        result += s + "\r\n";
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return result;
        }

        public override EventRow LoadEventRow(int KatID, string EventName)
        {

            EventRow result = new EventRow();

            OleDbConnection myConnection;
            OleDbCommand myCommand;
            OleDbParameter myParm;
            string mySelectQuery;

            if (KatID == 0)
            {
                mySelectQuery = "SELECT Id, V, D, DC, U, P, C FROM KV where K = ?";
                myConnection = new OleDbConnection(mdbConnectionString);
                myCommand = new OleDbCommand(mySelectQuery, myConnection);

                myParm = myCommand.Parameters.Add("@K", OleDbType.VarWChar, 50);
                myParm.Value = EventName;
            }
            else
            {
                mySelectQuery = "SELECT Id, V, D, DC, U, P, C FROM KV where KatID = ? and K = ?";
                myConnection = new OleDbConnection(mdbConnectionString);
                myCommand = new OleDbCommand(mySelectQuery, myConnection);

                myParm = myCommand.Parameters.Add("@KatID", OleDbType.Integer, 4);
                myParm.Value = KatID;

                myParm = myCommand.Parameters.Add("@K", OleDbType.VarWChar, 50);
                myParm.Value = EventName;
            }

            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        result.ID = dr.GetInt32(0); //Id
                        result.KatID = KatID; //KatID
                        result.EventName = EventName; //K
                        if (!dr.IsDBNull(1))
                        {
                            result.EventData = dr.GetString(1); //V
                        }

                        if (!dr.IsDBNull(2))
                        {
                            result.Description = dr.GetString(2); //D
                        }

                        if (!dr.IsDBNull(3))
                        {
                            result.CreationDate = dr.GetDateTime(3); //DC
                        }

                        if (!dr.IsDBNull(4))
                        {
                            result.UserName = dr.GetString(4); //U
                        }

                        if (!dr.IsDBNull(5))
                        {
                            result.PassWord = dr.GetString(5); //P                
                        }

                        if (!dr.IsDBNull(6))
                        {
                            result.Division = dr.GetString(6); //C                
                        }
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return result;
        }

        public override DataSet LoadDataSet()
        {
            try
            {
                DataSet ds = new DataSet();
                ds.Namespace = AdminSchemaNamespace;
                OleDbDataAdapter da = this.GetDataAdapterOleDb();
                da.Fill(ds, "KV");
                return ds;
            }
            catch
            {
                return null;
            }
        }

        public override DataSet SaveDataSet(DataSet ds)
        {
            DataSet diff = ds.GetChanges();
            OleDbDataAdapter da = this.GetDataAdapterOleDb();
            try
            {
                da.Update(diff, "KV");
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            ds.Clear();
            return LoadDataSet();
        }

        public override CustomerDetails GetCustomerDetails(string customerID)
        {
            string mySelectQuery = "SELECT FullName, PW, EmailAddress, FROM Customers where CustomerID = ?";
            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbParameter myParm = myCommand.Parameters.Add("CustomerID", OleDbType.Integer, 4);
            myParm.Value = customerID;
            CustomerDetails myCustomerDetails = new CustomerDetails();
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        myCustomerDetails.FullName = dr.GetString(0);
                        myCustomerDetails.Password = dr.GetString(1);
                        myCustomerDetails.Email = dr.GetString(2);
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            return myCustomerDetails;
        }

        public override string AddCustomer(string fullName, string email, string password)
        {
            int customerId;
            customerId = FindCustomerIDforEmail(email);
            if (customerId > 0)
            {
                return customerId.ToString();
            }

            string mySelectQuery = "INSERT INTO Customers (FullName, EmailAddress, [PW]) VALUES (?, ?, ?)";

            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);

            myCommand.Parameters.AddWithValue("FullName", fullName);
            myCommand.Parameters.AddWithValue("EmailAddress", email);
            myCommand.Parameters.AddWithValue("PW", password);

            try
            {
                myConnection.Open();
                try
                {
                    myCommand.ExecuteNonQuery();
                }
                finally
                {
                    myConnection.Close();
                }
                customerId = FindCustomerIDforEmail(email);
            }
            catch
            {
            }
            if (customerId > 0)
            {
                return customerId.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public override string Login(string email, string password)
        {
            // Create Instance of Connection and Command Object
            string mySelectQuery = "SELECT CustomerID FROM Customers where EmailAddress = ? and [PW] = ?";
            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            myCommand.Parameters.AddWithValue("EmailAddress", email);
            myCommand.Parameters.AddWithValue("PW", password);

            int customerId = 0;
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        customerId = dr.GetInt32(0);
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }

            if (customerId == 0)
            {
                return null;
            }
            else
            {
                return customerId.ToString();
            }
        }
        protected int FindCustomerIDforEmail(string email)
        {
            int customerId = 0;
            try
            {
                string mySelectQuery = "SELECT CustomerID FROM Customers where EmailAddress = ?";
                OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
                OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
                myCommand.Parameters.AddWithValue("EmailAddress", email);
                myConnection.Open();
                try
                {
                    OleDbDataReader dr;
                    dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        customerId = dr.GetInt32(0);
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return customerId;
        }
        protected OleDbDataAdapter GetDataAdapterOleDb()
        {
            // 
            // con
            // 
            OleDbConnection con = new OleDbConnection(mdbConnectionString);
            // 
            // selectCommand
            // 
            OleDbCommand selectCommand = new OleDbCommand();
            selectCommand.CommandText = "SELECT ID, KatID, K, C, U, P FROM KV";
            selectCommand.Connection = con;
            // 
            // updateCommand
            // 
            OleDbCommand updateCommand = new OleDbCommand();
            updateCommand.CommandText = "UPDATE KV SET KatID = ?, K = ?, C = ?, U = ?, P = ? WHERE ID = ?";
            updateCommand.Connection = con;
            updateCommand.Parameters.Add(new OleDbParameter("KatID", OleDbType.Integer, 4, "KatID"));
            updateCommand.Parameters.Add(new OleDbParameter("K", OleDbType.VarWChar, 50, "K"));
            updateCommand.Parameters.Add(new OleDbParameter("C", OleDbType.VarWChar, 50, "C"));
            updateCommand.Parameters.Add(new OleDbParameter("U", OleDbType.VarWChar, 50, "U"));
            updateCommand.Parameters.Add(new OleDbParameter("P", OleDbType.VarWChar, 50, "P"));
            updateCommand.Parameters.Add(new OleDbParameter("ID", OleDbType.Integer, 4, "ID"));
            // 
            // da
            // 
            OleDbDataAdapter da = new OleDbDataAdapter();
            da.SelectCommand = selectCommand;
            System.Data.OleDb.OleDbCommandBuilder cb = new OleDbCommandBuilder(da);
            da.UpdateCommand = updateCommand;
            return da;
        }

        public override DataSet GetEventNames(int paramKatID)
        {
            OleDbCommand oleDbSelectCommand1 = new OleDbCommand();
            OleDbConnection oleDbConnection1 = new OleDbConnection(mdbConnectionString);
            OleDbDataAdapter oleDbDataAdapter1 = new OleDbDataAdapter();
            DataSet dataSet1 = new DataSet();

            dataSet1.BeginInit();

            oleDbSelectCommand1.CommandText = "SELECT ID, K, D FROM KV where KatID = ?";
            oleDbSelectCommand1.Parameters.AddWithValue("KatID", paramKatID);
            oleDbSelectCommand1.Connection = oleDbConnection1;

            oleDbDataAdapter1.SelectCommand = oleDbSelectCommand1;

            DataTableMapping dtm = new DataTableMapping("Table", "KV");
            dtm.ColumnMappings.Add("ID", "ID");
            dtm.ColumnMappings.Add("K", "FileName");
            dtm.ColumnMappings.Add("D", "Description");

            oleDbDataAdapter1.TableMappings.Add(dtm);

            dataSet1.DataSetName = "DataSet1";

            dataSet1.EndInit();

            oleDbDataAdapter1.Fill(dataSet1);
            return dataSet1;
        }
        public override string LoadFile(int paramID)
        {
            string result = "";
            string mySelectQuery = "SELECT V FROM KV where ID = ?";
            OleDbConnection myConnection = new OleDbConnection(mdbConnectionString);
            OleDbCommand myCommand = new OleDbCommand(mySelectQuery, myConnection);
            OleDbParameter myParm = myCommand.Parameters.Add("@ID", OleDbType.Integer, 4);
            myParm.Value = paramID;
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        result = dr.GetString(0);
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return result;
        }
        public override string ImportDataFiles(string Dir)
        {
            int Counter = 0;
            EventRow result = new EventRow();
            OleDbConnection myConnection;
            OleDbCommand myCommand;
            string mySelectQuery;
            mySelectQuery = "SELECT Id, KatID, K, V from KV";
            myConnection = new OleDbConnection(mdbConnectionString);
            myCommand = new OleDbCommand(mySelectQuery, myConnection);
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    int ID;
                    int KatID;
                    string K;
                    string V;
                    string fn;
                    while (dr.Read())
                    {
                        ID = dr.GetInt32(0);
                        KatID = dr.GetInt32(1);
                        K = dr.GetString(2);
                        if (!dr.IsDBNull(3))
                        {
                            V = dr.GetString(3);
                        }
                        else
                        {
                            V = "";
                        }

                        fn = Dir + "\\" + K + ".txt";
                        FileInfo fi = new FileInfo(fn);
                        if (fi.Exists)
                        {
                            try
                            {
                                StreamReader sr = fi.OpenText();
                                V = sr.ReadToEnd();
                                sr.Close();
                                Save(KatID, K, V);
                                Counter++;
                            }
                            catch (Exception ex)
                            {
                                return ex.Message;
                            }
                        }
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return "imported " + Counter.ToString() + " files";
        }
        public override string ExportDataFiles(string Dir)
        {
            int Counter = 0;
            EventRow result = new EventRow();
            OleDbConnection myConnection;
            OleDbCommand myCommand;
            string mySelectQuery;
            mySelectQuery = "SELECT Id, KatID, K, V from KV";
            myConnection = new OleDbConnection(mdbConnectionString);
            myCommand = new OleDbCommand(mySelectQuery, myConnection);
            try
            {
                myConnection.Open();
                try
                {
                    OleDbDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    int ID;
                    int KatID;
                    string K;
                    string V;
                    string fn;
                    while (dr.Read())
                    {
                        ID = dr.GetInt32(0);
                        KatID = dr.GetInt32(1);
                        K = dr.GetString(2);
                        if (!dr.IsDBNull(3))
                        {
                            V = dr.GetString(3);
                        }
                        else
                        {
                            V = "";
                        }

                        DirectoryInfo di = new DirectoryInfo(Dir);
                        if (di.Exists)
                        {
                            try
                            {
                                fn = Dir + "\\" + K + ".txt";
                                FileInfo fi = new FileInfo(fn);
                                StreamWriter sw = fi.CreateText();
                                sw.Write(V);
                                sw.Close();
                                Counter++;
                            }
                            catch (Exception ex)
                            {
                                return ex.Message;
                            }
                        }
                    }
                    dr.Close();
                }
                finally
                {
                    myConnection.Close();
                }
            }
            catch
            {
            }
            return "exported " + Counter.ToString() + " files";
        }
    }
}
