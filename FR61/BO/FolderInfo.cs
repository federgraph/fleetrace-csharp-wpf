using System;
using System.IO;
using System.Reflection;

namespace RiggVar.FR
{
    public class TFolderInfo
    {
        private string FAppName;
        private string FAppDir;
        private string FWorkspacePath;
        private string FSettingsPath;
        private string FDataPath;
        private string FBackupPath;
        private string FTracePath;
        private string FHelpPath;
        private string FPublishPath;

        private const string StrWorkspace = "RiggVar Workspace";
        private const string StrData = "DBEvent";
        private const string StrBackup = "Backup";
        private const string StrHelp = "Help";
        private const string StrTrace = "Trace";
        private const string StrPublish = "Published";
        private const string StrSettings = "Settings VS2003";
        private const string StrConfigExtension = ".xml";

        public TFolderInfo()
        {
        }

        public string AppName
        {
            get
            {
                if (FAppName == null)
                {
                    if (TMain.IsWebApp)
                    {
                        FAppName = TMain.AppName;
                    }
                    else
                    {
                        FAppName = PlatformDiff.GetAppName();
                    }
                }
                return FAppName;
            }
        }

        public string AppDir
        {
            get
            {
                if (FAppDir == null)
                {
                    FAppDir = PlatformDiff.GetAppDir();
                }
                return FAppDir;
            }
        }

        public string ConfigFileName => SettingsPath + StrConfigExtension;

        private string GetWorkspacePath()
        {

            if (!(FWorkspacePath == null || FWorkspacePath == ""))
            {
                return FWorkspacePath;
            }

            int wt = TMain.WorkspaceInfo.WorkspaceType;
            if (wt < 1 || wt > 6)
            {
                if (TMain.IsWebApp)
                {
                    wt = 6; //WebBroker default
                }
                else if (TMain.IsService)
                {
                    wt = 6; //Service default
                }
                else
                {
                    wt = 1; //GUI App default
                }
            }

            switch (wt)
            {
                //Shared Workspace - use 'RiggVar Workspace' in user documents dir
                case 1:
                    string dn = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    FWorkspacePath = Path.Combine(dn, StrWorkspace);
                    CreateDir(FWorkspacePath);
                    break;

                //Local Workspace - create 'RiggVar Workspace' as subfolder
                case 2:
                    FWorkspacePath = Assembly.GetExecutingAssembly().FullName;
                    FWorkspacePath = Path.Combine(FWorkspacePath, StrWorkspace);
                    CreateDir(FWorkspacePath);
                    break;

                //Local DB
                case 3:
                    FWorkspacePath = "\\";
                    //CreateDir(FWorkspacePath); //( creat root dir within DB )
                    break;

                //Remote DB / WebService
                case 4:
                case 5:
                    FWorkspacePath = "\\";
                    break;

                //Fixed WorkspaceRoot in local FileSystem
                case 6:
                    string s = TMain.WorkspaceInfo.WorkspaceRoot;
                    if (s != null && s != "")
                    {
                        FWorkspacePath = IncludeTrailingPathDelimiter(s);
                    }
                    else
                    {
                        FWorkspacePath = IncludeTrailingPathDelimiter(Path.GetTempPath());
                    }
                    if (!Directory.Exists(FWorkspacePath))
                    {
                        Directory.CreateDirectory(FWorkspacePath); //creates all dirs (ForceDirectories)
                    }

                    break;

            }

            return FWorkspacePath;
        }

        private string IncludeTrailingPathDelimiter(string dn)
        {
            return !dn.EndsWith(Path.DirectorySeparatorChar.ToString()) ? dn + Path.DirectorySeparatorChar : dn;
        }

        private void CreateDir(string dn)
        {
            if (!TMain.Redirector.DBDirectoryExists(dn))
            {
                TMain.Redirector.DBCreateDir(dn);
            }
        }

        public string WorkspacePath
        {
            get
            {
                if (FWorkspacePath == null)
                {
                    return GetWorkspacePath();
                    //string dn = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    //FWorkspacePath = System.IO.Path.Combine(dn, TFolderInfo.StrWorkspace);
                    //if (!TMain.Redirector.DBDirectoryExists(FWorkspacePath))
                    //    TMain.Redirector.DBCreateDir(FWorkspacePath);
                }
                return FWorkspacePath;
            }
        }

        private string WorkspaceSubDir(string s)
        {
            string d = Path.Combine(WorkspacePath, s);
            if (!TMain.Redirector.DBDirectoryExists(d))
            {
                TMain.Redirector.DBCreateDir(d);
            }
            return d;
        }

        public string DataPath
        {
            get
            {
                if (FDataPath == null)
                {
                    FDataPath = WorkspaceSubDir(StrData) + Path.DirectorySeparatorChar;
                }
                return FDataPath;
            }
        }

        public string SettingsPath
        {
            get
            {
                if (FSettingsPath == null)
                {
                    FSettingsPath = WorkspaceSubDir(StrSettings);
                }
                return Path.Combine(FSettingsPath, AppName);
            }
        }

        public string BackupPath
        {
            get
            {
                if (FBackupPath == null)
                {
                    FBackupPath = WorkspaceSubDir(StrBackup);
                }
                return Path.Combine(FBackupPath, AppName);
            }
        }

        public string HelpPath
        {
            get
            {
                if (FHelpPath == null)
                {
                    FHelpPath = WorkspaceSubDir(StrHelp);
                }
                return Path.Combine(FHelpPath, AppName);
            }
        }

        public string TracePath
        {
            get
            {
                if (FTracePath == null)
                {
                    FTracePath = WorkspaceSubDir(StrTrace);
                }
                return Path.Combine(FTracePath, AppName);
            }
        }

        public string PublishPath
        {
            get
            {
                if (FPublishPath == null)
                {
                    FPublishPath = WorkspaceSubDir(StrPublish);
                }
                return Path.Combine(FPublishPath, AppName);
            }
        }

        public void WorkspaceInfoChanged()
        {
            Clear();
        }

        private void Clear()
        {
            FWorkspacePath = "";
            FSettingsPath = "";
            FDataPath = "";
            FBackupPath = "";
            FHelpPath = "";
            FTracePath = "";
            FPublishPath = "";
        }

    }
}
