namespace endy.EndpointDiscovery
{
    public static class EndpointDefinitionExtensions
    {
        public static void AddEndpointDefinitions(

        this IServiceCollection services, IConfiguration configuration, params Type[] scanMarkers)
        {
            var endpointDefinitions = new List<IEndpointDefinitionExtensions>();

            foreach (var marker in scanMarkers)
            {
                endpointDefinitions.AddRange(
                    marker.Assembly.ExportedTypes
                        .Where(x => typeof(IEndpointDefinitionExtensions).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                        .Select(Activator.CreateInstance).Cast<IEndpointDefinitionExtensions>()
                );
            }

            foreach (var endpointDefinition in endpointDefinitions)
            {
                endpointDefinition.DefineServices(services, configuration);
            }

            services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinitionExtensions>);
        }

        public static void UseEndpointDefinitions(WebApplication app)
        {
            var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinitionExtensions>>();

            foreach (var endpointDefinition in definitions)
            {
                endpointDefinition.DefineEndpoints(app);
            }
        }
    }
}

