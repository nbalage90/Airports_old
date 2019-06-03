﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Airports.Models
{
    public class Segment
    {
        public int Id { get; set; }

        [Column("airline")]
        public int AirlineId { get; set; }

        [Column("departureAirport")]
        public int DepartureAirportId { get; set; }

        [Column("arrivalAirport")]
        public int ArrivalAirportId { get; set; }

        public Airport DepartureAirport { get; set; }

        public Airport ArrivalAirport { get; set; }
    }
}
