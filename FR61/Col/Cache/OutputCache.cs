using System;

namespace RiggVar.FR
{

    public class TOutputCache : TCacheGrid
    {
        public const int CacheStatus_Idle = 0;
        public const int CacheStatus_HaveRequest = 1;
        public const int CacheStatus_WaitingForAnswer = 2;
        const string EmptyXML = @"<?xml version=""1.0"" ?></Empty></xml>";

        public static string CacheRequestToken = "FR.*.Output.";

        public int Status;
        public int RequestID;
        public string CurrentRequest;
        public bool Synchronized;

        public bool Modified { get; set; }

        public TOutputCache()
            : base()
        {
            Node.Load();
        }

        private string NextRequest()
        {
            string result = "";
            TCacheRowCollection cl = Node.Collection;
            TCacheRowCollectionItem cr;
            for (int i = 0; i < cl.Count; i++)
            {
                cr = cl[i];
                if (cr.Age <= 0)
                {
                    continue;
                }

                if ((i == 0) && Synchronized)
                {
                    continue;
                }

                result = cr.Request;
                RequestID = cr.BaseID;
                CurrentRequest = result;
                ColBO.CurrentRow = cr;
                break;
            }
            return result;
        }

        public void DoOnIdle()
        {
            switch (Status)
            {
                case CacheStatus_Idle:
                    string s = NextRequest();
                    if (s != "")
                    {
                        CurrentRequest = s;
                        Status = CacheStatus_HaveRequest;
                    }
                    //Grid.ShowData;
                    break;

                case CacheStatus_HaveRequest: break;
                case CacheStatus_WaitingForAnswer: break;
            }
        }

        public void StoreAnswer(string Request, string Answer, long Millies)
        {
            if (Request == CurrentRequest)
            {
                TCacheRowCollectionItem cr = Node.Collection.FindKey(RequestID);
                if (cr != null)
                {
                    cr.StoreData(Answer, Millies);
                    Modified = true;
                }
            }
            Status = CacheStatus_Idle;
        }

        public void UpdateParams(int RaceCount, int ITCount)
        {
            if ((Node.RaceCount != RaceCount) || (Node.ITCount != ITCount))
            {
                Node.RaceCount = RaceCount;
                Node.ITCount = ITCount;
                Node.Load();
                Modified = true;
            }
            TCacheRowCollectionItem cr = Node.Collection[0];
            if (cr != null)
            {
                cr.Age = Node.Age;
                cr.TimeStamp = DateTime.Now;
                cr.Data = string.Format("RaceCount={0}\r\nITCount={1}\r\n", RaceCount, ITCount);
            }
            Synchronized = true;
            Status = CacheStatus_Idle;
        }

        public void ProcessInput(string s)
        {
            Node.Age = Node.Age + 1;
            Modified = true;
        }

        public int EventType => LookupKatID.FR;

    }

}
