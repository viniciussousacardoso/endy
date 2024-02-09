using endy.Model;
using endy.Services;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using NetDevPack.Identity;
using endy.EndpointDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentityEntityFrameworkContextConfiguration(options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddEndpointDefinitions(builder.Configuration, typeof(IEndpointDefinitionExtensions));

var app = builder.Build();
EndpointDefinitionExtensions.UseEndpointDefinitions(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();