using Microsoft.AspNetCore.Builder;
using endy.EndpointDefinitions;
using endy.Model;
using endy.Services;
using endy.Tests.TestDoubles;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace endy.Tests.EndpointDefinitions
{
    public class RegistroClienteEmailEndpointDefinitionTests
    {
        private static Func<DatabaseContextService> InMemoryFactory(string dbName)
        {
            var options = new DbContextOptionsBuilder<DatabaseContextService>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return () => new DatabaseContextService(options);
        }

        [Fact]
        public void GetByEmail_SemClientesCorrespondentes_RetornaNotFound()
        {
            var definition = new TestableRegistroClienteEmailEndpointDefinition
            {
                ContextFactory = InMemoryFactory(Guid.NewGuid().ToString())
            };

            var resultado = definition.GetByEmail("naoexiste@a.com");

            Assert.IsType<NotFound<string>>(resultado);
        }

        [Fact]
        public void GetByEmail_ComClienteCorrespondente_RetornaOkComJson()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = InMemoryFactory(dbName);
            using (var context = factory())
            {
                context.ClienteModels.Add(new ClienteModel { Email = "cliente@a.com", Nome = "Cliente A" });
                context.SaveChanges();
            }

            var definition = new TestableRegistroClienteEmailEndpointDefinition { ContextFactory = factory };

            var resultado = definition.GetByEmail("cliente@a.com");

            var ok = Assert.IsType<Ok<string>>(resultado);
            Assert.Contains("cliente@a.com", ok.Value);
        }

        [Fact]
        public void GetAllClientes_SemClientes_RetornaNotFound()
        {
            var definition = new TestableRegistroClienteEmailEndpointDefinition
            {
                ContextFactory = InMemoryFactory(Guid.NewGuid().ToString())
            };

            var resultado = definition.GetAllClientes();

            Assert.IsType<NotFound<string>>(resultado);
        }

        [Fact]
        public void GetAllClientes_ComClientes_RetornaOkComJson()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = InMemoryFactory(dbName);
            using (var context = factory())
            {
                context.ClienteModels.Add(new ClienteModel { Email = "b@a.com", Nome = "Cliente B" });
                context.SaveChanges();
            }

            var definition = new TestableRegistroClienteEmailEndpointDefinition { ContextFactory = factory };

            var resultado = definition.GetAllClientes();

            var ok = Assert.IsType<Ok<string>>(resultado);
            Assert.Contains("Cliente B", ok.Value);
        }

        [Fact]
        public void SalvarCliente_EmailInvalido_RetornaBadRequest()
        {
            var definition = new TestableRegistroClienteEmailEndpointDefinition
            {
                ContextFactory = InMemoryFactory(Guid.NewGuid().ToString())
            };

            var resultado = definition.SalvarCliente(new ClienteModel { Email = "invalido", Nome = "Fulano" });

            var badRequest = Assert.IsType<BadRequest<string>>(resultado);
            Assert.Equal("Email Invalido por favor insira um email valido.", badRequest.Value);
        }

        [Fact]
        public void SalvarCliente_NomeVazio_RetornaBadRequest()
        {
            var definition = new TestableRegistroClienteEmailEndpointDefinition
            {
                ContextFactory = InMemoryFactory(Guid.NewGuid().ToString())
            };

            var resultado = definition.SalvarCliente(new ClienteModel { Email = "valido@teste.com", Nome = "   " });

            Assert.IsType<BadRequest<string>>(resultado);
        }

        [Theory]
        [InlineData("pessoa@dominio.com")]
        [InlineData("pessoa@dominio.net")]
        [InlineData("pessoa@dominio.org")]
        [InlineData("pessoa@dominio.gov")]
        [InlineData("PESSOA@DOMINIO.COM")]
        public void SalvarCliente_EmailValido_PersisteERetornaOk(string email)
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = InMemoryFactory(dbName);
            var definition = new TestableRegistroClienteEmailEndpointDefinition { ContextFactory = factory };

            var resultado = definition.SalvarCliente(new ClienteModel
            {
                Email = email,
                Nome = "Fulano de Tal",
                telefone = "11999999999",
                motivo_contato = "Duvida"
            });

            var ok = Assert.IsType<Ok<string>>(resultado);
            Assert.Contains(email, ok.Value);

            using var context = factory();
            Assert.Single(context.ClienteModels, c => c.Email == email);
        }

        [Fact]
        public void DefineServices_ArmazenaConfiguration()
        {
            var definition = new RegistroClienteEmailEndpointDefinition();
            var configuration = new ConfigurationBuilder().Build();

            var exception = Record.Exception(() => definition.DefineServices(new ServiceCollection(), configuration));

            Assert.Null(exception);
        }

        [Fact]
        public void DefineEndpoints_RegistraRotasSemLancarExcecao()
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            var definition = new RegistroClienteEmailEndpointDefinition();

            var exception = Record.Exception(() => definition.DefineEndpoints(app));

            Assert.Null(exception);
        }

        [Fact]
        public void ConstrutorComLoggerEConfiguration_InicializaCampos()
        {
            var logger = NullLogger<RegistroClienteEmailEndpointDefinition>.Instance;
            var configuration = new ConfigurationBuilder().Build();

            var definition = new RegistroClienteEmailEndpointDefinition(logger, configuration);

            Assert.NotNull(definition);
        }

        [Fact]
        public void CreateContext_ImplementacaoReal_RetornaInstanciaValida()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:DefaultConnection"] = "server=localhost;database=teste;user=root;password=x;"
                })
                .Build();

            var definition = new RegistroClienteEmailEndpointDefinition();
            definition.DefineServices(new ServiceCollection(), configuration);

            var metodo = typeof(RegistroClienteEmailEndpointDefinition)
                .GetMethod("CreateContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            using var resultado = (DatabaseContextService)metodo.Invoke(definition, null);

            Assert.Contains("MySql", resultado.Database.ProviderName);
        }
    }
}
