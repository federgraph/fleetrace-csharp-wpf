using System.Xml;
using System.Drawing;

namespace RiggVar.FR
{
    public class TEventRowCollectionItem : TBaseRowCollectionItem<
        TEventColGrid,
        TEventBO,
        TEventNode,
        TEventRowCollection,
        TEventRowCollectionItem,
        TEventColProps,
        TEventColProp
        >
    {
        public static Color clFleetMedal = Color.White;
        public static Color clFleetYellow = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xCC);
        public static Color clFleetBlue = Color.FromArgb(0xFF, 0xCC, 0xFF, 0xFF);
        public static Color clFleetRed = Color.FromArgb(0xFF, 0xFF, 0xCC, 0xCC);
        public static Color clFleetGreen = Color.FromArgb(0xFF, 0xCC, 0xFF, 0xCC);

        public static Color clFleetMedalBold = Color.YellowGreen;
        public static Color clFleetYellowBold = Color.Orange;
        public static Color clFleetBlueBold = Color.CornflowerBlue;
        public static Color clFleetRedBold = Color.Tomato;
        public static Color clFleetGreenBold = Color.YellowGreen;

        public TEventRaceEntry[] Race;
        public TEventRaceEntry GRace;
        public int Cup;
        public double RA; //Uniqua-Ranglistenpunkte
        public double QR; //WM Qualifikations-Punkte
        public bool isTied;
        public bool isGezeitet;
        public TEnumSet EntryErrors = new TEnumSet(typeof(TEntryError)); //set of TEntryError;

        public TEventRowCollectionItem()
            : base()
        {
            Race = new TEventRaceEntry[RaceCount + 1];
            for (int i = 0; i < RCount; i++)
            {
                Race[i] = new TEventRaceEntry(TMain.BO.EventNode);
            }
            GRace = Race[0];
        }

        private TStammdatenRowCollectionItem GetSDItem()
        {
            return Ru.StammdatenRowCollection.FindKey(SNR);
        }

        public override void Assign(TEventRowCollectionItem source)
        {
            if (source != null)
            {
                TEventRowCollectionItem o = source;
                SNR = o.SNR;
                Bib = o.Bib;
                for (int i = 0; i < Race.Length; i++)
                {
                    Race[i].Assign(o.Race[i]);
                }
            }
        }

        public void Assign(TEventEntry source)
        {
            if (source != null)
            {
                TEventEntry o = source;
                SNR = o.SNR;
                Bib = o.Bib;
                for (int i = 0; i < Race.Length; i++)
                {
                    Race[i].Assign(o.Race[i]);
                }
            }
        }

        public override void ClearResult()
        {
            TEventRaceEntry ere;
            base.ClearResult();
            for (int r = 1; r <= RaceCount; r++)
            {
                ere = Race[r];
                ere.Clear();
            }
        }
        public override void UpdateCellProp(TEventColProp cp, TCellProp cellProp)
        {
            //base.UpdateCellProp(cp, cellProp);
            int NumID;
            int r;
            cellProp.Color = ColumnToColorDef(cp, cellProp.Color);
            NumID = cp.NumID;
            if (Ru.UseFleets)
            {
                if (TEventColProp.IsRaceNumID(NumID))
                {
                    r = TEventColProp.RaceIndex(NumID);
                    cellProp.GroupColor = FleetColorBold(r, Color.Black);
                }
            }
        }
        public override Color ColumnToColorDef(TEventColProp cp, Color aColor)
        {
            TColorMode ColorMode = Ru.ColorMode;
            if (ColorMode == TColorMode.ColorMode_None)
            {
                return Color.White;
            }
            else
            {
                int NumID = cp.NumID;
                if (NumID == TEventColProp.NumID_Bib)
                {
                    return BibColor(aColor);
                }
                else if (NumID == TEventColProp.NumID_SNR)
                {
                    return SNRColor(aColor);
                }
                else if (TEventColProp.IsRaceNumID(NumID))
                {
                    return RaceColor(NumID, aColor);
                }
                else
                {
                    return aColor;
                }
            }
        }

        private Color FleetColorBold(int r, Color aColor)
        {
            int i = Race[r].Fleet;
            switch (i)
            {
                case 0: return clFleetMedalBold;
                case 1: return clFleetYellowBold;
                case 2: return clFleetBlueBold;
                case 3: return clFleetRedBold;
                case 4: return clFleetGreenBold;
                default: return aColor;
            }
        }

        private Color FleetColor(int r, Color aColor)
        {
            int i = Race[r].Fleet;
            switch (i)
            {
                case 0: return clFleetMedal;
                case 1: return clFleetYellow;
                case 2: return clFleetBlue;
                case 3: return clFleetRed;
                case 4: return clFleetGreen;
                default: return aColor;
            }
        }

        private Color BibColor(Color aColor)
        {
            if (EntryErrors.IsMember((int)TEntryError.error_Duplicate_Bib))
            {
                return Color.Cyan;
            }
            else if (EntryErrors.IsMember((int)TEntryError.error_OutOfRange_Bib))
            {
                return Color.Cyan;
            }
            else
            {
                return aColor;
            }
        }

        private Color SNRColor(Color aColor)
        {
            if (EntryErrors.IsMember((int)TEntryError.error_Duplicate_SNR))
            {
                return Color.Cyan;
            }
            else if (EntryErrors.IsMember((int)TEntryError.error_OutOfRange_SNR))
            {
                return Color.Cyan;
            }
            else
            {
                return aColor;
            }
        }

