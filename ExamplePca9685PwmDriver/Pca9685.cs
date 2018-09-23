/// #########################################################################################################
///
///  Blog: Meine Welt in meinem Kopf
///  Post: 16 Chanel Pwm Driver mit dem Netduino
///  Postdate: 20.09.2018
///  --------------------------------------------------------------------------------------------------------
///  Kurze Information:
///  Diese Solution dient als Quellcode Beispiel und zeigt lediglich 
///  die Funktionsweise für das Initialisieren des Sensors und abruf der Daten.
///  Fehlerbehandlung, sowie Logging oder andere Erweiterungen 
///  für eine stabile Laufzeit der Anwendung sind nicht vorhanden.
///  
///  Für Änderungsvorschläge oder ergänzende Informationen zu meiner
///  Beispiel Anwendung, der oder die kann mich unter der Mail Adresse 
///  j.langner@gmx.net erreichen.
///  
///  Referenzen:
///  https://github.com/adafruit/Adafruit-PWM-Servo-Driver-Library/blob/master/Adafruit_PWMServoDriver.cpp
///  https://cdn-shop.adafruit.com/datasheets/PCA9685.pdf
///  
///  Vorraussetzung:
///  Netduino
///  PCA9685 16 channel pwm driver
/// 
/// #########################################################################################################

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace ExamplePca9685PwmDriver
{
    public class Pca9685 : I2CDevice
    {
        private readonly byte PCA9685_MODE1 = 0x00;
        private readonly byte PCA9685_PRESCALE = 0xFE;

        private readonly byte LED0_ON_L = 0x06;
        private readonly byte LED0_ON_H = 0x07;

        private readonly byte LED0_OFF_L = 0x08;
        private readonly byte LED0_OFF_H = 0x09;

        private readonly byte ALL_LED_ON_L = 0xFA;
        private readonly byte ALL_LED_ON_H = 0xFB;
        
        private int _period;
        
        /// <summary>
        ///  Default I²C configuration are set address 0x40 and clock 100kHz
        /// </summary>
        /// <param name="period">Period (ms)</param>
        public Pca9685(int period) : base(new Configuration(0x40, 100)) 
        {
            this._period = period * 1000;
        }

        /// <summary>
        /// Return to default mode
        /// </summary>
        public void Reset()
        {
            this.Write(new byte[]{ PCA9685_MODE1, 0x00 });
            Thread.Sleep(10);
        }

        /// <summary>
        /// Set default and set frequence.
        /// </summary>
        internal void Start()
        {
            int hz = 1000000 / this._period;

            this.Reset();
            this.SetPwmFrequency(hz);
        }
        
        /// <summary>
        /// Set a new frequency.
        /// </summary>
        /// <param name="frequency"></param>
        public void SetPwmFrequency(float frequency)
        {
            // internal clock frequency
            frequency *= 0.9f;
            float prescaleval = 25000000;
            prescaleval /= 4096;
            prescaleval /= frequency;
            prescaleval -= 1;

            byte prescale = (byte)(prescaleval + 0.5);

            byte[] buffer = new byte[1] { PCA9685_MODE1 };
            
            if (this.Read(buffer) != 1)
            {
                throw new Exception("can not read mode1");
            }

            byte oldMode = buffer[0];

            // sleep
            byte newMode = (byte)(((byte)oldMode & (byte)0x7F) | (byte)0x10);

            // go to sleep
            this.Write(new byte[] { PCA9685_MODE1, newMode });
            this.Write(new byte[] { PCA9685_PRESCALE, prescale });
            this.Write(new byte[] { PCA9685_MODE1, oldMode });
            Thread.Sleep(5);
            this.Write(new byte[] { PCA9685_MODE1, (byte)(oldMode | 0x80) });

            byte[] bufferRead = new byte[] { PCA9685_MODE1 };
            this.Read(bufferRead);
            Debug.Print("Mode now: " + bufferRead[0].ToString());
        }
        
        /// <summary>
        /// Set new puls.
        /// </summary>
        /// <param name="outputNumber">Set the number output.</param>
        /// <param name="on">set high puls.</param>
        /// <param name="off">set low puls.</param>
        internal void SetPwm(byte outputNumber, int on, int off)
        {
            Debug.Print("Output: " + outputNumber.ToString() + ", ON: " + on.ToString() + ", OFF: " + off.ToString());

            byte targetOutput_ON_L = (byte)(LED0_ON_L + 4 * outputNumber);
            byte targetOutput_ON_H = (byte)(LED0_ON_H + 4 * outputNumber);
            byte targetOutput_OFF_L = (byte)(LED0_OFF_L + 4 * outputNumber);
            byte targetOutput_OFF_H = (byte)(LED0_OFF_H + 4 * outputNumber);

            int result1 = this.Write(new byte[] { targetOutput_ON_L, (byte)on });
            int result2 = this.Write(new byte[] { targetOutput_ON_H, (byte)(on >> 8) });
            int result3 = this.Write(new byte[] { targetOutput_OFF_L, (byte)(off) });
            int result4 = this.Write(new byte[] { targetOutput_OFF_H, (byte)(off >> 8) });
        }

        /// <summary>
        /// Send the commands.
        /// </summary>
        /// <param name="buffer">Commands and values set by byte array</param>
        /// <returns>Return the coun of byte sended.</returns>
        private int Write(byte[] buffer)
        {
            I2CTransaction[] trans = new I2CTransaction[]
            {
                CreateWriteTransaction(buffer)
            };

            return Execute(trans, 1000);
        }
        
        /// <summary>
        /// Liest den zu empfangene Byte Array
        /// </summary>
        /// <param name="buffer">Übergabe des zu beschreibenen Byte Arrays für die Messergebnissaufnahme.</param>
        /// <returns>Gibt die Anzahl der Bytes, die Empfangen wurden.</returns>
        private int Read(byte[] buffer)
        {
            I2CTransaction[] trans = new I2CTransaction[]
            {
                CreateReadTransaction(buffer)
            };

            return Execute(trans, 1000);
        }
    }
}
