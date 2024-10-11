using endy.EndpointDiscovery;
using endy.Model;
using endy.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace endy.EndpointDefinitions
{
    public class RegistroClienteEmailEndpointDefinition : IEndpointDefinitionExtensions
    {
        private readonly ILogger<RegistroClienteEmailEndpointDefinition> _logger;
        private IConfiguration _configuration;

        public RegistroClienteEmailEndpointDefinition(ILogger<RegistroClienteEmailEndpointDefinition> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public RegistroClienteEmailEndpointDefinition() { }

        public void DefineServices(IServiceCollection services, IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void DefineEndpoints(WebApplication app)
        {
            app.MapGet("api/v1/cliente/buscacliente", GetByEmail)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithName("GetByEmail")
                .WithTags("Cliente");

            app.MapPost("api/v1/cliente/salvarcliente", SalvarCliente)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithName("SaveCliente")
                .WithTags("Cliente");

            app.MapGet("api/v1/cliente/buscatodos", GetAllClientes)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces (StatusCodes.Status401Unauthorized)
                .WithName("GetAllClientes")
                .WithTags("Cliente");
        }

        [Authorize]
        public IResult GetByEmail(string email)
        {
            List<ClienteModel> clientes = new();

            using (var context = new DatabaseContextService(_configuration))
            {
                clientes = context.ClienteModels.Where(x => x.Email == email).ToList();
            }

            if (!clientes.Any())
            {
                return Results.NotFound("Nenhum dado encontrado.");
            }
            return Results.Ok(JsonConvert.SerializeObject(clientes));
        }

        [Authorize(Roles ="Admin")]
        public IResult GetAllClientes()
        {
            List<ClienteModel> clientes = new();

            using (var context = new DatabaseContextService(_configuration))
            {
                clientes = context.ClienteModels.ToList();
            }

            if (!clientes.Any())
            {
                return Results.NotFound("Nenhum dado encontrado.");
            }
            return Results.Ok(JsonConvert.SerializeObject(clientes));
        }

        [Authorize]
        public IResult SalvarCliente(ClienteModel model)
        {
            if (IsValidEmail(model.Email) && !string.IsNullOrWhiteSpace(model.Nome))
            {
                using (var context = new DatabaseContextService(_configuration))
                {
                    ClienteModel cliente = new()
                    {
                        Email = model.Email,
                        Nome = model.Nome,
                        telefone = model.telefone,
                        motivo_contato = model.motivo_contato,
                    };

                    context.ClienteModels.Add(cliente);
                    context.SaveChanges();
                }

                return Results.Ok($"Cliente {model.Nome} - {model.Email} salvo com sucesso!");
            }

            return Results.BadRequest("Email Invalido por favor insira um email valido.");
        }

        private static bool IsValidEmail(string email)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

            return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
    }
}
