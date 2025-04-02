using System;
using System.Globalization;
using System.Threading;

public class CurrentValues
{
    private readonly CultureInfo culture = CultureInfo.InvariantCulture;

    public void show(Func<string?> getNextSensorData)
    {
        while (true)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            {
                Console.WriteLine("⏹️ Beendet durch Benutzer.");
                break;
            }

            string? sensorData = getNextSensorData();
            if (string.IsNullOrEmpty(sensorData)) continue;

            ShowLive(sensorData);

            Thread.Sleep(100);
        }
    }

    private void ShowLive(string sensorData)
    {
        if (SensorParser.TryParseXYZ(sensorData, out double x, out double y, out double z))
        {
            Console.WriteLine($"📈 X: {x}, Y: {y}, Z: {z}");
        }
    }
}
