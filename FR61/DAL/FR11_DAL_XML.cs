using System;
using System.IO;
using System.Data;

namespace RiggVar.FR11.DAL
{
    public class FR_DAL_XML : DALBase, IDBEvent3
    {
        public FR_DAL_XML(string cs) : base()
        {
            Dir = cs;
        }
        public string Dir { get; }
        private DirectoryInfo GetDirectoryInfo(int KatID)
        {
            return new DirectoryInfo(Dir + "\\" + KatID.ToString());
        }
        private FileInfo GetFileInfo(int KatID, string EventName)
        {
            return new FileInfo(Dir + "\\" + KatID.ToString() + "\\" + EventName + ".txt");
        }
        private DataTable MakeTableKV()
        {
            DataTable dt = new DataTable("KV");

            DataColumn dc = null;

            dc = new DataColumn("ID", typeof(int));
            dt.Columns.Add(dc);
            dt.PrimaryKey = new System.Data.DataColumn[] { dc };

            dc = new DataColumn("KatID", typeof(int));
            dt.Columns.Add(dc);

            dc = new DataColumn("K", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("U", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("P", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("C", typeof(string));
            dt.Columns.Add(dc);

            return dt;
        }
        private DataTable MakeTableEventNames()
        {
            DataTable dt = new DataTable("KV");

            DataColumn dc = null;

            dc = new DataColumn("ID", typeof(int));
            dt.Columns.Add(dc);
            dt.PrimaryKey = new DataColumn[] { dc };

            dc = new DataColumn("FileName", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("Description", typeof(string));
            dt.Columns.Add(dc);

            return dt;
        }
        public override string Load(int KatID, string EventName)
        {
            string result = "empty";
            try
            {
                FileInfo fi = GetFileInfo(KatID, EventName);
                if (fi.Exists)
                {
                    StreamReader sr = fi.OpenText();
                    result = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch
            {
            }
            return result;
        }
        public override void Save(int KatID, string EventName, string Data)
        {
            try
            {
                FileInfo fi = GetFileInfo(KatID, EventName);
                if (fi.Exists)
                {
                    FileStream fs = fi.OpenWrite();
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(Data);
                    sw.Close();
                }
            }
            catch
            {
            }
        }
        public override void Delete(int KatID, string EventName)
        {
            try
            {
                FileInfo fi = GetFileInfo(KatID, EventName);
                if (fi.Exists)
                {
                    fi.Delete();
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
            eventNames.Clear();

            try
            {
                DirectoryInfo di = GetDirectoryInfo(KatID);
                foreach (FileInfo fi in di.GetFiles("*.txt"))
                {
                    s = fi.Name;
                    s = fi.Name.Substring(0, s.LastIndexOf(fi.Extension));
                    eventNames.Add(s);
                    result += s + "\r\n";
                }
            }
            catch
            {
            }
            return result;
        }
        public override DataSet LoadDataSet()
        {
            DataSet ds = new DataSet();
            try
            {
                ds.ReadXmlSchema(Dir + "\\KV01_Schema.xml");
                ds.ReadXml(Dir + "\\KV01_Data.xml");
            }
            catch
            {
                ds = MakeXMLDataSet();
            }
            return ds;
        }
        private DataSet MakeXMLDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = MakeTableKV();
            ds.Tables.Add(dt);
            DataRow dr;
            try
            {
                DirectoryInfo di = new DirectoryInfo(Dir);
                int i = 1;
                this.UpdateEventNames(0, "");
                foreach (string s in eventNames)
                {
                    dr = dt.NewRow();
                    dr[0] = i; //ID
                    dr[1] = 400; //KatID //###
                    dr[2] = s; //K
                    dt.Rows.Add(dr);
                    i++;
                }
            }
            catch
            {
            }
            return ds;
        }
        //        private void WriteXML(DataSet ds)
        //        {            
        //            ds.WriteXml(Dir + "\\KV01_Data.xml");    
        //            ds.WriteXmlSchema(Dir + "\\KV01_Schema.xml");
        //        }
        public override EventRow LoadEventRow(int KatID, string EventName)
        {
            EventRow result = new EventRow();

            result.KatID = KatID;
            result.EventName = EventName;
            result.EventData = Load(KatID, EventName);
            return result;
        }
        public override DataSet GetEventNames(int paramKatID)
        {
            DataSet ds = new DataSet();
            DataTable dt = MakeTableEventNames();
            ds.Tables.Add(dt);
            DataRow dr;
            try
            {
                //int i = 1;
                //this.UpdateEventNames(paramKatID, "");
                //foreach (string s in eventNames)
                //{
                //    dr = dt.NewRow();
                //    dr["ID"] = i;
                //    dr["FileName"] = s;
                //    dr["Description"] = paramKatID.ToString();
                //    dt.Rows.Add(dr);
                //    i++;
                //}

                DataSet xmlds = LoadDataSet();
                foreach (DataRow xmldr in xmlds.Tables["KV"].Rows)
                {
                    int KatID = Convert.ToInt32(xmldr["KatID"]);
                    if (KatID == paramKatID)
                    {
                        dr = dt.NewRow();
                        dr["ID"] = xmldr["ID"];
                        dr["FileName"] = xmldr["K"];
                        dr["Description"] = xmldr["D"];
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch
            {
            }
            return ds;
        }
        public override string LoadFile(int paramID)
        {
            try
            {
                DataSet ds = LoadDataSet();
                DataRow dr = ds.Tables["KV"].Rows.Find(paramID);
                if (dr != null)
                {
                    int KatID = int.Parse(dr["KatID"].ToString());
                    string EventName = dr["K"].ToString();
                    return Load(KatID, EventName);
                }
            }
            catch
            {
            }
            return string.Empty;
        }
        public override CustomerDetails GetCustomerDetails(string customerID)
        {
            CustomerDetails result = new CustomerDetails();
            if (customerID == "1")
            {
                result.Email = "FRUser@riggvar.org";
                result.Password = "abc";
                result.FullName = "FRUser";
            }
            else if (customerID == "2")
            {
                result.Email = "test@test.com";
                result.Password = "test";
                result.FullName = "test";
            }
            else if (customerID == "3")
            {
                result.Email = "FRAdmin@riggvar.org";
                result.Password = "admin";
                result.FullName = "FRAdmin";
            }
            return result;
        }
        public override string AddCustomer(string fullName, string email, string password)
        {
            return "2";
        }
        public override string Login(string email, string password)
        {
            if ((email == "FRUser@riggvar.org") && (password == "abc"))
            {
                return "1";
            }
            else if ((email == "test@test.com") && (password == "test"))
            {
                return "2";
            }
            else if ((email == "FRAdmin@riggvar.org") && (password == "admin"))
            {
                return "3";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
