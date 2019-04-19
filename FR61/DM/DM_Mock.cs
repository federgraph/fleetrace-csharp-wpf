using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiggVar.FR
{

    public class TDBEvent : IDBEvent
    {
        #region IDBEvent2 Member

        public string Load(int KatID, string EventName)
        {
            return null;
        }

        public void Save(int KatID, string EventName, string Data)
        {
        }

        public void Delete(int KatID, string EventName)
        {
        }

        public string GetEventNames(int KatID)
        {
            return null;
        }

        public void Close()
        {
        }

        #endregion
    }

}
