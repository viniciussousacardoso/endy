using endy.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace endy.Services.RegistraUsuarioService
{
    public class RegistraUsuarioService : IRegistraUsuarioService
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;

        public RegistraUsuarioService(IServiceCollection services, IConfiguration configuration)
        {
            _serviceProvider = services.BuildServiceProvider();
            _configuration = configuration;
        }
        public bool registrarUsuario(string userName, string pass)
        {
            try
            {
                var usuarioToSave = new CriptografiaService().CriptografaUsuario(userName, pass);

                using (var context = new DatabaseContextService(_configuration))
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

                using (var context = new DatabaseContextService(_configuration))
                {
                    usuarioModel = context.usuarioModels.Where(x => x.Usuario == usuarioToSend.Usuario).FirstOrDefault();
                }
                if (usuarioModel != null)
                {

                    string salt = Convert.ToBase64String(usuarioModel.Salt);
                    string senha = new CriptografiaService().CriptografarSenha(pass, usuarioModel.Salt);


                    using (var context = new DatabaseContextService(_configuration))
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
