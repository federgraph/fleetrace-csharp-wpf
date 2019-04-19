#define Desktop
#define RemoteScorer

using System;

namespace RiggVar.FR
{

    public class TCalcEventProxy
    {
        public bool WithTest;
        public string EventName;

        public virtual void Calc(TEventNode aqn)
        {
        }
        public virtual void GetScoringNotes(TStrings SL)
        {
            SL.Add("TCalcEventProxy.GetScoringNotes");
        }
        public bool HighestBibGoesFirst { get; set; }
    }

    public class TCalcEvent
    {
        internal static int _ScoringResult = -1;
        internal static string _ScoringExceptionLocation = "";
        internal static string _ScoringExceptionMessage = "";

        //public const int ScoringProvider_UseDefault = 0;
        public const int ScoringProvider_SimpleTest = 1;
        public const int ScoringProvider_Inline = 2;
        public const int ScoringProvider_ProxyDLL = 3;
        public const int ScoringProvider_ProxyXML = 4;
        public const int ScoringProvider_WebService = 5;

        public const int DefaultScoringProviderID = 2;

        protected int FProviderID;
        protected int ProviderID
        {
            get => FProviderID;
            set
            {
                FProviderID = value;
#if Desktop
                TMain.IniImage.ScoringProvider = value; //keep Inifile uptodate
#endif
            }
        }

        public TCalcEvent(int aProviderID)
        {
            if (aProviderID == 0)
            {
                ProviderID = DefaultScoringProviderID;
            }

            InitModule(aProviderID);
        }

        public void Calc(TEventNode aqn)
        {
            Proxy.Calc(aqn);
            TUniquaPoints.Calc(aqn);
        }

        public void GetScoringNotes(TStrings SL)
        {
            Proxy.GetScoringNotes(SL);
        }

        public bool HighestBibGoesFirst
        {
            get => Proxy.HighestBibGoesFirst;
            set => Proxy.HighestBibGoesFirst = value;
        }

        public TCalcEventProxy Proxy { get; private set; }

        public int ModuleType
        {
            get => FProviderID;
            set
            {
                if (value != FProviderID)
                {
                    InitModule(value);
                }
            }
        }
        public bool ScoringResult => _ScoringResult != -1;
        public string ScoringExceptionMessage => "";

        public void InitModule(int aProviderID)
        {
            if (Proxy == null || aProviderID != ProviderID)
            {
                try
                {
                    ProviderID = aProviderID;                    
                    Proxy = null;
                    switch (aProviderID)
                    {
                        case ScoringProvider_SimpleTest: 
                            Proxy = new TCalcEventProxy01(); //einfacher lokaler test
                            break;

                        case ScoringProvider_Inline:
                            Proxy = new TCalcEventProxy11();
                            break;

                        //FProxy = new TCalcEventProxy03(); //C# JavaScore
                        //FProxy = new TCalcEventProxy05(); //J# JavaScore
                        //FProxy = new TCalcEventProxy06(); //RS04 direct
                        //FProxy = new TCalcEventProxy11(); //RS04 via proxy

#if RemoteScorer
                        case ScoringProvider_ProxyXML:
                            Proxy = new TCalcEventProxy04(); //native javascore
                            break;
#endif

                        default: 
                            ProviderID = ScoringProvider_SimpleTest;
                            Proxy = new TCalcEventProxy01();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    _ScoringResult = -1;
                    _ScoringExceptionLocation = "TCalcEvent.InitModule";
                    _ScoringExceptionMessage = ex.Message;
                    ProviderID = ScoringProvider_SimpleTest;
                    Proxy = new TCalcEventProxy01();
                }
#if Desktop
                //EventNode is ungleich null, wenn mit GUI ScoringProvider ge�ndert wird.
                //Alles ist noch null, wenn BOContainer oder BO erzeugt wird,
                //dann mu� GUI-Update aber auch hier nicht angesto�en werden.
                    if (TMain.BO != null)
                {
                    if (TMain.BO.EventNode != null)
                        {
                            TMain.BO.EventNode.Modified = true;
                            TMain.DrawNotifier.ScheduleFullUpdate(
                                null, new DrawNotifierEventArgs(DrawNotifierEventArgs.DrawTargetEvent));
                        }
                }
#endif
            }
        }
        public bool UsesProxy
        {
            get
            {
                switch (ModuleType)
                {
                    case ScoringProvider_SimpleTest: return false;
                    case ScoringProvider_Inline: return false;
                    case ScoringProvider_ProxyDLL: return true;
                    case ScoringProvider_ProxyXML: return true;
                    case ScoringProvider_WebService: return true;
                    default: return false;
                }                
            }
        }
    }
}
