using Airports.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Airports
{
    public class AirportManager
    {
        public IDictionary<string, int> CountryList(IEnumerable<Airport> airports)
        {
            return airports.OrderBy(a => a.Country.Name)
                            .GroupBy(a => a.Country.Name)
                            .Select(a => a.Key)
                            .ToDictionary(a => a, a => a.Count());
        }

        public IEnumerable<string> CitiesByAirportCount(IEnumerable<Airport> airports)
        {
            var dict = airports.OrderBy(a => a.City.Name)
                               .GroupBy(a => a.City.Name)
                               .Select(a => new { CityName = a.Key, Count = a.Count() });
                               //.ToDictionary(a => a, a => a.Count());

            var max = dict.Max(a => a.Count);
            return dict.Where(d => d.Count == max).Select(d => d.CityName);
        }
    }
}
