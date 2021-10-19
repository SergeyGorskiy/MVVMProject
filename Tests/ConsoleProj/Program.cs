using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleProj
{
    class Program
    {
        private const string dataUrl =
            @"https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv";

        private static async Task<Stream> GetDataStream()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(dataUrl, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsStreamAsync();
        }
        private static IEnumerable<string> GetDataLines()
        {
            using var dataStream = GetDataStream().Result;
            using var dataReader = new StreamReader(dataStream);
            while (!dataReader.EndOfStream)
            {
                var line = dataReader.ReadLine();
                if(string.IsNullOrWhiteSpace(line)) continue;
                yield return line.Replace("Korea,", "Korea -"); 
            }
        }

        private static IEnumerable<(string Country, string Province, int[] Counts)> GetData()
        {
            var lines = GetDataLines().Skip(1).Select(l => l.Split(','));
            foreach (var row in lines)
            {
                var country = row[1].Trim(' ', '"');
                var province = row[0].Trim();
                var counts = row.Skip(4).Select(int.Parse).ToArray();

                yield return (country, province, counts);
            }
        }

        private static DateTime[] GetDates() => GetDataLines().First().Split(',').Skip(4)
            .Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        static void Main(string[] args)
        {
            //var client = new HttpClient();
            //var response = client.GetAsync(dataUrl).Result;
            //var csvStr = response.Content.ReadAsStringAsync().Result;
            var russiaData = GetData()
                .First(v => v.Country.Equals("Russia", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine(string.Join("\r\n", GetDates().Zip(russiaData.Counts, (date, count) => $"{date:dd:MM} - {count}")));
            Console.ReadLine();
        }
    }
}
