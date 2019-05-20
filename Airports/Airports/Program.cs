using Airports.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Airports
{
    class Program
    {
        static Logger logger;
        static List<Country> countries;
        static List<City> cities;
        static List<Location> locations;
        static List<Airport> airports;
        static List<AirportTimeZoneInfo> timeZones;

        static void Main(string[] args)
        {
            Initialize();
            TransormDatas();
            DeserializeTimeZones();
            LoadTimeZoneNames();
            timeZones.Clear(); // na foglalja a memóriát

            foreach (var airport in airports)
            {
                FindISOCodes(airport);
            }

            SerializeObjects();

            var airportManager = new AirportManager();
            airportManager.CountryList(airports);
            airportManager.CitiesByAirportCount(airports);
        }

        static void Initialize()
        {
            logger = LogManager.GetCurrentClassLogger();
            countries = new List<Country>();
            cities = new List<City>();
            locations = new List<Location>();
            airports = new List<Airport>();
            timeZones = new List<AirportTimeZoneInfo>();
        }

        static bool IsOwnDataFileExsists()
        {
            //TODO: IsOwnDataFileExsists
            return false;
        }

        static void TransormDatas()
        {
            var pattern = "^[0-9]{1,4},(\".*\",){3}(\"[A-Za-z]+\",){2}([-0-9]{1,4}(\\.[0-9]{0,})?,){2}";

            using (var reader = OpenStreamReader(@"Datas\airports.dat"))
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
                Name = datas[1].ToShortString(),
                FullName = GenerateFullName(datas[1]),
                CityId = city.Id,
                City = city,
                CountryId = country.Id,
                Country = country,
                Location = location,
                IATACode = datas[5].ToShortString(),
                ICAOCode = datas[6].ToShortString()
            };
            airports.Add(airport);
        }

        static string GenerateFullName(string name)
        {
            int airtportWordLength = 7;
            if (name.Length < airtportWordLength)
            {
                return name + " Airport";
            }
            else if (name.Substring(name.Length - airtportWordLength).ToLower() == "airport")
            {
                return name;
            }
            else
            {
                return name + " Airport";
            }
        }

        static Country CreateCountry(string[] datas)
        {
            var country = countries.SingleOrDefault(c => c.Name == datas[3].ToShortString());
            if (country == null)
            {
                var newCountry = new Country
                {
                    Id = countries.Count > 0 ? countries.Max(c => c.Id) + 1 : 0,
                    Name = datas[3].ToShortString()
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
            var city = cities.SingleOrDefault(c => c.Name == datas[2].ToShortString());
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
            if (airports.Count > 0)
            {
                using (var sr = OpenStreamReader(@"Datas\timezoneinfo.json"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        timeZones = JsonConvert.DeserializeObject<List<AirportTimeZoneInfo>>(line);
                    }
                }
            }
        }

        static void LoadTimeZoneNames()
        {
            foreach (var zone in timeZones)
            {
                var airport = airports.SingleOrDefault(a => a.Id == zone.AirportId);
                if (airport != null)
                {
                    var city = cities.Single(c => c.Id == airport.CityId);
                    airport.TimeZoneName = zone.TimeZoneInfoId;
                    city.TimeZoneName = zone.TimeZoneInfoId; 
                }
            }
        }

        static void FindISOCodes(Airport airport)
        {
            var culture = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                                    .FirstOrDefault(c => c.EnglishName.Contains(airport.Country.Name));

            if (culture != null)
            {
                try
                {
                    var regInfo = new RegionInfo(culture.Name);
                    airport.Country.TwoLetterISOCode = regInfo.TwoLetterISORegionName;
                    airport.Country.ThreeLetterISOCode = regInfo.ThreeLetterISORegionName;
                }
                catch (Exception)
                {
                    logger.Info($"Culture ({culture.EnglishName}) is not correct.");
                }
            }
        }

        static void SerializeObjects()
        {
            WriteObjectToFile(@"Datas\Output\airports.json", airports);
            WriteObjectToFile(@"Datas\Output\cities.json", cities);
            WriteObjectToFile(@"Datas\Output\countries.json", countries);
            WriteObjectToFile(@"Datas\Output\locations.json", locations);
        }

        static StreamReader OpenStreamReader(string path)
        {
            var stream = new FileStream(path, FileMode.Open);
            return new StreamReader(stream);
        }

        static void WriteObjectToFile<T>(string path, IEnumerable<T> list) where T : class
        {
            var sb = new StringBuilder();
            sb.Append(JsonConvert.SerializeObject(list, Formatting.Indented));

            using (var stream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(sb.ToString());
            }
        }
    }
}
