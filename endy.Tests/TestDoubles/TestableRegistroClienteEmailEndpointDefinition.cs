using endy.EndpointDefinitions;
using endy.Services;

namespace endy.Tests.TestDoubles
{
    /// <summary>
    /// Subclasse de teste que substitui a criação real de <see cref="DatabaseContextService"/>
    /// por uma fábrica configurável (ex.: provider InMemory), permitindo testar os endpoints
    /// de cliente sem depender de um banco de dados MySQL real.
    /// </summary>
    public class TestableRegistroClienteEmailEndpointDefinition : RegistroClienteEmailEndpointDefinition
    {
        public Func<DatabaseContextService> ContextFactory { get; set; }

        protected override DatabaseContextService CreateContext()
        {
            return ContextFactory();
        }
    }
}
