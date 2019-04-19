namespace RiggVar.FR
{
    public class TBaseFormAdapter
    {
        public virtual bool EditSwitchProps(object sender)
        {
            return false;
        }
        public virtual bool EditBridgeProps(object sender)
        {
            return false;
        }
        public virtual bool EditConnectionProps(TConfigSection section)
        {
            return false;
        }
        public virtual int EditBridgeProviderID(int CurrentProviderID)
        {
            return -1;
        }

        public virtual bool EditScoringModule(object sender)
        {
            return false;
        }

        public virtual bool EditRegattaProps(object sender)
        {
            return false;
        }

        public virtual bool EditUniquaProps(object sender)
        {
            return false;
        }

        public virtual bool EditFleetProps(object sender)
        {
            return false;
        }

        public virtual bool EditSchedule(object sender)
        {
            return false;
        }

        public virtual string ChooseNewEventName()
        {
            return "";
        }
        public virtual string GetNewDocName()
        {
            return "";
        }
        public virtual string ChooseDocAvail(TStringList SL)
        {
            return "";
        }

        public virtual bool ChooseDB()
        {
            return false;
        }

        public virtual void ShowError(string msg)
        {
        }
    }
}
