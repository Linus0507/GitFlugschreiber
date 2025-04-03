
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Diese Klasse wertet ADXL345 Sensordaten aus und berechnet G-Werte, Winkel und Orientierung
public class Adxl345Interpreter : ISensorInterpreter
{
    private CalibrationData? calibration;          // Kalibrierdaten (Min, Max, Zero) aus der Kalibrierung
    private AngleOffsets angleOffsets;            // Winkel-Offsets für "korrigierte Winkel"
    private readonly CultureInfo culture = CultureInfo.InvariantCulture; // Für saubere Float-Parsing-Operationen

    public Adxl345Interpreter(CalibrationData? calibrationData)
    {
        calibration = calibrationData;
        angleOffsets = new AngleOffsets(); // initiale Offsets = 0
    }

    // Wandelt Rohdaten (rawX, rawY, rawZ) in G-Werte um, basierend auf Kalibrierdaten
    // Die Kalibrierung erfolgt linear mit Bezug auf Min/Max/Zero-Werte
    public GVector GetGValues(double rawX, double rawY, double rawZ)
    {
        if (calibration == null)
            throw new InvalidOperationException("Kalibrierdaten fehlen. Ohne die geht's nicht weiter.");

        return new GVector
        {
            X = CalibrateAxis(rawX, calibration.XMin, calibration.XMax, calibration.ZeroX),
            Y = CalibrateAxis(rawY, calibration.YMin, calibration.YMax, calibration.ZeroY),
            Z = CalibrateAxis(rawZ, calibration.ZMin, calibration.ZMax, calibration.ZeroZ)
        };

    }

    // Berechnet einfache Winkel (in Grad) aus G-Werten mittels arcsin
    // Achtung: Für größere Neigungen (>60°) oder Z-Komponente ist das ungenau!
    public AngleVector GetAngles(GVector g)
    {
        return new AngleVector
        {
            X = Math.Asin(Clamp(g.X)) * 180 / Math.PI,
            Y = Math.Asin(Clamp(g.Y)) * 180 / Math.PI,
            Z = Math.Asin(Clamp(g.Z)) * 180 / Math.PI
        };
    }

    // Alternative Methode zur Winkelberechnung mit atan2, besser für größere Winkel und reale Neigungen
    // Wird in der Luftfahrt und Robotik bevorzugt verwendet
    public AngleVector GetTiltAngles(GVector g)
    {
        return new AngleVector
        {
            // Neigung nach vorne/hinten (Pitch)
            X = Math.Atan2(g.X, Math.Sqrt(g.Y * g.Y + g.Z * g.Z)) * 180 / Math.PI,

            // Neigung nach links/rechts (Roll)
            Y = Math.Atan2(g.Y, Math.Sqrt(g.X * g.X + g.Z * g.Z)) * 180 / Math.PI,

            // Kein Z-Winkel mehr – auf Wiedersehen, du illusorische Komponente
            Z = 0  // oder: lasse Z einfach komplett weg, wenn du `AngleVector` entsprechend anpasst
        };
    }


    // Zieht vorher gespeicherte Offset-Winkel ab, um "0°" im Ruhezustand zu erhalten
    public AngleVector GetCorrectedAngles(AngleVector angle)
    {
        return new AngleVector
        {
            X = angle.X - angleOffsets.X,
            Y = angle.Y - angleOffsets.Y,
            Z = angle.Z - angleOffsets.Z
        };
    }

    // Diese Methode speichert aktuelle Winkel als Referenz-Offsets für spätere Korrekturen
    public void MeasureAngleOffsets(AngleVector angle)
    {
        angleOffsets = new AngleOffsets
        {
            X = angle.X,
            Y = angle.Y,
            Z = angle.Z
        };
    }


    // Gibt die Lage des Sensors als String zurück basierend auf dominanter G-Achse
    // Diese Funktion schätzt nur grob die Hauptausrichtung des Moduls (z. B. "Flat", "Upside Down", etc.)
    public string GetOrientation(GVector g)
    {
        if (g.Z > 0.9) return "Flat";
        if (g.Z < -0.9) return "Upside Down";
        if (g.X > 0.9) return "Nose Up";
        if (g.X < -0.9) return "Nose Down";
        if (g.Y > 0.9) return "Curving Right";
        if (g.Y < -0.9) return "Curving Left";
        return "Unknown";
    }

    // Führt die Kalibrierung einer Achse durch: Skaliert Rohwert basierend auf Min/Max/Zero
    // Ergebnis: Normalisierte G-Werte zwischen ca. -1 und +1
    private double CalibrateAxis(double raw, double min, double max, double zero)
    {
        return (raw - zero) / ((max - min) / 2.0);
    }

    // Stellt sicher, dass Asin nur Werte zwischen -1 und +1 erhält
    private double Clamp(double value)
    {
        return Math.Max(-1.0, Math.Min(1.0, value));
    }

    //Aufruf
    public void StartLiveInterpretation(ReadUSBPort usbReader, SensorID sensor, CancellationToken token)
    {
        string targetId = ((int)sensor).ToString();
        var reader = SensorUtils.CreateSensorReader(usbReader, targetId);

        Console.WriteLine($"📡 Starte Interpretation der Sensordaten ({sensor})");

        while (!token.IsCancellationRequested)
        {
            string? sensorData = reader();
            if (string.IsNullOrEmpty(sensorData)) continue;

            if (!Adxl345Parser.TryParse(sensorData, out double x, out double y, out double z)) continue;

            var g = GetGValues(x, y, z);
            var tilt = GetTiltAngles(g);
            var orientation = GetOrientation(g);

            Console.WriteLine($"[{sensor}] G: X={g.X:F2}, Y={g.Y:F2}, Z={g.Z:F2} | Pitch={tilt.X:F1}°, Roll={tilt.Y:F1}° | Orientierung: {orientation}");

            Thread.Sleep(100);
        }

        Console.WriteLine($"🛑 Interpretation von {sensor} beendet.");
    }




}




// Struktur für berechnete G-Werte (normalisierte Beschleunigung entlang X, Y, Z)
public struct GVector { public double X, Y, Z; }

// Struktur für berechnete Winkel (in Grad)
public struct AngleVector { public double X, Y, Z; }

// Struktur zur Speicherung von Winkel-Offsets, um Startneigung zu kompensieren
public struct AngleOffsets { public double X, Y, Z; }

