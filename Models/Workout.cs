namespace WorkoutTracker.Models;

public class Workout
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
}
