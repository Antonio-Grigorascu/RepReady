using System.ComponentModel.DataAnnotations;

namespace RepReady.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string Name { get; set; }

        public virtual ICollection<Workout>? Workouts { get; set; } // afisam workouts dupa categorie
    }
}
