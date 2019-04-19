using System;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using RiggVar.FR;
using RiggVar.FR42.DAL;

namespace RiggVar.DM
{
    public class TDBEventMDB : IDBEvent
    {
        private FR42MDB DBEvent;

        public TDBEventMDB()
        {
            if (MdbConnectionString != "")
            {
                DBEvent = new FR42MDB(MdbConnectionString);
            }
        }

        protected virtual string MdbConnectionString
        {
            get
            {
                NameValueCollection c = PlatformDiff.GetAppSettings();
                string mdbName = c["mdbName"];

                if (File.Exists(mdbName))
                {
                    string s = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                        "Password=\"\";User ID=Admin;" +
                        "Data Source=";
                    return s + mdbName;
                }
                else
                {
                    return "";
                }
            }
        }

        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            if (DBEvent != null)
            {
                return DBEvent.GetFRData(EventName);
            }
            else
            {
                return "";
            }
        }

        public void Save(int KatID, string EventName, string Data)
        {
            if (DBEvent != null)
            {
                DBEvent.SaveEvent(KatID, EventName, Data);
            }
        }

        public void Delete(int KatID, string EventName)
        {
            if (DBEvent != null)
            {
                DBEvent.DeleteEvent(EventName);
            }
        }

        public string GetEventNames(int KatID)
        {
            if (DBEvent != null)
            {
                StringCollection sc = DBEvent.GetEventList();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < sc.Count; i++)
                {
                    sb.Append(sc[i]);
                    if (i < sc.Count - 1)
                    {
                        sb.Append(Environment.NewLine);
                    }
                }
                return sb.ToString();
            }
            else
            {
                return "";
            }
        }

        public void Close()
        {
            //not implemented
        }

        #endregion
    }

}
