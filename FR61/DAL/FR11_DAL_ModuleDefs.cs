using System.Collections;
using System.Data;

namespace RiggVar.FR
{
    public class ModuleDef
    {
        public string Name;
        public int KatID;
        public string TransferUrl;

        public ModuleDef(string aName, int aKatID, string aTransferUrl)
        {
            Name = aName;
            KatID = aKatID;
            TransferUrl = aTransferUrl;
        }
    }

    public class ModuleDefs : ArrayList
    {
        public ModuleDef defaultModuleDef;

        public ModuleDefs(string aName, int aKatID, string aTransferUrl) : base()
        {
            defaultModuleDef = new ModuleDef(aName, aKatID, aTransferUrl);
            this.Add(defaultModuleDef);
        }

        public void AddModuleDef(string aName, int aKatID, string aTransferUrl)
        {
            ModuleDef md = new ModuleDef(aName, aKatID, aTransferUrl);
            this.Add(md);
        }

        public ModuleDef FindModuleDef(string name)
        {
            foreach (object o in this)
            {
                ModuleDef md = (ModuleDef)o;
                if (md.Name == name)
                {
                    return md;
                }
            }
            return defaultModuleDef;
        }
    }

    /// <summary>
    /// Table of ModuleDefs
    /// </summary>
    public class ModuleManager
    {
        private ModuleDefs defs;
        public static int defaultKatID = 0;

        public ModuleManager()
        {
            defs = new ModuleDefs("Echo", 0, "FR11_Detail_Echo.aspx");

            defs.AddModuleDef("FleetRace", LookupKatID.FR, "FR11_Detail_FR.aspx");

            defs.AddModuleDef("SKK", LookupKatID.SKK, "FR11_Detail_SKK.aspx");
            defs.AddModuleDef("Rgg", LookupKatID.Rgg, "FR11_Detail_Rgg.aspx");
            defs.AddModuleDef("PGSF", LookupKatID.PGSF, "FR11_Detail_PGSF.aspx");
            defs.AddModuleDef("PGSQ", LookupKatID.PGSQ, "FR11_Detail_PGSQ.aspx");
            defs.AddModuleDef("PGSE", LookupKatID.PGSE, "FR11_Detail_PGSE.aspx");
            defs.AddModuleDef("PGSC", LookupKatID.PGSC, "FR11_Detail_PGSC.aspx");
            defs.AddModuleDef("PGSV", LookupKatID.PGSV, "FR11_Detail_PGSV.aspx");

            defs.AddModuleDef("Readme_en", 1, "FR11_Detail_Echo.aspx");
            defs.AddModuleDef("Readme_de", 2, "FR11_Detail_Echo.aspx");
        }
        public void InitDropDownKatID(IList lc)
        {
            lc.Clear();
            foreach (ModuleDef md in defs)
            {
                lc.Add(md.Name);
            }
        }
#if Web
        public void InitDropDownKatID(System.Web.UI.WebControls.ListItemCollection lc)
        {
            lc.Clear();
            foreach (ModuleDef md in defs)
            {
                lc.Add(md.Name);
            }
        }
#endif
        public int SelectedStringToKatID(string s)
        {
            foreach (ModuleDef md in defs)
            {
                if (md.Name == s)
                {
                    return md.KatID;
                }
            }
            return defaultKatID;
        }

        public string SelectedStringToTransferUrl(string s)
        {
            foreach (ModuleDef md in defs)
            {
                if (md.Name == s)
                {
                    return md.TransferUrl;
                }
            }
            return "";
        }

        public DataTable GetDataTable_K()
        {
            DataTable dt = new DataTable("K");

            //Add Columns
            DataColumn dc = null;

            dc = new DataColumn("Kategory", typeof(string));
            dt.Columns.Add(dc);

            //Add Rows
            DataRow dr;

            foreach (ModuleDef md in defs)
            {
                if (md.KatID != 0)
                {
                    dr = dt.NewRow();
                    dr[0] = md.Name;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

    }
}
