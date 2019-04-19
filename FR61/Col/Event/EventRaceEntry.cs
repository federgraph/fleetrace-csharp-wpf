using System;
using System.Xml;
using System.Globalization;

namespace RiggVar.FR
{

    public class TEventRaceEntry : TBOPersistent
    {
        private TEventNode FNode;
        public int CTime; //Points

        public bool IsRacing = true;
        public int Fleet = 1;
        public bool Drop;
        public int DG;
        public int OTime; //ORank
        public int Rank;
        public int PosR;
        public int PLZ;
        public TEnumSet FinishErrors = new TEnumSet(typeof(TFinishError));

        public TEventRaceEntry(TEventNode ru)
        {
            FNode = ru;
        }

        public void Clear()
        {
            Fleet = 0;
            IsRacing = true;
            Drop = false;
            CTime = 0;
            DG = 0;
            OTime = 0;
            Rank = 0;
            PosR = 0;
            Penalty.Clear();
        }

        public override void Assign(object source)
        {
            if (source is TEventRaceEntry e)
            {
                Fleet = e.Fleet;
                IsRacing = e.IsRacing;
                Drop = e.Drop;
                Penalty.Assign(e.Penalty); //QU := e.QU;
                DG = e.DG;
                OTime = e.OTime;
                CTime = e.CTime;
                Rank = e.Rank;
                PosR = e.PosR;
                PLZ = e.PLZ;
            }
            else
            {
                base.Assign(source);
            }
        }

        public string RaceValue
        {
            get
            {
                string sPoints;
                switch (Layout)
                {
                    case 0:
                        sPoints = Points; //Default
                        break;
                    case 1: sPoints = OTime.ToString(); //FinishPos
                        break;
                    case 2:
                        sPoints = Points; //Points
                        break;
                    //                    case 3: 
                    //                        sPoints = QU.ToString();
                    //                        break;
                    //                    case 4: 
                    //                        sPoints = Rank.ToString();
                    //                        break;
                    //                    case 5: 
                    //                        sPoints = PosR.ToString();
                    //                        break;
                    //                    case 6: 
                    //                        sPoints = PLZ.ToString();
                    //                        break;
                    default:
                        sPoints = CTime.ToString();
                        break;
                }
                string result = sPoints;

                string sQU = Penalty.ToString();
                if (sQU != "")
                {
                    result = result + "/" + sQU;
                }

                //{ Parenthesis, runde Klammer }
                //if ((CTime != OTime) && (Ttime l!= 0))
                //    result = result + " (" + OTime.ToString() + ")";

                //{ Bracket,  eckige Klammer }
                if (Drop)
                {
                    result = "[" + result + "]";
                }

                if (!IsRacing)
                {
                    result = "(" + result + ")";
                }

                return result;
            }
            set
            {
                /* use special value 0 to delete a FinishPosition, instead of -1,
                  so that the sum of points is not affected*/
                if (value == "0")
                {
                    OTime = 0;
                }
                else if (Utils.StrToIntDef(value, -1) > 0)
                {
                    OTime = int.Parse(value);
                }
                else if (value.Length > 1 && value[0] == 'F')
                {
                    Fleet = ParseFleet(value);
                }

                else if (value == "x")
                {
                    IsRacing = false;
                }
                else if (value == "-x")
                {
                    IsRacing = true;
                }
                else
                {
                    Penalty.Parse(value);
                }

                //  if (value == "dnf")
                //    QU = Status_DNF;
                //  else if (value == "dsq")
                //    QU = Status_DSQ;
                //  else if (value = 'dns' then
                //    QU = Status_DNS;
                //  else if (value == "ok")
                //    QU = Status_OK;
                //  else if (value == "*")
                //    QU = Status_DSQPending;              
            }
        }
        public int QU
        {
            get => Penalty.AsInteger;
            set => Penalty.AsInteger = value;
        }
        public TPenaltyISAF Penalty { get; } = new TPenaltyISAF();
        public string Points => CPoints.ToString("0.0#", NumberFormatInfo.InvariantInfo);
        public double CPoints
        {
            get => ((double)CTime) / 100; //Round(CTime / 100);
            set => CTime = Convert.ToInt32(value * 100);
        }
        public int CTime1
        {
            get => CTime / 100; //CTime div 100;
            set => CTime = value * 100;
        }
        public int Layout => FNode != null ? FNode.ShowPoints : 0;

        public int ParseFleet(string value)
        {
            if (value.Length >= 2 && value[0] == 'F')
            {
                string s = value.ToUpper().Substring(1);
                char c = s[0];
                switch (c)
                {
                    case 'Y': return 1;
                    case 'B': return 2;
                    case 'R': return 3;
                    case 'G': return 4;
                    case 'M': return 0;
                    default: return Utils.StrToIntDef(s, Fleet);
                }
            }
            return Fleet;
        }

        public void WriteXml(XmlWriter xw)
        {
            xw.WriteStartElement("Penalty");
            Penalty.WriteXml(xw);
            xw.WriteEndElement();

            xw.WriteElementString("CTime", CTime.ToString());
            xw.WriteElementString("Drop", Utils.BoolStr[Drop]);
            xw.WriteElementString("DG", DG.ToString());
            xw.WriteElementString("OTime", OTime.ToString());
            xw.WriteElementString("Rank", Rank.ToString());
            xw.WriteElementString("PosR", PosR.ToString());
            xw.WriteElementString("PLZ", PLZ.ToString());

            xw.WriteStartElement("FinishErrors");
            if (FinishErrors.IsMember(0))
            {
                xw.WriteAttributeString("OTime-Min", Utils.BoolStr[true]);
            }

            if (FinishErrors.IsMember(1))
            {
                xw.WriteAttributeString("OTime-Max", Utils.BoolStr[true]);
            }

            if (FinishErrors.IsMember(2))
            {
                xw.WriteAttributeString("OTime-Duplicate", Utils.BoolStr[true]);
            }

            xw.WriteEndElement();
        }
    }

}
