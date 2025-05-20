using EventHub.Core.Validation;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace EventHub.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }

        [TitleFormat]
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [FutureDate]
        [Required]
        public DateTime StartDate { get; set; }

        public string Organizer { get; set; }
        public bool IsActive { get; set; }

        [StringLength(200)]
        public string? Slug { get; set; }
    }
}