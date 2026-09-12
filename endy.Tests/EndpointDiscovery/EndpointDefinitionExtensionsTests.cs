using Microsoft.AspNetCore.Builder;
using endy.EndpointDefinitions;
using endy.EndpointDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace endy.Tests.EndpointDiscovery
{
    public class EndpointDefinitionExtensionsTests
    {
        [Fact]
        public void AddEndpointDefinitions_RegistraTodasAsImplementacoesEncontradas()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();

            services.AddEndpointDefinitions(configuration, typeof(IEndpointDefinitionExtensions));
            var provider = services.BuildServiceProvider();

            var definitions = provider.GetRequiredService<IReadOnlyCollection<IEndpointDefinitionExtensions>>();

            Assert.NotEmpty(definitions);
            Assert.Contains(definitions, d => d is LoginEndpointDefinition);
            Assert.Contains(definitions, d => d is RegistroClienteEmailEndpointDefinition);
        }

        [Fact]
        public void UseEndpointDefinitions_MapeiaEndpointsSemLancarExcecao()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddEndpointDefinitions(builder.Configuration, typeof(IEndpointDefinitionExtensions));
            var app = builder.Build();

            var exception = Record.Exception(() => EndpointDefinitionExtensions.UseEndpointDefinitions(app));

            Assert.Null(exception);
        }
    }
}
