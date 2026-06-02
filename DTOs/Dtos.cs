namespace WorkoutTracker.DTOs;

public record CreateWorkoutRequest(string Name, DateOnly Date, string? Notes);
public record UpdateWorkoutRequest(string Name, DateOnly Date, string? Notes);
public record WorkoutSummaryResponse(int Id, string Name, DateOnly Date, string? Notes, int ExerciseCount);
public record WorkoutDetailResponse(int Id, string Name, DateOnly Date, string? Notes, IEnumerable<ExerciseResponse> Exercises);

public record CreateExerciseRequest(string Name, int Order, string? Notes);
public record UpdateExerciseRequest(string Name, int Order, string? Notes);
public record ExerciseResponse(int Id, string Name, int Order, string? Notes, int WorkoutId, IEnumerable<SetResponse> Sets);

public record CreateSetRequest(int SetNumber, decimal WeightKg, int Reps, string? Notes);
public record UpdateSetRequest(int SetNumber, decimal WeightKg, int Reps, string? Notes);
public record SetResponse(int Id, int SetNumber, decimal WeightKg, int Reps, string? Notes, int ExerciseId);
