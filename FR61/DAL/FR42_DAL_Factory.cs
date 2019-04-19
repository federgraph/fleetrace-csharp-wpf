using RiggVar.FR;
using System.Collections.Specialized;

namespace RiggVar.FR42.DAL
{

    public class TDALFactory
    {
        public string dal = "";

        public TDALFactory()
        {
            dal = DalString;
        }

        public virtual IFR42DB DBFR42
        {
            get
            {
                if (dal == "MDB")
                {
                    return new FR42MDB(MdbConnectionString);
                }
                else if (dal == "MSDE")
                {
                    if (SqlConnectionString == null)
                    {
                        return null;
                    }

                    return new FR42SQL(SqlConnectionString);
                }
                else if (dal == "XML")
                {
                    return new FR42XML(XmlConnectionString);
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
