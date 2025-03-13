using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

class SensorData
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

class Program
{
    static void Main()
    {
        string csvFilePath = "SensorData1.csv";
        string jsonFilePath = "SensorData2.json"; 
        string outputFilePath = "MatchedSensorOutput.json";
        
        List<SensorData> sensor1Data = LoadCsv(csvFilePath);
        List<SensorData> sensor2Data = LoadJson(jsonFilePath);
        
        Dictionary<int, int> matches = MatchSensors(sensor1Data, sensor2Data);
        
        File.WriteAllText(outputFilePath, JsonConvert.SerializeObject(matches, Formatting.Indented));
        Console.WriteLine("Matching complete. Results saved to " + outputFilePath);
    }

    static List<SensorData> LoadCsv(string filePath)
    {
        List<SensorData> data = new List<SensorData>();
        foreach (var line in File.ReadLines(filePath).Skip(1)) // Skip header
        {
            var values = line.Split(',');
            double lat, lon;
            if (values.Length >= 3 && double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lat) &&
                                      double.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out lon) &&
                                      IsValidCoordinate(lat, lon))
            {
                data.Add(new SensorData { Id = int.Parse(values[0]), Latitude = lat, Longitude = lon });
            }
        }
        return data;
    }

    static List<SensorData> LoadJson(string filePath)
    {
        var jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<List<SensorData>>(jsonData);
        return data.Where(d => IsValidCoordinate(d.Latitude, d.Longitude)).ToList();
    }

    static bool IsValidCoordinate(double latitude, double longitude)
    {
        return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
    }

    static Dictionary<int, int> MatchSensors(List<SensorData> sensor1, List<SensorData> sensor2)
    {
        Dictionary<int, int> matches = new Dictionary<int, int>();
        foreach (var s1 in sensor1)
        {
            foreach (var s2 in sensor2)
            {
                if (GetDistance(s1.Latitude, s1.Longitude, s2.Latitude, s2.Longitude) <= 100)
                {
                    matches[s1.Id] = s2.Id;
                    break; // Stop after first match
                }
            }
        }
        return matches;
    }

    static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // Earth's radius in meters
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}
