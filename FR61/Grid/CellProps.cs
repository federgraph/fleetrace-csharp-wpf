using System;
using System.Drawing;
using System.Collections.Generic;

namespace RiggVar.FR
{

    public class TCellProp
    {
        public TColAlignment Alignment;
        public Color Color = Color.AliceBlue;
        public Color GroupColor = Color.Black;
        public string HTMLColor = "aliceblue";
        public TColGridColorClass ColorClass;
    }

    public class TCell : IEquatable<TCell>
    {
        /// <summary>
        /// initializes a class the can be used as Key for Dictionary TCells<T>
        /// </summary>
        /// <param name="x">Column</param>
        /// <param name="y">Row</param>
        public TCell(int x, int y)
        {
            C = x;
            R = y;
        }
        public int R { get; set; }
        public int C { get; set; }

        public bool Equals(TCell other)
        {
            return R == other.R && C == other.C;
        }

        public override int GetHashCode()
        {
            return C ^ R;
        }

    }

    public class TCells<T> : Dictionary<TCell, T>
        where T : class, new()
    {
        private static TCell key = new TCell(0, 0);

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
                    T cp = new T();
                    TCell k = new TCell(ACol, ARow);
                    Add(k, cp);
                    return cp;
                }
            }
        }
    }

    public class TCellProps : TCells<TCellProp>
    {
        public bool Test()
        {
            TCellProp cp;

            cp = this[2, 2];
            cp.Color = Color.Blue;

            cp = this[2, 2];

            return cp.Color == Color.Blue;
        }

    }

}
