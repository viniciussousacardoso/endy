namespace endy.EndpointDiscovery
{
    public interface IEndpointDefinitionExtensions
    {
        void DefineServices(IServiceCollection services, IConfiguration configuration);
        void DefineEndpoints(WebApplication app);
    }
}
