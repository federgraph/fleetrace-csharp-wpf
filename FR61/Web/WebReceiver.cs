namespace RiggVar.FR
{
    /// <summary>
    /// Zusammenfassung für TWebReceiver.
    /// </summary>
    public class TWebReceiver : TWebReceiverBase
    {
        protected string FEventName;

        protected int FRaceCount;
        protected int FITCount;
        protected int FStartlistCount;

        protected int FRace;
        protected int FIT;
        protected int FBib;

        protected bool FUseFleets;
        protected int FTargetFleetSize;
        protected int FFirstFinalRace;

        protected int FWorkspaceType;
        protected int FWorkspaceID;

        protected string FRequest;

        protected int FReportID;
        protected int FSortColumn;
        protected bool FFlag;

        public TWebReceiver() : base()
        {
        }

        protected override string DispatchMsg(int Action)
        {
            string result = "";

            switch (Action)
            {
                case WebAction.QueryProps:
                    SL.Clear();
                    TWebStub.QueryProps(SL);
                    result = SL.Text;
                    break;

                case WebAction.Open:
                    FEventName = PopStr();
                    TWebStub.OpenEventExecute(FEventName);
                    result = "OpenEvent OK: " + FEventName;
                    break;

                case WebAction.Save: TWebStub.SaveEventExecute(); break;

                case WebAction.SaveAs: 
                    FEventName = PopStr();
                    TWebStub.SaveEventAsExecute(FEventName);
                    result = "SaveEventAs OK: " + FEventName;
                    break;

                case WebAction.Delete:
                    FEventName = PopStr();
                    TWebStub.DeleteEventExecute(FEventName);
                    result = "DeleteEvent OK: " + FEventName;
                    break;

                case WebAction.Connect: 
                    TWebStub.ConnectExecute();
                    result = "Connect OK";
                    break;

                case WebAction.Disconnect: 
                    TWebStub.DisconnectExecute();
                    result = "Disconnect OK";
                    break;

                case WebAction.Backup: 
                    TWebStub.BackupExecute();
                    result = "Backup OK";
                    break;

                case WebAction.Recreate: 
                    TWebStub.RecreateExecute();
                    result = "Recreate OK";
                    break;

                case WebAction.Clear: 
                    TWebStub.ClearExecute();
                    result = "Clear OK";
                    break;

                case WebAction.Plugin: 
                    TWebStub.PluginExecute();
                    result = "Plugin OK";
                    break;

                case WebAction.Plugout: 
                    TWebStub.PlugoutExecute();
                    result = "Plugout OK";
                    break;

                case WebAction.Synchronize: 
                    TWebStub.SynchronizeExecute();
                    result = "Synchronize OK";
                    break;

                case WebAction.Upload: 
                    TWebStub.UploadExecute();
                    result = "Upload OK";
                    break;

                case WebAction.Download: 
                    TWebStub.DownloadExecute();
                    result = "Download OK";
                    break;

                case WebAction.Undo: 
                    TWebStub.UndoExecute();
                    result = "Undo OK";
                    break;

                case WebAction.Redo: 
                    TWebStub.RedoExecute();
                    result = "Redo OK";
                    break;

                case WebAction.LoadTestData: 
                    TWebStub.LoadTestDataExecute();
                    result = "LoadTestData OK";
                    break;

                case WebAction.SynchronizeCache: 
                    TWebStub.SynchronizeCacheExecute();
                    result = "SynchronizeCache OK";
                    break;

                case WebAction.StrictMode: 
                    TWebStub.StrictModeExecute();
                    result = "StrictMode OK";
                    break;

                case WebAction.RelaxedMode: 
                    TWebStub.RelaxedModeExecute();
                    result = "RelaxedMode OK";
                    break;

                case WebAction.ColorCycle: 
                    TWebStub.ColorCycleExecute();
                    result = "ColorCycle OK: " + TMain.BO.EventProps.ColorMode;
                    break;

                case WebAction.FillEventNameList:    
                    SL.Clear();
                    TWebStub.FillEventNameList(SL);
                    result = SL.Text;
                    break;

                case WebAction.GetReport:
                    FRequest = PopStr();
                    result = TWebStub.GetReport(FRequest);
                    break;

                case WebAction.ToTXT: result = TWebStub.ToTXT(); break;
                case WebAction.ToXML: result = TWebStub.ToXML(); break;

                case WebAction.GetJavaScoreXML: result = TWebStub.GetJavaScoreXML(); break;
                case WebAction.GetRaceDataXML: result = TWebStub.GetRaceDataXML(); break;

                case WebAction.GetUndoManagerLog: result = TWebStub.GetUndoManagerLog(); break;
                case WebAction.GetUndoManagerUndo: result = TWebStub.GetUndoManagerUndo(); break;
                case WebAction.GetUndoManagerRedo: result = TWebStub.GetUndoManagerRedo(); break;
                case WebAction.GetUndoManagerUndoRedo: result = TWebStub.GetUndoManagerUndoRedo(); break;

                //case WebAction.GetEventName: result = TWebStub.EventName; break;

                case WebAction.GetStatusString: result = TWebStub.GetStatusString(); break;
                case WebAction.WebStatusString: result = TWebStub.WebStatusString(); break;
                case WebAction.GetConvertedEventData: result = TWebStub.GetConvertedEventData(); break;
                case WebAction.GetIniImage: result = TWebStub.GetIniImage(); break;
                case WebAction.GetWorkspaceInfoReport: result = TWebStub.GetWorkspaceInfoReport(); break;

                case WebAction.FinishReport: result = TWebStub.FinishReport(); break;
                case WebAction.PointsReport: result = TWebStub.PointsReport(); break;
                case WebAction.TimePointReport: result = TWebStub.TimePointReport(); break;
                case WebAction.CacheReport: result = TWebStub.CacheReport(); break;

                case WebAction.GetEventReport:    
                    FReportID = PopInt();
                    FSortColumn = PopInt();
                    FFlag = PopBool();
                    result = TWebStub.GetEventReport(FReportID, FSortColumn, FFlag);
                    break;

                case WebAction.GetRaceReport:    
                    FReportID = PopInt();
                    FRace = PopInt();
                    FIT = PopInt();
                    result = TWebStub.GetRaceReport(FReportID, FRace, FIT);
                    break;

                case WebAction.GetRaceXML:    
                    FRace = PopInt();
                    FIT = PopInt();
                    result = TWebStub.GetRaceXML(FRace, FIT);
                    break;

                case WebAction.GetXMLReport:    
                    FReportID = PopInt();
                    result = TWebStub.GetXMLReport(FReportID);
                    break;

                case WebAction.UpdateWorkspace:    
                    FWorkspaceType = PopInt();
                    FWorkspaceID = PopInt();
                    TWebStub.UpdateWorkspace(FWorkspaceType, FWorkspaceID);
                    result = "UpdateWorkspace OK";
                    break;

                case WebAction.UpdateEventParams:    
                    FRaceCount = PopInt();
                    FITCount = PopInt();
                    FStartlistCount = PopInt();
                    TWebStub.UpdateEventParams(FRaceCount, FITCount, FStartlistCount);
                    result = "UpdateEventParams OK";
                    break;

                case WebAction.UpdateFleetProps:    
                    FUseFleets = PopBool();
                    FTargetFleetSize = PopInt();
                    FFirstFinalRace = PopInt();
                    TWebStub.UpdateFleetProps(FUseFleets, FTargetFleetSize, FFirstFinalRace);
                    result = "UpdateFleetProps OK";
                    break;

                case WebAction.UpdateEntry:    
                    FRequest = PopStr();
                    result = TWebStub.UpdateEntry(FRequest);
                    result = "UpdateEntry OK";
                    break;

                case WebAction.UpdateRaceValue:    
                    FRequest = PopStr();
                    result = TWebStub.UpdateRaceValue(FRequest);
                    break;

                case WebAction.ExecuteCommand:    
                    FRequest = PopStr();
                    result = TWebStub.ExecuteCommand(FRequest);
                    break;

                case WebAction.Handle_TW_Report:    
                    FRace = PopInt();
                    result = TWebStub.Handle_TW_Report(FRace);
                    break;

                case WebAction.Handle_TW_XML:    
                    FRace = PopInt();
                    FIT = PopInt();
                    FBib = PopInt();
                    result = TWebStub.Handle_TW_XML(FRace, FIT, FBib);
                    break;

                case WebAction.Handle_TW_Table:    
                    FRace = PopInt();
                    FIT = PopInt();
                    FBib = PopInt();
                    result = TWebStub.Handle_TW_Table(FRace, FIT, FBib);
                    break;

                case WebAction.Handle_TW_Ajax:    
                    FRace = PopInt();
                    FIT = PopInt();
                    FBib = PopInt();
                    result = TWebStub.Handle_TW_Ajax(FRace, FIT, FBib);
                    break;
            }
            return result;
        }

    }
}
