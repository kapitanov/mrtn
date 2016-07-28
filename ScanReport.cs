using System;
using System.Linq;
using Newtonsoft.Json;

namespace Mrtn
{
     public sealed class ScanReport
    {
        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("items")]
        public ScanReportItem[] Items { get; set; }

        public void Print()
        {
            Console.WriteLine($"Got {Items.Length} different variations:");

            foreach (var r in Items.OrderBy(_=>_.Prices.Max))
            {
                Console.Write(" * ");
                r.Print();
            }
        }
    }
}