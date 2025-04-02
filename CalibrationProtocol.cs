using System;


public static class Calibration
{


    public static void CalibrationProtocol(HashSet<SensorID> connectedSensors, ReadUSBPort usbReader)
    {
        Console.WriteLine("🔧 Welchen Sensor möchtest du kalibrieren?");
        Console.WriteLine("📋 Angeschlossene Sensoren:");

        // HashSet in Liste für geordnete Auswahl
        var sensorList = connectedSensors.ToList();

        for (int i = 0; i < sensorList.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {sensorList[i]}");
        }

        string? input = Console.ReadLine();

        if (int.TryParse(input, out int selection) &&
            selection >= 1 && selection <= sensorList.Count)
        {
            SensorID selectedSensor = sensorList[selection - 1];
            Console.WriteLine($"✅ Du hast Sensor {selectedSensor} ausgewählt.");
            
            calibrationADXL345.calibrationProtocolADXL345(selectedSensor, usbReader);
        }
        else
        {
            Console.WriteLine("❌ Ungültige Eingabe.");
        }
    }
}
