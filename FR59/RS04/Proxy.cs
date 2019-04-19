using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace RiggVar.Scoring
{
    public class TProxyProps 
    {
        public const string p_FRProxy = "FRProxy"; 
        public const string p_Gezeitet = "Gezeitet";

        public const string p_EventProps = "EventProps";

        public const string p_RaceProps = "RaceProps"; 
        public const string p_RCount = "RCount";

        public const string p_Race = "Race";
        public const string p_IsRacing = "IsRacing";
        public const string p_Index = "Index";
        public const string p_FResult = "FResult";
        public const string p_OTime = "OTime";
        public const string p_QU = "QU";
        public const string p_Penalty_Points = "P_Points";
        public const string p_Penalty_Note = "P_Note";
        public const string p_Penalty_Percent = "P_Percent";
        public const string p_Penalty_Time = "P_Time";

        public const string p_Entry = "Entry";  
        public const string p_CPoints = "CPoints";
        public const string p_Rank = "Rank";
        public const string p_PosR = "PosR";
        public const string p_PLZ = "PLZ";
        public const string p_Drop = "Drop";
        public const string p_SNR = "SNR";
        public const string p_IsGezeitet = "IsGezeitet"; 
        public const string p_IsTied = "IsTied";
  
        public const string p_ScoringSystem = "ScoringSystem";
        public const string p_ScoringSystem2 = "ScoringSystem2";
        public const string p_ThrowoutScheme = "ThrowoutScheme";
        public const string p_Throwouts = "Throwouts";
        public const string p_FirstIs75 = "FirstIs75";
        public const string p_ReorderRAF = "ReorderRAF";
        public const string p_DivisionName = "DivisionName";

        public const string p_Fleet = "Fleet";
        public const string p_UseFleets = "UseFleets";
        public const string p_TargetFleetSize = "TargetFleetSize";
        public const string p_FirstFinalRace = "FirstFinalRace";
    }

    /// <summary>
    /// data object,
    /// passed as parameter to calculation engine,
    /// also used in (end to end) unit-testing of scoring module.
    /// </summary>
    public class TFRProxy : TProxyProps
    {
        public TJSEventProps EventProps;
        /// <summary>
        /// RaceProps, at this time not more than a bool (for each race);
        /// RCount elements,
        /// element zero not used,
        /// one element for each real race, starting at index 1.
        /// </summary>
        public bool [] IsRacing;
        public TEntryInfoCollection EntryInfoCollection;
        /// <summary>
        /// Count of Entries with at least one race-result
        /// </summary>
        public int Gezeitet;
        /// <summary>
        /// contain ErrorCode,
        /// e.g. -1 if exeption occured when calculating results
        /// </summary>
        public int FResult;

        public bool UseFleets;
        public int TargetFleetSize;
        public int FirstFinalRace;

        public TFRProxy()
        {
            EventProps = new TJSEventProps();
            IsRacing = new bool [1];
            EntryInfoCollection = new TEntryInfoCollection();
        }

        public void Clear()
        {
            EventProps.Clear();
            IsRacing = new bool [1];
            EntryInfoCollection = new TEntryInfoCollection();
            Gezeitet = 0;
            FResult = 0;
            UseFleets = false;
            TargetFleetSize = 8;
            FirstFinalRace = 20;
        }

        public void Assign(TFRProxy p)
        {
            EventProps.Assign(p.EventProps);

            IsRacing = new bool [p.RCount];
            for (int i = 0; i < RCount; i++)
            {
                IsRacing[i] = p.IsRacing[i];
            }

            EntryInfoCollection.Clear();
            for (int i = 0; i < p.EntryInfoCollection.Count; i++)
            {
                TEntryInfo ei = EntryInfoCollection.Add();
                ei.Assign(p.EntryInfoCollection[i]);
            }

            Gezeitet = p.Gezeitet;
            FResult = p.FResult;
            UseFleets = p.UseFleets;
            TargetFleetSize = p.TargetFleetSize;
            FirstFinalRace = p.FirstFinalRace;
        }

        public bool Equals(TFRProxy p)
        {
            if (RCount != p.RCount)
            {
                return false;
            }

            if (!EventProps.Equals(p.EventProps))
            {
                return false;
            }

            if (Gezeitet != p.Gezeitet)
            {
                return false;
            }

            if (FResult != p.FResult)
            {
                return false;
            }

            if (UseFleets != p.UseFleets)
            {
                return false;
            }

            if (TargetFleetSize != p.TargetFleetSize)
            {
                return false;
            }

            if (FirstFinalRace != p.FirstFinalRace)
            {
                return false;
            }

            if (!EntryInfoCollection.Equals(p.EntryInfoCollection))
            {
                return false;
            }

            for (int i = 0; i < RCount; i++)
            {
                if (IsRacing[i] != p.IsRacing[i])
                {
                    return false;
                }
            }

            return true;
        }
        public void WriteXml(XmlWriter xw)
        {
            xw.WriteStartDocument(); //UTF-16
            xw.WriteStartElement(p_FRProxy);

            xw.WriteAttributeString(p_Gezeitet, Gezeitet.ToString());
            if (FResult != 0)
            {
                xw.WriteAttributeString(p_FResult, FResult.ToString());
            }

            if (UseFleets)
            {
                xw.WriteAttributeString(p_UseFleets, Utils.BoolStr[UseFleets]);
                xw.WriteAttributeString(p_TargetFleetSize, TargetFleetSize.ToString());
                xw.WriteAttributeString(p_FirstFinalRace, FirstFinalRace.ToString());
            }

            EventProps.WriteXml(xw);

            xw.WriteStartElement(p_RaceProps);
            xw.WriteAttributeString(p_RCount, RCount.ToString());
            for (int i = 0; i < RCount; i++)
            {
                if (!IsRacing[i])
                {
                    xw.WriteStartElement(p_Race);
                    xw.WriteAttributeString(p_Index, i.ToString());
                    xw.WriteAttributeString(p_IsRacing, Utils.BoolStr[false]);
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
            
            for (int i = 0; i < EntryInfoCollection.Count; i++)
            {
                EntryInfoCollection[i].WriteXml(xw);
            }

            xw.WriteEndElement();            
        }
        private void ReadXml(XmlReader xr)
        {
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;

                if (xname == p_Gezeitet)
                {
                    Gezeitet = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_FResult)
                {
                    FResult = Utils.StrToIntDef(xvalue, -1);
                }
                else if (xname == p_UseFleets)
                {
                    UseFleets = Utils.IsTrue(xvalue);
                }
                else if (xname == p_TargetFleetSize)
                {
                    TargetFleetSize = Utils.StrToIntDef(xvalue, -1);
                }
                else if (xname == p_FirstFinalRace)
                {
                    FirstFinalRace = Utils.StrToIntDef(xvalue, -1);
                }
            }
        }        
        public void ReadXml(string xmlText)
        {
            Clear();

//            if (! xmlText.StartsWith("<?xml"))
//                return;

            StringReader strReader;

            int i = xmlText.IndexOf("<?xml",0, 8);
            if (i < 0 || i > 3)
            {
                return;
            }

            if (i == 3)
            {
                //ignore unicode filemarker
                strReader = new StringReader(xmlText.Substring(i));
            }
            else
            {
                strReader = new StringReader(xmlText);
            }

            XmlTextReader xr = new XmlTextReader(strReader)
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {    
                    if (xr.HasAttributes)
                    {
                        if (xr.Name == p_FRProxy)
                        {
                            ReadXml(xr);
                        }
                        else if (xr.Name == p_EventProps)
                        {
                            EventProps.ReadXml(xr);
                        }
                        else if (xr.Name == p_RaceProps)
                        {
                            RaceProps_ReadXml(xr);
                        }
                        else if (xr.Name == p_Entry)
                        {
                            TEntryInfo ei = EntryInfoCollection.Add();
                            ei.ReadXml(xr);
                        }
                    }
                }
            }
        }
        private void RaceProps_ReadXml(XmlReader xr)
        {
            //read atrributes of RacePops element
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;

                if (xname == p_RCount)
                {
                    RCount = Utils.StrToIntDef(xvalue, 0);
                }
            }
            //read child elements of RaceProps element
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {    
                    if (xr.HasAttributes)
                    {
                        if (xr.Name == p_Race)
                        {
                            RaceProp_ReadXml(xr);
                        }
                    }
                }
                    //stop at </RaceProps>
                else if (xr.NodeType == XmlNodeType.EndElement)
                {
                    if (xr.Name == p_RaceProps)
                    {
                        break;
                    }
                }
            }
        }
        private void RaceProp_ReadXml(XmlReader xr)
        {
            int index = -1;
            bool isRacing = false;
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;

                if (xname == p_Index)
                {
                    index = Utils.StrToIntDef(xvalue, 0);
                }

                if (xname == p_IsRacing)
                {
                    isRacing = Utils.IsTrue(xvalue);
                }
            }
            if (index > 1 && index < RCount)
            {
                IsRacing[index] = isRacing;
            }
        }
        public int RCount
        {
            get => IsRacing.Length;
            set
            {
                IsRacing = new bool[value];
                for (int i = 1; i < value; i++)
                {
                    IsRacing[i] = true;
                }
            }
        }
    }

    public class TJSEventProps : TProxyProps
    {
        public const int LowPoint = 0;
        public const int Bonus = 1;
        public const int BonusDSV = 2;

        public int ScoringSystem;
        public int ScoringSystem2;
        public int ThrowoutScheme;
        public int Throwouts;
        public bool FirstIs75;
        public bool ReorderRAF;
        public string DivisionName;

        public TJSEventProps()
        {
            Clear();
        }

        public void Clear()
        {
            ScoringSystem = 0;
            ScoringSystem2 = 0;
            ThrowoutScheme = 0;
            Throwouts = 1;
            FirstIs75 = false;
            ReorderRAF = true;
            DivisionName = "*";
        }

        internal void Assign(TJSEventProps ep)
        {
            ScoringSystem = ep.ScoringSystem;
            ScoringSystem2 = ep.ScoringSystem2;
            ThrowoutScheme = ep.ThrowoutScheme;
            Throwouts = ep.Throwouts;
            FirstIs75 = ep.FirstIs75;
            ReorderRAF = ep.ReorderRAF;
            DivisionName = ep.DivisionName;
        }

        public bool Equals(TJSEventProps ep)
        {
            if (ScoringSystem != ep.ScoringSystem)
            {
                return false;
            }

            if (ScoringSystem2 != ep.ScoringSystem2)
            {
                return false;
            }

            if (ThrowoutScheme != ep.ThrowoutScheme)
            {
                return false;
            }

            if (Throwouts != ep.Throwouts)
            {
                return false;
            }

            if (FirstIs75 != ep.FirstIs75)
            {
                return false;
            }

            if (ReorderRAF != ep.ReorderRAF)
            {
                return false;
            }

            if (DivisionName != ep.DivisionName)
            {
                return false;
            }

            return true;
        }

        internal void WriteXml(XmlWriter xw)
        {
            xw.WriteStartElement(p_EventProps);

            xw.WriteAttributeString(p_ScoringSystem, ScoringSystem.ToString());
            xw.WriteAttributeString(p_ScoringSystem2, ScoringSystem2.ToString());
            xw.WriteAttributeString(p_ThrowoutScheme, ThrowoutScheme.ToString());
            xw.WriteAttributeString(p_Throwouts, Throwouts.ToString());
            if (FirstIs75)
            {
                xw.WriteAttributeString(p_FirstIs75, Utils.BoolStr[true]);
            }

            if (! ReorderRAF)
            {
                xw.WriteAttributeString(p_ReorderRAF, Utils.BoolStr[false]);
            }

            xw.WriteAttributeString(p_DivisionName, DivisionName);

            xw.WriteEndElement();            
        }
        internal void ReadXml(XmlReader xr)
        {
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;
                if (xname == p_ScoringSystem)
                {
                    ScoringSystem = Utils.StrToIntDef(xvalue, 0);
                }

                if (xname == p_ScoringSystem2)
                {
                    ScoringSystem2 = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_ThrowoutScheme)
                {
                    ThrowoutScheme = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Throwouts)
                {
                    Throwouts = Utils.StrToIntDef(xvalue, 1);
                }
                else if (xname == p_FirstIs75)
                {
                    FirstIs75 = Utils.IsTrue(xvalue);
                }
                else if (xname == p_DivisionName)
                {
                    DivisionName = xvalue;
                }
            }
        }
    }
    public class TRaceInfo : TProxyProps
    {
        //Input
        public int OTime; //Original Time/FinishPosition
        public int QU; //Encoded Penalty Type ('QUit Packet')
        public double Penalty_Points;
        public string Penalty_Note;
        public int Penalty_Percent;
        public long Penalty_TimePenalty;

        //Output
        public double CPoints; //Calculated Points
        public int Rank;
        public int PosR; //unique/eindeutiges Ranking
        public int PLZ; //PlatzZiffer
        public bool Drop; //IsThrowout
        public int Fleet;
        public bool IsRacing = true;

        private System.Globalization.NumberFormatInfo info = System.Globalization.NumberFormatInfo.InvariantInfo;

        public TRaceInfo()
        {
        }
        internal void Assign(TRaceInfo ri)
        {
            //Input
            OTime = ri.OTime;
            QU = ri.QU;
            Penalty_Points = ri.Penalty_Points;
            Penalty_Note = ri.Penalty_Note;
            Penalty_Percent = ri.Penalty_Percent;
            Penalty_TimePenalty = ri.Penalty_TimePenalty;
            Fleet = ri.Fleet;
            IsRacing = ri.IsRacing;

            //Output
            CPoints = ri.CPoints;
            Rank = ri.Rank;
            PosR = ri.PosR;
            PLZ = ri.PLZ;
            Drop = ri.Drop;
        }
        public bool Equals(TRaceInfo ri)
        {
            if (OTime != ri.OTime)
            {
                return false;
            }

            if (QU != ri.QU)
            {
                return false;
            }

            if (Math.Abs(Penalty_Points - ri.Penalty_Points) > 0.00001)
            {
                return false;
            }

            if (Penalty_Note != ri.Penalty_Note)
            {
                return false;
            }

            if (Penalty_Percent != ri.Penalty_Percent)
            {
                return false;
            }

            if (Penalty_TimePenalty != ri.Penalty_TimePenalty)
            {
                return false;
            }

            if (Fleet != ri.Fleet)
            {
                return false;
            }

            if (IsRacing != ri.IsRacing)
            {
                return false;
            }


            //Output
            if (Math.Abs(CPoints - ri.CPoints) > 0.00001)
            {
                return false;
            }

            if (Rank != ri.Rank)
            {
                return false;
            }

            if (PosR != ri.PosR)
            {
                return false;
            }

            if (PLZ != ri.PLZ)
            {
                return false;
            }

            if (Drop != ri.Drop)
            {
                return false;
            }

            return true;
        }
        internal void WriteXml(XmlWriter xw, int Index)
        {
            xw.WriteStartElement(p_Race);

            xw.WriteAttributeString(p_Index, Index.ToString());

            //Input
            if (OTime != 0)
            {
                xw.WriteAttributeString(p_OTime, OTime.ToString());
            }

            if (QU != 0)
            {
                xw.WriteAttributeString(p_QU, QU.ToString());
            }

            if (Penalty_Points != 0)
            {
                xw.WriteAttributeString(p_Penalty_Points, string.Format(info, "{0:G}", Penalty_Points));
            }

            if (Penalty_Note != null && Penalty_Note != "")
            {
                xw.WriteAttributeString(p_Penalty_Note, Penalty_Note);
            }

            if (Penalty_Percent != 0)
            {
                xw.WriteAttributeString(p_Penalty_Percent, Penalty_Percent.ToString());
            }

            if (Penalty_TimePenalty != 0)
            {
                xw.WriteAttributeString(p_Penalty_Time, Penalty_TimePenalty.ToString());
            }

            if (Fleet != 0)
            {
                xw.WriteAttributeString(p_Fleet, Fleet.ToString());
            }

            if (!IsRacing)
            {
                xw.WriteAttributeString(p_IsRacing, Utils.BoolStr[IsRacing]);
            }

            //Output
            xw.WriteAttributeString(p_CPoints, string.Format(info, "{0:G}", CPoints));
            if (Rank != 0)
            {
                xw.WriteAttributeString(p_Rank, Rank.ToString());
            }

            if (PosR != Rank)
            {
                xw.WriteAttributeString(p_PosR, PosR.ToString());
            }

            if (Index == 0)
            {
                xw.WriteAttributeString(p_PLZ, PLZ.ToString());
            }

            if (Drop)
            {
                xw.WriteAttributeString(p_Drop, Utils.BoolStr[Drop]);
            }

            xw.WriteEndElement();            
        }
        internal void ReadXml(XmlReader xr)
        {
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;

                //Input
                if (xname == p_OTime)
                {
                    OTime = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_QU)
                {
                    QU = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Penalty_Points)
                {
                    Penalty_Points = Convert.ToDouble(xvalue, info);
                }
                else if (xname == p_Penalty_Note)
                {
                    Penalty_Note = xvalue;
                }
                else if (xname == p_Penalty_Percent)
                {
                    Penalty_Percent = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Penalty_Time)
                {
                    Penalty_TimePenalty = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Fleet)
                {
                    Penalty_TimePenalty = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Penalty_Time)
                {
                    Fleet = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_IsRacing)
                {
                    IsRacing = Utils.IsTrue(xvalue);
                }

                //Output
                if (xname == p_CPoints)
                {
                    CPoints = Convert.ToDouble(xvalue, info);
                }
                else if (xname == p_Rank)
                {
                    Rank = Utils.StrToIntDef(xvalue, 0);
                    PosR = Rank;
                }
                else if (xname == p_PosR)
                {
                    PosR = Utils.StrToIntDef(xvalue, 0); //read after Rank!
                }
                else if (xname == p_PLZ)
                {
                    PLZ = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_Drop)
                {
                    Drop = Utils.IsTrue(xvalue);
                }
            }
        }
    }

    public class TEntryInfo : TProxyProps
    {
        public int Index; //Index in EntryInfoCollection
        public int SNR; //Primary Key (SportlerNummer/SailNumber)
        public bool IsGezeitet = true;
        public bool IsTied;
        public List<TRaceInfo> RaceList;
        public TRaceInfo this[int i] => RaceList[i] as TRaceInfo;

        public TEntryInfo()
        {
            RaceList = new List<TRaceInfo>();
        }
        
        internal void Assign(TEntryInfo ei)
        {        
            //FIndex intentionally not copied
            SNR = ei.SNR;
            IsGezeitet = ei.IsGezeitet;
            IsTied = ei.IsTied;

            RaceList.Clear();
            for (int i = 0; i < ei.RCount; i++)
            {
                TRaceInfo ri = new TRaceInfo();
                RaceList.Add(ri);
                ri.Assign(ei[i]);
            }
        }

        public bool Equals(TEntryInfo ei)
        {
            if (Index != ei.Index)
            {
                return false;
            }

            if (SNR != ei.SNR)
            {
                return false;
            }

            if (IsGezeitet != ei.IsGezeitet)
            {
                return false;
            }

            if (IsTied != ei.IsTied)
            {
                return false;
            }

            if (RCount != ei.RCount)
            {
                return false;
            }

            for (int i = 0; i < ei.RCount; i++)
            {
                if (!this[i].Equals(ei[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal void WriteXml(XmlWriter xw)
        {
            xw.WriteStartElement(p_Entry);

            xw.WriteAttributeString(p_Index, Index.ToString());
            xw.WriteAttributeString(p_SNR, SNR.ToString());
            if (!IsGezeitet)
            {
                xw.WriteAttributeString(p_IsGezeitet, Utils.BoolStr[IsGezeitet]);
            }

            if (IsTied)
            {
                xw.WriteAttributeString(p_IsTied, Utils.BoolStr[true]);
            }

            for (int i = 0; i < RCount; i++)
            {
                this[i].WriteXml(xw, i);
            }

            xw.WriteEndElement();            
        }

        internal void ReadXml(XmlReader xr)
        {
            //read attributs
            for (int k = 0; k < xr.AttributeCount; k++)
            {  
                xr.MoveToAttribute(k);
                string xname = xr.Name;
                string xvalue = xr.Value;

                if (xname == p_SNR)
                {
                    SNR = Utils.StrToIntDef(xvalue, 0);
                }
                else if (xname == p_IsGezeitet)
                {
                    IsGezeitet = Utils.IsTrue(xvalue);
                }
                else if (xname == p_IsTied)
                {
                    IsTied = Utils.IsTrue(xvalue);
                }
            }
            //read all Race Child-Elements
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {    
                    if (xr.HasAttributes)
                    {
                        if (xr.Name == p_Race)
                        {
                            TRaceInfo ri = new TRaceInfo();
                            RaceList.Add(ri);                                                        
                            ri.ReadXml(xr);
                        }
                    }
                }
                else if (xr.NodeType == XmlNodeType.EndElement)
                {
                    if (xr.Name == p_Entry)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// number of real races
        /// </summary>
        public int RaceCount => RCount - 1;

        /// <summary>
        /// count of elements in RaceList;
        /// index 0 stores series-info (total points, total rank, ...)
        /// index 1 stores info for 1st race
        /// </summary>
        public int RCount => RaceList.Count;
    }

    public class TEntryInfoCollection
    {
        private List<TEntryInfo> EntryList;
        public TEntryInfo this [int i]
        {
            get 
            {
                if (i < EntryList.Count) //&& i >= 0
                {
                    return EntryList[i] as TEntryInfo;
                }
                else
                {
                    return null;
                }
            }
        }
        public TEntryInfoCollection()
        {
            EntryList = new List<TEntryInfo>();
        }
        public void Clear()
        {
            EntryList.Clear();
        }
        public bool Equals(TEntryInfoCollection eic)
        {
            if (Count != eic.Count)
            {
                return false;
            }

            for (int i = 0; i < EntryList.Count; i++)
            {
                TEntryInfo ei = this[i];
                if (!ei.Equals(eic[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public TEntryInfo FindKey(int SNR)
        {
            foreach (TEntryInfo ei in EntryList)
            {
                if (ei.SNR == SNR)
                {
                    return ei;
                }
            }
            return null;
        }
        public TEntryInfo Add()
        {
            TEntryInfo ei = new TEntryInfo();
            ei.Index = Count;
            EntryList.Add(ei);
            return ei;
        }

        public int Count => EntryList.Count;
    }

}
