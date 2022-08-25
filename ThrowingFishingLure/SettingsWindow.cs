using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ThrowingFishingLure
{
    public partial class SettingsWindow : Form
    {
        DisplayWindow displayWindow;

        enum FishingReelType { FlyReel = 0, SpinningReel = 1, BaitcastingReel = 2 };

        public SettingsWindow()
        {
            InitializeComponent();
        }


        private void modeling(FishingBait bait, FishingReel reel, FishingReelType reelType, double dt)
        {
            const double g = 9.8;

            reel.w = 0;
            bait.x = bait.y = 0;

            double l = 0;
            double t = 0;
            double timeToDraw = 0;

            double vl = 0;
            double Moment = 0;
            double Fu = 0;

            while (bait.y >= 0)
            {
                double s = Math.Sqrt(Math.Pow(bait.x, 2) + Math.Pow(bait.y, 2));
                                
                double F = 0;

                if (reelType == FishingReelType.SpinningReel)
                {
                    double B = Math.Atan(reel.Width / (2 * (reel.Rs2 - reel.R)));
                    Fu = (Math.PI - 2 * B) / Math.PI * reel.u;

                    if (Math.Abs(s - l) < 0.001)
                        F = Fu;
                }
                else 
                {
                    if ((s - l) > 0)
                        F = reel.line.k * (s - l);

                    if (reelType == FishingReelType.FlyReel)
                    {
                        Moment = F * reel.R - reel.MechanicalBrake * reel.Rs1;
                    }
                    else 
                    {
                        Moment = F * reel.R - reel.MechanicalBrake * reel.Rs1
                                - reel.CentrifugalBrake * Math.Pow(reel.w * reel.Rs1, 2)
                                - reel.LinearBrake * reel.w * reel.Rs1 * reel.Rs1;
                    }
                }

                double Fx = -bait.q * Math.Abs(bait.vx) * bait.vx - ((s > 0) ? F * bait.x / s : 0);
                double Fy = -bait.q * Math.Abs(bait.vy) * bait.vy - (bait.m + reel.line.ml * l) * g -
                ((s > 0)? F * bait.y / s : 0);

                bait.vx += Fx / bait.m * dt;
                bait.vy += Fy / bait.m * dt;

                bait.x += bait.vx * dt;
                bait.y += bait.vy * dt;

                double dl;

                if (reelType == FishingReelType.SpinningReel)
                {
                    s = Math.Sqrt(Math.Pow(bait.x, 2) + Math.Pow(bait.y, 2));
                    vl += -Fu * dt;
                    if (vl <= 0)
                        vl = 0;
                    dl = vl * dt;

                    if ((l + dl) < s)
                    {
                        vl = (s - l) / dt;
                        dl = s - l;
                    }
                }
                else
                {
                    reel.w += ((double)Moment / (double)reel.J) * dt;
                    if (reel.w <= 0)
                        reel.w = 0;
                    vl = reel.w * reel.R;
                    dl = vl * dt;
                }
                l += reel.LineRelease(dl);

                if (t > timeToDraw)
                {
                    printData(bait.m, bait.vx, vl, bait.x, bait.y, l, t);
                    timeToDraw += 0.01;
                }
                t += dt;
            }
        }

        /* Функция void printData(...) для вывода результатов
         * Параметры:
         *      m - масса приманки с леской
         *      v - скороть приманки
         *      _v - скорость схода лески
         *      s - расстояние, пройденное приманкой
         *      l - длина лески в полете
         *      t - момент времени
         */
        private void printData(double m, double v, double _v, double s, double h, double l, double t)
        {
            displayWindow.chart1.Series[0].Points.AddXY(t, s);
            displayWindow.chart1.Series[1].Points.AddXY(t, v);
            displayWindow.chart1.Series[2].Points.AddXY(t, _v);
            displayWindow.chart1.Series[3].Points.AddXY(t, l);
            displayWindow.chart1.Series[4].Points.AddXY(t, h);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            displayWindow = ((Form1)this.MdiParent).displayWindow;
            if (displayWindow == null)
            {
                displayWindow = new DisplayWindow();
                displayWindow.MdiParent = (Form1)this.MdiParent;
                displayWindow.FormClosed += new FormClosedEventHandler(((Form1)this.MdiParent).DisplayWindow_FormClosed);
                displayWindow.Show();
                ((Form1)this.MdiParent).displayWindow = displayWindow;
            }

            for (int i = 0; i < displayWindow.chart1.Series.Count; i++)
                displayWindow.chart1.Series[i].Points.Clear();

            FishingBait fishingBait = new FishingBait();
            fishingBait.m = (double)numericUpDown1.Value / 1000;
            fishingBait.q = (double)numericUpDown4.Value;
            double v0 = (double)numericUpDown2.Value;
            double alpha = (double)numericUpDown3.Value * Math.PI / 180;
            fishingBait.vx = v0 * Math.Cos(alpha);
            fishingBait.vy = v0 * Math.Sin(alpha);

            FishingLine fishingLine = new FishingLine();
            fishingLine.k = (double)numericUpDown10.Value;
            fishingLine.width = (double)numericUpDown14.Value / 1000;
            fishingLine.ml = (double)numericUpDown9.Value / 1000;

            FishingReel fishingReel = new FishingReel();
            fishingReel.line = fishingLine;
            fishingReel.Ms = (double)numericUpDown5.Value / 1000;
            fishingReel.Rs1 = (double)numericUpDown6.Value / 1000 / 2;
            fishingReel.Rs2 = (double)numericUpDown8.Value / 1000 / 2;
            fishingReel.Width = (double)numericUpDown13.Value / 1000;
            fishingReel.MechanicalBrake = (double)numericUpDown12.Value;
            fishingReel.CentrifugalBrake = 0.0001 * (double)numericUpDown17.Value;
            fishingReel.LinearBrake = 0.005 * (double)numericUpDown7.Value;
            fishingReel.L = (double)numericUpDown15.Value;
            fishingReel.u = (double)numericUpDown11.Value;
            
            FishingReelType fishingReelType = (FishingReelType)comboBox2.SelectedIndex;

            double dt = (double)numericUpDown16.Value;

            //this.label22.Text = (fishingReel.R * 2).ToString();

            modeling(fishingBait, fishingReel, fishingReelType, dt);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FishingReelType fishingReelType = (FishingReelType)((ComboBox)sender).SelectedIndex;

            switch (fishingReelType)
            {
                case FishingReelType.FlyReel:
                    numericUpDown12.Enabled = true; // Осевой тормоз
                    numericUpDown5.Enabled = true; // Масса шпули
                    numericUpDown17.Enabled = false; // Центробежный тормоз
                    numericUpDown7.Enabled = false;  // Магнитный тормоз
                    numericUpDown11.Enabled = false; // коэффициент трения о борт шпули
                    break;
                case FishingReelType.BaitcastingReel:
                    numericUpDown12.Enabled = true; // Осевой тормоз
                    numericUpDown5.Enabled = true; // Масса шпули
                    numericUpDown17.Enabled = true; // Центробежный тормоз
                    numericUpDown7.Enabled = true;  // Магнитный тормоз
                    numericUpDown11.Enabled = false; // коэффициент трения о борт шпули
                    break;
                case FishingReelType.SpinningReel:
                    numericUpDown12.Enabled = false; // Осевой тормоз
                    numericUpDown5.Enabled = false; // Масса шпули
                    numericUpDown17.Enabled = false; // Центробежный тормоз
                    numericUpDown7.Enabled = false;  // Магнитный тормоз
                    numericUpDown11.Enabled = true; // коэффициент трения о борт шпули
                    break;
            }
        }

        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
        }

        static int GetDecimalDigitsCount(decimal number)
        {
            string str = number.ToString(new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "," });
            return str.Contains(",") ? str.Remove(0, Math.Truncate(number).ToString().Length + 1).Length : 0;
        }
        private void numericUpDownCommon_ValueChanged(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).DecimalPlaces = GetDecimalDigitsCount(((NumericUpDown)sender).Value);
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }
}
