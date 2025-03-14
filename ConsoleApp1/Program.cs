using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

// Class to represent sensor data, including an ID and geographic coordinates
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
        // Define file paths for CSV and JSON data sources using the directory of the executable
        Console.WriteLine(Directory.GetCurrentDirectory());
        string csvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SensorData1.csv");
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SensorData2.json");
        
        //output file to store the matched data
        string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MatchedSensorOutput.json");
        
        //load the data from files into lists
        List<SensorData> sensor1Data = LoadCsv(csvFilePath);
        List<SensorData> sensor2Data = LoadJson(jsonFilePath);
        
        // Match sensors from both lists based on geographic proximity
        Dictionary<int, int> matches = MatchSensors(sensor1Data, sensor2Data);
        
        // Save the matched sensor pairs as a JSON file
        File.WriteAllText(outputFilePath, JsonConvert.SerializeObject(matches, Formatting.Indented));
        Console.WriteLine("Matching complete. Results saved to " + outputFilePath);
    }

    // Method to load sensor data from a CSV file

    static List<SensorData> LoadCsv(string filePath)
    {
        List<SensorData> data = new List<SensorData>();
        // Read the CSV file line by line, skipping the first line (header)

        foreach (var line in File.ReadLines(filePath).Skip(1)) 
        {
            var values = line.Split(',');
            double lat, lon;
            // Ensure there are at least 3 values (ID, Latitude, Longitude) and they are valid numbers

            if (values.Length >= 3 && double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out lat) &&
                                      double.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out lon) &&
                                      IsValidCoordinate(lat, lon))
            {
                // Add a new SensorData object to the list

                data.Add(new SensorData { Id = int.Parse(values[0]), Latitude = lat, Longitude = lon });
            }
        }
        return data;
    }

    // Method to load sensor data from a JSON file

    static List<SensorData> LoadJson(string filePath)
    {
        // Read the entire JSON file into a string
        var jsonData = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<List<SensorData>>(jsonData);
        return data.Where(d => IsValidCoordinate(d.Latitude, d.Longitude)).ToList();
    }

    // Method to check if the given latitude and longitude are within valid ranges

    static bool IsValidCoordinate(double latitude, double longitude)
    {
        return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
    }

    // Method to match sensors based on their geographic proximity

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
                    break; 
                }
            }
        }
        return matches;
    }

    // Method to calculate the distance (in meters) between two geographic points using the Haversine formula

    static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; 
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        // Haversine formula to calculate the great-circle distance

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    // Helper method to convert degrees to radians

    static double ToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}
