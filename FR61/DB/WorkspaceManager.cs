using System.Diagnostics;

namespace RiggVar.FR
{
    public class TWorkspaceManager
    {

        public const int WorkspaceType_Unknown = 0;
        public const int WorkspaceType_SharedFS = 1;
        public const int WorkspaceType_PrivateFS = 2;
        public const int WorkspaceType_LocalDB = 3;
        public const int WorkspaceType_RemoteDB = 4;
        public const int WorkspaceType_WebService = 5;
        public const int WorkspaceType_FixedFS = 6;
        public const int WorkspaceType_IsolatedFS = 7;

        private readonly int WorkspaceType_Default = WorkspaceType_SharedFS;
        private readonly int WorkspaceID_Default = 1;

        public IDBWorkspace FDBWorkspace;

        public TWorkspaceManager()
        {
            TWorkspaceInfo info = TMain.WorkspaceInfo;
            Debug.Assert(info != null, "WorkspaceInfo must be created before WorkspaceManager");
            Init(info.WorkspaceType, info.WorkspaceID);
        }

        public void InitNew(TWorkspaceInfo wsi)
        {
            FDBWorkspace = null;
            Init(wsi.WorkspaceType, wsi.WorkspaceID);
        }

        public void Init(int aWorkspaceType, int aWorkspaceID)
        {
            WorkspaceID = aWorkspaceID;

            if (FDBWorkspace == null || aWorkspaceType != WorkspaceType)
            {
                switch (aWorkspaceType)
                {
                    case WorkspaceType_SharedFS:
                      FDBWorkspace = new TWorkspaceFiles();
                      UseDB = false;
                      break;
                    //case WorkspaceType_PrivateFS:
                    //  FDBWorkspace = new TWorkspaceFiles();
                    //  UseDB = false;
                    //  break;
                    //case WorkspaceType_LocalDB:
                    //  FDBWorkspace = new TdmPdxWorkspaceFiles(null);
                    //  UseDB = DBWorkspace != null;
                    //  break;
                    //case WorkspaceType_RemoteDB:
                    //  FDBWorkspace = new TSQLWorkspaceFiles();
                    //  UseDB = DBWorkspace != null;
                    //  break;
                    case WorkspaceType_WebService:
                        FDBWorkspace = new TRemoteWorkspace(); //WCF service
                        //FDBWorkspace = new TWorkspaceWeb(); //.asmx service
                        UseDB = DBWorkspace != null;
                        break;
                    case WorkspaceType_FixedFS:
                        FDBWorkspace = new TWorkspaceFiles();
                        UseDB = false;
                        break;

                    default:
                        FDBWorkspace = new TWorkspaceFiles();
                        UseDB = false;
                        break;
                }
            }

            if (DBWorkspace != null)
            {
                //update WorkspaceInfo, FolderInfo and DBWorkspace
                WorkspaceType = aWorkspaceType;
                WorkspaceID = aWorkspaceID;
            }
        }

        public int WorkspaceID
        {
            get => TMain.WorkspaceInfo != null ? TMain.WorkspaceInfo.WorkspaceID : WorkspaceID_Default;
            set
            {
                if (DBWorkspace != null && TMain.WorkspaceInfo != null)
                {
                    DBWorkspace.SetWorkspaceID(value);
                    TMain.WorkspaceInfo.WorkspaceID = value;
                }
            }
        }
        public int WorkspaceType
        {
            get => TMain.WorkspaceInfo != null ? TMain.WorkspaceInfo.WorkspaceType : WorkspaceType_Default;
            set
            {
                if (TMain.FolderInfo != null && TMain.WorkspaceInfo != null)
                {
                    TMain.WorkspaceInfo.WorkspaceType = value;
                    TMain.FolderInfo.WorkspaceInfoChanged();
                }
            }
        }
        public IDBWorkspace DBWorkspace => FDBWorkspace;

        private bool UseDB
        {
            set => TMain.UseDB = value;
        }
    }

}


