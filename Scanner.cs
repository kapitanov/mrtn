using System.Collections.Generic;
using System.Linq;

namespace Mrtn
{
    public static class Scanner
    {
        public static ScanReport Scan(FlatList list)
        {
            return new ScanReport
            {
                Source = list.Source,
                Items = ScanImpl(list)
                    .OrderBy(_ => _.Type)
                    .ThenBy(_ => _.Square)
                    .ThenBy(_ => _.HasBalcony)
                    .ToArray()
            };
        }

        private static IEnumerable<ScanReportItem> ScanImpl(FlatList list)
        {
            var similarFlats = list.Flats.GroupBy(_ => new { _.Type, _.Square, _.HasBalcony });

            foreach (var group in similarFlats)
            {
                var minPrice = group.Min(_ => _.Price);
                var maxPrice = group.Max(_ => _.Price);
                var avgPrice = group.Average(_ => _.Price);

                yield return new ScanReportItem
                {
                    Type = group.Key.Type,
                    Square = group.Key.Square,
                    HasBalcony = group.Key.HasBalcony,
                    Count = group.Count(),
                    Prices = new PriceRange
                    {
                        Min = minPrice,
                        Avg = avgPrice,
                        Max = maxPrice
                    },
                    PricesPerSquare = new PriceRange
                    {
                        Min = minPrice / (decimal)group.Key.Square,
                        Avg = avgPrice / (decimal)group.Key.Square,
                        Max = maxPrice / (decimal)group.Key.Square
                    }
                };
            }
        }
    }
}