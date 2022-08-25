using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThrowingFishingLure
{
    public partial class DisplayWindow : Form
    {
        public DisplayWindow()
        {
            InitializeComponent();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Minimum = (double)numericUpDown11.Value;
            chart1.ChartAreas[0].AxisX.Maximum = (double)numericUpDown7.Value;
            chart1.ChartAreas[0].AxisY.Maximum = (double)numericUpDown8.Value;
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.Maximum = (double)numericUpDown7.Value;
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Minimum = (double)numericUpDown11.Value;
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Maximum = (double)numericUpDown8.Value;
        }

        private void DisplayWindow_Load(object sender, EventArgs e)
        {
            this.numericUpDown7.ValueChanged += numericUpDownCommon_ValueChanged;
            this.numericUpDown8.ValueChanged += numericUpDownCommon_ValueChanged;
            this.numericUpDown11.ValueChanged += numericUpDownCommon_ValueChanged;
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
    }
}
