public static class SingleSensor
{
    public static void SingleSensorProtocol(HashSet<SensorID> connectedSensors, ReadUSBPort usbReader)
    {
        Console.WriteLine("Welchem Sensor möchtest du zuhören?");
        Console.WriteLine("Folgende Sensoren wurden erkannt:");

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

            // Jetzt kommt die Sensor-spezifische Entscheidung
            switch (selectedSensor)
            {
                case SensorID.ADXL345:
                    ShowCurrentValuesADXL345(usbReader, selectedSensor);
                    break;

                default:
                    break;
            }
        }
        else
        {
            Console.WriteLine("❌ Ungültige Eingabe.");
        }
    }


    static void ShowCurrentValuesADXL345(ReadUSBPort usbReader, SensorID sensor)
    {
        Console.WriteLine("Beenden durch 'q'");

        var reader = SensorUtils.CreateSensorReader(usbReader, ((int)sensor).ToString());
        var currentValues = new CurrentValues();
        currentValues.show(reader);
    }
}