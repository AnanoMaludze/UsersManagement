using System.Reflection;
using UsersManagement.Persistence;
using UsersManagement.Repositories;
using MediatR;
using UsersManagement.Behavior;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using UsersManagement.Errors;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using UsersManagement.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers(opt => opt.Filters.Add<UsersManagementExceptionHandlerAttribute>());

builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnectionString"),
        sqlServerOptionsAction: sqlOptions =>{}));

builder.Services.AddScoped<IMainDbContext, MainDbContext>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<ProblemDetailsFactory, UsersManagementProblemDetailsFactory>();
builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

