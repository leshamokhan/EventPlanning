using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class Field
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }



        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}