namespace RiggVar.FR
{
    public enum TThrowoutScheme
    {
        throwoutDefault,
        throwoutBYNUMRACES, // = 1;
        throwoutPERXRACES, // = 2;
        throwoutBESTXRACES, // = 3;
        throwoutNONE // = 4;
    }

    public struct TThrowoutSchemeStruct
    {
        public const int ThrowoutSchemeLow = 0;
        public const int ThrowoutSchemeHigh = 4;

        public string this[TThrowoutScheme o] => AsString(o);

        public int AsInteger(TThrowoutScheme o)
        {
            switch (o)
            {
                case TThrowoutScheme.throwoutDefault: return 0;
                case TThrowoutScheme.throwoutBYNUMRACES: return 1;
                case TThrowoutScheme.throwoutPERXRACES: return 2;
                case TThrowoutScheme.throwoutBESTXRACES: return 3;
                case TThrowoutScheme.throwoutNONE: return 4;
            }
            return 0;
        }
        public string AsString(TThrowoutScheme o)
        {
            switch (o)
            {
                case TThrowoutScheme.throwoutDefault: return "ByNumRaces";
                case TThrowoutScheme.throwoutBYNUMRACES: return "ByNumRaces";
                case TThrowoutScheme.throwoutPERXRACES: return "PerXRaces";
                case TThrowoutScheme.throwoutBESTXRACES: return "BestXRaces";
                case TThrowoutScheme.throwoutNONE: return "None";
            }
            return string.Empty;
        }
    }

    public enum TScoringSystem
    {
        LowPoint,
        BonusPoint,
        BonusPointDSV
    }

    public struct TScoringSystemStruct
    {
        public const int ScoringSystemLow = 0;
        public const int ScoringSystemHigh = 2;
        public TScoringSystem FromInteger(int i)
        {
            switch (i)
            {
                case 0: return TScoringSystem.LowPoint;
                case 1: return TScoringSystem.BonusPoint;
                case 2: return TScoringSystem.BonusPointDSV;
                default: return TScoringSystem.LowPoint;
            }
        }

        public string this[TScoringSystem o]
        {
            get
            {
                switch (o)
                {
                    case TScoringSystem.LowPoint: return "Low Point System";
                    case TScoringSystem.BonusPoint: return "Bonus Point System";
                    case TScoringSystem.BonusPointDSV: return "Bonus Point System DSV";
                    default: return string.Empty;
                }
            }
        }

        public string[] ComboStrings
        {
            get
            {
                string[] sa = new string[ScoringSystemHigh + 1];
                for (int i = ScoringSystemLow; i <= ScoringSystemHigh; i++)
                {
                    sa[i] = this[FromInteger(i)];
                }
                return sa;
            }
        }
    }

    public enum TInputMode
    {
        Strict,
        Relaxed
    }
    public struct TInputModeStruct
    {
        public string this[TInputMode o]
        {
            get
            {
                switch (o)
                {
                    case TInputMode.Relaxed: return "Relaxed";
                    case TInputMode.Strict: return "Strict";
                    default: return string.Empty;
                }
            }
        }
    }

    public class TEventProps : TLineParser
    {
        public static TInputModeStruct InputModeStrings;
        public static TThrowoutSchemeStruct ThrowoutSchemeStruct;
        public static TScoringSystemStruct ScoringSystemStruct;

        public TBO BO; //handle of event business object

        //JavaScore Props
        public string EventName;
        public string EventDates;
        public string HostClub;
        public string PRO; //Principal Race Officer (Wettfahrtleiter)
        public string JuryHead;
        public TScoringSystem ScoringSystem;
        public int ScoringSystem2;//external scoring system code, pass on as is via proxy
        public int Throwouts;
        public TThrowoutScheme ThrowoutScheme;
        public bool FirstIs75;
        public bool ReorderRAF;

        //Uniqua Props
        public bool ShowCupColumn;
        public bool EnableUniquaProps; //override calculated values
        public int UniquaGemeldet; //Count of Entries
        public int UniquaGesegelt; //Count of Races
        public int UniquaGezeitet; //Count of Entries at start
        public double FFaktor;
        public bool NormalizedOutput = false;
        public string SortColName = "";

        //Other
        public bool IsTimed;

