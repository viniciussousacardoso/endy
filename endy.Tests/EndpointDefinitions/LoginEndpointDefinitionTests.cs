using Microsoft.AspNetCore.Builder;
using endy.Services.RegistraUsuarioService;
using endy.Tests.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace endy.Tests.EndpointDefinitions
{
    public class LoginEndpointDefinitionTests
    {
        private static HttpContext BuildHttpContext(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(configuration);
            var provider = services.BuildServiceProvider();

            return new DefaultHttpContext
            {
                RequestServices = provider
            };
        }

        private static IConfiguration BuildJwtConfiguration(string issuer = "issuer", string audience = "audience", string key = "chave-super-secreta-com-tamanho-suficiente")
        {
            var dict = new Dictionary<string, string>();
            if (issuer != null) dict["Jwt:Issuer"] = issuer;
            if (audience != null) dict["Jwt:Audience"] = audience;
            if (key != null) dict["Jwt:Key"] = key;

            return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        }

        [Fact]
        public void GetToken_ConfiguracaoJwtIncompleta_RetornaBadRequest()
        {
            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = Mock.Of<IRegistraUsuarioService>()
            };
            var configuration = BuildJwtConfiguration(key: null);
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "user", "pass");

            var badRequest = Assert.IsType<BadRequest<string>>(resultado);
            Assert.Equal("Configurações JWT estão incompletas.", badRequest.Value);
        }

        [Theory]
        [InlineData(null, "audience", "chave-super-secreta-com-tamanho-suficiente")]
        [InlineData("issuer", null, "chave-super-secreta-com-tamanho-suficiente")]
        [InlineData("issuer", "audience", null)]
        public void GetToken_QualquerConfiguracaoJwtAusente_RetornaBadRequest(string issuer, string audience, string key)
        {
            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = Mock.Of<IRegistraUsuarioService>()
            };
            var configuration = BuildJwtConfiguration(issuer, audience, key);
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "user", "pass");

            Assert.IsType<BadRequest<string>>(resultado);
        }

        [Fact]
        public void GetToken_LoginComSucesso_RetornaTokenComRoleAdmin()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.loginUsuarioService("USER", "senha")).Returns(true);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };
            var configuration = BuildJwtConfiguration();
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "user", "senha");

            var ok = Assert.IsType<Ok<string>>(resultado);
            Assert.StartsWith("Bearer ", ok.Value);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(ok.Value.Substring("Bearer ".Length));
            Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user");
        }

        [Fact]
        public void GetToken_LoginComSucessoEUsuarioVazio_RetornaTokenComUserGenerico()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.loginUsuarioService(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };
            var configuration = BuildJwtConfiguration();
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "", "senha");

            var ok = Assert.IsType<Ok<string>>(resultado);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(ok.Value.Substring("Bearer ".Length));
            Assert.Contains(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "userGenerico");
        }

        [Fact]
        public void GetToken_LoginSemSucesso_RetornaTokenSemRoleAdmin()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.loginUsuarioService(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };
            var configuration = BuildJwtConfiguration();
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "", "senha");

            var ok = Assert.IsType<Ok<string>>(resultado);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(ok.Value.Substring("Bearer ".Length));
            Assert.DoesNotContain(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role);
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "userGenerico");
        }

        [Fact]
        public void GetToken_LoginSemSucessoEUsuarioNaoVazio_RetornaTokenComNomeDoUsuario()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.loginUsuarioService(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };
            var configuration = BuildJwtConfiguration();
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "algumUsuario", "senha");

            var ok = Assert.IsType<Ok<string>>(resultado);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(ok.Value.Substring("Bearer ".Length));
            Assert.DoesNotContain(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role);
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "algumUsuario");
        }

        [Fact]
        public void GetToken_QuandoServicoLancaExcecao_RetornaProblem()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.loginUsuarioService(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("falha"));

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };
            var configuration = BuildJwtConfiguration();
            var httpContext = BuildHttpContext(configuration);

            var resultado = definition.GetToken(httpContext, "user", "senha");

            Assert.IsType<ProblemHttpResult>(resultado);
        }

        [Fact]
        public void RegistrarUsuario_ComSucesso_RetornaOk()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.registrarUsuario("USUARIO", "senha")).Returns(true);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };

            var resultado = definition.RegistrarUsuario("usuario", "senha");

            var ok = Assert.IsType<Ok<string>>(resultado);
            Assert.Equal("Usuario cadastrado com sucesso!!!", ok.Value);
        }

        [Fact]
        public void RegistrarUsuario_QuandoServicoRetornaFalse_RetornaProblem()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            mockService.Setup(s => s.registrarUsuario(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };

            var resultado = definition.RegistrarUsuario("usuario", "senha");

            Assert.IsType<ProblemHttpResult>(resultado);
        }

        [Fact]
        public void RegistrarUsuario_QuandoUsuarioJaCadastrado_RetornaBadRequestEspecifico()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            var innerException = new Exception("Duplicate entry for key usuario.user_UNIQUE");
            mockService.Setup(s => s.registrarUsuario(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("erro ao salvar", innerException));

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };

            var resultado = definition.RegistrarUsuario("usuario", "senha");

            var badRequest = Assert.IsType<BadRequest<string>>(resultado);
            Assert.Equal("Erro, usuario já cadastrado.", badRequest.Value);
        }

        [Fact]
        public void RegistrarUsuario_QuandoOutraExcecao_RetornaBadRequestGenerico()
        {
            var mockService = new Mock<IRegistraUsuarioService>();
            var innerException = new Exception("outro erro qualquer");
            mockService.Setup(s => s.registrarUsuario(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("erro ao salvar", innerException));

            var definition = new TestableLoginEndpointDefinition
            {
                RegistraUsuarioServiceDouble = mockService.Object
            };

            var resultado = definition.RegistrarUsuario("usuario", "senha");

            Assert.IsType<BadRequest<Exception>>(resultado);
        }

        [Fact]
        public void DefineServices_ArmazenaServicesEConfiguration()
        {
            var definition = new endy.EndpointDefinitions.LoginEndpointDefinition();
            var services = new ServiceCollection();
            var configuration = BuildJwtConfiguration();

            definition.DefineServices(services, configuration);

            // Não há getters públicos; o teste garante apenas que a chamada não lança exceção
            // e que o construtor com factory real é exercitado indiretamente pelos testes de integração de endpoints.
        }

        [Fact]
        public void DefineEndpoints_RegistraRotasSemLancarExcecao()
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();
            var definition = new endy.EndpointDefinitions.LoginEndpointDefinition();

            var exception = Record.Exception(() => definition.DefineEndpoints(app));

            Assert.Null(exception);
        }

        [Fact]
        public void ConstrutorComLoggerEConfiguration_InicializaCampos()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<endy.EndpointDefinitions.LoginEndpointDefinition>>();
            var configuration = BuildJwtConfiguration();

            var definition = new endy.EndpointDefinitions.LoginEndpointDefinition(logger, configuration);

            Assert.NotNull(definition);
        }

        [Fact]
        public void CreateRegistraUsuarioService_ImplementacaoReal_RetornaInstanciaValida()
        {
            var definition = new endy.EndpointDefinitions.LoginEndpointDefinition();
            definition.DefineServices(new ServiceCollection(), BuildJwtConfiguration());

            var metodo = typeof(endy.EndpointDefinitions.LoginEndpointDefinition)
                .GetMethod("CreateRegistraUsuarioService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var resultado = metodo.Invoke(definition, null);

            Assert.IsType<RegistraUsuarioService>(resultado);
        }
    }
}
