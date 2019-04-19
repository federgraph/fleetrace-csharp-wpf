using System;
using System.Collections.Generic;

namespace RiggVar.FR
{
    /// <summary>
    /// Zusammenfassung für WorkspaceRepo.
    /// </summary>
    public class TRepoItems
    {
        public TStringList SLType;
        public TStringList SLUrl;
        public TStringList SLRoot;
        public TStringList SLRepo;

        private List<TWorkspaceInfo> Repo;

        public TRepoItems()
        {
            SLType = new TStringList();
            SLUrl = new TStringList();
            SLRoot = new TStringList();
            SLRepo = new TStringList();
            Init();
        }

        private void Init()
        {
            TStringList SL;

            SL = SLUrl;
            SL.Add("http://localhost/WebApplication/");
            SL.Add("http://gsup3/FR99/");
            SL.Add("http://gshsm/FR64/");
            SL.Add("http://riggvar.net/cgi-bin/FR99/");
            SL.Add("http://localhost:8199/FR99/");

            SL = SLRoot;
            SL.Add(@"<UserHome>\RiggVar Workspace");
            SL.Add(@"D:\Test\Workspace\FR64");
            SL.Add(@"C:\Users\Public\Documents\Workspace\FR64");

            SL = SLType;
            for (int i = 0; i <= 6; i++)
            {
                SL.Add(GetTypeName(i));
            }

            SL = SLRepo;
            Repo = new List<TWorkspaceInfo>();
            for (int i = 0; i <= 5; i++)
            {
                AddRepoItem(i);
                SL.Add(this[i].WorkspaceName);
            }
        }

        private string GetTypeName(int WST)
        {
            switch (WST)
            {
                case TWorkspaceManager.WorkspaceType_Unknown: return "unknown";
                case TWorkspaceManager.WorkspaceType_WebService: return "Web Service";
                case TWorkspaceManager.WorkspaceType_SharedFS: return "Shared FS";
                case TWorkspaceManager.WorkspaceType_PrivateFS: return "Private FS";
                case TWorkspaceManager.WorkspaceType_FixedFS: return "Fixed FS";
                case TWorkspaceManager.WorkspaceType_LocalDB: return "Local DB";
                case TWorkspaceManager.WorkspaceType_RemoteDB: return "Remote DB";
                default: return "invalid";
            }
        }

        private void AddRepoItem(int ScenarioID)
        {
            TWorkspaceInfo ri;
            ri = new TWorkspaceInfo();
            Repo.Add(ri);
            switch (ScenarioID)
            {
                case 0:
                    ri.WorkspaceName = "current"; //placeholder
                    break;

                case 1:
                    ri.WorkspaceName = "gsup3-shared";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_SharedFS;
                    ri.WorkspaceID = 1;
                    ri.WorkspaceUrl = "http://gsup3/FR99/WorkspaceFiles.asmx";
                    ri.WorkspaceRoot = @"D:\Test\Workspace\FR64";
                    ri.AutoSaveIni = false;        
                    break;

                case 2:
                    ri.WorkspaceName = "gsup3-web";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_WebService;
                    ri.WorkspaceID = 5;
                    ri.WorkspaceUrl = "http://gsup3/FR99/WorkspaceFiles.asmx";
                    ri.WorkspaceRoot = "n/a";
                    ri.AutoSaveIni = false;
                    break;

                case 3:
                    ri.WorkspaceName = "gshsm-web";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_WebService;
                    ri.WorkspaceID = 5;
                    ri.WorkspaceUrl = "http://gshsm/FR64/WorkspaceFiles.asmx";
                    ri.WorkspaceRoot = "n/a";
                    ri.AutoSaveIni = false;
                    break;

                case 4:
                    ri.WorkspaceName = "gsup3-fixed";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_FixedFS;
                    ri.WorkspaceID = 1;
                    ri.WorkspaceUrl = "http://gsup3/FR99/WorkspaceFiles.asmx";
                    ri.WorkspaceRoot = @"D:\Test\Workspace\FR64";
                    ri.AutoSaveIni = true;
                    break;

                case 5:
                    ri.WorkspaceName = "gsmac-fixed";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_FixedFS;
                    ri.WorkspaceID = 1;
                    ri.WorkspaceUrl = "http://gsmac/FR64/WorkspaceFiles.asmx";
                    ri.WorkspaceRoot = @"C:\Users\Public\Documents\Workspace\FR64";
                    ri.AutoSaveIni = true;
                    break;

                default:        
                    ri.WorkspaceName = "default-shared";
                    ri.WorkspaceType = TWorkspaceManager.WorkspaceType_SharedFS;
                    ri.WorkspaceID = 1;
                    ri.WorkspaceUrl = "n/a";
                    ri.WorkspaceRoot = "n/a";
                    ri.AutoSaveIni = false;
                    break;
            }
        }

        public int Count
        {
            get { return Repo.Count; }
        }

        public TWorkspaceInfo this [int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                {
                    return Repo[index];
                }
                else
                {
                    return null;
                }
            }
        }

//        public TWorkspaceInfo FindByName(string name)
//        {
//            foreach (TWorkspaceInfo wi in Repo)
//            {
//                if (wi.WorkspaceName == name)
//                    return wi;
//            }
//            return null;
//        }
        
    }
}
