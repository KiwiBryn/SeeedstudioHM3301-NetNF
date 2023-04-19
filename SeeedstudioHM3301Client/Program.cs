//---------------------------------------------------------------------------------
// Copyright (c) April 2023, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// nanoff --target ST_STM32F769I_DISCOVERY --update 
// nanoff --platform ESP32 --serialport COM7 --update
//
//---------------------------------------------------------------------------------
#define ST_STM32F769I_DISCOVERY 
//#define  SPARKFUN_ESP32_THING_PLUS
namespace devMobile.IoT.Device.SeeedstudioHM3301
{
    using System;
    using System.Device.I2c;
    using System.Threading;

#if SPARKFUN_ESP32_THING_PLUS
    using nanoFramework.Hardware.Esp32;
#endif

    class Program
    {
        static void Main(string[] args)
        {
            const int busId = 1;

            Thread.Sleep(5000);

#if SPARKFUN_ESP32_THING_PLUS
            Configuration.SetPinFunction(Gpio.IO23, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(Gpio.IO22, DeviceFunction.I2C1_CLOCK);
#endif
            I2cConnectionSettings i2cConnectionSettings = new(busId, SeeedstudioHM3301.DefaultI2cAddress);

            using I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings);
            {
                using (SeeedstudioHM3301 seeedstudioHM3301 = new SeeedstudioHM3301(i2cDevice))
                {
                    while (true)
                    {
                        SeeedstudioHM3301.ParticulateMeasurements particulateMeasurements = seeedstudioHM3301.Read();

                        Console.WriteLine($"Standard PM1.0: {particulateMeasurements.Standard.PM1_0} ug/m3   PM2.5: {particulateMeasurements.Standard.PM2_5} ug/m3  PM10.0: {particulateMeasurements.Standard.PM10_0} ug/m3 ");
                        Console.WriteLine($"Atmospheric PM1.0: {particulateMeasurements.Atmospheric.PM1_0} ug/m3   PM2.5: {particulateMeasurements.Atmospheric.PM2_5} ug/m3  PM10.0: {particulateMeasurements.Standard.PM10_0} ug/m3");

                        // Always 0, checked payload so not a conversion issue. will check in Seeedstudio forums
                        // Console.WriteLine($"Count 0.3um: {particulateMeasurements.Count.Diameter0_3}/l 0.5um: {particulateMeasurements.Count.Diameter0_5} /l 1.0um : {particulateMeasurements.Count.Diameter1_0}/l 2.5um : {particulateMeasurements.Count.Diameter2_5}/l 5.0um : {particulateMeasurements.Count.Diameter5_0}/l 10.0um : {particulateMeasurements.Count.Diameter10_0}/l");

                        Thread.Sleep(new TimeSpan(0,1,0));
                    }
                }
            }
        }
    }
}
