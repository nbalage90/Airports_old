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
        static LoadManager loadManager;

        static void Main(string[] args)
        {
            var v = CsvHelper.Parse<Airline>(@"Data\airlines.dat");




            loadManager = new LoadManager();

            if (!loadManager.IsOwnDataFileExsists())
            {
                loadManager.TransormData();
                loadManager.DeserializeTimeZones();
                loadManager.LoadTimeZoneNames();
                //loadManager.timeZones.Clear(); // ne foglalja a memóriát

                loadManager.FindISOCodes();

                loadManager.SerializeObjects();
            }
            else
            {
                loadManager.ReadImportedFiles();
            }

            var airportManager = new AirportManager(loadManager.Airports.Values);

            // Countries and counts
            var countrylist = airportManager.CountryList();
            foreach (var c in countrylist)
            {
                Console.WriteLine(c);
            }

            // Cities has the most aiports
            var cityList = airportManager.CitiesByAirportCount();
            foreach (var c in cityList)
            {
                Console.WriteLine(c);
            }

            // Closest airport
            double longitude = 0, latitude = 0;
            string longit = string.Empty, latit = string.Empty;

            while (!airportManager.IsCoordinateValid(longit) ||
                   !airportManager.IsCoordinateValid(latit) ||
                   !double.TryParse(longit, out longitude) || !double.TryParse(latit, out latitude))
            {
                Console.WriteLine("Please enter a longitude value:");
                longit = Console.ReadLine();

                Console.WriteLine("Please enter a latitude value:");
                latit = Console.ReadLine();
            }

            var nearest = airportManager.NearestAirport(longitude, latitude);
            Console.WriteLine($"{nearest.Name} ({nearest.City.Name} [{nearest.Country.Name}])");

            // IATA code
            var iata = string.Empty;

            while (!airportManager.IsIATACodeValid(iata))
            {
                Console.WriteLine("Please enter an IATA code:");
                iata = Console.ReadLine();
            }

            var iataAirport = airportManager.GetAirportByIATACode(iata);
            if (iataAirport == null)
            {
                Console.WriteLine("No airport found!");
            }
            else
            {
                Console.WriteLine($"{iataAirport.Name} ({iataAirport.City.Name} [{iataAirport.Country.Name}])");
            }

            Console.ReadKey();
        }

        #region ReadData

        

        #endregion
    }
}
