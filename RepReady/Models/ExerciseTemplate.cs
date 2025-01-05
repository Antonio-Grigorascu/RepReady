using System.ComponentModel.DataAnnotations.Schema;

namespace RepReady.Models
{
    public class ExerciseTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
