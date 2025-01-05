namespace RepReady.Models
{
    public class ApplicationUserExercise
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int ExerciseId { get; set; }
        public virtual Exercise Exercise { get; set; }
        public int Status { get; set; }

    }
}