        public TEventProps(TBO abo)
        {
            BO = abo;
            LoadDefaultData();
        }
        public int Gemeldet
        {
            get => EnableUniquaProps ? UniquaGemeldet : FRGemeldet();
            set => UniquaGemeldet = value;
        }
        public int Gesegelt
        {
            get => EnableUniquaProps ? UniquaGesegelt : FRGesegelt();
            set => UniquaGesegelt = value;
        }
        public int Gezeitet
        {
            get => EnableUniquaProps ? UniquaGezeitet : FRGezeitet();
            set => UniquaGezeitet = value;
        }
        public double Faktor
        {
            get => FFaktor;
            set
            {
                if ((value > 0.1) && (value < 10))
                {
                    FFaktor = value;
                }
            }
        }
        public string DivisionName
        {
            get => BO.cTokenB;
            set => BO.DivisionName = value;
        }
        public TInputMode InputMode
        {
            get => BO.EventBO.RelaxedInputMode ? TInputMode.Relaxed : TInputMode.Strict;
            set
            {
                if (value == TInputMode.Relaxed)
                {
                    BO.EventBO.RelaxedInputMode = true;
                }
                else
                {
                    BO.EventBO.RelaxedInputMode = false;
                }
            }
        }
        /// <summary>
        /// specify what is shown in Field DN (DisplayName)
        /// </summary>
        public string FieldMap
        {
            get => BO.StammdatenNode.Collection.FieldMap;
            set => BO.StammdatenNode.Collection.FieldMap = value;
        }
        public string FieldCaptions
        {
            get => BO.StammdatenNode.Collection.FieldCaptions;
            set => BO.StammdatenNode.Collection.FieldCaptions = value;
        }
        public string FieldCount
        {
            get => BO.StammdatenNode.Collection.FieldCount.ToString();
            set
            {
                int i = Utils.StrToIntDef(value, -1);
                if (i != -1)
                {
                    BO.StammdatenNode.Collection.FieldCount = i;
                }
            }
        }
        public string NameFieldCount
        {
            get => BO.EventBO.NameFieldCount.ToString();
            set
            {
                int i = Utils.StrToIntDef(value, -1);
                if (i != -1)
                {
                    BO.EventBO.NameFieldCount = i;
                }
            }
        }
        public string NameFieldOrder
        {
            get => BO.EventBO.NameFieldOrder;
            set => BO.EventBO.NameFieldOrder = value;
        }
        public string RaceLayout
        {
            get
            {
                if (BO.EventNode.ShowPoints == TEventNode.Layout_Finish)
                {
                    return "Finish";
                }
                else
                {
                    return "Points"; //default
                }
            }
            set
            {
                if (value == "Finish")
                {
                    BO.EventNode.ShowPoints = TEventNode.Layout_Finish;
                }
                else
                {
                    BO.EventNode.ShowPoints = TEventNode.Layout_Points;
                }
            }
        }
        public string NameSchema
        {
            get
            {
                switch (FieldNames.SchemaCode)
                {
                    case 2: return "NX";
                    case 1: return "LongNames";
                    default: return "";
                }
            }
            set
            {
                switch (value)
                {
                    case "NX": FieldNames.SchemaCode = 2; break;
                    case "LongNames": FieldNames.SchemaCode = 1; break;
                    default: FieldNames.SchemaCode = 0; break;
                }
            }
        }
        public string DetailUrl { get; set; } = "";
        public string EventNameID { get; set; } = "";
        public string NormalizeOutput
        {
            get => NormalizedOutput ? "true" : "false";
            set => NormalizedOutput = (value != null && value.ToLower().StartsWith("t"));
        }

        public bool ShowPosRColumn
        {
            get => BO.EventNode.ShowPosRColumn;
            set => BO.EventNode.ShowPosRColumn = value;
        }

        public bool ShowPLZColumn
        {
            get => BO.EventNode.ShowPLZColumn;
            set => BO.EventNode.ShowPLZColumn = value;
        }

        public string ColorMode
        {
            get
            {
                switch (BO.EventNode.ColorMode)
                {
                    case TColorMode.ColorMode_Fleet: return "Fleet";
                    case TColorMode.ColorMode_None: return "None";
                    default: return "Normal";
                }
            }
            set
            {
                if (value == "Fleet")
                {
                    BO.EventNode.ColorMode = TColorMode.ColorMode_Fleet;
                }
                else if (value == "None")
                {
                    BO.EventNode.ColorMode = TColorMode.ColorMode_None;
                }
                else
                {
                    BO.EventNode.ColorMode = TColorMode.ColorMode_Error;
                }
            }
        }
        public bool UseCompactFormat
        {
            get => BO.UseCompactFormat;
            set => BO.UseCompactFormat = value;
        }

        public bool UseFleets
        {
            get => BO.EventNode.UseFleets;
            set => BO.EventNode.UseFleets = value;
        }

