using System.Collections.Generic;

namespace RiggVar.FR
{

    public class TNodeList
    {
        private TEventNode EventNode;
        private List<TRaceNode> RaceNodeList;

        public TNodeList()
        {
            RaceNodeList = new List<TRaceNode>();
        }

        public void Add(TEventNode en)
        {
            EventNode = en;
        }

        public void Add(TRaceNode rn)
        {
            RaceNodeList.Add(rn);
        }

        private TRaceNode FindRaceNodeByNameID(string rd)
        {
            foreach (TRaceNode rn in RaceNodeList)
            {
                if (rn.NameID == rd)
                {
                    return rn;
                }
            }
            return null;
        }

        public void ClearList(string rd)
        {
            if (rd == EventNode.NameID)
            {
                EventNode.Collection.ClearList();
                EventNode.Modified = true;
                return;
            }

            TRaceNode bn = FindRaceNodeByNameID(rd);
            if (bn != null)
            {
                bn.Collection.ClearList();
                bn.Modified = true;
                return;
            }

            EventNode.Collection.ClearList();
            foreach (TRaceNode rn in RaceNodeList)
            {
                rn.Collection.ClearList();
            }
        }

        public void ClearResult(string rd)
        {
            if (rd == EventNode.NameID)
            {
                EventNode.Collection.ClearResult();
                if (!Loading)
                {
                    EventNode.Calc();
                }

                return;
            }

            TRaceNode bn = FindRaceNodeByNameID(rd);
            if (bn != null)
            {
                bn.Collection.ClearResult();
                if (!Loading)
                {
                    bn.Calc();
                }

                return;
            }

            EventNode.Collection.ClearResult();
            if (!Loading)
            {
                EventNode.Calc();
            }

            foreach (TRaceNode rn in RaceNodeList)
            {
                rn.Collection.ClearResult();
                if (!Loading)
                {
                    rn.Calc();
                }
            }
        }

        public void CalcNodes()
        {
            if (EventNode.Modified)
            {
                EventNode.Calc();
            }

            foreach (TRaceNode rn in RaceNodeList)
            {
                if (rn.Modified)
                {
                    rn.Calc();
                }
            }
        }

        public bool Loading => TMain.BO.Loading;

    }

}
