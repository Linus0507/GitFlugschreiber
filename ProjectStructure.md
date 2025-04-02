FlugschreiberNew031925/
├── Program.cs                          # Einstiegspunkt
├── FlugschreiberNew031925.csproj      # Projektdatei
├── FlugschreiberNew031925.sln         # Solution-Datei
│
├── Calibration/                       # Alles rund um Kalibrierung
│   ├── calibration.json               # Gespeicherte Kalibrierdaten
│   ├── CalibrationProtocol.cs         # Sensor-Auswahl und Protokollstart
│   │
│   ├── ADXL345/
│   │   ├── CalibrationProtocol.cs     # (vermutlich alt?) oder spezifisch für ADXL345
│   │   ├── calibrationStorage.cs      # (ggf. besser global?)
│   │   └── MinMaxCalibrationADXL345.cs # Logik zur Kalibrierung von ADXL345
│   │
│   └── BMP180/
│       └── (leer)                     # Hier kannst du die Kalibrierung für BMP180 bauen
│
├── Sensors/                           # Sensor-spezifische Logik
│   ├── util.cs                        # Hilfsmethoden?
│   │
│   └── ADXL345/
│       ├── ADXL345.cs                 # Klassenmodell oder Zugriff?
│       └── CurrentValuesADXL345.cs    # Aktuelle Sensorwerte?
│
├── sensorutils.cs                     # Utilities für Sensorhandling?
│
├── bin/                               # Build-Ausgaben
│   └── ...
│
├── obj/                               # Build-Zwischenspeicher
│   └── ...
