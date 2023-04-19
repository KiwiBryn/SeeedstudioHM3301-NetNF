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
// Seeedstudio Grove HM3301 - Laser PM1 PM2.5 PM10 Dust Sensor
//          https://www.seeedstudio.com/Grove-Laser-PM2-5-Sensor-HM3301.html
//
// Inspired by Arduino library https://github.com/tomoto/Arduino_Tomoto_HM330X thankyou Tomoto Shimizu Washio
//  
//---------------------------------------------------------------------------------
namespace devMobile.IoT.Device.SeeedstudioHM3301
{
    using System;
    using System.Device.I2c;
    using System.Device.Model;

    /// <summary>
    /// Particulate sensor Seeedstudio HM3301.
    /// </summary>
    [Interface("Seeedstudio Grove HM3301-Laser PM1 PM2.5 PM10 Dust Sensor")]

    public class SeeedstudioHM3301 : IDisposable
    {
        public const byte DefaultI2cAddress = 0x40;
        private const byte I2CReadCommand = 0x88;

        private I2cDevice _i2cDevice = null;

        public class StandardParticulate
        {
            public ushort PM1_0 { get; set; }
            public ushort PM2_5 { get; set; }
            public ushort PM10_0 { get; set; }
        }

        public class AtmosphericEnvironment
        {
            public ushort PM1_0 { get; set; }
            public ushort PM2_5 { get; set; }
            public ushort PM10_0 { get; set; }
        }

        public class ParticleCount
        {
            public ushort Diameter0_3 { get; set; }
            public ushort Diameter0_5 { get; set; }
            public ushort Diameter1_0 { get; set; }
            public ushort Diameter2_5 { get; set; }
            public ushort Diameter5_0 { get; set; }
            public ushort Diameter10_0 { get; set; }
        }

        /// <summary>
        ///  The standard particulate matter mass concentration value refers to the mass concentration
        ///  value obtained by density conversion of industrial metal particles as equivalent particles,
        ///  and is suitable for use in industrial production workshops and the like. The concentration
        ///  of particulate matter in the atmospheric environment is converted by the density of the
        ///  main pollutants in the air as equivalent particles, and is suitable for ordinary indoor and
        ///  outdoor atmospheric environments.
        /// </summary>
        public class ParticulateMeasurements
        {
            // public ushort SensorNumber { get; set; } // Not certain about this, does it ever change?

            /// <summary>
            /// Concentration based on CF=1 standard particulate matter (ug/m3).
            /// </summary>
            public StandardParticulate Standard { get; set; }

            /// <summary>
            /// Concentration based on the pollutants in the air (ug/m3).
            /// </summary>
            public AtmosphericEnvironment Atmospheric { get; set; }

            /// <summary>
            /// Number of particles with the specified diameter or above in 0.1L of air
            /// </summary>
            public ParticleCount Count { get; set; }
        }

        public SeeedstudioHM3301(I2cDevice i2cDevice)
        {
            _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(_i2cDevice));
        }

        /// <summary>
        /// Seeedstudio HM3301 particulate measurements.
        /// </summary>
        [Telemetry]
        public ParticulateMeasurements Read()
        {
            SpanByte writeBuffer = new byte[1];
            SpanByte readBuffer = new byte[29];

            writeBuffer[0] = I2CReadCommand;

            if ( _i2cDevice.WriteRead(writeBuffer, readBuffer).Status !=  I2cTransferStatus.FullTransfer)
            {
                throw new Exception("I2C Device WriteRead failed");
            }

            byte checksum = 0;
            for (int i = 0; i < readBuffer.Length - 1; i++)
            {
                checksum += readBuffer[i];
            }

            if (checksum != readBuffer[28])
            {
                throw new Exception("Checksum invalid");
            }

            return new ParticulateMeasurements()
            {
                // SensorNumber = ReadInt16BigEndian(readBuffer.Slice(4, 2)), // Not certain if this is necessary/good idea

                Standard = new StandardParticulate()
                {
                    PM1_0 = ReadInt16BigEndian(readBuffer.Slice(4, 2)),
                    PM2_5 = ReadInt16BigEndian(readBuffer.Slice(6, 2)),
                    PM10_0 = ReadInt16BigEndian(readBuffer.Slice(8, 2)),
                },

                Atmospheric = new AtmosphericEnvironment()
                {
                   PM1_0 = ReadInt16BigEndian(readBuffer.Slice(10, 2)),
                   PM2_5 = ReadInt16BigEndian(readBuffer.Slice(12, 2)),
                   PM10_0 = ReadInt16BigEndian(readBuffer.Slice(14, 2)),
                },

                Count = new ParticleCount()
                {
                    Diameter0_3 = ReadInt16BigEndian(readBuffer.Slice(16, 2)),
                    Diameter0_5 = ReadInt16BigEndian(readBuffer.Slice(18, 2)),
                    Diameter1_0 = ReadInt16BigEndian(readBuffer.Slice(20, 2)),
                    Diameter2_5 = ReadInt16BigEndian(readBuffer.Slice(22, 2)),
                    Diameter5_0 = ReadInt16BigEndian(readBuffer.Slice(24, 2)),
                    Diameter10_0 = ReadInt16BigEndian(readBuffer.Slice(26, 2)),
                }
            };
        }

        public void Dispose()
        {
            _i2cDevice?.Dispose();
            _i2cDevice = null;
        }

        private ushort ReadInt16BigEndian(SpanByte source)
        {
            if (source.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }

            ushort result = (ushort)(source[0] << 8);

            return result |= source[1];
        }
    }
}
