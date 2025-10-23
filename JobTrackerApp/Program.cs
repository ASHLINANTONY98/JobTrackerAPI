using Business.Mapping;
using Business.Services;
using Business.Interfaces;
using DataAccess.DbContext.DataAccess.DbContext;
using DataAccess.Repositories;
using DataAccess.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using JobTrackerApp.Extensions;
using Microsoft.EntityFrameworkCore;
using Common.Helpers;
using Microsoft.Extensions.DependencyInjection;
using JobTrackerApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Business.Validators.RegisterDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
