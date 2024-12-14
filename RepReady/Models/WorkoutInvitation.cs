namespace RepReady.Models
{
    public class WorkoutInvitation
    {
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public string UserId { get; set; }
        public string InvitedBy { get; set; }
        public bool Accepted { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Workout Workout { get; set; }
    }
}
