using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class ALLPROTOCOL
{
    public static void ListeningALLProtocol(ReadUSBPort usbReader, CalibrationData? calibration)
    {
        Console.WriteLine("📡 Starte ALLPROTOCOL – alle Sensoren gleichzeitig überwachen");
        Console.WriteLine("Beenden mit 'q'\n");

        var interpreters = new Dictionary<SensorID, object>
        {
            { SensorID.ADXL345, new Adxl345Interpreter(calibration) }
        };

        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var tasks = new List<Task>();

        // Starte alle Interpreter in Tasks
        foreach (var kvp in interpreters)
        {
            SensorID sensorId = kvp.Key;
            var interpreter = (Adxl345Interpreter)kvp.Value;

            var task = Task.Run(() =>
            {
                interpreter.StartLiveInterpretation(usbReader, sensorId, token);
            }, token);

            tasks.Add(task);
        }

        // Warten auf Benutzereingabe
        while (true)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            {
                Console.WriteLine("🧹 Stoppe alle Sensor-Tasks...");
                cts.Cancel();
                Task.WaitAll(tasks.ToArray()); // Warten auf sauberen Shutdown
                Console.WriteLine("✅ Alle Tasks sauber beendet.");
                break;
            }

            Thread.Sleep(100);
        }
    }
}
