using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CryptoChecker
{
    public class Price
    {
        public int Time { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double VolumeFrom { get; set; }
        public double VolumeTo { get; set; }
    }

    internal class Program
    {
        private static List<Price> prices = new List<Price>();

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static int DateToTimeStamp(DateTime date)
        {
            return (int)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Aro jan which cryptoCurrency do you want axper jan ?");
            string crypto = Console.ReadLine();

            Console.Write("Start from : ");
            string timeStart = Console.ReadLine();

            Console.Write("To : ");
            string timeEnd = Console.ReadLine();

            Console.Write("With interval in hours : ");
            int interval = int.Parse(Console.ReadLine());

            int startTimesStamp = DateToTimeStamp(DateTime.Parse(timeStart));
            int endTimesStamp = DateToTimeStamp(DateTime.Parse(timeEnd));

            while (endTimesStamp > startTimesStamp)
            {
                string uri =
                    $"https://min-api.cryptocompare.com/data/histohour?fsym={crypto}&tsym=USD&limit=2000&aggregate={interval}&toTs={endTimesStamp}";
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);
                request.AddHeader("X-CoinAPI-Key", "7A46F0F4-F003-46A5-8A72-2C4BAFF3077A");
                IRestResponse response = client.Execute(request);

                dynamic content = JsonConvert.DeserializeObject(response.Content);
                var datas = content.Data as JArray;

                foreach (var data in datas)
                {
                    var price = data.ToObject<Price>();
                    prices.Add(price);
                }

                endTimesStamp -= 2000 * 3600;
            }

            prices = prices.Where(p => p.Time > startTimesStamp)
                .Where(p => p.High > 0 && p.Close > 0)
                .OrderBy(p => p.Time).ToList();

            string json = JsonConvert.SerializeObject(prices, Formatting.Indented);
            File.WriteAllText("Prices.json", json);

            for (int i = 0; i < prices.Count - 1; ++i)
            {
                if (prices[i + 1].Time - prices[i].Time > interval * 3600)
                {
                    Console.WriteLine(prices[i + 1].Time - prices[i].Time);
                }
            }

            var csv = new StringBuilder();
            foreach (var price in prices)
            {
                var newLine = $"{UnixTimeStampToDateTime(price.Time)},{price.Close}";
                csv.Append(newLine);
                csv.Append(Environment.NewLine);
            }

            File.WriteAllText("Prices.csv", csv.ToString());
        }
    }
}