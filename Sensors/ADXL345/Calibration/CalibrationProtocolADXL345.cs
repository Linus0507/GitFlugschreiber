
public static class calibrationADXL345
{
    public static void calibrationProtocolADXL345(SensorID selectedSensor, ReadUSBPort usbReader)
    {
        Console.WriteLine($"Kalibrierung für {selectedSensor} wird gestartet...");

        // SensorID z. B. 1 für ADXL345, in String umwandeln
        string targetId = ((int)selectedSensor).ToString();

        // Reader-Funktion für diesen Sensor erzeugen
        Func<string?> sensorReader = SensorUtils.CreateSensorReader(usbReader, targetId);

        // Kalibrierklasse aufrufen
        var calibration = new MinMaxCalibrationADXL345();
        calibration.StartFullCalibration(sensorReader, targetId);
    }
}

