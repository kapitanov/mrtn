using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Mrtn
{
    public sealed class ScanReportItem
    {
        [JsonProperty("type")]
        public FlatType Type { get; set; }

        [JsonProperty("square")]
        public float Square { get; set; }

        [JsonProperty("balcony")]
        public bool HasBalcony { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("prices")]
        public PriceRange Prices { get; set; } = new PriceRange();

        [JsonProperty("pricesPerSquare")]
        public PriceRange PricesPerSquare { get; set; } = new PriceRange();

        public void Print()
        {
            switch (Type)
            {
                case FlatType.Studio:
                    Console.Write("CT");
                    break;
                case FlatType.Flat1:
                    Console.Write("1 ");
                    break;
                case FlatType.Flat2:
                    Console.Write("2 ");
                    break;
                case FlatType.Flat2E:
                    Console.Write("2E");
                    break;
                case FlatType.Flat3:
                    Console.Write("3 ");
                    break;
                case FlatType.Flat3E:
                    Console.Write("3E");
                    break;
                default:
                    Console.Write(Type);
                    break;
            }

            Console.WriteLine($" {Square,5}m2 {(HasBalcony ? "+" : "-")}balcony:\t{PrintPrice(Prices.Min)}\t{PrintPrice(Prices.Max)}\t{Count} flats");
        }

        private static string PrintPrice(decimal price)
        {
            return price.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}