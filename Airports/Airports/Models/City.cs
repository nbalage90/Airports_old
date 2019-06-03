using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Airports.Models
{
    public class City
    {
        public int CountryId { get; set; }

        public int Id { get; set; }

        [Column("city")]
        public string Name { get; set; }

        public string TimeZoneName { get; set; }

        public Country Country { get; set; }
    }
}
