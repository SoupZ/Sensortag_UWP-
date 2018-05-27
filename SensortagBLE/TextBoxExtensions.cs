using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Vives
{
    static class TextBoxExtensions
    {
        public static void KelToCel (this TextBox textbox,double kel) {

            textbox.Text = "Temperature in Kortrijk: " + (Convert.ToInt16 (kel - 273.15)) + "°C"; 
        }

        public static void CelToFormattedCel(this TextBox textbox, int cel)
        {
            textbox.Text = "Temperature Sensor: " + cel.ToString() + "°C";
        }
    }
}
