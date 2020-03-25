using Microsoft.AspNetCore.Mvc;
using System;

namespace MediaRecommender.Models.Request
{
    public class IntelligentBillboardParameters
    {
        [BindProperty(Name ="weeks")]
        public int? NumberOfWeeks { get; set; }

        [BindProperty(Name = "from")]
        public DateTime? From { get; set; }

        [BindProperty(Name = "bigrooms")]
        public int? NumberOfBigRooms { get; set; }

        [BindProperty(Name = "smallrooms")]
        public int? NumberOfSmallRooms { get; set; }

        [BindProperty(Name = "city")]
        public string City { get; set; }
    }
}
