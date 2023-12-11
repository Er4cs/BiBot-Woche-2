using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules; lolololololololol
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.VolkswagenBildungsinstitutGmbH;

namespace Backup_Project
{
    public partial class Program
    {
        byte poll = 0; // jeweils ein Byte pro Pollswitch
        bool Pollcheck = false;
        bool Linienstatus = false;
        int rescueCounter = 0; // Counter zählt immer hoch, wenn ein Poll tick auslöst
        GT.Timer Linetimer = null;
        GT.Timer PollTimer = null;
        GT.Timer followTimer = null;
        int LEDLcounter = 0;
        int LEDRcounter = 0;
        GT.Timer PollTimerBecher;
        void ProgramStarted()
        {


            #region Programminitialisierung
            //Debug.Print(reflector.LeftReflectorVoltage.ToString());
            //Debug.Print(reflector.RightReflectorVoltage.ToString());
            //Debug.Print("Prgramm Startet");
            multicolorLED.BlinkRepeatedly(GT.Color.White);
            Thread.Sleep(4000);
            multicolorLED.TurnGreen();
            Thread.Sleep(2000);
            pollSwitch.PollSwitchChanged += pollSwitch_PollSwitchChanged;
            //Linetimer = new GT.Timer(50);
            //Linetimer.Tick += Linetimer_Tick;// Timer Event deklarieren
            followTimer = new GT.Timer(50);
            followTimer.Tick += followTimer_Tick;// Timer Event deklarieren
            GT.Timer polltimer = new GT.Timer(50);
            polltimer.Tick += polltimer_Tick;// Timer Event deklarieren

            while (poll == 0) ; // Warten auf Pollswitch Tastendruck

            switch (poll)
            {
                case 1:
                    poll = 0;
                    polltimer.Start(); // Timer starten
                    SetMotors();
                    break;
                case 2:
                    Thread.Sleep(50);
                    poll = 0;
                    Linetimer.Start();

                    PollTimer = new GT.Timer(50, GT.Timer.BehaviorType.RunOnce);
                    PollTimer.Tick += polltimer_Tick;
                    SetMotors();
                    break;
                case 4:
                    poll = 0;
                    followTimer.Start();
                    SetMotors();
                    break;
                case 16:
                    poll = 0;
                    PollTimerBecher = new GT.Timer(50);
                    PollTimerBecher.Tick += polltimer_Tick1;
                    //Linetimer.Start(); // Startet für das Fahren innerhalb des Kreises
                    PollTimerBecher.Start();
                    SetMotors();
                    break;
            }
            #endregion
        }
        void polltimer_Tick1(GT.Timer timer)
        {
            double distance;
            //while (true)
            //    {
            //    Debug.Print(distanceUS3.GetDistance().ToString());
            //    Thread.Sleep(1000);
            //    if (distanceUS3 >= 25)
            //    {

            //    }

            //    }
            //Debug.Print("polltimer_Tick1");
            //Debug.Print(poll.ToString());

            if (Pollcheck == false)
                switch (poll)
                {
                    case 0:
                        distance = distanceUS3.GetDistance();

                        if (distance < 40) //Becher vorraus
                        {
                            multicolorLED.BlinkRepeatedly(GT.Color.Blue);
                            SetMotors(90, 90); // Becher wird beachtet
                        }
                        else
                        {
                            SetMotors(100, 70);
                        }
                        break;
                    case 1: 
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(-80, 80, 80);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 2: 
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(-80, 80, 70);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 3: 
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(-80, 80, 80);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 4: 
                        Pollcheck = true;
                        Linetimer.Stop();
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 8: 
                        Pollcheck = true;
                        Linetimer.Stop();
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 12: 
                        Pollcheck = true;
                        Linetimer.Stop();
                        multicolorLED.TurnColor(GT.Color.Red);
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(100, 100);
                        multicolorLED.TurnGreen();
                        break;
                    case 32: 
                        rescueCounter = 0;
                        poll = 0;
                        SetMotors(-100, -100, 600);
                        SetMotors(80, -80, 100);
                        SetMotors(100, 100);
                        break;
                }
            if (Pollcheck == true)
                BecherAbsetzen();
        }

