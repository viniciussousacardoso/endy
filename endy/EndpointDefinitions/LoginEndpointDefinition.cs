using endy.EndpointDiscovery;
using endy.Services.RegistraUsuarioService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace endy.EndpointDefinitions
{
    public class LoginEndpointDefinition : IEndpointDefinitionExtensions
    {
        private IServiceCollection _services;
        private IConfiguration _configuration;
        private readonly ILogger<LoginEndpointDefinition> _logger;

        public void DefineServices(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
        }

        public LoginEndpointDefinition(ILogger<LoginEndpointDefinition> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public LoginEndpointDefinition() { }

        public void DefineEndpoints(WebApplication app)
        {
            app.MapPost("api/v1/generatetoken", GetToken)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetToken")
                .WithTags("Login");

            app.MapPost("api/v1/registraUsuario", RegistrarUsuario)
               .ProducesValidationProblem()
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status404NotFound)
               .WithName("Registrar")
    .WithTags("Login");
        }

        public IResult GetToken(HttpContext httpContext, string user = "", string pass = "")
        {
            var logger = httpContext.RequestServices.GetService<ILogger<LoginEndpointDefinition>>();
            var configuration = httpContext.RequestServices.GetService<IConfiguration>();
            try
            {
                RegistraUsuarioService registraUsuarioService = new RegistraUsuarioService(_services, _configuration);

                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = configuration["Jwt:Key"];

                if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
                {
                    logger.LogError("Configurações JWT estão incompletas.");
                    return Results.BadRequest("Configurações JWT estão incompletas.");
                }

                var now = DateTime.UtcNow;
                var expiry = now.AddMinutes(30);

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>();

                if (registraUsuarioService.loginUsuarioService(user.ToUpper(), pass))
                {
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user == string.Empty ? "userGenerico" : user));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
                else
                {
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user == string.Empty ? "userGenerico" : user));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                }

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    notBefore: now,
                    expires: expiry,
                    signingCredentials: credentials);

                var tokenHandler = new JwtSecurityTokenHandler();

                var stringToken = tokenHandler.WriteToken(token);

                return Results.Ok($"Bearer {stringToken}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao gerar o token");
                return Results.Problem("Ocorreu um erro ao gerar o token.");
            }
        }

        public IResult RegistrarUsuario(string userName, string pass)
        {
            userName = userName.ToUpper();
            try
            {
                RegistraUsuarioService registraUsuarioService = new RegistraUsuarioService(_services, _configuration);

                if (registraUsuarioService.registrarUsuario(userName, pass))
                {
                    return Results.Ok("Usuario cadastrado com sucesso!!!");
                }
                return Results.Problem("Não foi possivel realizar o cadastro");
            }
            catch (Exception ex)
            { 
                if(ex.InnerException.ToString().Contains("usuario.user_UNIQUE"))
                    return Results.BadRequest("Erro, usuario já cadastrado.");
                return Results.BadRequest(ex);
            }
        }

    }
}
