using System.Collections.Generic;

namespace RiggVar.FR
{

    public class TGridCells<T> : Dictionary<TCell, T>
        where T : class
    {
        private static TCell key = new TCell(0, 0);
        public T DefaultValue;

        public T this[int ACol, int ARow]
        {
            get
            {
                key.C = ACol;
                key.R = ARow;
                if (ContainsKey(key))
                {
                    return this[key];
                }
                else
                {
                    return DefaultValue;
                }
            }
            set
            {
                key.C = ACol;
                key.R = ARow;
                if (ContainsKey(key))
                {
                    this[key] = value;
                }
                else
                {
                    this.Add(new TCell(ACol, ARow), value);
                } 
            }
        }

    }

}
