using GrowthOps.Api.Services;
using GrowthOps.Api.Dtos;
using GrowthOps.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using System.Text;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GoalService>();
builder.Services.AddScoped<ProgressEntryService>();
builder.Services.AddScoped<GoalProgressService>();
builder.Services.AddScoped<GoalStreakService>();
builder.Services.AddScoped<GoalAnalyticsService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddDbContext<GrowthOpsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("GrowthOpsDb")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GrowthOps API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var allowedOrigins =
    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod().SetPreflightMaxAge(TimeSpan.Zero);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();

//Token
app.MapGet("/me", [Authorize] (HttpContext http) =>
{
    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = http.User.FindFirst(ClaimTypes.Email)?.Value;

    return Results.Ok(new
    {
        userId,
        email
    });
});

//Goals

app.MapGet("/goals", [Authorize] async (GoalService service, HttpContext http, CancellationToken cancellationToken) =>
{
    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId is null)
        return Results.Unauthorized();

    var goals = await service.GetAllAsync(int.Parse(userId), cancellationToken);
    return Results.Ok(goals);
});

app.MapGet("/goals/{id:int}", [Authorize] async (int id, GoalService service, HttpContext http) =>
{
    var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId is null)
        return Results.Unauthorized();

    var goal = await service.GetByIdAsync(id, int.Parse(userId));
    if (goal is null)
        return Results.NotFound(new { error = $"Goal with id {id} not found." });

    return Results.Ok(goal);
});

app.MapPost("/goals", [Authorize] async (CreateGoalRequest request, GoalService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();

    if (string.IsNullOrWhiteSpace(request.Title))
        return Results.BadRequest(new { error = "Title is required." });


    if (request.TargetDate == default)
        return Results.BadRequest(new { error = "TargetDateUtc is required." });

    try
    {
        var goal = await service.CreateAsync(request, int.Parse(userIdValue));
        return Results.Created($"/goals/{goal.Id}", goal);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapDelete("/goals/{id:int}", async (int id, GoalService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userIdValue is null)
        return Results.Unauthorized();

    var deleted = await service.DeleteAsync(id, int.Parse(userIdValue));

    return deleted
        ? Results.NoContent()
        : Results.NotFound(new { error = $"Goal with id {id} not found." });
}).RequireAuthorization();

//Progress Entries
app.MapGet("/goals/{id:int}/entries", [Authorize] async (
    int id,
    int? page,
    int? pageSize,
    ProgressEntryService service,
    HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userIdValue is null)
        return Results.Unauthorized();

    var result = await service.GetForGoalAsync(
        id,
        int.Parse(userIdValue),
        page ?? 1,
        pageSize ?? 10
    );

    return result is null
        ? Results.NotFound(new { error = $"Goal with id {id} not found." })
        : Results.Ok(result);
});

app.MapPost("/goals/{id:int}/entries", [Authorize] async (int id, CreateProgressEntryRequest request, ProgressEntryService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();
    try
    {
        var created = await service.CreateAsync(id, request, int.Parse(userIdValue));

        return created is null
            ? Results.NotFound(new { error = $"Goal with id {id} not found." })
            : Results.Created($"/goals/{id}/entries/{created.Id}", created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

//Progress Summary
app.MapGet("/goals/{id:int}/summary", [Authorize] async (int id, GoalProgressService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();

    var summary = await service.GetSummaryAsync(id, int.Parse(userIdValue));

    return summary is null
        ? Results.NotFound(new { error = $"Goal with id {id} not found." })
        : Results.Ok(summary);
});

//Streak
app.MapGet("/goals/{id:int}/streak", [Authorize] async (int id, GoalStreakService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();

    var streak = await service.GetStreakAsync(id, int.Parse(userIdValue));

    return streak is null
        ? Results.NotFound(new { error = $"Goal with id {id} not found." })
        : Results.Ok(streak);
});

//Analytics
app.MapGet("/goals/{id:int}/analytics", [Authorize] async (int id, GoalAnalyticsService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();

    var result = await service.GetAsync(id, int.Parse(userIdValue));

    return result is null
        ? Results.NotFound(new { error = $"Goal with id {id} not found." })
        : Results.Ok(result);
});

//Chart
app.MapGet("/goals/{id:int}/chart", [Authorize] async (int id, ProgressEntryService service, HttpContext http) =>
{
    var userIdValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdValue is null)
        return Results.Unauthorized();
    var result = await service.GetChartDataAsync(id, int.Parse(userIdValue));
    return Results.Ok(result);
});

//Auth
app.MapPost("/auth/register", async (RegisterRequest request, AuthService service) =>
{
    try
    {
        var user = await service.RegisterAsync(request.Email, request.Password);

        return Results.Ok(new
        {
            user.Id,
            user.Email
        });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/auth/login", async (LoginRequest request, AuthService service, IConfiguration config) =>
{
    var user = await service.LoginAsync(request.Email, request.Password);

    if (user is null)
        return Results.Unauthorized();

    var token = service.GenerateToken(user, config);

    return Results.Ok(new
    {
        token,
        user.Id,
        user.Email
    });
});



app.Run();

