using BuisinessLayer.service.Iservice;
using BuisinessLayer.service.serviceImpl;
using RepositaryLayer.Context;
using RepositaryLayer.Repositary.IRepo;
using RepositaryLayer.Repositary.RepoImpl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();
builder.Services.AddScoped<ICollaborationService, CollaborationServiceImpl>();
builder.Services.AddScoped<IUserNotesService, UserNotesServiceImpl>();
builder.Services.AddScoped<IUserNotesRepo, UserNotesRepoImpl>();
builder.Services.AddScoped<IUserRepo, UserRepoImpl>();
builder.Services.AddScoped<ICollaborationRepo, CollaborationRepoImpl>();
builder.Services.AddScoped<IUserRepo, UserRepoImpl>();
builder.Services.AddScoped<ICollaborationRepo, CollaborationRepoImpl>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
