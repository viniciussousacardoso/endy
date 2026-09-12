using endy.EndpointDefinitions;
using endy.Services.RegistraUsuarioService;

namespace endy.Tests.TestDoubles
{
    /// <summary>
    /// Subclasse de teste que substitui a criação real de <see cref="RegistraUsuarioService"/>
    /// por um dublê configurável, permitindo testar unitariamente GetToken/RegistrarUsuario
    /// sem depender de um banco de dados real.
    /// </summary>
    public class TestableLoginEndpointDefinition : LoginEndpointDefinition
    {
        public IRegistraUsuarioService RegistraUsuarioServiceDouble { get; set; }

        protected override IRegistraUsuarioService CreateRegistraUsuarioService()
        {
            return RegistraUsuarioServiceDouble;
        }
    }
}
