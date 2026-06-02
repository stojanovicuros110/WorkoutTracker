namespace WorkoutTracker.Models;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? Notes { get; set; }
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
    public ICollection<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
}
