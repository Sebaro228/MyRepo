using BLL.Services;
using DLL.AiClients;
using DLL.Context;
using DLL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<RequestDbContext>(options =>
    options.UseSqlServer());
builder.Services.AddTransient<RequestRepository>();
builder.Services.AddTransient<RequestService>();
builder.Services.AddTransient<IConfiguration>(provider => builder.Configuration);
builder.Services.AddHttpClient<ClaudeClient>();
builder.Services.AddHttpClient<OpenAiClient>();
builder.Services.AddHttpClient<TtsClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
//hello