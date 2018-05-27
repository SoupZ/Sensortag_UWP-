using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vives
{
    public class WeatherInfo
    {

        public Main main { get; set; }
        public List<Weather> weather { get; set; } // This is a List<T> of Weather
                                                   // It can contain more than one entry 
                                                   // for weather

        public class Main
        {
            public double Temp { get; set; } // This is a double
        }

        public class Weather
        {
            public string Description { get; set; }
        }
    }
}
