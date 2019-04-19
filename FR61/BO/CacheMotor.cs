using System;

namespace RiggVar.FR
{
    public class TCacheMotor : ICacheMotor
    {
        private DateTime LastUpdateTime;

        private long FStartCounter;
        private long FStopCounter;
        private int FIdleDelay = 39;

        public TConnection InputConnection;
        public TConnection OutputConnection;
        public TOutputCache Cache;

        public TCacheMotor()
        {
            FIdleDelay = 39;

            Cache = new TOutputCache();

            CacheEnabled = true;
        }
        public void DestroyCacheMotor()
        {
            CacheEnabled = false;
            Cache = null;
        }
        protected void DoRequest(string RequestString)
        {
            string answer;

            if (InputConnection != null)
            {
                StartQuery();
                answer = InputConnection.HandleMsg(RequestString);
                StopQuery();
                if (answer.Length > 0)
                {
                    Cache.Status = TOutputCache.CacheStatus_WaitingForAnswer;
                }
            }
            else
            {
                answer = "no connection";
                Millies = 0;
            }

            if(Utils.Pos("RiggVar.Params", RequestString) >= 1)
            {
                HandleParams(answer);
            }
            else
            {
                Cache.StoreAnswer(RequestString, answer, Millies);
                this.Cache.Modified = true;
            }
        }
        private void DoRequest2(TCacheRowCollectionItem cr)
        {
            string answer;
            if (InputConnection != null)
            {
                StartQuery();
                answer = InputConnection.HandleMsg(cr.Request);
                StopQuery();
            }
            else
            {
                answer = "no connection";
                Millies = 0;
            }
            cr.StoreData(answer, Millies);
        }
        private string DoRequest3(string RequestString)
        {
            string answer;
            if (InputConnection != null)
            {
                StartQuery();
                answer = InputConnection.HandleMsg(RequestString);
                StopQuery();
            }
            else
            {
                answer = "<RaceXml>no connection</RaceXml>";
                Millies = 0;
            }
            return answer;
        }

        public void SwapEvent()
        {
            string answer;
            TCacheRowCollectionItem cr;
            Cache.Synchronized = false;
            if (Active)
            {
                Cache.ProcessInput("");
            }
            else if (Cache.Node.Collection.Count > 0)
            {
                cr = Cache.Node.Collection[0];
                if (InputConnection != null)
                {
                    Millies = 0;
                    StartQuery();
                    answer = InputConnection.HandleMsg(cr.Request);
                    StopQuery();
                    if (cr.Report < 0) //Params
                    {
                        HandleParams(answer);
                    }

                    cr = Cache.Node.Collection[0];
                    cr.Age = cr.Ru.Age;
                }
            }
        }
        public void SetOutputMsg(object sender, TContextMsg cm)
        {
            Cache.ProcessInput(cm.msg);
        }
        protected void HandleParams(string response)
        {
            int RaceCount;
            int ITCount;
        
            RaceCount = Cache.Node.RaceCount;
            ITCount = Cache.Node.ITCount;
            TStrings SL = new TStringList
            {
                Text = response
            };

            //{RaceCount}
            string s = SL.Values("RaceCount");
            if (s != "")
            {
                RaceCount = Utils.StrToIntDef(s, 1);
            }

            //{ITCount}
            s = SL.Values("ITCount");
            if (s != "")
            {
                ITCount = Utils.StrToIntDef(s, 0);
            }

            Cache.UpdateParams(RaceCount, ITCount);
        }

