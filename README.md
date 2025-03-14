# Sensor Matching Program

## Overview
This program reads sensor data from two different sources: a CSV file (`SensorData1.csv`) and a JSON file (`SensorData2.json`). It then finds matching sensor records based on their geographical proximity and outputs the matched pairs to a JSON file (`MatchedSensorOutput.json`).

## Files
- **`Program.cs`**: The main C# program that loads sensor data, matches them based on location, and outputs results.
- **`SensorData1.csv`**: Input CSV file containing sensor data (ID, Latitude, Longitude).
- **`SensorData2.json`**: Input JSON file containing sensor data (ID, Latitude, Longitude).
- **`MatchedSensorOutput.json`**: Output JSON file containing the matched sensor IDs.

## How It Works
1. **Reads Sensor Data**:
   - The CSV file is parsed line by line, extracting sensor IDs, latitudes, and longitudes.
   - The JSON file is deserialized into objects, and invalid coordinates are filtered out.

2. **Matches Sensors**:
   - Each sensor from the CSV file is compared against sensors from the JSON file.
   - If the geographical distance between two sensors is **≤ 100 meters**, they are considered a match.
   - The first match found for each sensor is stored.

3. **Saves Matched Pairs**:
   - The program writes matched sensor ID pairs to `MatchedSensorOutput.json` in the same directory as the executable.

## How to Run
1. **Ensure the required files are in the same directory as the executable:**
   - `SensorData1.csv`
   - `SensorData2.json`
2. **Run the program:**
   ```sh
   ./ConsoleApp1
   ```
3. **Check the output:**
   - The program generates `MatchedSensorOutput.json`, containing the matched sensor pairs in JSON format.

## Code Explanation
- **File Paths Handling**: Uses `AppDomain.CurrentDomain.BaseDirectory` to ensure file operations occur in the correct directory.
- **Distance Calculation**: Implements the **Haversine formula** to calculate real-world distances between latitude/longitude points.
- **Efficiency**: Filters out invalid coordinates before processing and stops searching after the first match for each sensor.



## Requirements
- .NET 6.0 or later
- Newtonsoft.Json (`JsonConvert`) for JSON parsing

## Notes
- Ensure that `SensorData1.csv` and `SensorData2.json` exist before running the program.
- The program stops searching after the first match for each sensor in the CSV file.
- If no sensors are found within 100 meters, they are not included in the output.

---
**Author:** Diba

