using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.DTOs;
using WorkoutTracker.Models;

namespace WorkoutTracker.Endpoints;

public static class ExerciseEndpoints
{
    public static void MapExerciseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workouts/{workoutId:int}/exercises").WithTags("Exercises");

        group.MapGet("/", async (int workoutId, AppDbContext db) =>
        {
            if (!await db.Workouts.AnyAsync(w => w.Id == workoutId)) return Results.NotFound("Workout not found.");
            var exercises = await db.Exercises
                .Where(e => e.WorkoutId == workoutId).Include(e => e.Sets).OrderBy(e => e.Order)
                .Select(e => new ExerciseResponse(e.Id, e.Name, e.Order, e.Notes, e.WorkoutId,
                    e.Sets.OrderBy(s => s.SetNumber).Select(s => new SetResponse(s.Id, s.SetNumber, s.WeightKg, s.Reps, s.Notes, s.ExerciseId))))
                .ToListAsync();
            return Results.Ok(exercises);
        }).WithSummary("Get all exercises for a workout");

        group.MapGet("/{id:int}", async (int workoutId, int id, AppDbContext db) =>
        {
            var exercise = await db.Exercises.Include(e => e.Sets)
                .FirstOrDefaultAsync(e => e.Id == id && e.WorkoutId == workoutId);
            if (exercise is null) return Results.NotFound();
            return Results.Ok(new ExerciseResponse(exercise.Id, exercise.Name, exercise.Order, exercise.Notes, exercise.WorkoutId,
                exercise.Sets.OrderBy(s => s.SetNumber).Select(s => new SetResponse(s.Id, s.SetNumber, s.WeightKg, s.Reps, s.Notes, s.ExerciseId))));
        }).WithSummary("Get a single exercise with sets");

        group.MapPost("/", async (int workoutId, CreateExerciseRequest req, AppDbContext db) =>
        {
            if (!await db.Workouts.AnyAsync(w => w.Id == workoutId)) return Results.NotFound("Workout not found.");
            var exercise = new Exercise { Name = req.Name, Order = req.Order, Notes = req.Notes, WorkoutId = workoutId };
            db.Exercises.Add(exercise);
            await db.SaveChangesAsync();
            return Results.Created($"/api/workouts/{workoutId}/exercises/{exercise.Id}",
                new ExerciseResponse(exercise.Id, exercise.Name, exercise.Order, exercise.Notes, exercise.WorkoutId, []));
        }).WithSummary("Add an exercise to a workout");

        group.MapPut("/{id:int}", async (int workoutId, int id, UpdateExerciseRequest req, AppDbContext db) =>
        {
            var exercise = await db.Exercises.FirstOrDefaultAsync(e => e.Id == id && e.WorkoutId == workoutId);
            if (exercise is null) return Results.NotFound();
            exercise.Name = req.Name; exercise.Order = req.Order; exercise.Notes = req.Notes;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Update an exercise");

        group.MapDelete("/{id:int}", async (int workoutId, int id, AppDbContext db) =>
        {
            var exercise = await db.Exercises.FirstOrDefaultAsync(e => e.Id == id && e.WorkoutId == workoutId);
            if (exercise is null) return Results.NotFound();
            db.Exercises.Remove(exercise);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Delete an exercise (cascades to sets)");
    }
}
