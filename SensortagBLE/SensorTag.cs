using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Vives
{
    class Sensortag
    {
        private ulong bleadress;
        private int irsensorservice;
        private byte wakesensor;
        private int configregister;
        private int sensordataregister;


        private BluetoothLEDevice bluetoothLeDevice;
        private GattCharacteristicsResult allCharacteristics;
        private DataReader dataReader;
        private GattCommunicationStatus communicationStatus;
        private DataWriter dataWriter;
        private GattDeviceService deviceService;
        private GattDeviceServicesResult deviceSerivceResult;
        private BluetoothLEDevice device;
        GattReadResult readResult;


        // Constructor
        public Sensortag(ulong _bleadress, int _irsensorservice, byte _wakesensor, int _configregister, int _sensordataregister) 
            
        {
            bleadress = _bleadress;
            irsensorservice = _irsensorservice;
            wakesensor = _wakesensor;
            configregister = _configregister;
            sensordataregister = _sensordataregister;

           
        }
        // Pairs device with Sensortag
        public async Task PairSensorTagAsync()
        {
             device = await BluetoothLEDevice.FromBluetoothAddressAsync(bleadress);


            bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device.DeviceId);

            

            var status = await device.DeviceInformation.Pairing.PairAsync();
            



            await GetGATTServicesDataAsync(device);


        }
        // Get all the GATT services from the Sensortag
        public async Task GetGATTServicesDataAsync(BluetoothLEDevice device)
        {
            deviceSerivceResult = await device.GetGattServicesAsync();

            deviceService = deviceSerivceResult.Services[irsensorservice];

            await GetGATTCharacteristicsDataAsync(deviceService);
        }
        // Get all the GATT Characteristics from the Sensortag
        public async Task GetGATTCharacteristicsDataAsync(GattDeviceService service)
        {
            allCharacteristics = await service.GetCharacteristicsAsync();

            await WakeUpSensorAsync(allCharacteristics);



        }
        // Wake up the Sensortag
        public async Task WakeUpSensorAsync(GattCharacteristicsResult gattCharacteristics)
        {
            dataWriter = new DataWriter();
            dataWriter.WriteByte(wakesensor);

            communicationStatus = await gattCharacteristics.Characteristics[configregister].WriteValueAsync((dataWriter.DetachBuffer()));
        

        }
        // Get raw byte with data from sensor
        public async Task<byte[]> ReadRawSensorValueAsync()
        {
            byte[] input = { 0 };
            try
            {
              

              
                readResult = await allCharacteristics.Characteristics[sensordataregister].ReadValueAsync
                     (BluetoothCacheMode.Cached);

               
                    dataReader = DataReader.FromBuffer(readResult.Value);
               


               
                     input = new byte[dataReader.UnconsumedBufferLength];
                    dataReader.ReadBytes(input);


                    return input;

                


            }
            catch(ObjectDisposedException ex)
            {
                Debug.Write(ex);
                return input;
            }

            return null; 

        }
        // Unpairs the device with the Sensortag.
        public async Task Unpair()
        {
            bluetoothLeDevice.Dispose();
            await bluetoothLeDevice.DeviceInformation.Pairing.UnpairAsync();

        }
     
    }
}

