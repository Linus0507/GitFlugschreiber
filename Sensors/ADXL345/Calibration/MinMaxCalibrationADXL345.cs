using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;


public class MinMaxCalibrationADXL345
{
    private readonly Dictionary<string, CalibrationMeasurement> measurements = new();
    private readonly CultureInfo culture = CultureInfo.InvariantCulture;

    public void StartFullCalibration(Func<string?> getNextSensorData, string targetId)
    {
        string[] steps = { "xmax", "xmin", "ymax", "ymin", "zmax", "zmin" };

        foreach (string step in steps)
        {
            Console.WriteLine($"\n👉 Richte das Gerät so aus, dass **{step.ToUpper()}** erfasst wird.");
            Console.WriteLine("Drücke ENTER, um den aktuellen Wert zu speichern.\n");

            string latestData = "";

            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    SaveMeasurement(step, latestData);
                    break;
                }

                string? sensorData = getNextSensorData();
                if (string.IsNullOrEmpty(sensorData)) continue;

                latestData = sensorData;
                ShowLive(sensorData);

                Thread.Sleep(100);
            }
        }

        Console.WriteLine("\n✅ Kalibrierung abgeschlossen. Ergebnisse:");
        foreach (var pair in measurements)
        {
            Console.WriteLine($"{pair.Key.ToUpper()}: X={pair.Value.X}, Y={pair.Value.Y}, Z={pair.Value.Z}");
        }

        SaveMinMaxToJson(targetId);
    }

    private void ShowLive(string sensorData)
    {
        if (Adxl345Parser.TryParse(sensorData, out double x, out double y, out double z))
        {
            Console.WriteLine($"📈 X: {x}, Y: {y}, Z: {z}");
        }
    }

    private void SaveMeasurement(string key, string sensorData)
    {
        if (!Adxl345Parser.TryParse(sensorData, out double x, out double y, out double z))
        {
            Console.WriteLine($"⚠️ Fehler beim Parsen – {key.ToUpper()} nicht gespeichert.");
            return;
        }

        measurements[key] = new CalibrationMeasurement { X = x, Y = y, Z = z };
        Console.WriteLine($"✅ {key.ToUpper()} gespeichert: X={x}, Y={y}, Z={z}");
    }


    private void SaveMinMaxToJson(string targetId)
    {
        try
        {
            var data = new CalibrationData
            {
                XMax = measurements["xmax"].X,
                XMin = measurements["xmin"].X,
                YMax = measurements["ymax"].Y,
                YMin = measurements["ymin"].Y,
                ZMax = measurements["zmax"].Z,
                ZMin = measurements["zmin"].Z,

                ZeroX = (measurements["xmax"].X + measurements["xmin"].X) / 2.0,
                ZeroY = (measurements["ymax"].Y + measurements["ymin"].Y) / 2.0,
                ZeroZ = (measurements["zmax"].Z + measurements["zmin"].Z) / 2.0,


                // 🎁 Komplette Messvektoren mit reinpacken
                AllMeasurements = new Dictionary<string, CalibrationMeasurement>(measurements)
            };

            CalibrationStorage.Save(data, targetId);
            Console.WriteLine("💾 Kalibrierung in calibration.json gespeichert.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Speichern der Kalibrierung: {ex.Message}");
        }
    }

}