        public string GetEventReport(int Report, int Sort, bool ShowPoints)
        {
            string result = "EventReport";
            TCacheRowCollection cl = Cache.Node.Collection;
            TCacheRowCollectionItem cr;
            TCacheRowCollectionItem crf = null;
            for (int i = 0; i < cl.Count-1; i++)
            {
                cr = cl[i];
                if (cr.Report == Report && cr.Sort == Sort)
                {
                    if (ShowPoints && cr.Mode == 1)
                    {
                        crf = cr;
                        break;
                    }
                    else if (!ShowPoints && cr.Mode == 0)
                    {
                        crf = cr;
                        break;
                    }
                }
            }
            if (crf != null)
            {
                if (crf.Age > 0)
                {
                    DoRequest2(crf);
                }

                crf.Hits = crf.Hits + 1;
                result = crf.ReportHeader + crf.Data;
            }
            return result;
        }
        public string GetRaceReport(int Report, int Race, int IT)
        {
            string result = "RaceReport";
            TCacheRowCollection cl = Cache.Node.Collection;
            TCacheRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.Report == Report && cr.Race == Race && cr.IT == IT)
                {
                    if (cr.Age > 0)
                    {
                        DoRequest2(cr);
                    }

                    cr.Hits = cr.Hits + 1;
                    result = cr.ReportHeader + cr.Data;
                    break;
                }
            }
            return result;
        }
        public string GetRaceXml(int Race, int IT)
        {
            string result = "<RaceXml/>";

            string s = string.Format("{0}RiggVar.FR.Race{1}.IT{2}", 
                TOutputCache.CacheRequestToken, Race, IT);
            result = DoRequest3(s);
            if (result.StartsWith("<?xml"))
            {
                result = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>"  + result;
            }

            return result;
        }
        public string GetXmlReport(int Report)
        {
            string result = "Report " + Report.ToString();
            TCacheRowCollection cl = Cache.Node.Collection;
            TCacheRowCollectionItem cr;
            for (int i = 0; i < cl.Count-1; i++)
            {
                cr = cl[i];
                if (cr.Report == Report)
                {
                    if (cr.Age > 0)
                    {
                        DoRequest2(cr);
                    }

                    cr.Hits = cr.Hits + 1;
                    result = cr.Data;
                    switch (Report)
                    {
                        case 1006: break; //preformatted text
                        default:        
                            if (!cr.Data.StartsWith("<?xml"))
                            {
                                result = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>"  + result;
                            }

                            break;
                    }
                    break;
                }
            }
            return result;
        }
        public void DoOnIdle()
        {
            if (CacheEnabled)
            {
                TimeSpan ts = DateTime.Now - LastUpdateTime;
                if (Active && (IdleDelay > 0) 
                    && (ts.Seconds * 1000 + ts.Milliseconds > IdleDelay))
                {
                    Cache.DoOnIdle();
                    if (Cache.Status == TOutputCache.CacheStatus_HaveRequest)
                    {
                        DoRequest(Cache.CurrentRequest);
                    }
                    LastUpdateTime = DateTime.Now;
                }
        
            }
        }
        public void SynchronizeIfNotActive()
        {
            if (!Active)
            {
                Synchronize();
            }
        }
        public void Synchronize()
        {
            Cache.Synchronized = false;
            Cache.Node.Age = Cache.Node.Age + 1;
        }
        public bool CacheEnabled { get; set; } = false;
        public int IdleDelay
        {
            get => FIdleDelay;
            set
            {
                if (value > 9)
                {
                    FIdleDelay = value;
                }
                else
                {
                    FIdleDelay = 0;
                }
            }
        }
        public bool Active { get; set; } = false;
        public long Millies { get; private set; } = 0;

        private void StartQuery()
        {
            FStartCounter = DateTime.Now.Ticks;
        }
        private void StopQuery()
        {
            FStopCounter = DateTime.Now.Ticks;
            Millies = (FStopCounter - FStartCounter) / 10000;
        }

        public string Handle_TW_TimePointReport(int Race)
        {
            string s = string.Format("{0}Report.TW_TimePointTable.R{1}", TOutputCache.CacheRequestToken, Race);
            return DoRequest3(s);
        }

        public string FinishReport
        {
            get
            {
                string s = string.Format("{0}Report.FinishReport", TOutputCache.CacheRequestToken);
                return DoRequest3(s);
            }
        }

        public string PointsReport
        {
            get
            {
                string s = string.Format("{0}Report.PointsReport", TOutputCache.CacheRequestToken);
                return DoRequest3(s);
            }
        }

        public string TimePointReport
        {
            get
            {
                //xml report with xsl reference for client-side execution
                string s = string.Format("{0}Report.TimePointReport", TOutputCache.CacheRequestToken);
                return DoRequest3(s);
            }
        }

        public string Handle_Entries(string input)
        {
            string s = string.Format("{0}HTM.Data.A", TOutputCache.CacheRequestToken);
            if (input != "")
            {
                s = s + Environment.NewLine + input;
            }

            return DoRequest3(s);
        }

        public string Handle_RV(string input)
        {
            string result = "";
            if (input.IndexOf(".RV") > 0)
            {
                result = DoRequest3(input);
                result = "OK";
            }
            else
            {
                result = "invalid cmd";
            }

            return result;
        }

        public string Handle_Manage(string input)
        {
            string result = "";
            if (input.StartsWith("Manage."))
            {
                result = DoRequest3(input);
                result = "OK";
            }
            else
            {
                result = "invalid cmd";
            }

            return result;
        }

        public string Handle_TW_TimePointXML(int race, int it, int bib)
        {
            string t = DateTime.Now.ToString("HH:mm:ss.ff");
            string s = string.Format("{0}Report.TW_TimePointXML.R{1}.IT{2}.Bib{3}",
              TOutputCache.CacheRequestToken, race, it, bib);
            s = s + Environment.NewLine + string.Format("FR.*.W{0}.Bib{1}.IT{2}={3}\r\n", race, bib, it, t) + s;
            string result = DoRequest3(s);
            return result;
        }

        public string Handle_TW_TimePointTable(int race, int it, int bib)
        {
            //send a single line input msg, then the request for the report,
            //so that the input msg is broadcast to listeners on output.
            string t = DateTime.Now.ToString("HH:mm:ss.ff");
            string s = string.Format("FR.*.W{0}.Bib{1}.IT{2}={3}", race, bib, it, t);
            InputConnection.InjectMsg(s);
            s = string.Format("{0}Report.TW_TimePointTable.R{1}", TOutputCache.CacheRequestToken, race);
            string result = DoRequest3(s);
            return result;
        }

        public string Handle_TW_Ajax(int race, int it, int bib)
        {
            //send a single line input msg, then the request for the report,
            //so that the input msg is broadcast to listeners on output.
            string t = DateTime.Now.ToString("HH:mm:ss.ff");
            string s = string.Format("FR.*.W{0}.Bib{1}.IT{2}={3}", race, bib, it, t);
            InputConnection.InjectMsg(s);
            string result = string.Format("R{0}.IT{1}.Bib{2}.Time={3}", race, it, bib, t);
            return result;
        }

    }
}
