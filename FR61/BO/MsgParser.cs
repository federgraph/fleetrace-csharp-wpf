namespace RiggVar.FR
{
    public class TMsgParser : TBaseMsgParser
    {
        public string sAthlete;
        public string sPos;
        public string sMsgID;
        
        public string sDivision;
        public string sRunID;
        public string sBib;
        public string sCommand;
        public string sValue;
        
        public TBO BO;

        public TMsgParser() : base()
        {
            BO = TMain.BO;
        }

        public override string LongToken(string t)
        {
            return FRLongToken(t);
        }

        public static string FRLongToken(string t)
        {
            string result = "";

            if (t == "XX" )
            {
                result = "TestMessage";
            }

            //Command
            if (t == "QU" )
            {
                result = "Status";
            }
            else if (t == "FM")
            {
                result = "Fleet"; //FleetMembership
            }
            else if (t == "ST" )
            {
                result = "StartTime";
            }
            else if (Utils.Copy(t, 1, 2) == "IT" )
            {
                result = "IntermediateTime" + Utils.Copy(t, 3, t.Length);
            }
            else if (t == "FT" )
            {
                result = "FinishTime";
            }
            else if (t == "DG" )
            {
                result = "DSQGate";
            }

            //Startlist
            else if (t == "STL" )
            {
                result = "Startlist";
            }
            else if (t == "Bib" )
            {
                result = "Bib";
            }
            else if (t == "SNR" )
            {
                result = "AthleteID";
            }

            //Athlete
            else if (t == "FN" )
            {
                result = "FirstName";
            }
            else if (t == "LN" )
            {
                result = "LastName";
            }
            else if (t == "SN" )
            {
                result = "ShortName";
            }
            else if (t == "PB" )
            {
                result = "PersonalBest";
            }
            else
            {
                result = t;
            }

            return result;
        }
                            
        private bool ParseDivision()
        {
            NextToken();
            SLCompare.Clear();
            SLCompare.Add(BO.cTokenB);
            SLCompare.Add("*");
            sDivision = CompareToken(sToken);
            bool result = (sDivision != string.Empty);
            if (!result)
            {
                FLastError = TMsgParserError.Error_Division;
            }

            return result;
        }

        private bool ParseRunID()
        {
            NextToken();
            SLCompare.Clear();
            SLCompare.Add(BO.cTokenRace + "1");
            sRunID = CompareToken(sToken);
            bool result = (sRunID != string.Empty);
            if (!result)
            {
                FLastError = TMsgParserError.Error_RunID;
            }

            return result;
        }

        protected override bool ParseCommand()
        {
            SLCompare.Clear();

            SLCompare.Add("XX");
            SLCompare.Add("IsRacing");

            SLCompare.Add("FM");
            SLCompare.Add("QU");
            SLCompare.Add("ST");
            if (BO != null)
            {
                for (int i = 0; i <= BO.AdapterParams.ITCount; i++)
                {
                    SLCompare.Add("IT" + i.ToString());
                }
            }

            SLCompare.Add("FT");
            SLCompare.Add("DG");
            SLCompare.Add("Rank");
            SLCompare.Add("RV");

            if (sPos != string.Empty)
            {
                SLCompare.Add("SNR");
                SLCompare.Add("Bib");
            }

            if (sAthlete != string.Empty)
            {
                SLCompare.Add("SNR");

                SLCompare.Add("FN");
                SLCompare.Add("LN");
                SLCompare.Add("SN");
                SLCompare.Add("NC");
                SLCompare.Add("GR");
                SLCompare.Add("PB");

                SLCompare.Add(FieldNames.FN);
                SLCompare.Add(FieldNames.LN);
                SLCompare.Add(FieldNames.SN);
                SLCompare.Add(FieldNames.NC);
                SLCompare.Add(FieldNames.GR);
                SLCompare.Add(FieldNames.PB);

                for (int i = 1; i <= BO.AdapterParams.FieldCount; i++)
                {
                    SLCompare.Add("N" + i.ToString());
                }
            }

            SLCompare.Add("Count");

            sCommand = CompareToken(sRest);
            return (sCommand != string.Empty);
        }
        
        private bool ParseAthlete()
        {
            if (NextTokenX(BO.cTokenID) > -1)
            {
                sAthlete = sToken;
                return true;
            }
            else
            {
                sAthlete = string.Empty;
                FLastError = TMsgParserError.Error_Athlete;
                return false;
            }
        }

        private bool ParseRace()
        {
            if (NextTokenX(BO.cTokenRace) > -1)
            {
                sRunID = "W" + sToken;
                return true;
            }
            else
            {
                sRunID = string.Empty;
                FLastError = TMsgParserError.Error_Race;
                return false;
            }
        }

        private bool ParseBib()
        {
            if (NextTokenX("Bib") > -1)
            {
                sBib = sToken;
                return true;
            }
            else
            {
                sBib = string.Empty;
                FLastError = TMsgParserError.Error_Bib;
                return false;
            }
        }

        private bool ParsePos()
        {
            if (NextTokenX("Pos") > -1)
            {
                sPos = sToken;
                return true;
            }
            else
            {
                sPos = string.Empty;
                FLastError = TMsgParserError.Error_Pos;
                return false;
            }
        }

        private bool ParseMsgID()
        {
            if (NextTokenX("Msg") > -1)
            {
                sMsgID = sToken;
                return true;
            }
            else
            {
                sMsgID = string.Empty;
                FLastError = TMsgParserError.Error_MsgID;
                return false;
            }
        }
        
        protected override bool ParseValue()
        {
            sValue = string.Empty;
            bool result = false;

            if (sCommand == "XX")
            {
                result = true;
            }
            else if (sCommand == "QU")
            {
                result = ParseStatusValue();
            }
            else if (IsTimeCommand())
            {
                result = ParseTimeValue();
            }
            else if ((sCommand == "DG")
                || (sCommand == "Bib")
                || (sCommand == "SNR")
                || (sCommand == "Count")
                || (sCommand == "Rank")
                || (sCommand == "FM")
                )
            {
                result = ParsePositiveIntegerValue();
            }
            else if (IsAthleteCommand())
            {
                result = true;
            }
            else if (sCommand == "IsRacing")
            {
                result = ParseBooleanValue();
            }
            else if (sCommand == "RV")
            {
                result = true; //ParseRaceValue()
            }

            if (result)
            {
                sValue = FValue;
            }

            return result;
        }

        private bool ParseTimeValue()
        {
            return true;
        }

        private bool ParseStatusValue()
        {
            return true;
            //###
            /*
            return ((FValue == "dnf")
              || (FValue == "dsq")
              || (FValue == "dns")
              || (FValue == "ok")
              || (FValue == "*"));
            */
        }
        
        private bool IsTimeCommand()
        {
            return (sCommand == "ST")
                || (Utils.Copy(sCommand, 1, 2) == "IT")
                || (sCommand == "FT");
        }

        private bool IsRunID()
        {
            int RaceCount = BO != null ? BO.AdapterParams.RaceCount : 1;
            string s = Utils.Copy(sRunID, 1, 1);
            int i = Utils.StrToIntDef(Utils.Copy(sRunID, 2, sRunID.Length), -1);
            return (s == "W") && (i > 0) && (i <= RaceCount);
        }

        private bool IsStartlistCommand()
        {
            return (sCommand == "Bib") || (sCommand == "SNR") || (sCommand == "Count");
        }

        private bool IsAthleteCommand()
        {
            return (sCommand == FieldNames.FN)
                || (sCommand == FieldNames.LN)
                || (sCommand == FieldNames.SN)
                || (sCommand == FieldNames.NC)
                || (sCommand == FieldNames.GR)
                || (sCommand == FieldNames.PB)

                || (sCommand == "FN")
                || (sCommand == "LN")
                || (sCommand == "SN")
                || (sCommand == "NC")
                || (sCommand == "GR")
                || (sCommand == "PB")

                || IsProp(sCommand)

                || IsNameCommand(sCommand);
        }

        protected override void Clear()
        {
            base.Clear();
            sAthlete = "";
            sPos = "";

            sDivision = "";
            sRunID = "";
            sBib = "";
            sCommand = "";
            sValue = "";
        }

        protected override bool ParseKeyValue(string aKey, string aValue)
        {
            Clear();
            FKey = aKey;
            FValue = aValue;
            FInput = aKey + "=" + aValue;
            sRest = aKey;

            if (TestTokenName(BO.cTokenA))
            {
                //consume Token
                NextToken();
            }

            if ((sToken == BO.cTokenA) && TestTokenName("Input"))
            {
                //consume Token Input
                NextToken();
            }

            if (TestTokenName("Msg"))
            {
                //Msg
                if (!ParseMsgID())
                {
                    return false;
                }
            }

            //Division
            if (!ParseDivision())
            {
                return false;
            }

            if (ParseLeaf())
            {
                return true;
            }

            if (TestTokenName(BO.cTokenID))
            {
                //Athlete
                if (!ParseAthlete())
                {
                    return false;
                }
            }
            else
            {

                if (TestTokenName(BO.cTokenRace))
                {
                    //Race
                    if (!ParseRace())
                    {
                        return false;
                    }

                    //property RaceX.IsRacing
                    if (ParseLeaf())
                    {
                        return true;
                    }
                }

                //RunID
                else if (!ParseRunID())
                {
                    return false;
                }

                if (TestTokenName("STL"))
                {
                    //consume Token Startlist
                    NextToken();

                    //property Startlist.Count
                    if (ParseLeaf())
                    {
                        return true;
                    }

                    //Pos
                    if (!ParsePos())
                    {
                        return false;
                    }
                }

                else if (IsRunID())
                {
                    if (TestTokenName("Pos"))
                    {
                        //Pos
                        if (!ParsePos())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //Bib
                        if (!ParseBib())
                        {
                            return false;
                        }
                    }
                }
            }

            return ParseLeaf();
        }

        public bool IsValid()
        {
            return FIsValid;
        }

        public int ErrorVal()
        {
            return (int) FLastError;
        }

        public void Report(TStrings Memo)
        {
            Memo.Add("Input: " + FInput);
            Memo.Add("Division: " + sDivision);
            Memo.Add("RunID: " + sRunID);
            Memo.Add("Bib: " + sBib);
            if (IsStartlistCommand())
            {
                if (sCommand == "Count")
                {
                    Memo.Add("Startlist.Count: " + sValue);
                }
                else
                {
                    Memo.Add("Pos: " + sPos);
                }
            }
            if (IsAthleteCommand())
            {
                Memo.Add("Athlete: " + sAthlete);
            }

            Memo.Add("Command: " + sCommand);
            Memo.Add("Value: " + sValue);
            if (!IsValid())
            {
                Memo.Add("Error: " + MsgParserErrorStrings(FLastError) + "(" + ErrorVal().ToString() + ")");
            }
        }

        public string ReportProt()
        {
            //not implemented
            return string.Empty;
        }

        public string ReportLongProt()
        {
            //not implemented
            return string.Empty;
        }
    }

}