        public bool UseInputFilter
        {
            get => BO.UseInputFilter;
            set => BO.UseInputFilter = value;
        }

        public int TargetFleetSize
        {
            get => BO.EventNode.TargetFleetSize;
            set => BO.EventNode.TargetFleetSize = value;
        }

        public int FirstFinalRace
        {
            get => BO.EventNode.FirstFinalRace;
            set => BO.EventNode.FirstFinalRace = value;
        }

        public bool UseOutputFilter
        {
            get => BO.UseOutputFilter;
            set => BO.UseOutputFilter = value;
        }

        protected override bool ParseKeyValue(string Key, string Value)
        {
            if (Key.StartsWith("EP."))
            {
                Key = Key.Substring("EP.".Length);
            }
            else if (Key.StartsWith("Event.Prop_"))
            {
                Key = Key.Substring("Event.Prop_".Length);
            }

            if (Key == "Name")
            {
                EventName = Value;
            }
            else if (Key == "Dates")
            {
                EventDates = Value;
            }
            else if (Key == "HostClub")
            {
                HostClub = Value;
            }
            else if (Key == "PRO")
            {
                PRO = Value;
            }
            else if (Key == "JuryHead")
            {
                JuryHead = Value;
            }
            else if (Key == "ScoringSystem")
            {
                if (Utils.Pos("DSV", Value) > 0)
                {
                    ScoringSystem = TScoringSystem.BonusPointDSV;
                }

                if (Utils.Pos("onus", Value) > 0)
                {
                    ScoringSystem = TScoringSystem.BonusPoint;
                }
                else
                {
                    ScoringSystem = TScoringSystem.LowPoint;
                }
            }
            else if (Key == "ScoringSystem2")
            {
                ScoringSystem2 = Utils.StrToIntDef(Value, ScoringSystem2);
            }
            else if (Key == "Throwouts")
            {
                Throwouts = Utils.StrToIntDef(Value, Throwouts);
            }
            else if (Key == "ThrowoutScheme")
            {
                switch (Value)
                {
                    case "ByNumRaces":
                        ThrowoutScheme = TThrowoutScheme.throwoutBYNUMRACES;
                        break;
                    case "ByBestXRaces":
                        ThrowoutScheme = TThrowoutScheme.throwoutBESTXRACES;
                        break;
                    case "PerXRaces":
                        ThrowoutScheme = TThrowoutScheme.throwoutPERXRACES;
                        break;
                    default:
                        ThrowoutScheme = TThrowoutScheme.throwoutNONE;
                        break;
                }
            }
            else if (Key == "FirstIs75")
            {
                FirstIs75 = Value.ToLower().StartsWith("t");
            }
            else if (Key == "ReorderRAF")
            {
                ReorderRAF = Value.ToLower().StartsWith("t");
            }
            else if (Key == "ColorMode")
            {
                ColorMode = Value;
            }
            else if (Key == "UseFleets")
            {
                UseFleets = Utils.IsTrue(Value);
            }
            else if (Key == "TargetFleetSize")
            {
                TargetFleetSize = Utils.StrToIntDef(Value, TargetFleetSize);
            }
            else if (Key == "FirstFinalRace")
            {
                FirstFinalRace = Utils.StrToIntDef(Value, FirstFinalRace);
            }
            else if (Key == "IsTimed")
            {
                IsTimed = Utils.IsTrue(Value);
            }
            else if (Key == "UseCompactFormat")
            {
                UseCompactFormat = Utils.IsTrue(Value);
            }
            else if (Key == "ShowPosRColumn")
            {
                ShowPosRColumn = Utils.IsTrue(Value);
            }
            else if (Key == "ShowCupColumn")
            {
                ShowCupColumn = Utils.IsTrue(Value);
            }
            else if (Key == "Uniqua.Enabled")
            {
                EnableUniquaProps = Utils.IsTrue(Value);
            }
            else if (Key == "Uniqua.Gesegelt")
            {
                UniquaGesegelt = Utils.StrToIntDef(Value, Gesegelt);
            }
            else if (Key == "Uniqua.Gemeldet")
            {
                UniquaGemeldet = Utils.StrToIntDef(Value, Gemeldet);
            }
            else if (Key == "Uniqua.Gezeitet")
            {
                UniquaGezeitet = Utils.StrToIntDef(Value, Gezeitet);
            }
            else if (Key == "Uniqua.Faktor")
            {
                Faktor = Utils.StrToFloatDef(Value, Faktor);
            }
            else if (Key == "DivisionName")
            {
                DivisionName = Value;
            }
            else if (Key == "InputMode" || Key == "IM")
            {
                if (Value.ToLower() == "strict")
                {
                    InputMode = TInputMode.Strict;
                }
                else
                {
                    InputMode = TInputMode.Relaxed;
                }
            }
            else if (Key == "FieldMap")
            {
                FieldMap = Value;
            }
            else if (Key == "FieldCaptions")
            {
                FieldCaptions = Value;
            }
            else if (Key == "FieldCount")
            {
                FieldCount = Value;
            }
            else if (Key == "NameFieldCount")
            {
                NameFieldCount = Value;
            }
            else if (Key == "NameFieldOrder")
            {
                NameFieldOrder = Value;
            }
            else if (Key == "RaceLayout")
            {
                RaceLayout = Value;
            }
            else if (Key == "NameSchema")
            {
                NameSchema = Value;
            }
            else if (Key == "DetailUrl")
            {
                DetailUrl = Value;
            }
            else if (Key == "EventNameID")
            {
                EventNameID = Value;
            }
            else if (Key == "NormalizeOutput")
            {
                NormalizeOutput = Value;
            }
            else if (Key == "SortColName")
            {
                SortColName = Value;
            }
            else
            {
                return false;
            }

            return true;
        }
        public void SaveProps(TStrings SLBackup)
        {
            SLBackup.Add("EP.Name = " + EventName);

            if (!string.IsNullOrEmpty(EventDates))
            {
                SLBackup.Add("EP.Dates = " + EventDates);
            }

            if (!string.IsNullOrEmpty(HostClub))
            {
                SLBackup.Add("EP.HostClub = " + HostClub);
            }

            if (!string.IsNullOrEmpty(PRO))
            {
                SLBackup.Add("EP.PRO = " + PRO);
            }

            if (!string.IsNullOrEmpty(JuryHead))
            {
                SLBackup.Add("EP.JuryHead = " + JuryHead);
            }

            SLBackup.Add("EP.ScoringSystem = " + ScoringSystemStruct[ScoringSystem]);

            if (ScoringSystem2 != 0)
            {
                SLBackup.Add("EP.ScoringSystem2 = " + ScoringSystem2.ToString());
            }

            SLBackup.Add("EP.Throwouts = " + Throwouts.ToString());

            if (ThrowoutScheme != TThrowoutScheme.throwoutBYNUMRACES)
            {
                SLBackup.Add("EP.ThrowoutScheme = " + ThrowoutSchemeStruct[ThrowoutScheme]);
            }

            if (FirstIs75)
            {
                SLBackup.Add("EP.FirstIs75 = " + Utils.BoolStr[FirstIs75]);
            }

            if (!ReorderRAF)
            {
                SLBackup.Add("EP.ReorderRAF = " + Utils.BoolStr[ReorderRAF]);
            }

            SLBackup.Add("EP.DivisionName = " + DivisionName);
            SLBackup.Add("EP.InputMode = " + InputModeStrings[InputMode]);
            SLBackup.Add("EP.RaceLayout = " + RaceLayout);
            SLBackup.Add("EP.NameSchema = " + NameSchema);
            SLBackup.Add("EP.FieldMap = " + FieldMap);
            SLBackup.Add("EP.FieldCaptions = " + FieldCaptions);
            SLBackup.Add("EP.FieldCount = " + FieldCount);
            SLBackup.Add("EP.NameFieldCount = " + NameFieldCount);
            SLBackup.Add("EP.NameFieldOrder = " + NameFieldOrder);
            SLBackup.Add("EP.ColorMode = " + ColorMode);
            SLBackup.Add("EP.UseFleets = " + Utils.BoolStr[UseFleets]);
            SLBackup.Add("EP.TargetFleetSize = " + TargetFleetSize.ToString());
            SLBackup.Add("EP.FirstFinalRace = " + FirstFinalRace.ToString());
            SLBackup.Add("EP.IsTimed = " + Utils.BoolStr[IsTimed]);

            SLBackup.Add("EP.UseCompactFormat = " + Utils.BoolStr[UseCompactFormat]);
            if (ShowPosRColumn)
            {
                SLBackup.Add("EP.ShowPosRColumn = " + Utils.BoolStr[ShowPosRColumn]);
            }

            if (ShowCupColumn)
            {
                SLBackup.Add("EP.ShowCupColumn = " + Utils.BoolStr[ShowCupColumn]);
            }

            if (ShowCupColumn)
            {
                SLBackup.Add("EP.Uniqua.Faktor = " + Faktor.ToString("F2"));
                SLBackup.Add("EP.Uniqua.Enabled  = " + Utils.BoolStr[EnableUniquaProps]);
                SLBackup.Add("EP.Uniqua.Gesegelt = " + Gesegelt.ToString());
                SLBackup.Add("EP.Uniqua.Gemeldet = " + Gemeldet.ToString());
                SLBackup.Add("EP.Uniqua.Gezeitet = " + Gezeitet.ToString());
            }
            //note: do not write out dynamic props.
        }
        public void LoadDefaultData()
        {
            EventName = "";
            EventDates = "";
            HostClub = "";
            ScoringSystem = TScoringSystem.LowPoint;
            ScoringSystem2 = 0;
            Throwouts = 1;
            ThrowoutScheme = TThrowoutScheme.throwoutBYNUMRACES; //1
            //DivisionName = "*";

            //Ranglisten Props von BO �bernehmen
            ShowCupColumn = false;
            EnableUniquaProps = false;
            Gemeldet = Gemeldet;
            Gezeitet = Gezeitet;
            Gesegelt = Gesegelt;
            Faktor = 1.10;
        }

