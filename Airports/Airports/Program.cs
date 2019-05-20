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
        const string InputFolderPath = @"Data\";
        const string OutputFolderPath = @"Data\Output\";

        static Logger logger;
        static IDictionary<string, Country> countries;
        static IDictionary<string, City> cities;
        static IList<Location> locations;
        static IDictionary<int, Airport> airports;
        static IDictionary<string, AirportTimeZoneInfo> timeZones;
        static string[] FileNames =
        {
            "airports.json",
            "cities.json",
            "countries.json",
            "locations.json"
        };

        static void Main(string[] args)
        {
            Initialize();
            if (!IsOwnDataFileExsists())
            {
                TransormData();
                DeserializeTimeZones();
                LoadTimeZoneNames();
                timeZones.Clear(); // na foglalja a memóriát

                foreach (var airport in airports)
                {
                    FindISOCodes(airport.Value);
                }

                SerializeObjects();
            }
            else
            {
                ReadImportedFiles();
            }

            var airportManager = new AirportManager();
            var countrylist = airportManager.CountryList(airports.Values);
            foreach (var c in countrylist)
            {
                Console.WriteLine(c);
            }
            var cityList = airportManager.CitiesByAirportCount(airports.Values);
            foreach (var c in cityList)
            {
                Console.WriteLine(c);
            }

            Console.ReadKey();
        }

        static void Initialize()
        {
            logger = LogManager.GetCurrentClassLogger();
            countries = new Dictionary<string, Country>();
            cities = new Dictionary<string, City>();
            locations = new List<Location>();
            airports = new Dictionary<int, Airport>();
            timeZones = new Dictionary<string, AirportTimeZoneInfo>();
        }

        static bool IsOwnDataFileExsists()
        {
            if (!Directory.Exists(OutputFolderPath))
            {
                return false;
            }

            bool filesExsist = true;

            foreach (var path in FileNames)
            {
                if (!File.Exists(OutputFolderPath + path))
                {
                    filesExsist = false;
                }
            }

            return filesExsist;
        }

        #region ReadData

        static void TransormData()
        {
            var pattern = "^[0-9]{1,4},(\".*\",){3}(\"[A-Za-z]+\",){2}([-0-9]{1,4}(\\.[0-9]{0,})?,){2}";

            using (var reader = OpenStreamReader(InputFolderPath + @"airports.dat"))
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
            var data = line.AirportSplit(',');

            var country = CreateCountry(data);

            var city = CreateCity(data, country);

            var location = CreateLocation(data);

            var airport = new Airport
            {
                Id = int.Parse(data[0]),
                Name = data[1].Trim('"'),
                FullName = GenerateFullName(data[1].Trim('"')),
                CityId = city.Id,
                City = city,
                CountryId = country.Id,
                Country = country,
                Location = location,
                IATACode = data[5].Trim('"'),
                ICAOCode = data[6].Trim('"')
            };
            airports.Add(airport.Id, airport);
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

        static Country CreateCountry(string[] data)
        {
            var country = countries.SingleOrDefault(c => c.Key == data[3].Trim('"')).Value;
            if (country == null)
            {
                var newCountry = new Country
                {
                    Id = countries.Count > 0 ? countries.Values.Max(c => c.Id) + 1 : 1,
                    Name = data[3].Trim('"')
                };
                countries.Add(newCountry.Name, newCountry);
                country = newCountry;
            }

            return country;
        }

        static Location CreateLocation(string[] data)
        {
            var location = locations.SingleOrDefault(l => l.Longitude.ToString() == data[6]
                                              && l.Latitude.ToString() == data[7]
                                              && l.Altitude.ToString() == data[8]);
            if (location == null)
            {
                var newLocation = new Location
                {
                    Longitude = decimal.Parse(data[6], CultureInfo.InvariantCulture),
                    Latitude = decimal.Parse(data[7], CultureInfo.InvariantCulture),
                    Altitude = decimal.Parse(data[8], CultureInfo.InvariantCulture)
                };

                locations.Add(newLocation);
                location = newLocation;
            }

            return location;
        }

        static City CreateCity(string[] data, Country country)
        {
            var city = cities.SingleOrDefault(c => c.Key == data[2].Trim('"') + "_" + country.Name).Value;
            if (city == null)
            {
                var newCity = new City
                {
                    Id = cities.Count > 0 ? cities.Values.Max(c => c.Id) + 1 : 1,
                    Name = data[2].Trim('"'),
                    CountryId = country.Id,
                    Country = country
                };

                cities.Add(newCity.Name.Trim('"') + "_" + country.Name.Trim('"'), newCity);
                city = newCity;
            }

            return city;
        }

        static void DeserializeTimeZones()
        {
            timeZones = JsonConvert.DeserializeObject<List<AirportTimeZoneInfo>>(File.ReadAllText(InputFolderPath + @"timezoneinfo.json"))
                        .ToDictionary(t => t.AirportId.ToString(), t => t);
        }

        static void LoadTimeZoneNames()
        {
            foreach (var zone in timeZones.Values)
            {
                var airport = airports.SingleOrDefault(a => a.Key == zone.AirportId).Value;
                if (airport != null)
                {
                    var city = cities.Single(c => c.Value.Id == airport.CityId).Value;
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
            WriteObjectToFile(OutputFolderPath + @"airports.json", airports.Values);
            WriteObjectToFile(OutputFolderPath + @"cities.json", cities.Values);
            WriteObjectToFile(OutputFolderPath + @"countries.json", countries.Values);
            WriteObjectToFile(OutputFolderPath + @"locations.json", locations);
        }

        static void ReadImportedFiles()
        {
            airports = JsonConvert.DeserializeObject<List<Airport>>(File.ReadAllText(OutputFolderPath + "airports.json")).ToDictionary(a => a.Id, a => a);
            cities = JsonConvert.DeserializeObject<List<City>>(File.ReadAllText(OutputFolderPath + "cities.json")).ToDictionary(a => a.Name + "_" + a.Country.Name, a => a);
            countries = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText(OutputFolderPath + "countries.json")).ToDictionary(a => a.Name, a => a);
            locations = JsonConvert.DeserializeObject<List<Location>>(File.ReadAllText(OutputFolderPath + "locations.json"));
        }

        static StreamReader OpenStreamReader(string path)
        {
            var stream = new FileStream(path, FileMode.Open);
            return new StreamReader(stream);
        }

        static void WriteObjectToFile<T>(string path, IEnumerable<T> list) where T : class
        {
            var folderPath = path.Substring(0, path.LastIndexOf('\\'));
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var sb = new StringBuilder();
            sb.Append(JsonConvert.SerializeObject(list, Formatting.Indented));

            using (var stream = new FileStream(path, FileMode.Create))
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(sb.ToString());
            }
        }

        #endregion
    }
}
