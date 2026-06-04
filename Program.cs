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
        opt.UseSqlServer(connectionString));
}

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapWorkoutEndpoints();
app.MapExerciseEndpoints();
app.MapSetEndpoints();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();
