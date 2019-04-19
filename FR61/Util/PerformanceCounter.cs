using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Xml;

namespace RiggVar.FR
{
    public class PerformanceCounterItem
    {
        public string NameID;
        public int NumID;

        public int EnterCounter = 0;
        public int LeaveCounter = 0;
        public int Pos = 0;

        public int EnterTicks;
        public int LeaveTicks;
        public int TotalTicks;
        public int MaxTicks;
        public int MinTicks;

        public void Enter(int pos)
        {
            EnterCounter++;
            Pos = pos;
            EnterTicks = System.Environment.TickCount;
            LeaveTicks = EnterTicks;
        }
        public void Leave()
        {
            LeaveCounter++;
            LeaveTicks = System.Environment.TickCount;            
            if (LeaveCounter == EnterCounter)
            {
                TotalTicks += DiffTicks;
                if (DiffTicks > MaxTicks)
                {
                    MaxTicks = DiffTicks;
                }

                if (LeaveCounter == 1)
                {
                    MinTicks = DiffTicks;
                }
                else
                {
                    if (DiffTicks < MinTicks)
                    {
                        MinTicks = DiffTicks;
                    }
                }
            }
        }
        public int DiffTicks
        {
            get { return LeaveTicks - EnterTicks; }
        }
        public void Clear()
        {
            EnterCounter = 0;
            EnterTicks = 0;
            LeaveCounter = 0;
            LeaveTicks = 0;
            TotalTicks = 0;
            MaxTicks = 0;
            MinTicks = 0;
        }
        public void Report(TextWriter tw)
        {            
            tw.WriteLine("NameID: {0}", NameID);
            tw.WriteLine("NumID: {0}", NumID);
            tw.WriteLine("EnterCounter: {0}", EnterCounter);
            if (EnterCounter != LeaveCounter)
            {
                tw.WriteLine("LeaveCounter: {0}", LeaveCounter);
            }

            tw.WriteLine("TotalTicks: {0}", TotalTicks);
            tw.WriteLine("MaxTicks: {0}", MaxTicks);
            tw.WriteLine("MinTicks: {0}", MinTicks);
        }
        public void Report(XmlWriter xw)
        {            
            xw.WriteElementString("NameID", NameID);
            xw.WriteStartElement("NumID");
            xw.WriteString(NumID.ToString());
            xw.WriteEndElement();
            xw.WriteElementString("EnterCounter", EnterCounter.ToString());
            if (EnterCounter != LeaveCounter)
            {
                xw.WriteElementString("LeaveCounter", LeaveCounter.ToString());
            }

            xw.WriteElementString("TotalTicks", TotalTicks.ToString());
            xw.WriteElementString("MaxTicks", MaxTicks.ToString());
            xw.WriteElementString("MinTicks", MinTicks.ToString());
        }
    }

    public class PerformanceTraceListener : TraceListener
    {
        private int pos;            
        private System.Collections.Hashtable ht = new Hashtable();
        public PerformanceTraceListener()
        {
        }
        public override void Write(string message)
        {            
        }
        public override void WriteLine(string message)
        {
        }
        public override void Write(string message, string category)
        {
            if (category.StartsWith("E"))
            {
                Enter(message);
            }
            else if (category.StartsWith("L"))
            {
                Leave(message);
            }
        }
        public override void Write(object o, string category)
        {
            if (category.StartsWith("E"))
            {
                Enter(o);
            }
            else if (category.StartsWith("L"))
            {
                Leave(o);
            }
        }
        private PerformanceCounterItem this [int NumID]
        {
            get
            {
                object o = ht[NumID];
                if (o != null && o is PerformanceCounterItem)
                {
                    return o as PerformanceCounterItem;
                }
                else
                {
                    PerformanceCounterItem pc = new PerformanceCounterItem();                    
                    pc.NumID = NumID;
                    pc.NameID = this.CounterName(NumID);
                    ht.Add(NumID, pc);
                    return pc;
                }
            }
        }
        private PerformanceCounterItem this [string NameID]
        {
            get
            {
                object o = ht[NameID];
                if (o != null && o is PerformanceCounterItem)
                {
                    return o as PerformanceCounterItem;
                }
                else
                {
                    PerformanceCounterItem pc = new PerformanceCounterItem();                    
                    pc.NameID = NameID;
                    pc.NumID = -(ht.Count + 1);
                    ht.Add(NameID, pc);
                    return pc;
                }
            }
        }

