namespace WorkoutTracker.Models;

public class ExerciseSet
{
    public int Id { get; set; }
    public int SetNumber { get; set; }
    public decimal WeightKg { get; set; }
    public int Reps { get; set; }
    public string? Notes { get; set; }
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
}
