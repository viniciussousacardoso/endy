using endy.Model;
using endy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace endy.Tests.Services
{
    public class RegistraUsuarioServiceTests
    {
        private static DbContextOptions<DatabaseContextService> CreateInMemoryOptions(string dbName)
        {
            return new DbContextOptionsBuilder<DatabaseContextService>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        [Fact]
        public void RegistrarUsuario_ComSucesso_PersisteUsuarioERetornaTrue()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = CreateInMemoryOptions(dbName);
            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => new DatabaseContextService(options));

            var resultado = service.registrarUsuario("NOVO_USUARIO", "senha123");

            Assert.True(resultado);

            using var context = new DatabaseContextService(options);
            Assert.Single(context.usuarioModels, u => u.Usuario == "NOVO_USUARIO");
        }

        [Fact]
        public void RegistrarUsuario_QuandoContextoLancaExcecao_RelancaExcecao()
        {
            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => throw new InvalidOperationException("falha de banco"));

            Assert.Throws<InvalidOperationException>(() => service.registrarUsuario("USUARIO", "senha"));
        }

        [Fact]
        public void LoginUsuarioService_UsuarioNaoEncontrado_RetornaFalse()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = CreateInMemoryOptions(dbName);
            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => new DatabaseContextService(options));

            var resultado = service.loginUsuarioService("INEXISTENTE", "senha");

            Assert.False(resultado);
        }

        [Fact]
        public void LoginUsuarioService_UsuarioComSenhaCorreta_RetornaTrue()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = CreateInMemoryOptions(dbName);
            var cripto = new CriptografiaService();
            var usuario = cripto.CriptografaUsuario("USUARIO_VALIDO", "senhaCorreta");

            using (var context = new DatabaseContextService(options))
            {
                context.usuarioModels.Add(usuario);
                context.SaveChanges();
            }

            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => new DatabaseContextService(options));

            var resultado = service.loginUsuarioService("USUARIO_VALIDO", "senhaCorreta");

            Assert.True(resultado);
        }

        [Fact]
        public void LoginUsuarioService_UsuarioComSenhaIncorreta_RetornaFalse()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = CreateInMemoryOptions(dbName);
            var cripto = new CriptografiaService();
            var usuario = cripto.CriptografaUsuario("USUARIO_SENHA_ERRADA", "senhaCorreta");

            using (var context = new DatabaseContextService(options))
            {
                context.usuarioModels.Add(usuario);
                context.SaveChanges();
            }

            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => new DatabaseContextService(options));

            var resultado = service.loginUsuarioService("USUARIO_SENHA_ERRADA", "senhaErrada");

            Assert.False(resultado);
        }

        [Fact]
        public void LoginUsuarioService_QuandoContextoLancaExcecao_RelancaExcecao()
        {
            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(
                new ServiceCollection(), null, () => throw new InvalidOperationException("falha de banco"));

            Assert.Throws<InvalidOperationException>(() => service.loginUsuarioService("USUARIO", "senha"));
        }

        [Fact]
        public void Construtor_SemContextFactory_UsaFabricaPadrao()
        {
            // Cobre o construtor de dois parâmetros (encaminhado ao construtor com contextFactory = null).
            var service = new endy.Services.RegistraUsuarioService.RegistraUsuarioService(new ServiceCollection(), null);

            Assert.NotNull(service);
        }
    }
}
