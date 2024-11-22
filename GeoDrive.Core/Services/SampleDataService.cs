using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GeoDrive.Core.Models;

namespace GeoDrive.Core.Services
{
    public class SampleDataService
    {
        public void GenerateSampleData(string localStorageDirectory)
        {
            List<SampleCoordinateData> samples = new List<SampleCoordinateData>();
            Random random = new Random();
            var latMin = 48.3000;
            var latMax = 60.0000;

            var longMin = -114.0000;
            var longMax = -139.0000;

            for (int i = 0; i < 10; i++)
            {
                samples.Add(new SampleCoordinateData()
                {
                    Latitude = latMin + (random.NextDouble() * (latMax - latMin)),
                    Longitude = longMin + (random.NextDouble() * (longMax - longMin)),
                    Data = (float)random.NextDouble()
                });
            }

            if (!Directory.Exists(localStorageDirectory))
            {
                Directory.CreateDirectory(localStorageDirectory);
            }

            foreach (var sample in samples)
            {
                string jsonString = JsonSerializer.Serialize(sample, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path.Combine(localStorageDirectory, $"sample-data-{Guid.NewGuid().ToString()}.json"), jsonString);
            }
        }
    }
}
