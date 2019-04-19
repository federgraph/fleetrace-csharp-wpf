using System;

namespace RiggVar.FR
{
    public class TCacheRowCollectionItem : TBaseRowCollectionItem<
        TCacheColGrid,
        TCacheBO,
        TCacheNode,
        TCacheRowCollection,
        TCacheRowCollectionItem,
        TCacheColProps,
        TCacheColProp
        >
    {
        private int FAge;
        private string FRequest;
        private string FData;
        public bool Requesting;

        public TCacheRowCollectionItem()
            : base()
        {
        }

        public void StoreData(string answer, long aMillies)
        {
            Data = answer;
            TimeStamp = DateTime.Now;
            Millies = aMillies;
        }

        public override void Assign(TCacheRowCollectionItem o)
        {
            if (o != null)
            {
                Report = o.Report;
                Race = o.Race;
                IT = o.IT;
                Sort = o.Sort;
                Mode = o.Mode;
                Age = o.Age;
                Updates = o.Updates;
                Hits = o.Hits;
                Millies = o.Millies;
                TimeStamp = o.TimeStamp;
            }
        }

        public string Request
        {
            get
            {
                if (Report > 999)
                {
                    return TOutputCache.CacheRequestToken + FRequest;
                }
                else if (Report < 0)
                {
                    return TOutputCache.CacheRequestToken + "RiggVar.Params";
                }
                else if (Report < 100)
                {
                    return string.Format("{0}HTM.Web.Event.Report{1}.Sort{2}.{3}",
                        TOutputCache.CacheRequestToken, Report, Sort, Modus);
                }
                else
                {
                    return string.Format("{0}HTM.Web.Race.Report{1}.R{2}.IT{3}",
                        TOutputCache.CacheRequestToken, Report - 100, Race, IT);
                }
            }
            set => FRequest = value;
        }

        public string Modus => Mode == 0 ? "Finish" : "Points";

        public string Data
        {
            get
            {
                if (IsXml)
                {
                    if (FData == "no connection")
                    {
                        return "<answer>no connection</answer>";
                    }
                    else if (FData == "")
                    {
                        return "<answer>empty</answer>";
                    }
                    else
                    {
                        return FData;
                    }
                }
                else
                {
                    return FData;
                }
            }
            set
            {
                FData = value;
                Updates = Updates + 1;
                FAge = Ru.Age;
            }
        }

        public string ReportHeader
        {
            get
            {
                string fs = "<pre>Age={0} Updates={1} Hits={2} TimeStamp={3}</pre>";
                return string.Format(fs, FAge, Updates, Hits, TimeStamp.ToString());
            }
        }

        public int Report { get; set; }
        public int Race { get; set; }
        public int IT { get; set; }
        public int Sort { get; set; }
        public int Mode { get; set; }
        public int Age {
            get => Ru.Age - FAge;
            set => FAge = value;
        }
        public int Updates { get; set; }
        public int Hits { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsXml { get; set; }
        public bool AddXmlHeader { get; set; }
        public long Millies { get; set; }
    }

}
