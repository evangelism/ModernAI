using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleDialogBot
{
    public enum Measurement { Temp = 1, Humidity = 2, Pressure = 4, None = 0 };

    public class WeatherParam
    {
        public DateTime When { get; set; }
        public string Location { get; set; }
        public Measurement MeasurementType { get; set; }
        public WeatherParam()
        {
            Location = "Moscow";
            When = DateTime.Now;
            MeasurementType = Measurement.Temp;
        }
        public void Today()
        {
            When = DateTime.Now;
        }
        public void Tomorrow()
        {
            When = DateTime.Now.AddDays(1);
        }

        public void AlsoMeasure(Measurement M)
        {
            MeasurementType |= M;
        }
        
        public bool Measure(Measurement M)
        {
            return (M & MeasurementType) > 0;
        }

        public int Offset
        {
            get
            {
                return (int)(((float)(When - DateTime.Now).Hours) / 24.0 + 0.5) ;
            }
        }
    }
}