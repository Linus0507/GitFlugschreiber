using System;
using System.Linq; // wichtig für .ToList() und LINQ
using System.Collections.Generic; // für HashSet etc.

public static class Calibration
{
    public static void CalibrationProtocol(HashSet<SensorID> connectedSensors, ReadUSBPort usbReader)
    {
        Console.WriteLine("🔧 Welchen Sensor möchtest du kalibrieren?");
        Console.WriteLine("📋 Angeschlossene Sensoren (nur kalibrierbare):");

        // Sensoren filtern, die nicht in SensorsWOCali stehen
        var filteredSensors = connectedSensors
            .Where(sensor => !SensorConfig.SensorsWOCali.Contains(sensor.ToString()))
            .ToList();

        // Leere Liste? Nichts zu tun.
        if (filteredSensors.Count == 0)
        {
            Console.WriteLine("🛑 Keine kalibrierbaren Sensoren erkannt.");
            return;
        }

        for (int i = 0; i < filteredSensors.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {filteredSensors[i]}");
        }

        string? input = Console.ReadLine();

        if (int.TryParse(input, out int selection) &&
            selection >= 1 && selection <= filteredSensors.Count)
        {
            SensorID selectedSensor = filteredSensors[selection - 1];
            Console.WriteLine($"✅ Du hast Sensor {selectedSensor} ausgewählt.");

            calibrationADXL345.calibrationProtocolADXL345(selectedSensor, usbReader);
        }
        else
        {
            Console.WriteLine("❌ Ungültige Eingabe.");
        }
    }
}
