using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;
using System.Text;
using Microsoft.Extensions.Caching.StackExchangeRedis;


var builder = WebApplication.CreateBuilder(args);

//loggers
builder.Services.AddLogging(config =>
{
    config.ClearProviders(); // Clear default providers
    config.AddConsole();
    config.AddDebug();
});// Add console logger

//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<IUserRepo, UserRepoImpl>();

builder.Services.AddScoped<ICollaborationService, CollaborationServiceImpl>();
builder.Services.AddScoped<ICollaborationRepo, CollaborationRepoImpl>();

builder.Services.AddScoped<IUserNotesService, UserNotesServiceImpl>();
builder.Services.AddScoped<IUserNotesRepo, UserNotesRepoImpl>();

builder.Services.AddScoped<INotesLabelRepo, NotesLabelRepoImpl>();
builder.Services.AddScoped<INotesLabelService, NotesLabelServiceImpl>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FundooNotes", Version = "v1" });
    //For Authorization
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "127.0.0.1:6379"; // Redis server address
    options.InstanceName = "FundooNotesCache"; // Instance name for cache keys
});


builder.Services.AddDistributedMemoryCache();

//jwt
// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
builder.Services.AddAuthentication(au =>
{
    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    au.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt => {
    jwt.RequireHttpsMetadata = true;
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        //Validate the expiration and not before values in the token
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//Ending...
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(policy =>
    {
        policy.WithOrigins("http://localhost:7254", "https://localhost:7254")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithHeaders(HeaderNames.ContentType);
    });
}

app.UseHttpsRedirection();
app.UseSession();

app.UseAuthentication(); // Add authentication middleware

app.UseAuthorization();

app.MapControllers();

app.Run();
