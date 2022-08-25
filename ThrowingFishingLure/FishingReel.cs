using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrowingFishingLure
{
    public class FishingReel
    {
        public double Ms;           // масса шпули
        public double Rs1;          // радиус широкой части шпули
        public double Rs2;          // радиус бортов
        public double Width;        // ширина шпули
        public double w = 0;            // угловая скорость вращения шпули

        public FishingLine line;    // леска
        public int n;            // глубина намотки в витках
        public double ln;           // остаточная длина на верхнем слое

        public double MechanicalBrake; // механический тормоз
        public double CentrifugalBrake; // центробежный тормоз
        public double LinearBrake; // магнитный линейный тормоз
        public double u; // коэффициент трения лески о шпулю

        // длина намотанной лески
        public double L 
        {
            get 
            {
                if (n <= 0)
                    return 0;
                else
                    return ln + Math.PI * (n - 1) * (2 * Rs1 + line.width * n) * Width / line.width;
            }
            set
            {
                double L = value;
                n = 0;
                double Ln = 0;
                while (L >= Ln)
                {
                    L -= Ln;
                    n++;
                    Ln = 2 * Math.PI * (Rs1 + n * line.width) * Width / line.width;
                }
                ln = L;
            }
        }

        // радиус намотки
        public double R
        {
            get 
            {
                return Rs1 + line.width * n;
            }
        }

        // момент инерции шпули
        public double J
        {
            get 
            {
                return Ms * Rs1 * Rs1 * 0.5 + (line.ml * L) * (Rs1 * Rs1 + R * R) * 0.5;
            }
        }

        public double LineRelease(double dl)
        {
            if (n > 0)
            {
                if (n == 1 && ln <= dl)
                {
                    dl = ln;
                    ln = n = 0;
                }
                else
                {
                    ln -= dl;

                    if (ln <= 0)
                    {
                        n--;
                        ln += 2 * Math.PI * (Rs1 + n * line.width) * Width / line.width;
                    }
                }
            }
            else 
            {
                dl = 0;
            }

            return dl;
        }
    }
}
