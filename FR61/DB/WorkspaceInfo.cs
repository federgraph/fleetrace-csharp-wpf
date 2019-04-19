using System;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;

namespace RiggVar.FR
{
    public class TWorkspaceInfo
    {
        public TWorkspaceInfo()
        {
            WorkspaceType = TWorkspaceManager.WorkspaceType_Unknown;
            WorkspaceID = 1;
        }

        public void Assign(TWorkspaceInfo wi)
        {
            WorkspaceType = wi.WorkspaceType;
            WorkspaceID = wi.WorkspaceID;
            AutoSaveIni = wi.AutoSaveIni;
            WorkspaceUrl = wi.WorkspaceUrl;
            WorkspaceRoot = wi.WorkspaceRoot;
        }

        public string WorkspaceName { get; set; }

        public int WorkspaceType { get; set; }

        public int WorkspaceID { get; set; }

        public bool AutoSaveIni { get; set; }

        public string WorkspaceUrl { get; set; }

        public string WorkspaceRoot { get; set; }

        public string WorkspaceTypeName
        {
            get
            {
                switch (WorkspaceType)
                {
                    case TWorkspaceManager.WorkspaceType_SharedFS: return "Shared FS";
                    case TWorkspaceManager.WorkspaceType_PrivateFS: return "Private FS";
                    case TWorkspaceManager.WorkspaceType_LocalDB: return "Local DB";
                    case TWorkspaceManager.WorkspaceType_RemoteDB: return "Remote DB";
                    case TWorkspaceManager.WorkspaceType_WebService: return "Web Service";
                    case TWorkspaceManager.WorkspaceType_FixedFS: return "Fixed FS";
                    default: return "";
                }
            }
        }

        public void WorkspaceReport(TStrings Memo)
        {
            Memo.Add(string.Format("WorkspaceInfo.WorkspaceType={0} [{1}]", WorkspaceType, WorkspaceTypeName));
            Memo.Add("WorkspaceInfo.WorkspaceID=" + WorkspaceID.ToString());
            Memo.Add("WorkspaceInfo.AutoSaveIni=" + Utils.BoolStr[AutoSaveIni]);
            Memo.Add("WorkspaceInfo.WorkspaceUrl=" + WorkspaceUrl);
            Memo.Add("WorkspaceInfo.WorkspaceRoot=" + WorkspaceRoot);
        }

        public void WorkspaceReport(StringBuilder sb)
        {
            string crlf = Environment.NewLine;
            sb.Append("WorkspaceTypeName: " + WorkspaceTypeName + crlf);
            sb.Append("WorkspaceType: " + WorkspaceType + crlf);
            sb.Append("WorkspaceID: " + WorkspaceID + crlf);
            sb.Append("AutoSaveIni: " + AutoSaveIni + crlf);
            sb.Append("WorkspaceUrl: " + WorkspaceUrl + crlf);
            sb.Append("WorkspaceRoot: " + WorkspaceRoot + crlf);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            WorkspaceReport(sb);
            return sb.ToString();
        }

        public void Load()
        {
            NameValueCollection c = PlatformDiff.GetAppSettings();
            if (c == null)
            {
                return;
            }

            string s;

            s = c["WorkspaceType"];
            WorkspaceType = Utils.StrToIntDef(s, WorkspaceType);

            s = c["WorkspaceID"];
            WorkspaceID = Utils.StrToIntDef(s, WorkspaceID);

            s = c["WorkspaceUrl"];
            if (s != null && s != "")
            {
                WorkspaceUrl = s;
            }

            s = c["WorkspaceRoot"];
            if (s != null && s != "")
            {
                WorkspaceRoot = s;
            }

            s = c["AutoSaveIni"];
                AutoSaveIni = ReadBool(s, AutoSaveIni);

        }

        private bool ReadBool(string val, bool def)
        {
            return val == null ? def : Utils.IsTrue(val);
        }

    }
}
