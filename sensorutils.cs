using System;
using System.IO.Ports;



public class ReadUSBPort : IDisposable
{
    private SerialPort port;        //nutzt externe Klasse SerialPort um ein objekt port zu instanziieren

    public ReadUSBPort(string portName = "/dev/ttyUSB0", int baudRate = 115200) //hier wird das port objekt instanziiert. Mit portname und baudrate
    {
        port = new SerialPort(portName, baudRate);      
        port.ReadTimeout = 500;
        port.Open();
    }

    public string? ReadData()        //returned schlicht was der port ausgibt als string
    {
        try
        {
            if (port.IsOpen)
                return port.ReadLine();
        }
        catch (TimeoutException)
        {
            return null;
        }
        return null;
    }

    public void Close()     //schhließt die Verbindung zum Port
    {
        if (port.IsOpen)
            port.Close();
    }

    public void Dispose()
    {
        Close();
        port?.Dispose(); // Für saubere Aufräumarbeiten
    }
}



public static class SensorUtils
{
    public static Func<string?> CreateSensorReader(ReadUSBPort usbReader, string targetId)
    {
        return () =>
        {
            while (true)
            {
                string? data = usbReader.ReadData();
                if (string.IsNullOrEmpty(data)) return null;

                string IDent = data.Substring(0, 1);
                string sensorData = data.Substring(1);

                if (IDent == targetId)
                    return sensorData;
            }
        };
    }
}

public enum SensorID
{
    ADXL345 = 1,
    // Hier kannst du später weitere Sensoren ergänzen:
    // Gyroskop = 2,
    // Magnetometer = 3,
}

public static class SensorDiscovery
{
    public static HashSet<SensorID> DiscoverConnectedSensors(ReadUSBPort usbReader, int timeoutMs = 3000)
    {
        var foundSensors = new HashSet<SensorID>();
        var endTime = DateTime.Now.AddMilliseconds(timeoutMs);

        Console.WriteLine("🔍 Suche nach verbundenen Sensoren...");

        while (DateTime.Now < endTime)
        {
            string? data = usbReader.ReadData();
            if (string.IsNullOrEmpty(data)) continue;

            string id = data.Substring(0, 1);

            if (int.TryParse(id, out int numericId) &&
                Enum.IsDefined(typeof(SensorID), numericId))
            {
                var sensorId = (SensorID)numericId;
                if (foundSensors.Add(sensorId))
                {
                    Console.WriteLine($"✅ Sensor erkannt: {sensorId}");
                }
            }
        }

        if (foundSensors.Count == 0)
        {
            Console.WriteLine("⚠️ Keine Sensoren erkannt.");
        }

        return foundSensors;
    }
}
