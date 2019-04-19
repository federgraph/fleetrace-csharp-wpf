using System;

namespace RiggVar.FR
{

    public enum TGuiAction
    {
        AlwaysDoOnIdle,
        SometimesDoOnIdle,
        acRestore,
        acClear,
        acUndo,
        acRedo,
        acColor,
        acStrict,
        acRelaxed,
        ITChanged,
        RaceChanged,
        CaptionChanged,
        WorkspaceStatusChanged,
        ScheduleEventUpdate,
        ScheduleRaceUpdate,
        ScheduleEventDraw,
        ScheduleRaceDraw,
        InitBridge
    }

    public interface IGuiInterface
    {
        void HandleInform(TGuiAction action);

        void InitViews();
        void DisposeViews();

        void InitCacheGui();
        void DisposeCacheGui();
    }

    public class TGuiManager
    {
        string ExceptionLabel_Caption;

        private int FRace;
        private int FIT;

        public IGuiInterface GuiInterface; //injected reference
        public string AppName;

        public TWebReceiver WebReceiver;
        public TCacheMotor CacheMotor;

        public int Race
        {
            get => FRace;
            set
            {
                if (value > 0 && value <= TMain.BO.BOParams.RaceCount && value != FRace)
                {
                    FRace = value;
                    if (GuiInterface != null)
                    {
                        GuiInterface.HandleInform(TGuiAction.RaceChanged);
                    }
                }
            }
        }

        public int IT
        {
            get => FIT;
            set
            {
                if (value >= 0 && value <= TMain.BO.BOParams.ITCount && value != FIT)
                {
                    FIT = value;
                    if (GuiInterface != null)
                    {
                        GuiInterface.HandleInform(TGuiAction.ITChanged);
                    }
                }
            }
        }

        public TGuiManager()
        {
            AppName = TMain.FolderInfo.AppName;
            TOutputCache.CacheRequestToken = "FR.*.Request.";

            WebReceiver = new TWebReceiver();

            if (TMain.IsWebApp)
            {
                TMain.GuiManager = this;
                InitCache();
            }

            FIT = -1;
            FRace = 0;

            IT = 0;
            Race = 1;
        }

        public void DoOnException(object sender, Exception ex)
        {
            TMain.Logger.Log(ex.Message);
            ExceptionLabel_Caption = ex.Message;
        }

        public void DoOnIdle()
        {
            if (GuiInterface == null)
            {
                TMain.BO.OnIdle();
                CacheMotor.DoOnIdle();
                TMain.PeerController.DoOnIdle();
            }
            else
            {
                //do everything from within GUI
            }
        }

        public void InitViews()
        {
            if (GuiInterface != null)
            {
                GuiInterface.InitViews();
            }
            else
            {
                InitCache();
                InitPeer();
            }
        }

        public void DisposeViews()
        {
            if (GuiInterface != null)
            {
                GuiInterface.DisposeViews();
            }
            else
            {
                DisposeCache();
                DisposePeer();
            }
        }

        public void InitNewBO()
        {
            //if BO is recreated, Views must immediately be reinitialized,
            //because object references become invalid;
            //in MDI all Views are destroyed, in SDI this is not possible
            DisposeViews();
            InitViews();
        }

        #region PeerMethods

