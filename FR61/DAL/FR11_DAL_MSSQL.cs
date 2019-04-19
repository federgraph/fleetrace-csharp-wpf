using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;

namespace RiggVar.FR11.DAL
{
    /// <summary>
    /// MSDE Data Access Layer
    /// </summary>
    public class FR_DAL_MSSQL : DALBase, IDBEvent3
    {
        public string sqlConnectionString;

        public FR_DAL_MSSQL(string cs) : base()
        {
            sqlConnectionString = cs;
        }

        protected bool KeyExists(int KatID, string EventName)
        {
            int result = -1;
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);

            string mySelectQuery = "SELECT Count(*) FROM KV where KatID = @KatID and K = @K";
            SqlCommand myCommand = new SqlCommand(mySelectQuery, myConnection);

            SqlParameter myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
            myParm.Value = KatID;
            myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
            myParm.Value = EventName;

            myConnection.Open();
            try
            {
                result = (int)myCommand.ExecuteScalar();
            }
            finally
            {
                myConnection.Close();
            }

            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string Load(int KatID, string EventName)
        {
            string result = "";
            string mySelectQuery = "SELECT V FROM KV where KatID = @KatID and K = @K";
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand(mySelectQuery, myConnection);

            SqlParameter myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
            myParm.Value = KatID;

            myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
            myParm.Value = EventName;

            myConnection.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                result = dr.GetString(0);
            }
            dr.Close();
            myConnection.Close();
            return result;
        }
        public override void Save(int KatID, string EventName, string Data)
        {
            string sQuery;

            if (KeyExists(KatID, EventName))
            {
                sQuery = "Update KV Set V = @V where KatID = @KatID and K = @K";
            }
            else
            {
                sQuery = "Insert Into KV (KatID, K, V) Values (@KatID, @K, @V)";
            }
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand(sQuery, myConnection);
            System.Data.SqlClient.SqlParameter myParm;
            myParm = myCommand.Parameters.Add("@KatID", System.Data.SqlDbType.Int, 4);
            myParm.Value = KatID;
            myParm = myCommand.Parameters.Add("@K", System.Data.SqlDbType.VarChar, 50);
            myParm.Value = EventName;
            myParm = myCommand.Parameters.Add("@V", System.Data.SqlDbType.VarChar, 2147483647);
            myParm.Value = EnsureCRLF(Data);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();
        }
        public override void Delete(int KatID, string EventName)
        {
            string mySelectQuery = "DELETE FROM KV where KatID = @KatID and K = @K";

            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand(mySelectQuery, myConnection);
            SqlParameter myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
            myParm.Value = KatID;
            myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
            myParm.Value = EventName;

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();
        }
        public override string UpdateEventNames(int KatID, string EventFilter)
        {
            string result = "";
            string s;

            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            string sQuery;
            SqlCommand myCommand;
            SqlParameter myParm;

            if (KatID == 0)
            {
                sQuery = "SELECT TOP 100 K FROM KV";
                myCommand = new SqlCommand(sQuery, myConnection);
            }
            else if (EventFilter != "")
            {
                sQuery = "SELECT TOP 100 K FROM KV where (KatID = @KatID) and (K like @K)";
                myCommand = new SqlCommand(sQuery, myConnection);

                myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
                myParm.Value = KatID;

                myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
                myParm.Value = EventFilter;
            }
            else
            {
                //sQuery = "SELECT TOP 100 K FROM KV where (KatID = @KatID)";
                myCommand = new SqlCommand("SelectCommand", myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
                myParm.Value = KatID;
            }

            myConnection.Open();
            try
            {
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
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
            return result;
        }
        public override EventRow LoadEventRow(int KatID, string EventName)
        {
            EventRow result = new EventRow();

            string mySelectQuery;
            SqlConnection myConnection;
            SqlCommand myCommand;
            SqlParameter myParm;

            if (KatID == 0)
            {
                mySelectQuery = "SELECT Id, V, D, DC, U, P, C FROM KV where K = @K";
                myConnection = new SqlConnection(sqlConnectionString);
                myCommand = new SqlCommand(mySelectQuery, myConnection);

                myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
                myParm.Value = EventName;
            }
            else
            {
                mySelectQuery = "SELECT Id, V, D, DC, U, P, C FROM KV where KatID = @KatID and K = @K";
                myConnection = new SqlConnection(sqlConnectionString);
                myCommand = new SqlCommand(mySelectQuery, myConnection);

                myParm = myCommand.Parameters.Add("@KatID", SqlDbType.Int, 4);
                myParm.Value = KatID;

                myParm = myCommand.Parameters.Add("@K", SqlDbType.VarChar, 50);
                myParm.Value = EventName;
            }

            myConnection.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
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
            myConnection.Close();
            return result;
        }
        public override DataSet LoadDataSet()
        {
            DataSet ds = new DataSet();
            ds.Namespace = AdminSchemaNamespace;
            SqlDataAdapter da = this.GetDataAdapterSql();
            da.Fill(ds, "KV");
            return ds;
        }
        public override DataSet SaveDataSet(DataSet ds)
        {
            DataSet diff = ds.GetChanges();
            SqlDataAdapter da = this.GetDataAdapterSql();
            da.Update(diff, "KV");
            ds.Clear();
            return LoadDataSet();
        }
        public override CustomerDetails GetCustomerDetails(string customerID)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand("CustomerDetail", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterCustomerID = new SqlParameter("@CustomerID", SqlDbType.Int, 4);
            parameterCustomerID.Value = int.Parse(customerID);
            myCommand.Parameters.Add(parameterCustomerID);

            SqlParameter parameterFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
            parameterFullName.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterFullName);

            SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
            parameterEmail.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterEmail);

            SqlParameter parameterPassword = new SqlParameter("@PW", SqlDbType.NVarChar, 50);
            parameterPassword.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterPassword);

            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();

