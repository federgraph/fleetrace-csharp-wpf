using System.Collections.Generic;
using System.Windows.Media;

namespace RiggVar.FR
{
    public class TColorCache : Dictionary<Color, Brush>
    {
        public Brush ColoredBrush(Color color)
        {
            if (ContainsKey(color))
            {
                return this[color];
            }
            else
            {
                Brush b = new SolidColorBrush(color);
                Add(color, b);
                return b;                
            }
        }
    }
}