        public void InitPeer()
        {
            TMain.Controller.InitPeer();
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.InitBridge);
            }
            else
            {
                TMain.PeerController.Connect();
                TMain.PeerController.OnBackup = new THandleMsgEvent(HandleBackup);
            }
        }
        public void DisposePeer()
        {
            TMain.PeerController.OnBackup = null;
            TMain.PeerController.Disconnect();
        }
        public void HandleBackup(object sender, string EventData)
        {
            if (GuiInterface != null)
            {
                //see FrmFR64, EventHandler is set up in Gui Form using invoke
                //this.Invoke(FSynchronizedHandleBackup, new object[] { null, EventData });
            }
            else
            {
                lock (TMain.Controller)
                {
                    SynchronizedHandleBackup(this, EventData);
                }
            }
        }
        public void SynchronizedHandleBackup(object sender, string EventData)
        {
            SwapEvent(EventData);
        }
        #endregion

        public void OpenEvent(string en)
        {
            string s = TMain.DocManager.DocDownloadByName(en);
            if (s != "")
            {
                SwapEvent(s);
            }
        }

        public void SaveEvent()
        {
            TMain.DocManager.DocSave(TMain.BO);
            TMain.BO.UndoManager.Clear();
        }

        public void SaveEventAs(string en)
        {
            //called from WebInterface, after input of new event name
            TMain.DocManager.EventName = en;
            TMain.DocManager.DocSave(TMain.BO); //not DocSaveAs, because new EventName (en) already given
            UpdateCaption();
            TMain.BO.UndoManager.Clear();
        }

        public void DeleteEvent(string en)
        {
            TMain.DocManager.RawDelete(en);
        }

        public void UpdateWorkspace(int WorkspaceType, int WorkspaceID)
        {
            TMain.Controller.UpdateWorkspace(WorkspaceType, WorkspaceID);
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.WorkspaceStatusChanged);
            }
        }

        public void UpdateEventParams(int RaceCount, int ITCount, int StartlistCount)
        {
            TBOParams newParams = new TBOParams
            {
                RaceCount = RaceCount,
                ITCount = ITCount,
                StartlistCount = StartlistCount
            };
            if (newParams.IsWithinLimits())
            {
                bool wasConnected = TMain.BO.Connected;
                DisposeViews();
                TMain.Controller.RecreateBO(newParams);
                InitNewBO();
                if (wasConnected)
                {
                    TMain.BO.Connect();
                }
            }
        }

        public void UpdateFleetProps(bool UseFleets, int TargetFleetSize, int FirstFinalRace)
        {
            TEventProps Model = TMain.BO.EventProps;
            Model.UseFleets = UseFleets;
            Model.FirstFinalRace = FirstFinalRace;
            Model.TargetFleetSize = TargetFleetSize;
            TMain.BO.EventNode.Modified = true;
        }

        public void SwapEvent(string EventData)
        {
            if (EventData != "")
            {
                bool wasConnected = TMain.BO.Connected;
                DisposeViews();
                FRace = 1;
                FIT = 0;
                TMain.BO.LoadNew(EventData);
                InitNewBO();
                if (wasConnected)
                {
                    TMain.BO.Connect();
                }

                UpdateCaption();

                if (TMain.PeerController.IsEnabled(SwitchOp.Upload))
                {
                    if (TMain.AutoUpload)
                    {
                        TMain.PeerController.Upload(TMain.BO.Save());
                    }
                }

                PlaySound(SoundID.Recycle);
            }
        }

        public void UpdateCaption()
        {
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.CaptionChanged);
            }
        }

        public void AcBackupExecute(object sender)
        {
            TMain.BO.BackupBtnClick();
            TMain.BO.UndoManager.Clear();
        }

        public void AcRecreateExecute(object sender)
        {
            bool wasConnected = TMain.BO.Connected;
            DisposeViews();
            TMain.Controller.RecreateBOFromBackup();
            InitNewBO();
            if (wasConnected)
            {
                TMain.BO.Connect();
            }
        }

        public void AcRestoreExecute(object sender)
        {
            DisposeViews();
            TMain.BO.RestoreBtnClick();
            TMain.BO.Calc();
            TMain.BO.UndoManager.Clear();
            InitViews();
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acRestore);
            }
        }

        public void AcClearExecute(object sender)
        {
            TMain.BO.ClearBtnClick();
            TMain.BO.Calc();
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acClear);
            }

            TMain.BO.UndoManager.Clear();
        }

        public void LoadTestDataItemClick(object sender)
        {
            SwapEvent(TMain.Controller.GetTestData());
        }

        public void InitCache()
        {
            //in GUI-App CacheMotor is created with parent, from within GUI
            if (CacheMotor == null)
            {
                CacheMotor = new TCacheMotor();
            }

            CacheMotor.InputConnection = TMain.BO.InputServer.Server.Connect("Cache.Input");
            if (GuiInterface != null)
            {
                CacheMotor.OutputConnection = TMain.BO.OutputServer.Server.Connect("Cache.Output");
                CacheMotor.OutputConnection.SetOnSendMsg(new THandleContextMsgEvent(CacheMotor.SetOutputMsg));
            }

            CacheMotor.SwapEvent();
            if (GuiInterface != null)
            {
                GuiInterface.InitCacheGui();
            }
        }

        public void DisposeCache()
        {
            if (CacheMotor != null)
            {
                CacheMotor.CacheEnabled = false;
                CacheMotor.InputConnection = null;
                if (GuiInterface != null)
                {
                    CacheMotor.OutputConnection = null;
                    GuiInterface.DisposeCacheGui();
                }
                CacheMotor = null;
            }
        }

        public void ScheduleFullUpdate(object sender, DrawNotifierEventArgs e)
        {
            if (GuiInterface != null && e != null)
            {
                switch (e.DrawTarget)
                {
                    case DrawNotifierEventArgs.DrawTargetEvent:
                        GuiInterface.HandleInform(TGuiAction.ScheduleEventUpdate);
                        break;
                    case DrawNotifierEventArgs.DrawTargetRace:
                        GuiInterface.HandleInform(TGuiAction.ScheduleRaceUpdate);
                        break;
                }
            }
        }

        public void ScheduleDraw(object sender, DrawNotifierEventArgs e)
        {
            if (GuiInterface != null && e != null)
            {
                switch (e.DrawTarget)
                {
                    case DrawNotifierEventArgs.DrawTargetEvent:
                        GuiInterface.HandleInform(TGuiAction.ScheduleEventDraw);
                        break;
                    case DrawNotifierEventArgs.DrawTargetRace:
                        GuiInterface.HandleInform(TGuiAction.ScheduleRaceDraw);
                        break;
                }
            }
        }

        public void AcUndoExecute()
        {
            if (TMain.BO.UndoManager.UndoCount > 0)
            {
                string s = TMain.BO.UndoManager.Undo();
                if (TMain.BO.UndoConnection != null)
                {
                    TMain.BO.EventBO.UndoAgent.UndoLock = true;
                    try
                    {
                        TMain.BO.UndoConnection.InjectMsg(s);
                    }
                    finally
                    {
                        TMain.BO.EventBO.UndoAgent.UndoLock = false;
                    }
                    if (GuiInterface != null)
                    {
                        GuiInterface.HandleInform(TGuiAction.acUndo);
                    }
                }
            }                                    
        }

        public void AcRedoExecute()
        {
            if (TMain.BO.UndoManager.RedoCount > 0)
            {
                string s = TMain.BO.UndoManager.Redo();
                if (TMain.BO.UndoConnection != null)
                {
                    TMain.BO.EventBO.UndoAgent.UndoLock = true;
                    try
                    {
                        TMain.BO.UndoConnection.InjectMsg(s);
                    }
                    finally
                    {
                        TMain.BO.EventBO.UndoAgent.UndoLock = false;
                    }
                    if (GuiInterface != null)
                    {
                        GuiInterface.HandleInform(TGuiAction.acRedo);
                    }
                }
            }                                                        
        }

        public void AcColorModeExecute(TColorMode cm)
        {
            TMain.BO.EventNode.ColorMode = cm;
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acColor);
            }
        }

        public void AcColorCycleExecute()
        {
            TColorMode cm = TMain.BO.EventNode.ColorMode;
            cm = IncColorMode(cm);
            TMain.BO.EventNode.ColorMode = cm;
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acColor);
            }
        }

        public void AcStrictModeExecute()
        {
            TMain.BO.EventBO.RelaxedInputMode = false;
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acStrict);
            }
        }

        public void AcRelaxedModeExecute()
        {
            TMain.BO.EventBO.RelaxedInputMode = true;
            if (GuiInterface != null)
            {
                GuiInterface.HandleInform(TGuiAction.acRelaxed);
            }
        }

        private TColorMode IncColorMode(TColorMode cm)
        {
            if (cm == TColorMode.ColorMode_Fleet)
            {
                cm = TColorMode.ColorMode_None;
            }
            else
            {
                cm++;
            }
            return cm;
        }

        public void PlaySound(SoundID soundID)
        {
            TMain.SoundManager.PlaySound(soundID);
        }

    }

}
