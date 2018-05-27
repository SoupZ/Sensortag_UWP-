using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vives;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensortagBLE
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
        private List<string> devices = new List<string>();
        private Sensortag TemperatureSensor;
        private Temperature rawSensorData;
        private int? actualTemperature;
        ReceiveJsonData receiveJson = new ReceiveJsonData();
        double? TemperatureKortrijk;



        private ulong sensortagId; //STATIC ID

        
        private readonly int sensorService = 5; //A40 HANDLE
        private readonly byte wakeSensor = 01; // 01 TO WAKE SENSOR
        private readonly int configRegister = 1; // THE SECOND REGISTER IS THE CONFIG REGISTER
        private readonly int dataRegister = 0; // THE FIRST REGISTER IS THE DATA REGISTER

        public MainPage()
        {
            this.InitializeComponent();
            btnGetValue.IsEnabled = false;
            btnUnpair.IsEnabled = false;
            btnPair.IsEnabled = false;

        }


        // Pairs the client with the server (Sensortag is the server)
        private async void btnPair_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                TemperatureSensor = new Sensortag(sensortagId, sensorService, wakeSensor, configRegister, dataRegister);
                await TemperatureSensor.PairSensorTagAsync();
            }
            catch (Exception ex)
            {
                Debug.Write(ex); 
            }
            //await Task.Delay(TimeSpan.FromMilliseconds(500));
            btnGetValue.IsEnabled = true;
            btnUnpair.IsEnabled = true;
        }
        // Scan for BLE devices
        private void btnScanBleDevices_Click(object sender, RoutedEventArgs e)
        {
            GetTemperatureKortrijkAsync(); 
            watcher.Received += Watcher_ReceivedAsync;


            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            // Set the in-range threshold to -70dBm. This means advertisements with RSSI >= -70dBm 
            // will start to be considered "in-range" (callbacks will start in this range).
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;

            // Set the out-of-range threshold to -75dBm (give some buffer). Used in conjunction 
            // with OutOfRangeTimeout to determine when an advertisement is no longer 
            // considered "in-range".
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;

            // Set the out-of-range timeout to be 2 seconds. Used in conjunction with 
            // OutOfRangeThresholdInDBm to determine when an advertisement is no longer 
            // considered "in-range"
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);




            watcher.Start();
        }
        // BLE device received delegate
        private async void Watcher_ReceivedAsync(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            devices.Add(args.Advertisement.LocalName + " " + args.BluetoothAddress);


            //(args.Advertisement.LocalName +  " " + args.BluetoothAddress.ToString());


            //List<string> distinct = devices.Distinct().ToList();

            //var codes = distinct.Where(x => x.Any(n => char.IsLetter(n)))
            //                .Select(x => x)
            //                .ToList();


            //var UniqueDevices = from device in devices
            //                    orderby device descending
            //                    select device;


            var Filter = from device in devices
                         let lowerdevice = device.ToLower()
                         where lowerdevice.Contains(tbxFilter.Text.ToLower())
                         orderby lowerdevice
                         select lowerdevice;






            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                lbxBLEDevices.Items.Clear();

                try
                {
                    foreach (var item in Filter.Distinct().ToList())
                    {
                        lbxBLEDevices.Items.Add(item);
                    }

                }
                catch(InvalidOperationException e)
                {
                    Debug.Write(e); 
                }
            });
        }
        // Gets temperature values
        private  void btnGetValue_ClickAsync(object sender, RoutedEventArgs e)
        {
            GetTemperatureKortrijkAsync();

             GetTemperatureSensor(); 


        }
        // Get's temperature sensor
        private async void GetTemperatureSensor ()
        {
            try { 
            // Getting Raw Temperature
            byte[] RawTemperature = await TemperatureSensor.ReadRawSensorValueAsync();
            // Procession Raw Temperature Bytes

            if (RawTemperature != null || RawTemperature.Length != null)
            {
                rawSensorData = new Temperature(RawTemperature, "HDC1000");


                actualTemperature = rawSensorData.ProcessData();

            }
            }
            catch (Exception e)
            {

            }



            #region Temperatuur SensorTag


            if (actualTemperature != null)
            {


                Debug.WriteLine(actualTemperature);
                txtbxCelsius.CelToFormattedCel(Convert.ToInt16(actualTemperature));
            }
            else
                txtblxStatus.Text = "Using cached value";
            #endregion
        }
        // Get's temperature Kortrijk
        private async void GetTemperatureKortrijkAsync ()
        {
            #region Temperatuur Kortrijk
            TemperatureKortrijk = await receiveJson.GetJSONAsync();


            if (TemperatureKortrijk != null)
                txtKortrijk.KelToCel(Convert.ToDouble(TemperatureKortrijk));

            else
                txtblxStatus.Text = "An error occurred while trying to get the JSON object. Check Internet connection.";
            #endregion
        }
        // Unpair the Sensortag
        private async void btnUnpair_ClickAsync(object sender, RoutedEventArgs e)
        {
            await TemperatureSensor.Unpair();
            txtblxStatus.Text = "N/A";
            btnGetValue.IsEnabled = false;
            btnPair.IsEnabled = false; 
        }
        //Double tapped ID to var, checks if the item is a sensortag. 
        private void lbxBLEDevices_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            try
            {
                var Device = (lbxBLEDevices.SelectedItem.ToString());


                var result = String.Join(" ", Device.Split(' ').Skip(2));

               // sensortagId = Convert.ToUInt64(result);

                bool isLong = ulong.TryParse(result.ToString(), out sensortagId); 

                if (isLong)
                {
                    btnPair.IsEnabled = true;
                    txtblxStatus.Text = "OK";
                }
                else
                {
                    throw new WrongBLEDeviceException("Unable to connect: device is not a sensortag.");
                }
               
            }
            catch (WrongBLEDeviceException ex)
            {

                txtblxStatus.Text = ex.Message.ToString(); 
            }
         
        }
        // Stops scanning when pointer enters listbox 
        //private void lbxBLEDevices_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    watcher.Stop();
            
        //}
    }
}
