using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Vives;

namespace SensortagBLE
{
    // This method gets a JSON object with the current weather in Kortrijk
    // Return value can be null. This means there is an error. 
    // It reads the entire body and converts it to class properties. 
    class ReceiveJsonData
    {
       
       readonly private string jsonUrl = "http://api.openweathermap.org/data/2.5/weather?q=Kortrijk&appid=d7d36960509ba1af2626f5ef5883f79a";
       private StreamReader streamReader;
       private Stream responseString;
       private HttpResponseMessage httpResponseMessage;
       public double? temperature; 
       WeatherInfo dataClass;


        
        public async Task<double?> GetJSONAsync()
        {
            

            using (var client = new HttpClient())
            {
                try { 
                

                httpResponseMessage = await client.GetAsync(jsonUrl);

                responseString = await httpResponseMessage.Content.ReadAsStreamAsync();

                streamReader = new StreamReader(responseString);


                    dataClass = JsonConvert.DeserializeObject<WeatherInfo>(streamReader.ReadToEnd());
                    temperature = dataClass.main.Temp;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex); 
                    temperature = null;
                }

                return temperature;
               

            }



        }

    }
}
