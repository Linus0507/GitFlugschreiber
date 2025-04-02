using System;
using System.IO;
using System.Text.Json;

public static class CalibrationStorage
{
    private const string FilePath = "calibration.json";

    public static CalibrationData Load()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("⚠️ Keine Kalibrierdatei gefunden – lege neue an.");
            var empty = new CalibrationData();
            Save(empty, "default");  // oder irgendeine sinnvolle targetId
            return empty;
        }

        string json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<CalibrationData>(json)!;
    }

    public static void Save(CalibrationData data, string targetId)
    {
        data.TargetId = targetId;  // 🧠 speichere die Ziel-ID im Objekt
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
        Console.WriteLine($"💾 Kalibrierungsdaten für '{targetId}' gespeichert.");
    }

}



