using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Vives
{
    // Abstract sensordata class 
   abstract class SensorData
    {
        public string Name { get; set; }

        public SensorData (string _name)
        {
            Name = _name;
        }

        // all sensor data needs to be processed.
       public abstract int? ProcessData();
       public abstract void DataToIoFileAsync(string input); 

        public string SensorName ()
        {
            return Name; 
        }

    }
    // Temperature data class
    class Temperature : SensorData
    {
     
        private byte[] rawtemp;
        public int? temp { get; set; }
        StorageFolder storageFolder;
       


        // Constructor 
        public Temperature (byte[] _rawtemp,string _name) :base(_name)
        {
            rawtemp = _rawtemp;
        }

        // Process data
        public override int? ProcessData()
        {
            try
            {
                
                    byte[] tempArr = { rawtemp[0], rawtemp[1], rawtemp[2] };

                temp = (Convert.ToInt16(BitConverter.ToInt16(tempArr, 0) / 100));

                DataToIoFileAsync(temp.ToString());
            


            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                temp = null; 
            }
            return (temp);

        }

        //Processed data to file
        public override async void DataToIoFileAsync(string input)
        {

            await Task.Run(async () =>
            {
                await Task.Yield();

                storageFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;

                StorageFile sampleFile =
                   await storageFolder.GetFileAsync("TemperatureOutput.txt");


                await Windows.Storage.FileIO.AppendTextAsync(sampleFile, input + "\r\n");

                
            });


           
        }
    }
}
