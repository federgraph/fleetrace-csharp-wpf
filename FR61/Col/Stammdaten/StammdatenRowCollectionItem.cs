namespace RiggVar.FR
{
    public class TStammdatenRowCollectionItem : TBaseRowCollectionItem<
        TStammdatenColGrid,
        TStammdatenBO,
        TStammdatenNode,
        TStammdatenRowCollection,
        TStammdatenRowCollectionItem,
        TStammdatenColProps,
        TStammdatenColProp
        >
    {
        public int FSNR;

        private string FFN = string.Empty;
        private string FLN = string.Empty;
        private string FSN = string.Empty;
        private string FNC = string.Empty;
        private string FGR = string.Empty;
        private string FPB = string.Empty;

        public TStammdatenRowCollectionItem()
            : base()
        {
        }

        public override void Assign(TStammdatenRowCollectionItem e)
        {
            if (e != null)
            {
                SNR = e.SNR;
                FN = e.FN;
                LN = e.LN;
                SN = e.SN;
                NC = e.NC;
                GR = e.GR;
                PB = e.PB;

                Props.Assign(e.Props);
            }
        }

        public void AssignEntry(TStammdatenEntry e)
        {
            if (e != null)
            {
                SNR = e.SNR;
                FN = e.FN;
                LN = e.LN;
                SN = e.SN;
                NC = e.NC;
                GR = e.GR;
                PB = e.PB;
            }
        }

        public int SNR { get => FSNR; set => FSNR = value; }

        public string FN
        {
            get => FFN;
            set { FFN = value; CalcDisplayName(); }
        }
        public string LN
        {
            get => FLN;
            set { FLN = value; CalcDisplayName(); }
        }
        public string SN
        {
            get => FSN;
            set { FSN = value; CalcDisplayName(); }
        }
        public string NC
        {
            get => FNC;
            set { FNC = value; CalcDisplayName(); }
        }
        public string GR
        {
            get => FGR;
            set { FGR = value; CalcDisplayName(); }
        }
        public string PB
        {
            get => FPB;
            set { FPB = value; CalcDisplayName(); }
        }
        public string DN { get; set; } = string.Empty;
        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1: return FFN;
                    case 2: return FLN;
                    case 3: return FSN;
                    case 4: return FNC;
                    case 5: return FGR;
                    case 6: return FPB;
                    default:
                        {
                            if (index > 0 && index <= FieldCount)
                            {
                                return Props["N" + index.ToString()];
                            }
                            break;
                        }
                }
                return "";
            }
            set
            {
                switch (index)
                {
                    case 1: FFN = value; break;
                    case 2: FLN = value; break;
                    case 3: FSN = value; break;
                    case 4: FNC = value; break;
                    case 5: FGR = value; break;
                    case 6: FPB = value; break;
                    default:
                        {
                            if (index > 0 && index <= FieldCount)
                            {
                                Props["N" + index.ToString()] = value;
                            }
                            break;
                        }
                }
                CalcDisplayName();
            }
        }

        public bool GetFieldUsed(int f)
        {
            foreach (TStammdatenRowCollectionItem cr in Collection)
            {
                if (cr[f] != "")
                {
                    return true;
                }
            }
            return false;
        }

        public TProps Props { get; } = new TProps();

        public void CalcDisplayName()
        {
            Collection.CalcDisplayName(this);
        }

        public int FieldCount => TMain.BO.AdapterParams.FieldCount;

    }

}
