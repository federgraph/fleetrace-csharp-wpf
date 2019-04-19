using RiggVar.FR;
using System.Collections.Specialized;

namespace RiggVar.FR11.DAL
{

    public class TDALFactoryBase
    {
        public string dal = "";
        public TDALFactoryBase()
        {
            dal = DalString;
        }

        public virtual IDBEvent3 DBEvent
        {
            get
            {
                if (dal == "MDB")
                {
                    return new FR_DAL_MDB(MdbConnectionString);
                }
                else if (dal == "MSDE")
                {
                    return new FR_DAL_MSSQL(SqlConnectionString);
                }
                else if (dal == "XML")
                {
                    return new FR_DAL_XML(XmlConnectionString);
                }

                return null;
            }
        }

        public virtual string DalString => ConfigValue("DAL");
        public virtual string XmlConnectionString => ConfigValue("xmlConnectionString");
        public virtual string SqlConnectionString => ConfigValue("sqlConnectionString");

        public virtual string MdbConnectionString
        {
            get
            {
                string mdbName = ConfigValue("mdbName");
                string s = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                    "Password=\"\";User ID=Admin;" +
                    "Data Source=";
                return s + mdbName;
            }
        }

        protected string ConfigValue(string key)
        {
            NameValueCollection c = PlatformDiff.GetAppSettings();
            return c[key];
        }
    }

}
