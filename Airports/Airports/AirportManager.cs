using Airports.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Elect.Location.Coordinate.DistanceUtils;

namespace Airports
{
    public class AirportManager
    {
        IEnumerable<Airport> airports;

        public AirportManager(IEnumerable<Airport> airports)
        {
            this.airports = airports;
        }

        public IDictionary<string, int> CountryList()
        {
            return airports.OrderBy(a => a.Country.Name)
                            .GroupBy(a => a.Country.Name)
                            .Select(a => a.Key)
                            .ToDictionary(a => a, a => a.Count());
        }

        public IEnumerable<string> CitiesByAirportCount()
        {
            var dict = airports.OrderBy(a => a.City.Name)
                               .GroupBy(a => a.City.Name)
                               .Select(a => new { CityName = a.Key, Count = a.Count() });

            var max = dict.Max(a => a.Count);
            return dict.Where(d => d.Count == max).Select(d => d.CityName);
        }

        public Airport NearestAirport(double longitude, double latitude)
        {
            var airport = new Airport();
            double distance = double.MaxValue;

            foreach (var a in airports)
            {
                var dist = DistanceHelper.GetDistance(longitude, latitude, (double)a.Location.Longitude, (double)a.Location.Latitude);
                if (dist < distance)
                {
                    airport = a;
                    distance = dist;
                }
            }

            return airport;
        }
    }
}
