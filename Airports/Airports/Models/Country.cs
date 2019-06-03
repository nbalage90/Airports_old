using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Airports.Models
{
    public class Country
    {
        public int Id { get; set; }

        [Column("country")]
        public string Name { get; set; }

        public string ThreeLetterISOCode { get; set; }

        public string TwoLetterISOCode { get; set; }
    }
}
