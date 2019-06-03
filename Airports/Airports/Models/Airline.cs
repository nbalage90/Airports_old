using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Airports.Models
{
    public class Airline
    {

        [Column("icao")]
        public string ICAOCode { get; set; }
        public int Id { get; set; }

        [Column("iata")]
        [NotEmpty]
        public string IATACode { get; set; }

        public string Name { get; set; }

        public string CallSign { get; set; }
    }
}
