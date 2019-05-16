using Airports.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Airports
{
    class Program
    {
        static Logger logger;
        static List<Country> countries;
        static List<City> cities;
        static List<Location> locations;

        static void Main(string[] args)
        {
            Initialize();
            TryTransormDatas();
        }

        static void Initialize()
        {
            logger = LogManager.GetCurrentClassLogger();
            countries = new List<Country>();
            cities = new List<City>();
            locations = new List<Location>();
        }

        static bool IsOwnDataFileExsists()
        {
            //TODO: IsOwnDataFileExsists
            return false;
        }

        static void TryTransormDatas()
        {
            var pattern = "^[0-9]{1,4},(\".*\",){3}(\"[A-Za-z]+\",){2}([-0-9]{1,4}(\\.[0-9]{0,})?,){2}";

            using (var stream = new FileStream(@"Datas\airports.dat", FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!Regex.Match(line, pattern).Success)
                    {
                        logger.Info($"The next row (\"{line}\") is not match with the pattern.");
                        count++;
                        continue;
                    }

                    CreateAirport(line);
                }

                logger.Info($"There was {count} elements, wich not matched with the pattern.");
            }
        }

        static void CreateAirport(string line)
        {
            var datas = line.AirportSplit(',');

            var country = CreateCountry(datas);

            var city = CreateCity(datas, country);

            var location = CreateLocation(datas);

            var airport = new Airport
            {
                Id = int.Parse(datas[0]),
                Name = datas[1],
                FullName = datas[1] + " Airport",
                CityId = city.Id,
                City = city,
                CountryId = country.Id,
                Country = country,
                Location = location,
                IATACode = datas[5],
                ICAOCode = datas[6]
            };
        }

        static Country CreateCountry(string[] datas)
        {
            var country = countries.SingleOrDefault(c => c.Name == datas[3]);
            if (country == null)
            {
                var newCountry = new Country
                {
                    Id = countries.Count > 0 ? countries.Max(c => c.Id) + 1 : 0,
                    Name = datas[3]
                };
                countries.Add(newCountry);
                country = newCountry;
            }

            return country;
        }

        static Location CreateLocation(string[] datas)
        {
            var location = locations.SingleOrDefault(l => l.Longitude.ToString() == datas[6]
                                              && l.Latitude.ToString() == datas[7]
                                              && l.Altitude.ToString() == datas[8]);
            if (location == null)
            {
                var newLocation = new Location
                {
                    Longitude = decimal.Parse(datas[6]),
                    Latitude = decimal.Parse(datas[7]),
                    Altitude = decimal.Parse(datas[8])
                };

                locations.Add(newLocation);
                location = newLocation;
            }

            return location;
        }

        static City CreateCity(string[] datas, Country country)
        {
            var city = cities.SingleOrDefault(c => c.Name == datas[2]);
            if (city == null)
            {
                var newCity = new City
                {
                    Id = cities.Count > 0 ? cities.Max(c => c.Id) + 1 : 0,
                    Name = datas[2],
                    CountryId = country.Id,
                    Country = country
                };

                cities.Add(newCity);
                city = newCity;
            }

            return city;
        }

        static void DeserializeTimeZones()
        {
            //using (resource)
            //{

            //}
            //JsonTextReader reader = new JsonTextReader();
        }

        static Stream OpenStream(string path)
        {

        }
    }
}
