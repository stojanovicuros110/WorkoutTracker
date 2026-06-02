using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.DTOs;
using WorkoutTracker.Models;

namespace WorkoutTracker.Endpoints;

public static class SetEndpoints
{
    public static void MapSetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/workouts/{workoutId:int}/exercises/{exerciseId:int}/sets").WithTags("Sets");

        group.MapGet("/", async (int workoutId, int exerciseId, AppDbContext db) =>
        {
            if (!await db.Exercises.AnyAsync(e => e.Id == exerciseId && e.WorkoutId == workoutId))
                return Results.NotFound("Exercise not found.");
            var sets = await db.Sets.Where(s => s.ExerciseId == exerciseId).OrderBy(s => s.SetNumber)
                .Select(s => new SetResponse(s.Id, s.SetNumber, s.WeightKg, s.Reps, s.Notes, s.ExerciseId))
                .ToListAsync();
            return Results.Ok(sets);
        }).WithSummary("Get all sets for an exercise");

        group.MapGet("/{id:int}", async (int workoutId, int exerciseId, int id, AppDbContext db) =>
        {
            var set = await db.Sets.FirstOrDefaultAsync(s => s.Id == id && s.ExerciseId == exerciseId);
            if (set is null) return Results.NotFound();
            return Results.Ok(new SetResponse(set.Id, set.SetNumber, set.WeightKg, set.Reps, set.Notes, set.ExerciseId));
        }).WithSummary("Get a single set");

        group.MapPost("/", async (int workoutId, int exerciseId, CreateSetRequest req, AppDbContext db) =>
        {
            if (!await db.Exercises.AnyAsync(e => e.Id == exerciseId && e.WorkoutId == workoutId))
                return Results.NotFound("Exercise not found.");
            var set = new ExerciseSet { SetNumber = req.SetNumber, WeightKg = req.WeightKg, Reps = req.Reps, Notes = req.Notes, ExerciseId = exerciseId };
            db.Sets.Add(set);
            await db.SaveChangesAsync();
            return Results.Created($"/api/workouts/{workoutId}/exercises/{exerciseId}/sets/{set.Id}",
                new SetResponse(set.Id, set.SetNumber, set.WeightKg, set.Reps, set.Notes, set.ExerciseId));
        }).WithSummary("Add a set to an exercise");

        group.MapPut("/{id:int}", async (int workoutId, int exerciseId, int id, UpdateSetRequest req, AppDbContext db) =>
        {
            var set = await db.Sets.FirstOrDefaultAsync(s => s.Id == id && s.ExerciseId == exerciseId);
            if (set is null) return Results.NotFound();
            set.SetNumber = req.SetNumber; set.WeightKg = req.WeightKg; set.Reps = req.Reps; set.Notes = req.Notes;
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Update a set");

        group.MapDelete("/{id:int}", async (int workoutId, int exerciseId, int id, AppDbContext db) =>
        {
            var set = await db.Sets.FirstOrDefaultAsync(s => s.Id == id && s.ExerciseId == exerciseId);
            if (set is null) return Results.NotFound();
            db.Sets.Remove(set);
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).WithSummary("Delete a set");
    }
}
