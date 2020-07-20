using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class EventView
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Название мероприятия")]
        [Required(ErrorMessage = "Укажите название мероприятия")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Укажите дату события")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Укажите время события")]
        [Display(Name = "Время")]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "Укажите кол-во участников")]
        [Display(Name = "Кол-во участников")]
        [Range(1, 20)]
        public int Count { get; set; }


        public UserProfile UserProfileCreate { get; set; }
        public int CountNow { get; set; }


        public bool State { get; set; }


        public ICollection<Field> Fields { get; set; }
        public ICollection<UserProfile> UserProfiles { get; set; }

        public EventView()
        {
            Fields = new List<Field>();
            UserProfiles = new List<UserProfile>();
        }
    }
}