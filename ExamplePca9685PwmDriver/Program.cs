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
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using System.Threading;

namespace ExamplePca9685PwmDriver
{
    public class Program
    {
        public static void Main()
        {
            Pca9685 pca = new Pca9685(20);
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);

            pca.Start();
            led.Write(true);
            
            while(true)
            {
                for (int i = 7; i < 8; i++)
                {
                    Debug.Print("Output:" + i.ToString());

                    for (int i1 = 150; i1 < 450; i1++)
                    {
                        pca.SetPwm((byte)i, 0, i1);
                        Thread.Sleep(20);
                    }
                   
                    led.Write(!led.Read());
                    pca.SetPwm((byte)i, 0, 300);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
