using System;
using System.Collections.Generic;
using System.Text;

namespace Airports.Models
{
    class Datas
    {
        public class Rootobject
        {
            public SwitchTable[] Root { get; set; }
        }

        public class SwitchTable
        {
            public int AirportId { get; set; }
            public string TimeZoneInfoId { get; set; }
        }

    }
}
