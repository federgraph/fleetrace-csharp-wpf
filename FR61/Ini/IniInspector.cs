namespace RiggVar.FR
{
    public class IniInspector
    {
        private IniModifier m = new IniModifier();
        private TIniImage ini;

        public const string C_Options = "Options";
        public const string C_Connection = "Connections";
        public const string C_Bridge = "Bridge";
        public const string C_Switch = "Switch";

        public IniInspector(TIniImage iniImage)
        {
            ini = iniImage;
        }

        public IniModifier Modifier
        {
            get => m;
            set
            {
                if (value != null)
                {
                    m = value;
                }
            }
        }
        public void InspectorOnLoad(object sender)
        {
            TNameValueRowCollection cl;
            TNameValueRowCollectionItem cr;
        
            if (!(sender is TNameValueRowCollection))
            {
                return;
            }

            cl = (TNameValueRowCollection)sender;

            if (m.W_AutoSave)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "AutoSave";
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.FieldValue = Utils.BoolStr[ini.AutoSave];
                cr.Caption = "AutoSave";
                cr.Description = "always save, dominant over NoAutoSave";
            }

            if (m.W_NoAutoSave)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "NoAutoSave";
                cr.FieldValue = Utils.BoolStr[ini.NoAutoSave];
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.Caption = "NoAutoSave";
                cr.Description = "do not ask if AutoSave=false";
            }

            if (m.W_CopyRankEnabled)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "CopyRankEnabled";
                cr.FieldValue = Utils.BoolStr[ini.CopyRankEnabled];
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.Caption = "CopyRankEnabled";
                cr.Description = "allow transfer of FinishPos from race to event";
            }

            if (m.W_WantSockets)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "WantSockets";
                cr.FieldValue = Utils.BoolStr[ini.WantSockets];
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.Caption = "WantSockets";
                cr.Description = "yes triggers firewall popup at startup";
            }

            if (m.W_LogProxyXML)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "LogProxyXML";
                cr.FieldValue = Utils.BoolStr[ini.LogProxyXML];
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.Caption = "LogProxyXML";
                cr.Description = "save proxy xml to disk for debugging";
            }

            if (m.W_IsMaster)
            {
                cr = cl.AddRow();
                cr.Category = C_Options;
                cr.FieldName = "IsMaster";
                cr.FieldValue = Utils.BoolStr[ini.IsMaster];
                cr.FieldType = NameValueFieldType.FTBoolean;
                cr.Caption = "IsMaster";
                cr.Description = "master can upload backup";
            }
        }

        public void InspectorOnSave(object sender)
        {
            TNameValueRowCollection cl;
            TNameValueRowCollectionItem cr;
    
            if (!(sender is TNameValueRowCollection))
            {
                return;
            }

            cl = (TNameValueRowCollection)sender;

            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if ( cr.FieldName == "AutoSave" )
                {
                    ini.AutoSave = Utils.IsTrue(cr.FieldValue);
                }
                else if ( cr.FieldName == "NoAutoSave" )
                {
                    ini.NoAutoSave = Utils.IsTrue(cr.FieldValue);
                }
                else if ( cr.FieldName == "CopyRankEnabled" )
                {
                    ini.CopyRankEnabled = Utils.IsTrue(cr.FieldValue);
                }
                else if ( cr.FieldName == "WantSockets" )
                {
                    ini.WantSockets = Utils.IsTrue(cr.FieldValue);
                }
                else if ( cr.FieldName == "LogProxyXML" )
                {
                    ini.LogProxyXML = Utils.IsTrue(cr.FieldValue);
                }
                else if ( cr.FieldName == "IsMaster" )
                {
                    ini.IsMaster = Utils.IsTrue(cr.FieldValue);
                }
            }
        }

    }

}
