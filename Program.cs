using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO.Ports;


class Programm    //Klassen müssen existieren, ansonsten klappt in Csharp gar nichts.
{
    static CalibrationData? calibration;     //Klasse CalibrationData wird instanziiert

    static void Main()
    {
        calibration = CalibrationStorage.Load();

        using ReadUSBPort usbReader = new ReadUSBPort();

        var connectedSensors = SensorDiscovery.DiscoverConnectedSensors(usbReader);

        StartingProtocol(connectedSensors, usbReader);
    }

    static void StartingProtocol(HashSet<SensorID> connectedSensors, ReadUSBPort usbReader)
    {
        Console.WriteLine("🔌 Starte System...");
        Console.WriteLine("\n🧭 Was möchtest du tun?");
        Console.WriteLine("Kalibration von Sensoren: 1");
        Console.WriteLine("Einzelnen Sensoren zuhören: 2");
        Console.WriteLine("Allen Sensoren zuhören: 3");

        string? inputOfUser = Console.ReadLine();
        switch (inputOfUser)
        {
            case "1":
                Calibration.CalibrationProtocol(connectedSensors, usbReader);
                break;
            case "2":
                SingleSensor.SingleSensorProtocol(connectedSensors, usbReader);
                break;
            case "3":
                ALLPROTOCOL.ListeningALLProtocol(usbReader, calibration);
                break;
            default:
                Console.WriteLine("❌ Ungültige Eingabe.");
                break;
        }
    }
    
}
