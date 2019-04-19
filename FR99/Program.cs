using System;

using RiggVar.FR;

namespace FR62
{
    class Program
    {
        public static void Init()
        {
            if (TMain.Controller == null)
            {

                //if IsWinGUI - try to load config from FR62.xml in workspace
                //set to false where not appropriate
                TMain.IsWinGUI = true;
                TMain.ReadWorkspaceInfo = true; //read from FR*.exe.config

                TWorkspaceInfo wi = new TWorkspaceInfo();
                wi.Load();

                int HardcodedScenario = 1;

                switch (Environment.MachineName.ToLower())
                {
                    case "thinkpad":
                        HardcodedScenario = 1;
                        break;
                    case "gsup3":
                        HardcodedScenario = 2;
                        break;
                    case "gsmac":
                        HardcodedScenario = 1;
                        break;
                    case "gswin":
                        HardcodedScenario = 1;
                        break;

                }
                switch (HardcodedScenario)
                {
                    case 1:
                        wi.WorkspaceType = TWorkspaceManager.WorkspaceType_SharedFS;
                        wi.AutoSaveIni = true;
                        wi.WorkspaceID = 1;
                        break;

                    case 2: //fixed ws on gsup3
                        wi.WorkspaceType = TWorkspaceManager.WorkspaceType_FixedFS;
                        wi.AutoSaveIni = true;
                        wi.WorkspaceID = 1;
                        wi.WorkspaceRoot = @"D:\Test\Workspace\FR64\";
                        break;

                    case 3: //fixed ws on gsmac/vista
                        wi.WorkspaceType = TWorkspaceManager.WorkspaceType_FixedFS;
                        wi.AutoSaveIni = true;
                        wi.WorkspaceID = 1;
                        wi.WorkspaceRoot = @"C:\Users\Public\Documents\Workspace\FR64";
                        break;

                    case 4: //web service on gshsm
                        wi.WorkspaceType = TWorkspaceManager.WorkspaceType_WebService;
                        wi.AutoSaveIni = false;
                        wi.WorkspaceID = 5;
                        wi.WorkspaceUrl = @"http://gshsm/FR64/WorkspaceFiles.asmx";
                        TMain.IsWinGUI = false;
                        break;
                }

                TMain.WorkspaceInfo = wi;

                TMain m = new TMain();
            }

        }

    }

}
