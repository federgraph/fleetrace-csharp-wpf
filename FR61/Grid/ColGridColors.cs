using System.Drawing;
using System.Diagnostics;

namespace RiggVar.FR
{
    public class MissingColors
    {
        public static Color MoneyGreen = Color.FromArgb(0xFF, 0xC0, 0xDC, 0xC0);
        public static Color Cream = Color.FromArgb(0xFF, 0xF0, 0xFB, 0xFF);
    }

    [DebuggerStepThrough]
    public class TColGridColors
    {
        public static Color clFocusCell = Color.Yellow;
        public static Color clEditable = Color.FromArgb(0xFF, 0xE8, 0xFF, 0xFF);

        public static Color clNormal = Color.White;
        public static Color clAlternate = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xE0);

        public static Color clMoneyGreen = MissingColors.MoneyGreen;
        public static Color clCream = MissingColors.Cream;

        public static Color clHellRot = Color.FromArgb(0xFF, 0xFF, 0x80, 0x80);
        public static Color clTransRot = Color.FromArgb(0xFF, 0xF5, 0xAA, 0x89);
        public static Color clHellBlau = Color.FromArgb(0xFF, 0x88, 0x88, 0xFF);
        public static Color clTransBlau = Color.FromArgb(0xFF, 0x93, 0xAD, 0xEC);

        public static Color clSkyBlue = Color.FromArgb(0xFF, 0xEB, 0xCE, 0x87);

        public static Color clBtnFace = Color.Silver;
        public static Color clDefault = Color.Black;
        public static Color clYellow = Color.Yellow;
        public static Color clAqua = Color.Aqua;
        public static Color clLime = Color.Lime;
        public static Color clRed = Color.Red;

        public static string HTMLColor(Color c)
        {

            if (c == clEditable)
            {
                return "#E8FFFF";
            }
            else if (c == clNormal)
            {
                return "white";
            }
            else if (c == clAlternate)
            {
                return "#FFFFE0";
            }
            else if (c == clMoneyGreen)
            {
                return "#C0DCC0";
            }
            else if (c == clCream)
            {
                return "#F0FBFF";
            }
            else if (c == clHellBlau)
            {
                return "#8080FF";
            }
            else if (c == clTransBlau)
            {
                return "#89AAF5";
            }
            else if (c == clHellRot)
            {
                return "#FF8888";
            }
            else if (c == clTransRot)
            {
                return "#ECAD93";
            }
            else if (c.Equals(Color.FromArgb(0, 0, 0x80, 0xFF)))
            {
                return "#FF8000";
            }
            else if (c == clBtnFace)
            {
                return "silver";
            }
            else if (c == clSkyBlue)
            {
                return "#87BCEB";
            }
            else if (c == Color.Red)
            {
                return "red";
            }
            else if (c == Color.Maroon)
            {
                return "maroon";
            }
            else if (c == Color.Black)
            {
                return "black";
            }
            else if (c == Color.Yellow)
            {
                return "yellow";
            }
            else if (c == Color.Olive)
            {
                return "olive";
            }
            else if (c == Color.Lime)
            {
                return "lime";
            }
            else if (c == Color.Green)
            {
                return "green";
            }
            else if (c == Color.Teal)
            {
                return "teal";
            }
            else if (c == Color.Aqua)
            {
                return "gray";
            }
            else if (c == Color.Blue)
            {
                return "blue";
            }
            else if (c == Color.Navy)
            {
                return "navy";
            }
            else if (c == Color.Silver)
            {
                return "silver";
            }
            else if (c == Color.Purple)
            {
                return "purple";
            }
            else if (c == Color.Fuchsia)
            {
                return "fuchsia";
            }
            else if (c == Color.White)
            {
                return "white";
            }
            else
            {
                return TranslateColor(c);
            }
        }

        public static string TranslateColor(Color c)
        {
            return string.Format("#{0,2:X}{1,2:X}{2,2:X}", c.B, c.G, c.R);
        }

        public static string CSSClass(TCellProp cellProp)
        {
            Color c = cellProp.Color;
            string s = "";
            switch (cellProp.ColorClass)
            {
                case TColGridColorClass.AlternatingColor:
                    s = "a";
                    break;
                case TColGridColorClass.AlternatingEditableColor:
                    s = "ae";
                    break;
                case TColGridColorClass.Blank:
                    return "";
                case TColGridColorClass.CurrentColor:
                    s = "c";
                    break;
                case TColGridColorClass.CustomColor:
                    break;
                case TColGridColorClass.DefaultColor:
                    s = "n";
                    break;
                case TColGridColorClass.EditableColor:
                    s = "e";
                    break;
                case TColGridColorClass.FocusColor:
                    return "";
                case TColGridColorClass.HeaderColor:
                    s = "h";
                    break;
            }
            if (s != "")
            {
                if (cellProp.Alignment == TColAlignment.taLeftJustify)
                {
                    s += "l";
                }

                return string.Format(@" class=""{0}""", s);
            }
            else
            {
                if (cellProp.Alignment == TColAlignment.taLeftJustify)
                {
                    return string.Format(@" bgcolor=""{0}"" align=""left""", HTMLColor(c));
                }
                else
                {
                    return string.Format(@" bgcolor=""{0}"" align=""right""", HTMLColor(c));
                }
            }
        }

    }
}
