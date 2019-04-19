namespace RiggVar.FR
{
    public class TAdapterParams : TBOPersistent
    {
        public bool IsAdapter = false;

        public int MaxStartlistCount = 128;
        public int MinStartlistCount = 1;
        public int StartlistCount = 1;
        
        public int MinRaceCount = 1;
        public int MaxRaceCount = 1;
        public int RaceCount = 1;
        public int FieldCount = 6;
        
        public int MinITCount = 0;
        public int MaxITCount = 10;
        public int ITCount = 0;
        
        public string DivisionName = "*";

        public TAdapterParams()
        {
        }

        public override void Assign(object Source)
        {
            if (Source is TAdapterParams)
            {
                TAdapterParams o = Source as TAdapterParams;
                IsAdapter = o.IsAdapter;
                
                MaxStartlistCount = o.MaxStartlistCount;
                MinStartlistCount = o.MinStartlistCount;
                StartlistCount = o.StartlistCount;
                
                MinRaceCount = o.MinRaceCount;
                MaxRaceCount = o.MaxRaceCount;
                RaceCount = o.RaceCount;
                
                MinITCount = o.MinITCount;
                MaxITCount = o.MaxITCount;
                ITCount = o.ITCount;
                
                DivisionName = o.DivisionName;
            }
            else
            {
                base.Assign(Source);
            }
        }    

        public void ForceWithinLimits()
        {
            if (RaceCount < MinRaceCount)
            {
                RaceCount = MinRaceCount;
            }

            if (RaceCount > MaxRaceCount)
            {
                RaceCount = MaxRaceCount;
            }

            if (ITCount < MinITCount)
            {
                ITCount = MinITCount;
            }

            if (ITCount > MaxITCount)
            {
                ITCount = MaxITCount;
            }

            if (StartlistCount < MinStartlistCount)
            {
                StartlistCount = MinStartlistCount;
            }

            if (StartlistCount > MaxStartlistCount)
            {
                StartlistCount = MaxStartlistCount;
            }
        }

    }
}
