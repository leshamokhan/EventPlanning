using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class UserProfile
    {
        private int? age;

        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }


        [Display(Name = "Возраст")]
        [Range(10, 120)]
        public int? Age
        {
            get
            {
                return age;
            }
            set
            {
                if (10 <= value && value <= 120)
                {
                    age = value;
                }
                else
                {
                    age = null;
                }
            }
        }





        public string UserID { get; set; }
        public ApplicationUser User { get; set; }


        public ICollection<Event> Events { get; set; }
        public ICollection<Event> EventsCreates { get; set; }
        public UserProfile()
        {
            Events = new List<Event>();
            EventsCreates = new List<Event>();
        }
    }
}