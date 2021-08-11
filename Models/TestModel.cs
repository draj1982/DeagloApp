using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeagloApp.Models
{
    public class TestModel
    {
        public int years { get; set; }

        [JsonProperty("base")]
        public string Base {get; set; }
        public string term { get; set; }
        public string quadrant { get; set; }


    }
}