        public void Enter(object o)
        {
            PerformanceCounterItem pc = null;
            if (o is int)
            {
                int i = (int) o;
                pc = this[i];
            }
            else if (o is string)
            {
                string s = (string) o;
                pc = this[s];
            }
            if (pc != null)
            {
                pos++;
                pc.Enter(pos);
            }
        }
        public void Leave(object o)
        {
            PerformanceCounterItem pc = null;
            if (o is int)
            {
                int i = (int) o;
                pc = this[i];
            }
            else if (o is string)
            {
                string s = (string) o;
                pc = this[s];
            }
            if (pc != null)
            {
                pc.Leave();
            }
        }
        public void Clear()
        {
            foreach (PerformanceCounterItem pc in ht.Values)
            {
                pc.Clear();
            }
        }
        public void Reset()
        {
            ht.Clear();
        }
        public void Report(TextWriter tw)
        {
            int index = 0;
            foreach(PerformanceCounterItem pc in ht.Values)
            {
                pc.Report(tw);
                tw.WriteLine();
                index++;
            }
        }
        public void Report(XmlWriter xw)
        {
            foreach(PerformanceCounterItem pc in ht.Values)
            {
                xw.WriteStartElement("PerformanceCounter");
                pc.Report(xw);
                xw.WriteEndElement();
            }
        }
        protected virtual string CounterName(int i)
        {
            return i.ToString();
        }
    }    

    public class PC : PerformanceTraceListener
    {
        public void Enter(int i)
        {
            base.Enter(i);
        }
        public void Leave(int i)
        {
            base.Leave(i);
        }

        public const int TColGrid_Constructor = 1000;
        public const int TColGrid_SetupGrid = 1001;
        public const int TColGrid_ShowData = 1002;
        public const int TColGrid_InitCellProps = 1003;
        public const int TColGrid_InitDisplayOrder = 1004;
        public const int TColGrid_UpdateAll = 1005;
        public const int TRaceBO_InitColsActiveLayout = 1006;
        public const int TBaseColProps_Init = 1007;
        public const int THashGrid_Constructor = 1008;
        public const int TColDataSet_Constructor = 1009;
        public const int TColDataSet_Fill = 1010;
        public const int TestTotal = 1011;
        public const int TBaseColProps_UpdateRow = 1012;
        public const int TColDataSet_FillRows = 1013;
        public const int TRaceColProp_GetTextDefault = 1014;
        public const int TNTime_ConvertTimeToStr3 = 1015;
        public const int Utils_Copy = 1016;
        public const int GetText_IT = 1017;
        public const int Time_ToString_1 = 1018;
        public const int Utils_StrToIntDef = 1019;
        public const int TSimpleHashGrid_GetItem = 1020;
        public const int TColDataSet_FillRows_ClearRows = 1021;
        public const int TColDataSet_FillRows_NewStringArray = 1022;
        public const int TColDataSet_FillRows_AcceptChanges = 1023;
        public const int TColDataSet_FillRows_AddRow = 1024;
        public const int TColGrid_SetupGrid_ClearRow = 1025;
        public const int TColGrid_SetupGrid_ShowHeader = 1026;
        public const int THashGrid_SetupGrid_CreateTableStyle = 1027;

        protected override string CounterName(int i)
        {
            switch (i)
            {
                case 1000 : return "TColGrid_Constructor";
                case 1001: return "TColGrid_SetupGrid";
                case 1002: return "TColGrid_ShowData";
                case 1003: return "TColGrid_InitCellProps";
                case 1004: return "TColGrid_InitDisplayOrder";
                case 1005: return "TColGrid_UpdateAll";
                case 1006: return "TRaceBO_InitColsActiveLayout";
                case 1007: return "TBaseColProps_Init";
                case 1008: return "THashGrid_Constructor";
                case 1009: return "TColDataSet_Constructor";
                case 10010: return "TColDataSet_Fill";
                case 10011: return "TestTotal";
                case 1012: return "TBaseColProps_UpdateRow";
                case 1013: return "TColDataSet_FillRows";
                case 1014: return "TRaceColProp_GetTextDefault";
                case 1015: return "TNTime_ConvertTimeToStr3";
                case 1016: return "Utils_Copy";
                case 1017: return "GetText_IT";
                case 1018: return "Time_ToString_1";
                case 1019: return "Utils_StrToIntDef";
                case 1020: return "TSimpleHashGrid_GetItem";
                case 1021: return "TColDataSet_FillRows_ClearRows";
                case 1022: return "TColDataSet_FillRows_NewStringArray";
                case 1023: return "TColDataSet_FillRows_AcceptChanges";
                case 1024: return "TColDataSet_FillRows_AddRow";
                case 1025: return "TColGrid_SetupGrid_ClearRow";
                case 1026: return "TColGrid_SetupGrid_ShowHeader";
                case 1027: return "THashGrid_SetupGrid_CreateTableStyle";
                default: return i.ToString();
            }
        }

    }
}
