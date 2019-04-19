namespace RiggVar.FR
{
    /// <summary>
    /// Specifies StartlistCount, RaceCount, ITCount;
    /// Min, Max and Current Values.
    /// Also PortIn, PortOut, inherited from AdapterParams.
    /// Also has a readonly property DivisionID.
    /// </summary>
    public class TBOParams : TAdapterParams
    {
        public TBOParams()
        {            
            MaxStartlistCount = 256;
            MinStartlistCount = 2;
            StartlistCount = 8;
        
            MinRaceCount = 1;
            MaxRaceCount = 20;
            RaceCount = 7;
            FieldCount = 6;

            MinITCount = 0;
            MaxITCount = 10;
            ITCount = 0;
        }

        public bool IsWithinLimits()
        {
            return RaceCount >= MinRaceCount
              && ITCount >= MinITCount
              && StartlistCount >= MinStartlistCount

              && RaceCount <= MaxRaceCount
              && ITCount <= MaxITCount
              && StartlistCount <= MaxStartlistCount;
        }

        public int DivisionID
        {
            get
            {
                switch (DivisionName)
                {
                    case "Europe": return 1;
                    case "Laser": return 2;
                    case "Finn": return 3;
                    case "470women": return 4;
                    case "470men": return 5;
                    case "49er": return 6;
                    case "Tornado": return 7;
                    case "Yngling": return 8;
                    case "Star": return 9;
                    case "MistralWomen": return 10;
                    case "MistralMen": return 11;
                    default: return 0;
                }
            }
        }
    }
}
