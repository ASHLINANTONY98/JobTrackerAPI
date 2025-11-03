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
using JobTrackerApp.Middlewares;
using Business.BackgroundServices;

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
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddHostedService<OtpCleanupService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
//validate all dto vali in that folder
builder.Services.AddValidatorsFromAssemblyContaining<Business.Validators.RegisterDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/////////////////////////////////////first came here//////////////////////////////////////

var app = builder.Build();   // create the application

// Configure the HTTP request pipeline. and a ui for testing api
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>(); //then it came to this line and Wraps everything in try catch

app.UseHttpsRedirection();//then here Redirects HTTP → HTTPS

app.UseAuthentication();//then Validates JWT token if present

app.UseAuthorization();//then  Enforces Authorize rules

app.MapControllers();//then to your controller actions goo

app.Run();// starts the server and active pipeline and swagger