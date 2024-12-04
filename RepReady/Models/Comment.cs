using System.ComponentModel.DataAnnotations;

namespace RepReady.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
        [StringLength(100, ErrorMessage = "Continutul nu poate avea mai mult de 100 de caractere")]
        [MinLength(3, ErrorMessage = "Continutul trebuie sa aiba mai mult de 3 caractere")]
        public string Content { get; set; }

        public DateTime Date { get; set; } // SE PUNE AUTOMAT

        public bool WasEdited { get; set; } // SE PUNE AUTOMAT - by default false

        public int ExerciseId { get; set; }

        public string UserId { get; set; } // SE PUNE AUTOMAT

        public virtual ApplicationUser? User { get; set; }

        public virtual Exercise? Exercise { get; set; }
    }
}
