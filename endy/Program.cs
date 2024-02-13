using endy.EndpointDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
var jwtAudience = builder.Configuration.GetSection("Jwt:Audience").Get<string>();

builder.Services.AddAuthentication("Bearer")
          .AddJwtBearer("Bearer", options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = jwtIssuer,
                  ValidAudience = jwtAudience,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
              };
          });

builder.Services.AddControllers();
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

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

app.Run();