        private Color RaceErrorColor(int r, Color aColor)
        {
            if (Race[r].FinishErrors.IsMember((int)TFinishError.error_Duplicate_OTime))
            {
                return TColGridColors.clHellRot;
            }
            else if (Race[r].FinishErrors.IsMember((int)TFinishError.error_Contiguous_OTime))
            {
                return Color.Cyan;
            }
            else if (Race[r].FinishErrors.IsMember((int)TFinishError.error_OutOfRange_OTime_Max))
            {
                return Color.Olive;
            }
            else if (Race[r].FinishErrors.IsMember((int)TFinishError.error_OutOfRange_OTime_Min))
            {
                return Color.Olive;
            }
            else
            {
                return aColor;
            }
        }

        private Color RaceColor(int NumID, Color aColor)
        {
            Color result = aColor;
            int r = TEventColProp.RaceIndex(NumID);
            if (r > 0)
            {
                TRaceNode rn = TMain.BO.FindNode('W' + r.ToString());
                if (!rn.IsRacing)
                {
                    result = TColGridColors.clBtnFace;
                }
                else if (Ru.ColorMode == TColorMode.ColorMode_Error)
                {
                    result = RaceErrorColor(r, aColor);
                }
                else if (Ru.ColorMode == TColorMode.ColorMode_Fleet)
                {
                    result = FleetColor(r, aColor);
                }
            }
            return result;
        }

        public string this[int Index] //RaceValue
        {
            get => (Index >= 0) && (Index < Race.Length) ? Race[Index].RaceValue : string.Empty;
            set
            {
                //Debug.WriteLine(string.Format("  RaceValue setter: R{0}.Bib{1} = {2}", Index, Bib, value));
                if ((Index >= 0) && (Index < Race.Length))
                {
                    Race[Index].RaceValue = value;
                }
            }
        }

        public int RCount { get { return Race.Length; } }
        public int RaceCount { get { return TMain.BO.BOParams.RaceCount; } }

        public int Bib { get; set; }

        public int SNR { get; set; }
        public string FN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.FN : string.Empty;
            }
        }

        public string LN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.LN : string.Empty;
            }
        }

        public string SN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.SN : string.Empty;
            }
        }

        public string NC
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.NC : string.Empty;
            }
        }

        public string GR
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.GR : string.Empty;
            }
        }

        public string PB
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.PB : string.Empty;
            }
        }

        public string DN
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd != null ? sd.DN : string.Empty;
            }
        }
        public TProps Props
        {
            get
            {
                TStammdatenRowCollectionItem sd = GetSDItem();
                return sd?.Props;
            }
        }

        public string GPoints => Race[0].Points;
        public int GTime1 => Race[0].CTime1;
        public int GRank => Race[0].Rank;
        public int GPosR => Race[0].PosR;
        public int PLZ => Race[0].PLZ;

        public void WriteXml(XmlWriter xw)
        {
            xw.WriteStartElement("EventRowCollectionItem");

            //data
            xw.WriteStartElement("Data");

            xw.WriteElementString("Bib", Bib.ToString());
            xw.WriteElementString("SNR", SNR.ToString());

            for (int r = 0; r < Race.Length; r++)
            {
                xw.WriteStartElement("Race");
                xw.WriteAttributeString("Index", string.Format("R{0}", r + 1));
                Race[r].WriteXml(xw);
                xw.WriteEndElement();
            }

            xw.WriteElementString("Cup", Cup.ToString());
            xw.WriteElementString("RA", RA.ToString()); //double
            xw.WriteElementString("QR", QR.ToString()); //double
            xw.WriteElementString("isTied", Utils.BoolStr[isTied]);
            xw.WriteElementString("isGezeitet", Utils.BoolStr[isGezeitet]);

            xw.WriteStartElement("EntryErrors");
            if (EntryErrors.IsMember(0))
            {
                xw.WriteAttributeString("Duplicate-SNR", Utils.BoolStr[true]);
            }

            if (EntryErrors.IsMember(1))
            {
                xw.WriteAttributeString("Duplicate-Bib", Utils.BoolStr[true]);
            }

            if (EntryErrors.IsMember(2))
            {
                xw.WriteAttributeString("OutOfRange-Bib", Utils.BoolStr[true]);
            }

            if (EntryErrors.IsMember(3))
            {
                xw.WriteAttributeString("OutOfRange-SNR", Utils.BoolStr[true]);
            }

            xw.WriteEndElement();

            xw.WriteEndElement(); //Data

            //lookups
            xw.WriteStartElement("Lookups");

            xw.WriteElementString("N1", FN);
            xw.WriteElementString("N2", LN);
            xw.WriteElementString("N3", SN);
            xw.WriteElementString("N4", NC);
            xw.WriteElementString("N5", GR);
            xw.WriteElementString("N6", PB);
            //public TProps Props 
            xw.WriteElementString("DN", DN);
            for (int r = 0; r < Race.Length; r++)
            {
                xw.WriteStartElement("RaceValue");
                xw.WriteAttributeString("Race", string.Format("{0}", (r + 1)));
                xw.WriteString(this[r]);
                xw.WriteEndElement();
            }
            xw.WriteElementString("GPoints", GPoints);
            xw.WriteElementString("GTime1", GTime1.ToString());
            xw.WriteElementString("GRank", GRank.ToString());
            xw.WriteElementString("GPosR", GPosR.ToString());
            xw.WriteElementString("PLZ", PLZ.ToString());

            xw.WriteEndElement(); //Lookups


            xw.WriteEndElement(); //EventRowCollectionItem
        }
    }

}
