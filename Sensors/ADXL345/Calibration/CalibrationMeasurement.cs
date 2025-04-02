public class CalibrationMeasurement
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
}

public class CalibrationData
{
    public string? TargetId { get; set; }
    public double XMin { get; set; }
    public double XMax { get; set; }
    public double YMin { get; set; }
    public double YMax { get; set; }
    public double ZMin { get; set; }
    public double ZMax { get; set; }

    public double ZeroX { get; set; }
    public double ZeroY { get; set; }
    public double ZeroZ { get; set; }

    public Dictionary<string, CalibrationMeasurement> AllMeasurements { get; set; } = new();
}
