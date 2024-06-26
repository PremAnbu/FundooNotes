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
using Confluent.Kafka;
using NLog.Web;
using NLog;


    var builder = WebApplication.CreateBuilder(args);
    // NLog: Setup NLog for Dependency injection
    //builder.Logging.ClearProviders();
    //builder.Host.UseNLog();

    // Register the ApacheKafkaConsumerService as a singleton hosted service
    builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var producerConfig = new ProducerConfig{
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<string, string>(producerConfig).Build();
});
 
// Register the ApacheKafka CONSUMER Service as a singleton hosted service
builder.Services.AddSingleton<IConsumer<string, string>>(sp =>
{
    var consumer = new ConsumerBuilder<String, String>(new ConsumerConfig{
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"],
        GroupId = builder.Configuration["Kafka:ConsumerGroupId"]
    }).Build();
    consumer.Subscribe(builder.Configuration["Kafka:Topic"]);
    return consumer;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://localhost:7004")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
            .AllowAnyOrigin();
        });
});


builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddScoped<BuisinessLayer.service.Iservice.IUser, UserImplementationBL>();
builder.Services.AddScoped<IUserRepo, UserImplementationRL>();

builder.Services.AddScoped<ICollaboration, CollaborationImplementationBL>();
builder.Services.AddScoped<ICollaborationRepo, CollaborationImplementationRL>();

builder.Services.AddScoped<INotes, NotesImplementationBL>();
builder.Services.AddScoped<INotesRepo, NotesImplementationRL>();

builder.Services.AddScoped<ILabelRepo, LabelImplementationRL>();
builder.Services.AddScoped<INotesLabelService, NotesLabelServiceImpl>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Get the secret key from the configuration
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]);

// Add JWT Bearer authentication options
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate the JWT signature
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            // For simplicity in development, disable issuer and audience validation
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    // Define the Swagger document metadata
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add security definition for JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Add security requirements for Swagger endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Swagger/OpenAPI and Swagger generation options
//builder.Services.AddSwaggerGen(c =>
//{
//    // Define Swagger document Meta Data (version and Title)
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FundooNotes", Version = "v1" });
//    //For Authorization
//    var securitySchema = new OpenApiSecurityScheme
//    {
//        Description = "Using the Authorization header with the Bearer scheme.",
//        Name = "Authorization",         // JWT Token Header Name
//        In = ParameterLocation.Header,  // Location of the JWT token in the request headers
//        Type = SecuritySchemeType.Http, // Http type of Security Scheme
//        Scheme = "bearer",
//        Reference = new OpenApiReference
//        {
//            Type = ReferenceType.SecurityScheme,
//            Id = "Bearer"
//        }
//    };
//    c.AddSecurityDefinition("Bearer", securitySchema);

//    // Specify security requirements for Swagger endpoints
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    { securitySchema, new[] { "Bearer" } }
//                });
//});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "127.0.0.1:6379"; // Redis server address
    options.InstanceName = "FundooNotesCache"; // Instance name for cache keys
});

builder.Services.AddDistributedMemoryCache();
//jwt
// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

//builder.Services.AddAuthentication(au =>
//{
//    au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    au.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(jwt => {
//    jwt.RequireHttpsMetadata = true;
//    jwt.SaveToken = true;
//    jwt.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        //Validate the expiration and not before values in the token
//        ValidateLifetime = true,
//        ClockSkew = TimeSpan.Zero,
//        // Specify whether the server should validate the issuerSigningkey
//        ValidateIssuerSigningKey = true,
//        // Set the issuerSigningkey to verify the JWT signature
//        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});

//loggers
//builder.Services.AddLogging(config =>
//{
//    config.ClearProviders(); // Clear default providers
//    config.AddConsole();
//    config.AddDebug();
//});// Add console logger

var logpath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
NLog.GlobalDiagnosticsContext.Set("LogDirectory", logpath);
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();
//session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust timeout as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Ending...
var app = builder.Build();

app.UseCors("AllowSpecificOrigin");
app.UseCors();

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

// Configure the HTTP request pipeline
app.UseHttpsRedirection();

app.UseSession();
// Enable authentication middleware
app.UseAuthentication();
// Enable authorization middleware
app.UseAuthorization();
// Map controller routes
app.MapControllers();
// Execute the request pipeline
app.Run();
