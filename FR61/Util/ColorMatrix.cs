using System;
using System.Drawing;

namespace RiggVar.FR
{
    public class ColorMatrix
    {
        public static int BibCount = 12;
        internal static int OldAge = -256;
        internal static int minBands = 3;
        internal static int maxBands = 16;
        internal static int StopBand = maxBands / 4;
    
        public static Color [] BlendColor;

        public static TMatrix Model;

        static ColorMatrix()
        {
            int t = RowCount * ColCount;
            if (BibCount > t)
            {
                BibCount = t;
            }

            PrepareColors(Color.Orange, Color.White);
            Model = new TMatrix();
        }
        public static void PrepareColors(Color Col1, Color Col2)
        {
            BlendColor = new Color[maxBands + 1];

            int [] dRGB = new int[3];
                                
            dRGB[0] = Col2.R - Col1.R;
            dRGB[1] = Col2.G - Col1.G;
            dRGB[2] = Col2.B - Col1.B;
            
            BlendColor[0] = Col1; 
            for (int z = 1; z < maxBands; z++)
            {                
                double dr = BlendColor[0].R + z * (dRGB[0]/maxBands);
                double dg = BlendColor[0].G + z * (dRGB[1]/maxBands);
                double db = BlendColor[0].B + z * (dRGB[2]/maxBands);

                byte ir = (byte) Math.Round(dr, 0);
                byte ig = (byte) Math.Round(dg, 0);
                byte ib = (byte) Math.Round(db, 0);

                BlendColor[z] = Color.FromArgb(255, ir, ig, ib);
            }
            BlendColor[maxBands] = Col2;
            
        }

        public static int ColCount
        {
            get
            {
                if (BibCount <= 80)
                {
                    return 20;
                }
                else
                {
                    return 25;
                }
            }
        }

        public static int RowCount
        {
            get
            {
                int c = ColCount;
                int r = BibCount / c;
                if (BibCount % c > 0)
                {
                    r++;
                }

                return r;
            }
        }

    }

    public class TMatrixItem
    {
        internal int FAge;
        private TMatrix FOwner;

        public bool IsAmpelMode = false;

        public TMatrixItem(TMatrix aOwner)
        {
            FOwner = aOwner;
            FAge = ColorMatrix.OldAge;
        }

        public string Value { get; set; }

        public Color Color
        {
            get
            {
                int i = FOwner.FAge - FAge;

                if (FAge < 0)
                {
                    return Color.White;
                }
                else if (i > ColorMatrix.maxBands-ColorMatrix.StopBand)
                {
                    return ColorMatrix.BlendColor[ColorMatrix.maxBands-ColorMatrix.StopBand];
                }
                else if (i < 0)
                {
                    return Color.White;
                }

                //Ampel
                else if (IsAmpelMode)
                {
                    if (i > 10)
                    {
                        return Color.White;
                    }
                    else if (i > 5)
                    {
                        return Color.Yellow;
                    }
                    else if (i >= 0)
                    {
                        return Color.Lime;
                    }
                    else
                    {
                        return Color.White;
                    }
                }

                //oder Farbverlauf
                else
                {
                    return ColorMatrix.BlendColor[i];
                }
            }            
        }
    }

    public class TMatrix
    {
        int size = 256;
        private TMatrixItem [] FData;
        internal int FAge;

        public TMatrix()
        {
            FData = new TMatrixItem[size];
            for (int i = 0; i < size; i++)
            {
                TMatrixItem mi = new TMatrixItem(this);
                mi.Value = (i + 1).ToString();
                FData[i] = mi;
            }
        }

        public int Size
        {
            get
            {
                if (FData != null)
                {
                    return FData.Length;
                }
                else
                {
                    return size;
                }
            }
        }

        public int Count
        {
            get
            {
                return ColorMatrix.ColCount * ColorMatrix.RowCount; 
            }
        }

        public void TimerTick()
        {
            FAge++;
        }

        public void ResetAge()
        {
            int count = FData.Length;
            for (int i = 0; i < count; i++)
            {
                FData[i].FAge = ColorMatrix.OldAge;
            }

            FAge = 0;
        }

        public string GetValue(int idx)
        {
            if (idx >= 0 && idx < ColorMatrix.BibCount)
            {
                return FData[idx].Value;
            }
            return "";
        }

        public string GetValue(int ACol, int ARow)
        {
            if (ACol >= 0
                && ARow >= 0
                && ACol < ColorMatrix.ColCount
                && ARow < ColorMatrix.RowCount)
            {
                int i = GetIndex(ACol, ARow);
                return GetValue(i);
            }
            return "";
        }

        public void SetAge(int ACol, int ARow)
        {
            if (ACol >= 0
                && ARow >= 0
                && ACol < ColorMatrix.ColCount
                && ARow < ColorMatrix.RowCount)
            {
                int i = GetIndex(ACol, ARow);
                FData[i].FAge = FAge;
            }
        }

        /// <summary>
        /// property Color
        /// </summary>
        public Color GetColor(int ACol, int ARow)
        {
            if (ACol >= 0
                && ARow >= 0
                && ACol < ColorMatrix.ColCount
                && ARow < ColorMatrix.RowCount)
            {
                int i = GetIndex(ACol, ARow);
                return FData[i].Color;
            }
            return Color.White;
        }

        /// <summary>
        /// Flat Index of Item (zero based)
        /// </summary>
        /// <param name="ACol">virtual Column (zero based)</param>
        /// <param name="ARow">virtual Row (zero based)</param>
        /// <returns></returns>
        public int GetIndex(int ACol, int ARow)
        {
            int i = ACol + ARow * ColorMatrix.ColCount;
            int c = Size;
            if (i < 0)
            {
                return 0;
            }
            else if (i > c - 1)
            {
                return c;
            }
            else
            {
                return i;
            }
        }
    }

}
