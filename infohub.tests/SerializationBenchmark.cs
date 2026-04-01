using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace infohub.tests
{
    public class SerializationBenchmark
    {
        private readonly ITestOutputHelper _output;

        public SerializationBenchmark(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BenchmarkStreamAndJson()
        {
            int iterations = 10000;
            string locationParam = "London";
            string jsonBody = "{\"location\":\"New York\"}";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);

            // Baseline: Always read and deserialize
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                string location = locationParam;
                using (var stream = new MemoryStream(bodyBytes))
                {
                    string requestBody = new StreamReader(stream).ReadToEnd();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    if (string.IsNullOrEmpty(location))
                    {
                        location = data?.location;
                    }
                }
            }
            sw.Stop();
            long baselineTime = sw.ElapsedMilliseconds;
            _output.WriteLine($"Baseline (Always read/deserialize): {baselineTime}ms");

            // Optimized: Only read and deserialize if param is missing
            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                string location = locationParam;
                if (string.IsNullOrEmpty(location))
                {
                    using (var stream = new MemoryStream(bodyBytes))
                    {
                        string requestBody = new StreamReader(stream).ReadToEnd();
                        dynamic data = JsonConvert.DeserializeObject(requestBody);
                        location = data?.location;
                    }
                }
            }
            sw.Stop();
            long optimizedTime = sw.ElapsedMilliseconds;
            _output.WriteLine($"Optimized (Conditional read/deserialize): {optimizedTime}ms");

            _output.WriteLine($"Improvement: {baselineTime - optimizedTime}ms ({(double)(baselineTime - optimizedTime) / baselineTime * 100:F2}%)");
        }
    }
}
