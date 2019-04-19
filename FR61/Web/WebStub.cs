namespace RiggVar.FR
{
    public class TWebStub
    {
        private static readonly object syncroot = new object();

        public static string EventName => TMain.DocManager.EventName;

        public static int RaceCount => TMain.BO.BOParams.RaceCount;

        public static int ITCount => TMain.BO.BOParams.ITCount;

        public static int StartlistCount => TMain.BO.BOParams.StartlistCount;

        public static bool UseFleets => TMain.BO.EventProps.UseFleets;

        public static int TargetFleetSize => TMain.BO.EventProps.TargetFleetSize;

        public static int FirstFinalRace => TMain.BO.EventProps.FirstFinalRace;

        public static int Race => TMain.GuiManager.Race;

        public static int IT => TMain.GuiManager.IT;

        public static int WorkspaceID => TMain.WorkspaceInfo.WorkspaceID;

        public static int WorkspaceType => TMain.WorkspaceInfo.WorkspaceType;

        public static void QueryProps(TStrings SL)
        {
            lock (syncroot)
            {
                SL.Add(EventName);

                SL.Add(Utils.IntToStr(RaceCount));
                SL.Add(Utils.IntToStr(ITCount));
                SL.Add(Utils.IntToStr(StartlistCount));

                SL.Add(Utils.IntToStr(Race));
                SL.Add(Utils.IntToStr(IT));

                SL.Add(Utils.BoolStr[UseFleets]);
                SL.Add(Utils.IntToStr(TargetFleetSize));
                SL.Add(Utils.IntToStr(FirstFinalRace));

                SL.Add(Utils.IntToStr(WorkspaceType));
                SL.Add(Utils.IntToStr(WorkspaceID));
            }
        }

        public static void OpenEventExecute(string EventName)
        {
            lock (syncroot)
            {
                TMain.GuiManager.OpenEvent(EventName);
            }
        }

        public static void SaveEventExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.SaveEvent();
            }
        }
        
        public static void SaveEventAsExecute(string EventName)
        {
            lock (syncroot)
            {
                TMain.GuiManager.SaveEventAs(EventName);
            }
        }
        
        public static void DeleteEventExecute(string EventName)
        {
            lock (syncroot)
            {
                TMain.GuiManager.DeleteEvent(EventName);
            }
        }

        public static void ConnectExecute()
        {
            lock (syncroot)
            {
                TMain.BO.Connect();
            }
        }
        
        public static void DisconnectExecute()
        {
            lock (syncroot)
            {
                TMain.BO.Disconnect();
            }
        }
        
        public static void BackupExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcBackupExecute(null);
            }
        }
        
        public static void RecreateExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcRecreateExecute(null);
            }
        }
        
        public static void ClearExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcClearExecute(null);
            }
        }

        public static void PluginExecute()
        {
            lock (syncroot)
            {
                TMain.PeerController.Plugin();
            }
        }
        
        public static void PlugoutExecute()
        {
            lock (syncroot)
            {
                TMain.PeerController.Plugout();
            }
        }
        
        public static void SynchronizeExecute()
        {
            lock (syncroot)
            {
                TMain.PeerController.Synchronize();
            }
        }
        
        public static void UploadExecute()
        {
            lock (syncroot)
            {
                TMain.PeerController.Upload(TMain.BO.Save());
            }
        }
        
        public static void DownloadExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.SwapEvent(TMain.PeerController.Download());
            }
        }

        public static void UndoExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcUndoExecute();
            }
        }
        
        public static void RedoExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcRedoExecute();
            }
        }

        public static void LoadTestDataExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.LoadTestDataItemClick(null);
            }
        }
        
        public static void SynchronizeCacheExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.CacheMotor.Synchronize();
            }
        }

        public static void StrictModeExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcStrictModeExecute();
            }
        }
        
        public static void RelaxedModeExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcRelaxedModeExecute();
            }
        }
        
        public static void ColorCycleExecute()
        {
            lock (syncroot)
            {
                TMain.GuiManager.AcColorCycleExecute();
            }
        }

        public static void FillEventNameList(TStringList SL)
        {
            lock (syncroot)
            {
                TMain.DocManager.FillEventNameList(SL);
            }
        }

        public static void FillEventNameList(TWebStringList NL)
        {
            TStringList SL = new TStringList();
            lock (syncroot)
            {
                TMain.DocManager.FillEventNameList(SL);
            }
            NL.Clear();
            for (int i = 0; i < SL.Count; i++)
            {
                NL.Add(SL[i]);
            }
        }

        public static string FillEventNameList()
        {
            TStringList SL = new TStringList();
            lock (syncroot)
            {
                TMain.DocManager.FillEventNameList(SL);
            }
            return SL.Text;
        }

        public static string GetReport(string request)
        {
            lock (syncroot)
            {
                return TMain.BO.Output.GetMsg(request);
            }
        }

        public static string ToTXT()
        {
            lock (syncroot)
            {
                return TMain.BO.ToTXT();
            }
        }

        public static string ToXML()
        {
            lock (syncroot)
            {
                return TMain.BO.ToXML();
            }
        }

        public static string GetJavaScoreXML()
        {
            lock (syncroot)
            {
                return TMain.BO.JavaScoreXML.ToString();
            }
        }

        public static string GetRaceDataXML()
        {
            lock (syncroot)
            {
                return TMain.BO.RaceDataXML.ToString();
            }
        }

        public static string GetUndoManagerLog()
        {
            lock (syncroot)
            {
                return TMain.BO.UndoManager.GetLog();
            }
        }

        public static string GetUndoManagerUndo()
        {
            lock (syncroot)
            {
                return TMain.BO.UndoManager.GetUndo();
            }
        }
        
        public static string GetUndoManagerRedo()
        {
            lock (syncroot)
            {
                return TMain.BO.UndoManager.GetRedo();
            }
        }
        
        public static string GetUndoManagerUndoRedo()
        {
            lock (syncroot)
            {
                return TMain.BO.UndoManager.GetUndoRedo();
            }
        }

        public static string GetStatusString()
        {
            lock (syncroot)
            {
                return TMain.Controller.StatusString;
            }
        }
        
        public static string WebStatusString()
        {
            lock (syncroot)
            {
                return TMain.Controller.WebStatusString;
            }
        }
        
        public static string GetConvertedEventData()
        {
            lock (syncroot)
            {
                return TMain.BO.ConvertedData;
            }
        }
        
        public static string GetIniImage()
        {
            lock (syncroot)
            {
                return TMain.IniImage.ToString();
            }
        }
        
        public static string GetWorkspaceInfoReport()
        {
            lock (syncroot)
            {
                return TMain.WorkspaceInfo.ToString();
            }
        }
        
        public static string FinishReport()
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.FinishReport;
            }
        }
        
        public static string PointsReport()
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.PointsReport;
            }
        }
        
        public static string TimePointReport()
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.TimePointReport;
            }
        }
        
        public static string CacheReport()
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Cache.GetHTM();
            }
        }

        public static string GetEventReport(int ReportID, int SortColumn, bool b)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.GetEventReport(ReportID, SortColumn, b);
            }
        }

        public static string GetRaceReport(int ReportID, int Race, int IT)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.GetRaceReport(ReportID, Race, IT);
            }
        }
        
        public static string GetRaceXML(int Race, int IT)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.GetRaceXml(Race, IT);
            }
        }
        
        public static string GetXMLReport(int ReportID)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.GetXmlReport(ReportID);
            }
        }

        public static void UpdateWorkspace(int WorkspaceType, int WorkspaceID)
        {
            lock (syncroot)
            {
                TMain.GuiManager.UpdateWorkspace(WorkspaceType, WorkspaceID);
            }
        }
        
        public static void UpdateEventParams(int RaceCount, int ITCount, int StartlistCount)
        {
            lock (syncroot)
            {
                TMain.GuiManager.UpdateEventParams(RaceCount, ITCount, StartlistCount);
            }
        }
        
        public static void UpdateFleetProps(bool UseFleets, int TargetFleetSize, int FirstFinalRace)
        {
            lock (syncroot)
            {
                TMain.GuiManager.UpdateFleetProps(UseFleets, TargetFleetSize, FirstFinalRace);
            }
        }

        public static string UpdateEntry(string request)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_Entries(request);
            }
        }
        
        public static string UpdateRaceValue(string request)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_RV(request);
            }
        }
        
        public static string ExecuteCommand(string request)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_Manage(request);
            }
        }
        
        public static string Handle_TW_Report(int Race)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_TW_TimePointReport(Race);
            }
        }
        
        public static string Handle_TW_XML(int Race, int IT, int Bib)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_TW_TimePointXML(Race, IT, Bib);
            }
        }

        public static string Handle_TW_Table(int Race, int IT, int Bib)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_TW_TimePointTable(Race, IT, Bib);
            }
        }

        public static string Handle_TW_Ajax(int Race, int IT, int Bib)
        {
            lock (syncroot)
            {
                return TMain.GuiManager.CacheMotor.Handle_TW_Ajax(Race, IT, Bib);
            }
        }

    }

}
