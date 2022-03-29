using EFCore领域事件发布的时机;
using EFCore领域事件发布的时机.Controllers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.Load("EFCore领域事件发布的时机"));
//Data Source=blog.db
builder.Services.AddDbContext<UserDbContext>(opt => opt.UseSqlite("Data Source=dddtest1.db"));
builder.Services.AddScoped(typeof(IMiddleware<string>),typeof(TestMiddleware1));
builder.Services.AddScoped(typeof(IMiddleware<string>), typeof(TestMiddleware2));
builder.Services.AddScoped(typeof(IMiddleware<string>), typeof(TestMiddleware3));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
