using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection");

if (connectionString is null)
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=WorkoutTrackerDb;Trusted_Connection=True;"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(connectionString,
        sql =>
        {
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapWorkoutEndpoints();
app.MapExerciseEndpoints();
app.MapSetEndpoints();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();
