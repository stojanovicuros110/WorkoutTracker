using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.DTOs;
using WorkoutTracker.Models;

namespace WorkoutTracker.Endpoints;

public static class WorkoutEndpoints
{
    public static void MapWorkoutEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workouts").WithTags("Workouts");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var workouts = await db.Workouts
                .Include(w => w.Exercises)
                .OrderByDescending(w => w.Date)
                .Select(w => new WorkoutSummaryResponse(w.Id, w.Name, w.Date, w.Notes, w.Exercises.Count))
                .ToListAsync();
            return Results.Ok(workouts);
        }).WithSummary("Get all workouts");

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var workout = await db.Workouts
                .Include(w => w.Exercises).ThenInclude(e => e.Sets)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (workout is null) return Results.NotFound();
            return Results.Ok(MapToDetail(workout));
        }).WithSummary("Get workout with all exercises and sets");

        group.MapPost("/", async (CreateWorkoutRequest req, AppDbContext db) =>
        {
            var workout = new Workout { Name = req.Name, Date = req.Date, Notes = req.Notes };
            db.Workouts.Add(workout);
            await db.SaveChangesAsync();
            return Results.Created($"/api/workouts/{workout.Id}", MapToDetail(workout));
        }).WithSummary("Create a workout");

        group.MapPut("/{id:int}", async (int id, UpdateWorkoutRequest req, AppDbContext db) =>
        {
            var workout = await db.Workouts.FindAsync(id);
            if (workout is null) return Results.NotFound();
            workout.Name = req.Name; workout.Date = req.Date; workout.Notes = req.Notes;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Update a workout");

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var workout = await db.Workouts.FindAsync(id);
            if (workout is null) return Results.NotFound();
            db.Workouts.Remove(workout);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Delete a workout (cascades to exercises and sets)");
    }

    private static WorkoutDetailResponse MapToDetail(Workout w) => new(
        w.Id, w.Name, w.Date, w.Notes,
        w.Exercises.OrderBy(e => e.Order).Select(e => new ExerciseResponse(
            e.Id, e.Name, e.Order, e.Notes, e.WorkoutId,
            e.Sets.OrderBy(s => s.SetNumber)
                  .Select(s => new SetResponse(s.Id, s.SetNumber, s.WeightKg, s.Reps, s.Notes, s.ExerciseId))
        ))
    );
}
