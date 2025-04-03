using System;
using System.Globalization;
using System.Threading;


public class ListenBMP180
{
    private readonly CultureInfo culture = CultureInfo.InvariantCulture;

    public void showBMP180(Func<string?> getNextSensorData)
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
            Console.WriteLine(sensorData);

            Thread.Sleep(100);
        }
    }




}