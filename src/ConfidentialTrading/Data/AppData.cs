namespace ConfidentialTrading.Data;

// This is old code from the original GreenHouse Monitor demo.
// In the future, we should replace this section with a trading algorithm.

// When the app first starts up, we want it to be prepopulated with some plausible data.
// Most of the code in this file is responsible for generating that data. Note that the SensorSimulator
// CLI tool also references this code and uses it when generating further simulated data to send via gRPC-Web.

public class AppData
{
    // There could be more than one building, and the data could all be persisted via a DbContext
    // (e.g., in Sqlite) but neither is necessary for this demo
    public Building ConfidentialTrading { get; } = CreateSampleData();

    private static readonly DateTime UtcNow = DateTime.UtcNow;

    static Building CreateSampleData()
    {
        var result = new Building { Name = "Neonatal Intensive Care Unit" };
        result.AddSensor(CreateSampleSensorHumidity("humidity"));
        result.AddSensor(CreateSampleSensorTemperature("temp-point-0"));
        result.AddSensor(CreateSampleSensorTemperature("temp-point-1"));
        result.AddSensor(CreateSampleSensorTemperature("temp-point-2"));
        result.AddSensor(CreateSampleSensorTemperature("temp-point-3"));
        result.AddSensor(CreateSampleSensorMoisture("moisture-plant-0", "Incubator 1", "Alice"));
        result.AddSensor(CreateSampleSensorMoisture("moisture-plant-1", "Incubator 2", "Bob"));
        result.AddSensor(CreateSampleSensorMoisture("moisture-plant-2", "Incubator 3", "Carol"));
        result.AddSensor(CreateSampleSensorMoisture("moisture-plant-3", "Incubator 4", "David"));
        result.AddSensor(CreateSampleSensorMoisture("moisture-plant-4", "Incubator 5", "Eve"));
        return result;
    }

    private static Sensor CreateSampleSensorMoisture(string name, string displayCategory, string displayName)
    {
        var result = new Sensor(name, displayCategory, displayName);
        var now = UtcNow;
        var interval = TimeSpan.FromMinutes(5);
        var sensorIndex = name.GetHashCode();

        for (var timestamp = now.AddHours(-6); timestamp < now; timestamp += interval)
        {
            result.Readings.Enqueue(new SensorReading(timestamp, GetSimulatedMoistureReading(sensorIndex, timestamp)));
        }

        return result;
    }

    static Sensor CreateSampleSensorHumidity(string name)
    {
        var result = new Sensor(name);
        var now = UtcNow;
        var interval = TimeSpan.FromMinutes(5);

        for (var timestamp = now.AddHours(-6); timestamp < now; timestamp += interval)
        {
            result.Readings.Enqueue(new SensorReading(timestamp, GetSimulatedHumidityReading(timestamp)));
        }

        return result;
    }

    static Sensor CreateSampleSensorTemperature(string name)
    {
        var result = new Sensor(name);
        var now = UtcNow;
        var interval = TimeSpan.FromMinutes(5);
        var sensorIndex = name.GetHashCode();

        for (var timestamp = now.AddHours(-6); timestamp < now; timestamp += interval)
        {
            result.Readings.Enqueue(new SensorReading(timestamp, GetSimulatedTemperatureReading(sensorIndex, timestamp)));
        }

        return result;
    }

    public static double GetSimulatedTemperatureReading(int sensorIndex, DateTime timestamp)
    {
        const int WAVELENGTH_HOURS = 6;
        const double RESULT_MIN = 27;
        const double RESULT_MAX = 30;
        var secondsToday = timestamp.Ticks / 10_000_000d; // Convert to seconds
        var x = (secondsToday + 15*60*(sensorIndex % 5)) / (WAVELENGTH_HOURS*60*60); // Convert to number of wavelengths
        var y = Math.Sin(x * 2 * Math.PI);

        // Add some noise
        y += Math.Sin(secondsToday / (15 * 60 + (sensorIndex % 5))) / 3
            + Math.Sin(secondsToday / (40 * 60 + (sensorIndex % 8))) / 9;

        return RESULT_MIN + (RESULT_MAX - RESULT_MIN) * ((y + 1) / 2);
    }

    public static double GetSimulatedHumidityReading(DateTime timestamp)
    {
        const int WAVELENGTH_HOURS = 12;
        const double RESULT_MIN = 60;
        const double RESULT_MAX = 70;
        var secondsToday = timestamp.Ticks / 10_000_000d; // Convert to seconds
        var x = (secondsToday) / (WAVELENGTH_HOURS * 60 * 60); // Convert to number of wavelengths
        var y = Math.Sin(x * 2 * Math.PI);
        // Add some noise
        y += Math.Sin(secondsToday / (15 * 60)) / 3;
        return RESULT_MIN + (RESULT_MAX - RESULT_MIN) * ((y + 1) / 2);
    }

    public static double GetSimulatedMoistureReading(int sensorIndex, DateTime timestamp)
    {
        const double WAVELENGTH_HOURS = 2.5;
        const double RESULT_MIN = 27;
        const double RESULT_MAX = 30;
        var secondsToday = timestamp.Ticks / 10_000_000d; // Convert to seconds
        var x = 2 * Math.PI * (secondsToday + 160 * (sensorIndex % 60)) / (WAVELENGTH_HOURS * 60 * 60); // Convert to number of rotations
        var y = 2d;
        for (var i = 1; i <= 6; i++)
            y += Math.Sin(i * x) / i;
        // Add some noise
        y += Math.Sin(secondsToday / (15 * 60 + (sensorIndex % 5))) / 3
            + Math.Sin(secondsToday / (40 * 60 + (sensorIndex % 8))) / 9;
        return RESULT_MIN + (RESULT_MAX - RESULT_MIN) * y / 4;
    }
}
