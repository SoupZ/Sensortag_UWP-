using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensortagBLE
{
    // Throws exception when the double tapped item is not in the right format, it's prob not a Sensortag. 
    class WrongBLEDeviceException :Exception
    {
        // Not a sensortag device
        public WrongBLEDeviceException(string message)
            : base(message)
        {
            Debug.Write(message);
        }

    }
}
