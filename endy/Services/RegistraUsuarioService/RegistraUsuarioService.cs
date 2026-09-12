using endy.Model;

namespace endy.Services.RegistraUsuarioService
{
    public class RegistraUsuarioService : IRegistraUsuarioService
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;
        private readonly Func<DatabaseContextService> _contextFactory;

        public RegistraUsuarioService(IServiceCollection services, IConfiguration configuration)
            : this(services, configuration, null)
        {
        }

        // Construtor adicional que permite injetar a criação do DbContext (ex.: para testes
        // unitários com um provider em memória), sem alterar o comportamento padrão em produção.
        public RegistraUsuarioService(IServiceCollection services, IConfiguration configuration, Func<DatabaseContextService> contextFactory)
        {
            _serviceProvider = services.BuildServiceProvider();
            _configuration = configuration;
            _contextFactory = contextFactory ?? (() => new DatabaseContextService(_configuration));
        }
        public bool registrarUsuario(string userName, string pass)
        {
            try
            {
                var usuarioToSave = new CriptografiaService().CriptografaUsuario(userName, pass);

                using (var context = _contextFactory())
                {
                    context.usuarioModels.Add(usuarioToSave);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool loginUsuarioService(string userName, string pass)
        {
            try
            {
                UsuarioModel usuarioModel;

                var usuarioToSend = new CriptografiaService().CriptografaUsuario(userName, pass);

                using (var context = _contextFactory())
                {
                    usuarioModel = context.usuarioModels.Where(x => x.Usuario == usuarioToSend.Usuario).FirstOrDefault();
                }
                if (usuarioModel != null)
                {

                    string salt = Convert.ToBase64String(usuarioModel.Salt);
                    string senha = new CriptografiaService().CriptografarSenha(pass, usuarioModel.Salt);


                    using (var context = _contextFactory())
                    {
                        usuarioModel = context.usuarioModels.Where(x => x.Usuario == usuarioToSend.Usuario && x.Senha == senha).FirstOrDefault();
                    }
                }

                if (usuarioModel != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
