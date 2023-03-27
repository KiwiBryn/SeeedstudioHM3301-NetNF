//---------------------------------------------------------------------------------
// Copyright (c) March 2023, devMobile Software
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
//
//---------------------------------------------------------------------------------
namespace devMobile.IoT.nanoFramework.HM3301
{
    using System;
    using System.Device.I2c;
    using System.Threading;

    public class Program
    {
        public static void Main()
        {
            I2cConnectionSettings i2cConnectionSettings = new(1, 0x40);

            // i2cDevice.Dispose
            I2cDevice i2cDevice = I2cDevice.Create(i2cConnectionSettings);

            while (true)
            {
                byte[] writeBuffer = new byte[1];
                byte[] readBuffer = new byte[29];

                writeBuffer[0] = 0x88;

                i2cDevice.WriteRead(writeBuffer, readBuffer);

                //i2cDevice.WriteByte(0x88);
                //i2cDevice.Read(readBuffer);

                ushort standardParticulatePm1 = (ushort)(readBuffer[4] << 8);
                standardParticulatePm1 |= readBuffer[5];

                ushort standardParticulatePm25 = (ushort)(readBuffer[6] << 8);
                standardParticulatePm25 |= readBuffer[7];

                ushort standardParticulatePm10 = (ushort)(readBuffer[8] << 8);
                standardParticulatePm10 |= readBuffer[9];

                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Standard particulate    PM 1.0: {standardParticulatePm1}  PM 2.5: {standardParticulatePm25}  PM 10.0: {standardParticulatePm10} ug/m3");

                ushort atmosphericPm1 = (ushort)(readBuffer[10] << 8);
                atmosphericPm1 |= readBuffer[11];

                ushort atmosphericPm25 = (ushort)(readBuffer[12] << 8);
                atmosphericPm25 |= readBuffer[13];

                ushort atmosphericPm10 = (ushort)(readBuffer[14] << 8);
                atmosphericPm10 |= readBuffer[15];

                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Atmospheric particulate PM 1.0: {atmosphericPm1:3}  PM 2.5: {atmosphericPm25}  PM 10.0: {atmosphericPm10} ug/m3");


                ushort particlateCountPm03 = (ushort)(readBuffer[16] << 8);
                particlateCountPm03 |= readBuffer[17];

                ushort particlateCountPm05 = (ushort)(readBuffer[18] << 8);
                particlateCountPm05 |= readBuffer[19];

                ushort particlateCountPm1 = (ushort)(readBuffer[20] << 8);
                particlateCountPm1 |= readBuffer[21];

                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Particulate count       PM 0.3: {particlateCountPm03:3}  PM 0.5: {particlateCountPm05}  PM 1.0: {particlateCountPm1} ug/m3");


                ushort particleCountPm25 = (ushort)(readBuffer[22] << 8);
                particleCountPm25 |= readBuffer[23];

                ushort particleCountPm5 = (ushort)(readBuffer[24] << 8);
                particleCountPm5 |= readBuffer[25];

                ushort particleCountPm10 = (ushort)(readBuffer[26] << 8);
                particleCountPm10 |= readBuffer[27];

                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Particle count/0.1L     PM 2.5: {particleCountPm25}  PM 5.0: {particleCountPm5}  PM 10.0: {particleCountPm10} particles/0.1L");


                byte checksum = 0;
                for (int i = 0; i < readBuffer.Length - 1; i++)
                {
                    checksum += readBuffer[i];
                }
                Console.WriteLine($"{DateTime.UtcNow:HH:mm:ss} Checksum payload:{readBuffer[28]} calculated:{checksum}");
                Console.WriteLine("");

                Thread.Sleep(5000);
            }
        }
    }
}
