using System.ComponentModel.DataAnnotations;

namespace EventHub.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public string Organizer { get; set; }
        public bool IsActive { get; set; }
    }
}