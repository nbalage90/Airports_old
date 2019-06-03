using System;
using System.Collections.Generic;
using System.Text;

namespace Airports.Models
{
    public class Flight
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public int SegmentId { get; set; }

        public Segment Segment { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public TimeSpan ArrivalTime { get; set; }
    }
}