        public bool EditRegattaProps()
        {
            //result := Main.FormAdapter.EditRegattaProps(Self);
            return false;
        }
        public bool EditUniquaProps()
        {
            //result := Main.FormAdapter.EditUniquaProps(Self);
            return false;
        }
        public bool EditFleetProps()
        {
            //result := Main.FormAdapter.EditFleetProps(Self);
            return false;
        }
        public int FRGemeldet()
        {
            return BO.Gemeldet;
        }
        public int FRGesegelt()
        {
            return BO.Gesegelt;
        }
        public int FRGezeitet()
        {
            return BO.Gezeitet;
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

            cr = cl.AddRow();
            cr.Category = "File";
            cr.FieldName = "UseCompactFormat";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[UseCompactFormat];
            cr.Caption = "UseCompactFormat";
            cr.Description = "use delimited-value tables";

            cr = cl.AddRow();
            cr.Category = "File";
            cr.FieldName = "IsTimed";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[IsTimed];
            cr.Caption = "IsTimed";
            cr.Description = "save space if event is not timed";

            cr = cl.AddRow();
            cr.Category = "Scoring";
            cr.FieldName = "ReorderRAF";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[ReorderRAF];
            cr.Caption = "ReorderRAF";
            cr.Description = "if false, do not shuffle finish position";

            cr = cl.AddRow();
            cr.Category = "Layout";
            cr.FieldName = "ShowPLZColumn";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[ShowPLZColumn];
            cr.Caption = "ShowPLZColumn";
            cr.Description = "show index column for debugging...";

            cr = cl.AddRow();
            cr.Category = "Layout";
            cr.FieldName = "ShowPosRColumn";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[ShowPosRColumn];
            cr.Caption = "ShowPosRColumn";
            cr.Description = "show unique ranking";

            cr = cl.AddRow();
            cr.Category = "File";
            cr.FieldName = "UseOutputFilter";
            cr.FieldType = NameValueFieldType.FTBoolean;
            cr.FieldValue = Utils.BoolStr[UseOutputFilter];
            cr.Caption = "UseOutputFilter";
            cr.Description = "apply filter when saving once";

            cr = cl.AddRow();
            cr.Category = "Layout";
            cr.FieldName = "NameFieldCount";
            cr.FieldType = NameValueFieldType.FTString;
            cr.FieldValue = NameFieldCount;
            cr.Caption = "NameFieldCount";
            cr.Description = "count of name columns in event table display";

            cr = cl.AddRow();
            cr.Category = "Layout";
            cr.FieldName = "NameFieldOrder";
            cr.FieldType = NameValueFieldType.FTString;
            cr.FieldValue = NameFieldOrder;
            cr.Caption = "NameFieldOrder";
            cr.Description = "namefield index string";

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
                if (cr.FieldName == "UseCompactFormat")
                {
                    UseCompactFormat = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "IsTimed")
                {
                    IsTimed = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "ReorderRAF")
                {
                    ReorderRAF = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "ShowPLZColumn")
                {
                    ShowPLZColumn = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "ShowPosRColumn")
                {
                    ShowPosRColumn = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "UseOutputFilter")
                {
                    UseOutputFilter = Utils.IsTrue(cr.FieldValue);
                }
                else if (cr.FieldName == "NameFieldCount")
                {
                    NameFieldCount = cr.FieldValue;
                }
                else if (cr.FieldName == "NameFieldOrder")
                {
                    NameFieldOrder = cr.FieldValue;
                }
            }
        }

    }
}

