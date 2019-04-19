namespace RiggVar.FR
{

    public class TInputNCP : TBaseNCP
    {
        public TContextMsgQueue MsgQueue;

        public TInputNCP(TServerIntern ts) : base(ts)
        {
            MsgQueue = new TContextMsgQueue();
        }

        public override void InjectMsg(object sender, TMsgSource ms, string s)
        {
            TContextMsg cm = new TContextMsg
            {
                MsgSource = ms,
                Sender = sender,
                msg = s
            };
            HandleMsg(cm);
        }

        public override void HandleMsg(TContextMsg cm)
        {
            TBaseMsgList msg = TMain.MsgFactory.CreateMsg();
            msg.Prot = cm.msg;
            if (msg.DispatchProt())
            {
                if (TMain.WantAutoSync && TMain.GuiManager.CacheMotor != null)
                {
                    TMain.GuiManager.CacheMotor.SynchronizeIfNotActive();
                }

                //Quittung senden
                if ((msg.MsgResult == 0) && (msg.DBID > 0))
                {
                    Server.Reply(cm.Sender, Utils.IntToStr(msg.DBID));
                }

                if (cm.IsSwitchMsg && msg.OutputRequestList != null && msg.OutputRequestList.Count > 0)
                {
                    //bei SKK Calc und Paint auslösen.
                    if (msg.KatID == LookupKatID.SKK)
                    {
                        TMain.BO.Calc();
                        TMain.BO.OutputServer.SendMsg(msg.KatID, cm); //nur intern weitersenden
                    }
                }
                else
                {
                    //msg erst nach Neuberechnung weitersenden
                    cm.IsQueued = true;
                    if (msg.Cmd == TMain.BO.cTokenOutput || msg.Cmd == TMain.BO.cTokenAnonymousOutput)
                    {
                        cm.HasRequest = true;
                        if (msg.OutputRequestList.Count > 1)
                        {
                            cm.OutputRequestList = new TStringList();
                            cm.OutputRequestList.Assign(msg.OutputRequestList);
                        }
                        else
                        {
                            cm.msg = msg.MsgValue;
                        }
                    }
                    MsgQueue.Enqueue(cm);
                }
            }
            //Adapter-Ausgang wieder freischalten
            MsgContext.SwitchLocked = false;
        }

        public void ProcessQueue()
        {
            TContextMsg cm;
            while (MsgQueue.Count() > 0)
            {
                cm = MsgQueue.Dequeue();
                if (cm != null)
                {
                    if (cm.HasRequest)
                    {
                        //todo: compare this with Delphi code
                        if (Utils.Pos(TMain.BO.cTokenOutputXML, cm.msg) > 0)
                        {
                            TMain.BO.Output.WantPageHeader = true;
                        }

                        if (cm.OutputRequestList != null)
                        {
                            System.Diagnostics.Debug.Assert(cm.OutputRequestList.Count > 1);
                            cm.Answer = TMain.BO.Output.GetAll(cm.OutputRequestList);
                        }
                        else
                        {
                            cm.Answer = TMain.BO.Output.GetMsg(cm.msg);
                        }
                        if (!cm.IsAdapterMsg)
                        {
                            Server.Reply(cm.Sender, cm.Answer);
                        }
                        //else if (cm.IsAdapterMsg) //see AdapterInputNCP
                        //    TMain.AdapterBO.InputServer.Server.Reply(cm.Sender, cm.Answer);
                    }
                    else if (TMain.BO.OutputServer != null)
                    {
                        TMain.BO.OutputServer.SendMsg(cm.KatID, cm);
                        TMain.DrawNotifier.ScheduleFullUpdate(
                            this, new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetRace));
                    }
                }
            }
        }

    }
}
