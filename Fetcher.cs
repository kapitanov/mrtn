using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

namespace Mrtn
{
     public static class Fetcher
    {
        public static async Task<FlatList> FetchAsync(string baseUrl)
        {
            var pageUrl = await GetPageUrlAsync(new Uri(baseUrl));
            var flats = await FetchPagesAsync(pageUrl);
            return new FlatList
            {
                Source = baseUrl,
                Flats = flats
            };
        }

        private static async Task<Uri> GetPageUrlAsync(Uri baseUrl)
        {
            using (var http = new HttpClient())
            {
                var html = await http.GetStringAsync(baseUrl);
                var parser = new HtmlParser();

                using (var document = parser.Parse(html))
                {
                    var elements = document.QuerySelectorAll(".corpus-view .pagination a");
                    var element = elements.First();

                    var href = baseUrl.Scheme + "://" + baseUrl.Host + element.GetAttribute("href");
                    var url = new UriBuilder(href) { Query = "" };
                    return url.Uri;
                }
            }
        }

        private static async Task<Flat[]> FetchPagesAsync(Uri baseUrl)
        {
            var rootUrl = baseUrl.Scheme + "://" + baseUrl.Host;
            var flats = new List<Flat>();

            using (var http = new HttpClient())
            {
                var currentPos = 0;
                while (true)
                {
                    var url = baseUrl + $"?curPos={currentPos}&isNaked=1&flats_table=1";
                    var html = await http.GetStringAsync(url);
                    var parser = new HtmlParser();

                    using (var document = parser.Parse(html))
                    {
                        var n = ParseFlatsTable(rootUrl, document, flats);
                        if (n == 0)
                        {
                            break;
                        }
                        
                        currentPos += n;
                    }
                }
            }

            return flats.ToArray();

        }

        private static int ParseFlatsTable(string rootUrl, IHtmlDocument document, IList<Flat> flats)
        {
            var trs = document.QuerySelectorAll(".apt-table table tr");

            var n = 0;
            foreach (var tr in trs.Skip(1))
            {
                var tds = tr.QuerySelectorAll("> td");
                if (tds.Length <= 0)
                {
                    continue;
                }
                
                var flat = new Flat
                {
                    Building = tds[0].Text(),
                    Block = ParseInt(tds[1]),
                    Floor = ParseInt(tds[2]),
                    Number = ParseInt(tds[3]),
                    Type = ParseType(tds[4].Text()),
                    Square = ParseFloat(tds[5]),
                    HasBalcony = ParseInt(tds[6]) > 0,
                    Price = ParseDecimal(tds[7]),
                    PlanUrl = rootUrl + tds[10].QuerySelector("a").GetAttribute("href")
                };

                n++;
                flats.Add(flat);
            }

            return n;
        }

        private static int ParseInt(IElement e)
        {
            var text = e.Text();
            text = text.Replace(" ", "");
            text = text.Replace(".", ",");

            try
            {
                
                return int.Parse(text);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private static float ParseFloat(IElement e)
        {
            var text = e.Text();
            text = text.Replace(" ", "");
            text = text.Replace(".", ",");

            try
            {
                return float.Parse(text);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private static decimal ParseDecimal(IElement e)
        {
            var text = e.Text();
            text = text.Replace(" ", "");
            text = text.Replace(".", ",");

            try
            {
                return decimal.Parse(text);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private static FlatType ParseType(string t)
        {
            switch (t)
            {
                case "СТ":
                    return FlatType.Studio;
                case "1":
                    return FlatType.Flat1;
                case "2":
                    return FlatType.Flat2;
                case "2Е":
                    return FlatType.Flat2E;
                case "3":
                    return FlatType.Flat3;
                case "3Е":
                    return FlatType.Flat3E;
            }

            throw new Exception($"Unknown flat type: '{t}");
        }
    }
}