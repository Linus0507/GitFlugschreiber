using System;
using System.Globalization;

public static class SensorParser
{
    public static bool TryParseXYZ(string sensorData, out double x, out double y, out double z)
    {
        x = y = z = 0;

        int indexX = sensorData.IndexOf('x');
        int indexY = sensorData.IndexOf('y');
        int indexZ = sensorData.IndexOf('z');

        if (indexX == -1 || indexY == -1 || indexZ == -1) return false;

        string rawX = sensorData.Substring(indexX + 1, indexY - (indexX + 1));
        string rawY = sensorData.Substring(indexY + 1, indexZ - (indexY + 1));
        string rawZ = sensorData.Substring(indexZ + 1);

        return double.TryParse(rawX, NumberStyles.Any, CultureInfo.InvariantCulture, out x) &&
               double.TryParse(rawY, NumberStyles.Any, CultureInfo.InvariantCulture, out y) &&
               double.TryParse(rawZ, NumberStyles.Any, CultureInfo.InvariantCulture, out z);
    }
}