        void polltimer_Tick(GT.Timer timer) // Pollswitch Zeitsteuerung
        {
            Debug.Print("polltimer_Tick");
            Debug.Print(poll.ToString());

            switch (poll)
            {
                case 1: // Drehung Rechts 90°
                    multicolorLED.TurnColor(GT.Color.Red);
                    rescueCounter = 0;
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(100, -100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnGreen();
                    break;
                case 2:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(100, -100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnWhite();
                    break;
                case 3:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(100, -100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnGreen();
                    break;
                case 16:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(-100, 100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnWhite();
                    break;
                case 18:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(100, -100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnWhite();
                    break;
                case 32:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(-90, -90, 1500);
                    SetMotors(-100, 100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnGreen();
                    break;
                case 48:
                    multicolorLED.TurnColor(GT.Color.Red);
                    poll = 0;
                    LEDLcounter = 0;
                    LEDRcounter = 0;
                    rescueCounter = 0;
                    SetMotors(0, 0, 50);
                    SetMotors(-90, -90, 2000);
                    SetMotors(100, -100, 700);
                    SetMotors(100, 100);
                    multicolorLED.TurnGreen();
                    break;
            }

            rescueCounter++; // Rettungsprogramm, nur wenn kein Poll für 15sek auslöst

            if (rescueCounter > 300)
                Rescue();

        }

        void pollSwitch_PollSwitchChanged(object sender, PollSwitchChangedEventArgs e)
        {
            Debug.Print("PollswitchChanged");
            poll = e.PollSwitch;
        }

        void SetMotors(int motorSpeed1 = 100, int motorSpeed2 = 100, int delay = 0) // Abkürzung zur Motorsteuerung
        {
            motorControllerL298.MoveMotor(MotorControllerL298.Motor.Motor1, motorSpeed1);
            motorControllerL298.MoveMotor(MotorControllerL298.Motor.Motor2, motorSpeed2);
            Thread.Sleep(delay);
        }
        void BecherAbsetzen()
        {
            double leftreflector = reflector.LeftReflectorVoltage;
            double rightreflector = reflector.RightReflectorVoltage;
            double grenzwert = 2.1;

            if (leftreflector >= grenzwert)
            {
                SetMotors(-100, -100, 1000);
                SetMotors(-100, 100, 250);
                SetMotors();
                followTimer.Start();
                PollTimerBecher.Stop();
            }
            else if (rightreflector >= grenzwert)
            {
                SetMotors(-100, -100, 1000);
                SetMotors(-100, 100, 250);
                SetMotors();
                followTimer.Start();
                PollTimerBecher.Stop();
            }
            else if (leftreflector > grenzwert && rightreflector >= grenzwert) // wenn beides nicht in Kraft tritt, dann prüfe ob beide Sensoren ausgelöst wurden. Wenn ja dann:
            {
                multicolorLED.BlinkRepeatedly(GT.Color.Red); 
                rescueCounter = 0; // Rettungscounter zurücksetzen
                SetMotors(-90, -90, 1000);
                SetMotors(-90, 90, 250);
                SetMotors();
                followTimer.Start();
                PollTimerBecher.Stop();
            }
        }
        void followTimer_Tick(GT.Timer timer)
        {
            double grenzwert = 2.1;
            if (rescueCounter >= 0)
                //{
                //    PollTimer = new GT.Timer(50);
                //    polltimer_Tick(PollTimer);
                //}
                if (reflector.RightReflectorVoltage >= grenzwert)
                {
                    LEDRcounter++;
                    rescueCounter = 0;
                    SetMotors(100, 50, 650);
                    SetMotors();
                    Linienstatus = true;
                }
            if (reflector.LeftReflectorVoltage >= grenzwert)
            {
                LEDLcounter++;
                rescueCounter = 0;
                SetMotors(50, 100, 650);
                SetMotors();
                Linienstatus = true;
            }
            if (reflector.LeftReflectorVoltage > grenzwert && reflector.RightReflectorVoltage >= grenzwert) // Wenn beide Reflektoren auslösen
            {
                SetMotors(-100, -100, 500);
                SetMotors(100, -100, 500);
                SetMotors();
                Linienstatus = true; //Linienstatus wird gesetzt
            }
            if (Linienstatus == true)
            {
                if (reflector.RightReflectorVoltage < grenzwert && reflector.LeftReflectorVoltage < 2.3)
                {
                    SetMotors();
                    multicolorLED.TurnColor(GT.Color.Purple);
                }
                if (reflector.RightReflectorVoltage >= 2.3)
                {
                    SetMotors(90, 100, 250);
                    SetMotors();
                    multicolorLED.TurnBlue();
                }
                if (reflector.LeftReflectorVoltage >= 2.3)
                {
                    SetMotors(100, 90, 250);
                    SetMotors();
                    multicolorLED.TurnWhite();
                }

                //if (LEDLcounter > LEDRcounter)
                //{
                //    multicolorLED.TurnColor(GT.Color.Green);
                //}
                //else if (LEDRcounter > LEDLcounter)
                //{
                //    multicolorLED.TurnColor(GT.Color.Red);
                //}
                if (poll != 0)
                {
                    SetMotors(-100, -100, 600);
                    SetMotors(0, 0);
                    Pollcheck = false;
                    Linienstatus = false;
                    followTimer.Stop();
                    ProgramStarted();
                    multicolorLED.TurnGreen();
                }
            }
        }

        // Timer für das Fahren auf der schwarzen Linie

        void Rescue() // Ablauf Rettungsprogramm
        {
            Debug.Print("Rescue gestartet"); // Ausgabe in Debug Konsole
            multicolorLED.BlinkRepeatedly(GT.Color.Red);
            SetMotors(-90, -90);
            Thread.Sleep(2000);
            SetMotors(-90, 80);
            Thread.Sleep(2000);
            SetMotors(100, 100);
            rescueCounter = 0;
            SetMotors();
            multicolorLED.BlinkRepeatedly(GT.Color.Green);
        }
        void Linetimer_Tick(GT.Timer timer) // Timer für das überfahren der schwarzen Linie
        {
            double leftreflector = reflector.LeftReflectorVoltage; // verkürzen der Eingabewerte für den Reflektor L
            double rightreflector = reflector.RightReflectorVoltage; // verkürzen der Eingabewerte für den Reflektor R
            double grenzwert = 2.3; // ersatz der Zahlen durch *Grenzwert* -> einfacher änderbar

            //Debug.Print("Linetimer");
            //Debug.Print(reflector.RightReflectorVoltage.ToString());
            //Debug.Print(reflector.LeftReflectorVoltage.ToString());

            if (rightreflector > grenzwert) // wenn Reflektor R den Grenzwert überschreitet, dann:
            {
                multicolorLED.BlinkRepeatedly(GT.Color.Red); // LED Rot
                rescueCounter = 0; // Rettungscounter zurücksetzen (Da Signal ausgelöst wurde)
                SetMotors(90, -90, 800); // Motoren einstellen (L 90, R -90, 800ms Delay)
                SetMotors(); //Motoren auf Standardwert zurücksetzen (oben definiert)
                multicolorLED.TurnGreen(); // LED Grün
            }
            else if (leftreflector > grenzwert) // wenn "if" nicht in Kraft tritt, überprüfe ob Reflektor L den Grenzwert überschreitet. Wenn ja dann:
            {
                multicolorLED.BlinkRepeatedly(GT.Color.Red); // LED Rot
                rescueCounter = 0; // Rettungscounter zurücksetzen
                SetMotors(-90, 90, 800); // Motoren einstellen
                SetMotors(); // Motoren auf Standardwert zurücksetzen (Geradeaus)
                multicolorLED.TurnGreen(); // LED grün
            }
            else if (leftreflector > grenzwert && rightreflector >= grenzwert) // wenn beides nicht in Kraft tritt, dann prüfe ob beide Sensoren ausgelöst wurden. Wenn ja dann:
            {
                multicolorLED.BlinkRepeatedly(GT.Color.Red); // LED Rot
                rescueCounter = 0; // Rettungscounter zurücksetzen
                SetMotors(-90, -90, 500);
                SetMotors(-90, 90, 1500);
                SetMotors();
                multicolorLED.TurnGreen();
            }
            //rescueCounter++; // Rettungscounter -> zählt alle 50ms hoch

            //if (rescueCounter > 300)   // Wenn Wert 300 erreicht -> starte Rettungsprogramm
            //    Rescue();

            //if (rescueCounter >= 0)
            //    //polltimer_Tick(Linetimer);
            //    PollTimer.Start();
        }

    }
}