            // Create CustomerDetails Struct
            CustomerDetails myCustomerDetails = new CustomerDetails();

            // Populate Struct using Output Params from SPROC
            myCustomerDetails.FullName = (string)parameterFullName.Value;
            myCustomerDetails.Password = (string)parameterPassword.Value;
            myCustomerDetails.Email = (string)parameterEmail.Value;

            return myCustomerDetails;
        }
        public override string AddCustomer(string fullName, string email, string password)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand("CustomerAdd", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
            parameterFullName.Value = fullName;
            myCommand.Parameters.Add(parameterFullName);

            SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
            parameterEmail.Value = email;
            myCommand.Parameters.Add(parameterEmail);

            SqlParameter parameterPassword = new SqlParameter("@PW", SqlDbType.NVarChar, 50);
            parameterPassword.Value = password;
            myCommand.Parameters.Add(parameterPassword);

            SqlParameter parameterCustomerID = new SqlParameter("@CustomerID", SqlDbType.Int, 4);
            parameterCustomerID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterCustomerID);

            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myConnection.Close();

                // Calculate the CustomerID using Output Param from SPROC
                int customerId = (int)parameterCustomerID.Value;

                return customerId.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public override string Login(string email, string password)
        {
            // Create Instance of Connection and Command Object
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand("CustomerLogin", myConnection);

            // Mark the Command as a SPROC
            myCommand.CommandType = CommandType.StoredProcedure;

            // Add Parameters to SPROC
            SqlParameter parameterEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
            parameterEmail.Value = email;
            myCommand.Parameters.Add(parameterEmail);

            SqlParameter parameterPassword = new SqlParameter("@PW", SqlDbType.NVarChar, 50);
            parameterPassword.Value = password;
            myCommand.Parameters.Add(parameterPassword);

            SqlParameter parameterCustomerID = new SqlParameter("@CustomerID", SqlDbType.Int, 4);
            parameterCustomerID.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(parameterCustomerID);

            // Open the connection and execute the Command
            myConnection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();

            int customerId = (int)(parameterCustomerID.Value);

            if (customerId == 0)
            {
                return string.Empty;
            }
            else
            {
                return customerId.ToString();
            }
        }
        protected SqlDataAdapter GetDataAdapterSql()
        {
            // 
            // con
            // 
            SqlConnection con = new SqlConnection(sqlConnectionString);
            // 
            // selectCommand
            // 
            SqlCommand selectCommand = new SqlCommand();
            selectCommand.CommandText = "SELECT ID, KatID, K, C, D, DC, U, P FROM KV";
            selectCommand.Connection = con;
            // 
            // updateCommand
            // 
            SqlCommand updateCommand = new SqlCommand();
            updateCommand.CommandText = "Update KV set KatID = @KatID, K = @K, C = @C, U = @U, P = @P" +
                " where (ID = @ID)";
            updateCommand.Connection = con;
            updateCommand.Parameters.Add(new SqlParameter("@KatID", SqlDbType.Int, 4, "KatID"));
            updateCommand.Parameters.Add(new SqlParameter("@K", SqlDbType.VarChar, 50, "K"));
            updateCommand.Parameters.Add(new SqlParameter("@C", SqlDbType.VarChar, 50, "C"));
            updateCommand.Parameters.Add(new SqlParameter("@U", SqlDbType.VarChar, 50, "U"));
            updateCommand.Parameters.Add(new SqlParameter("@P", SqlDbType.VarChar, 50, "P"));
            updateCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int, 4, "ID"));
            // 
            // da
            // 
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = selectCommand;
            da.UpdateCommand = updateCommand;
            return da;
        }

        public override DataSet GetEventNames(int paramKatID)
        {
            SqlCommand SqlSelectCommand1 = new SqlCommand();
            SqlConnection SqlConnection1 = new SqlConnection(sqlConnectionString);
            SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter();
            DataSet dataSet1 = new DataSet();

            dataSet1.BeginInit();

            SqlSelectCommand1.CommandText = "SELECT ID, K, D FROM KV where KatID = @KatID";
            SqlParameter p = new SqlParameter("@KatID", paramKatID);
            SqlSelectCommand1.Parameters.Add(p);
            SqlSelectCommand1.Connection = SqlConnection1;

            SqlDataAdapter1.SelectCommand = SqlSelectCommand1;

            DataTableMapping dtm = new DataTableMapping("Table", "KV");
            dtm.ColumnMappings.Add("ID", "ID");
            dtm.ColumnMappings.Add("K", "FileName");
            dtm.ColumnMappings.Add("D", "Description");

            SqlDataAdapter1.TableMappings.Add(dtm);

            dataSet1.DataSetName = "DataSet1";

            dataSet1.EndInit();

            SqlDataAdapter1.Fill(dataSet1);
            return dataSet1;
        }
        public override string LoadFile(int paramID)
        {
            string result = "";
            string mySelectQuery = "SELECT V FROM KV where ID = @ID";
            SqlConnection myConnection = new SqlConnection(sqlConnectionString);
            SqlCommand myCommand = new SqlCommand(mySelectQuery, myConnection);

            SqlParameter myParm = myCommand.Parameters.Add("@ID", SqlDbType.Int, 4);
            myParm.Value = paramID;

            myConnection.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                result = dr.GetString(0);
            }
            dr.Close();
            myConnection.Close();
            return result;
        }
        public override string ImportDataFiles(string Dir)
        {
            int Counter = 0;
            EventRow result = new EventRow();

            SqlConnection myConnection;
            SqlCommand myCommand;
            string mySelectQuery;

            mySelectQuery = "SELECT Id, KatID, K, V from KV";
            myConnection = new SqlConnection(sqlConnectionString);
            myCommand = new SqlCommand(mySelectQuery, myConnection);

            myConnection.Open();
            SqlDataReader dr;
            try
            {
                dr = myCommand.ExecuteReader();
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

            return "imported " + Counter.ToString() + " files";
        }
        public override string ExportDataFiles(string Dir)
        {
            int Counter = 0;
            EventRow result = new EventRow();

            SqlConnection myConnection;
            SqlCommand myCommand;
            string mySelectQuery;

            mySelectQuery = "SELECT Id, KatID, K, V from KV";
            myConnection = new SqlConnection(sqlConnectionString);
            myCommand = new SqlCommand(mySelectQuery, myConnection);

            myConnection.Open();
            SqlDataReader dr;
            try
            {
                dr = myCommand.ExecuteReader();
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

                    //make sure we use crlf
                    string crlf = "\r\n";
                    if (V.IndexOf(crlf) == -1)
                    {
                        V = V.Replace(crlf, "\n");
                        V = V.Replace("\n", crlf);
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
            return "exported " + Counter.ToString() + " files";
        }
    }
}
