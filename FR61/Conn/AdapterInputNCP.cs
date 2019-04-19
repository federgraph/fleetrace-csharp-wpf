namespace RiggVar.FR
{
    public class TAdapterInputNCP : TAdapterBaseNCP
    {

        public TAdapterInputNCP(TBaseServer ts) : base(ts)
        {
        }

        public override void HandleMsg(TContextMsg cm)
        {
            cm.DecodeHeader();

            if (cm.MsgType == 'W')
            {
                Server.Reply(cm.Sender, TMain.GuiManager.WebReceiver.Receive(cm.msg));
                return;
            }

//#if Switch
            if (cm.msg == "switch connect")
            {
                MsgContext.SwitchSender = cm.Sender;
                return;
            }
            else if (cm.msg == "switch disconnect")
            {
                MsgContext.SwitchSender = null;
                return;
            }
            if (cm.Sender == MsgContext.SwitchSender && MsgContext.SwitchSender != null)
            {
                cm.MsgSource = TMsgSource.Switch;
            }
            //#endif
            if (TMain.AdapterBO.AdapterInputConnection != null)
            {
                //auf mehrzeilige msg (die einen Request enthält) antworten
                if (cm.msg.IndexOf(".Request.") > -1)
                {
                    //Alternative 1 mit direkter Verwendung von cm
                    TMain.AdapterBO.AdapterInputConnection.HandleContextMsg(cm);
                    Server.Reply(cm.Sender, cm.Answer);

                    //Alternative 2 mit Umkopieren von cm
                    //cm.Answer = TMain.AdapterBO.AdapterInputConnection.HandleMsg(cm.msg);
                    //Server.Reply(cm.Sender, cm.Answer);
                }
                else
                {
                    TGlobalWatches.Instance.MsgIn = cm.msg;
                    MsgContext.SwitchLocked = true; //lock adapter-output for generated messages (Redo)
                    TMain.AdapterBO.AdapterInputConnection.HandleContextMsg(cm);
                    TMain.GuiManager.PlaySound(SoundID.Click01);    
                }
                TMain.DrawNotifier.ScheduleFullUpdate(
                    this, new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetEvent));
            }
        }

    }
}
