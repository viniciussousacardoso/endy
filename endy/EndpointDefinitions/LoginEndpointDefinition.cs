using endy.EndpointDiscovery;
using endy.Model;
using endy.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace endy.EndpointDefinitions
{
    public class LoginEndpointDefinition : IEndpointDefinitionExtensions
    {
        private readonly ILogger<LoginEndpointDefinition> _logger;
        private IConfiguration _configuration;

        public void DefineServices(IServiceCollection services, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("api/v1/generatetoken", GetToken)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetToken")
                .WithTags("Login");
        }

        public IResult GetToken()
        {

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var now = DateTime.Now;
            var expiry = Convert.ToDateTime(now).AddMinutes(10);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: issuer, audience: audience, notBefore: now, expires: expiry, signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);

            return Results.Ok(stringToken);
        }

        public DateTime ToCustomFormat(DateTime dateTime)
        {
            // Crie uma nova instância de DateTime com o formato desejado
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}
