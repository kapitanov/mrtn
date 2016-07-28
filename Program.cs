using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mrtn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintUsage();
                return;
            }

            if (args[0] == "fetch")
            {
                await Fetch(args);
                return;
            }

            if (args[0] == "fetch-zel")
            {
                if (!Directory.Exists("data"))
                {
                    Directory.CreateDirectory("data");
                }

                foreach (var arg in args.Skip(1))
                {
                    var n = int.Parse(args[1]);
                    await Fetch(new[]
                    {
                        "fetch",
                        $"http://www.morton.ru/novostroyki/zhemchuzhina-zelenograda/kvartiry-nalichie/korpus-{n}.html",
                        $"data/k{n}.json"
                    });
                }
                return;
            }

            if (args[0] == "scan")
            {
                Scan(args);
                return;
            }

            if (args[0] == "scan-zel")
            {
                if (!Directory.Exists("data"))
                {
                    Directory.CreateDirectory("data");
                }

                foreach (var arg in args.Skip(1))
                {
                    var n = int.Parse(args[1]);
                    Scan(new[]{
                        "scan",
                        $"data/k{n}.json"
                    });
                }
                return;
            }


            PrintUsage();
        }

        private static async Task Fetch(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return;
            }

            var url = args[1];
            var ds = await Fetcher.FetchAsync(url);
            var json = JsonConvert.SerializeObject(ds, Formatting.Indented);
            File.WriteAllText(args[2], json, Encoding.UTF8);

            var countPerType = ds.Flats.GroupBy(_ => _.Type).Select(_ => new { Type = _.Key, N = _.Count() });

            Console.Write($"Got {ds.Flats.Length} flats: ");

            foreach (var x in countPerType)
            {
                Console.Write($"{x.N} {x.Type}; ");
            }
            Console.WriteLine();
            Console.WriteLine($"See <{args[2]}>");
        }

        private static void Scan(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return;
            }

            Console.WriteLine($"Scanning <{args[1]}>:");

            var json = File.ReadAllText(args[1]);
            var ds = JsonConvert.DeserializeObject<FlatList>(json);
            var report = Scanner.Scan(ds);

            if (args.Length > 2)
            {
                File.WriteAllText(
                    args[2],
                    JsonConvert.SerializeObject(report, Formatting.Indented),
                    Encoding.UTF8);
            }

            var countPerType = ds.Flats.GroupBy(_ => _.Type).Select(_ => new { Type = _.Key, N = _.Count() });

            Console.Write($"Scanned {ds.Flats.Length} flats: ");

            foreach (var x in countPerType)
            {
                Console.Write($"{x.N} {x.Type}; ");
            }
            Console.WriteLine();
            Console.WriteLine();

            report.Print();
        }

        private static void PrintUsage()
        {
            Console.WriteLine("usage:");
            Console.WriteLine("mrtn fetch URL FILE");
            Console.WriteLine("mrtn fetch-zel N1 [N2 N3 ...]");
            Console.WriteLine("mrtn scan FILE [OUTPUT]");
            Console.WriteLine("mrtn scan-zel N1 [N2 N3 ...]");
        }
    }
}
