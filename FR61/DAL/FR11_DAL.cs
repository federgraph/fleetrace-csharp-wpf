using System;
using System.Data;
using System.Collections.Specialized;

namespace RiggVar.FR11.DAL
{

    public enum FRRole
    {
        User,
        Manager,
        Admin
    }

    public class CustomerDetails
    {
        public string FullName;
        public string Email;
        public string Password;
    }

    public class EventRow
    {
        public int ID;
        public int KatID;
        public string EventName = "";
        public string EventData = "";
        public string Division = "*"; //###DivisionName
        public string Description = "";
        public DateTime CreationDate = System.DateTime.Now;
        public string UserName = "FRUser";
        public string PassWord = "";
    }

    public interface IDBEvent3
    {
        //for desktop application (classic interface)
        string Load(int KatID, string EventName);
        void Save(int KatID, string EventName, string Data);
        void Delete(int KatID, string EventName);
        string UpdateEventNames(int KatID, string EventFilter);

        //for event selection on DataManager page
        StringCollection EventNames();

        //for admin page, 
        //eventName (E), userName (U), passWord (P), divisionName (C) included
        //eventData (V) not included
        DataSet LoadDataSet();
        DataSet SaveDataSet(DataSet ds);

        //for DataManager page
        EventRow LoadEventRow(int KatID, string EventName);
        void SaveEventRow(EventRow er); //not implementd yet

        //for Login and Register pages
        CustomerDetails GetCustomerDetails(string customerID);
        string AddCustomer(string fullName, string email, string password);
        string Login(string email, string password);

        //for Default_Page
        //ID, EventName (K), Desciption (D)
        //where KatID
        DataSet GetEventNames(int paramKatID);
        string LoadFile(int paramID);

        //Import/Export of Data Files
        string ImportDataFiles(string Dir);
        string ExportDataFiles(string Dir);
    }

    public class DALBase
    {
        public static string AdminSchemaNamespace = "http://www.riggvar.net/FR11Admin.xsd";
        public string errormsg = ""; //debugging    
        internal StringCollection eventNames = new StringCollection();

        /// <summary>
        /// Call before saving data to database.
        /// Not usually called when retrieving data because this does not
        /// help in case that CRLF is lost again during transmission to client.
        /// Direct clients should be able to assume that data is in proper 
        /// condition in DB.
        /// </summary>
        /// <param name="s">input string with unknown line ending</param>
        /// <returns>output string with CRLF istead of LF</returns>
        public virtual string EnsureCRLF(string s)
        {
            if (s.Length > 0)
            {
                s = s.Replace("\r\n", "\n");
                s = s.Replace("\n", "\r\n");
            }
            return s;
        }

        public virtual string Load(int KatID, string EventName)
        {
            return string.Empty;
        }
        public virtual void Save(int KatID, string EventName, string Data)
        {
        }
        public virtual void Delete(int KatID, string EventName)
        {
        }
        public virtual string UpdateEventNames(int KatID, string EventFilter)
        {
            return string.Empty;
        }
        public virtual StringCollection EventNames()
        {
            return eventNames;
        }
        public virtual DataSet LoadDataSet()
        {
            return null;
        }
        public virtual DataSet SaveDataSet(DataSet ds)
        {
            return null;
        }
        public virtual EventRow LoadEventRow(int KatID, string EventName)
        {
            return new EventRow();
        }
        public virtual void SaveEventRow(EventRow er)
        {
            //not implemented
        }
        public virtual CustomerDetails GetCustomerDetails(string customerID)
        {
            return new CustomerDetails();
        }
        public virtual string AddCustomer(string fullName, string email, string password)
        {
            return string.Empty;
        }
        public virtual string Login(string email, string password)
        {
            return string.Empty;
        }
        public virtual bool TestCase1()
        {
            return false;
        }
        public virtual DataSet GetEventNames(int paramKatID)
        {
            return null;
        }
        public virtual string LoadFile(int paramID)
        {
            return string.Empty;
        }
        public virtual string ImportDataFiles(string Dir)
        {
            return "not impemented";
        }
        public virtual string ExportDataFiles(string Dir)
        {
            return "not impemented";
        }
    }
}
