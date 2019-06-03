using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Airports.Models
{
    [Serializable()]
    public class Airport
    {
        public int CityId { get; set; }

        public int CountryId { get; set; }

        public string FullName { get; set; }

        [Column("iata")]
        public string IATACode { get; set; }

        [Column("icao")]
        public string ICAOCode { get; set; }

        [Column("id")]
        public int Id { get; set; }

        [Column("airport")]
        public string Name { get; set; }

        public string TimeZoneName { get; set; }

        public Location Location { get; set; }

        public City City { get; set; }

        public Country Country { get; set; }
    }
}
