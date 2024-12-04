using System.ComponentModel.DataAnnotations;

namespace RepReady.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public bool WasEdited { get; set; }

        public int ExerciseId { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Exercise? Exercise { get; set; }
    }
}
