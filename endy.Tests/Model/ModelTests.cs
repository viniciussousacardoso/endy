using endy.Model;

namespace endy.Tests.Model
{
    public class ModelTests
    {
        [Fact]
        public void ClienteModel_PermiteDefinirELerTodasAsPropriedades()
        {
            var cliente = new ClienteModel
            {
                IdCliente = 1,
                Email = "a@a.com",
                Nome = "Fulano",
                Visualizado = true,
                telefone = "11999999999",
                motivo_contato = "Duvida"
            };

            Assert.Equal(1, cliente.IdCliente);
            Assert.Equal("a@a.com", cliente.Email);
            Assert.Equal("Fulano", cliente.Nome);
            Assert.True(cliente.Visualizado);
            Assert.Equal("11999999999", cliente.telefone);
            Assert.Equal("Duvida", cliente.motivo_contato);
        }

        [Fact]
        public void UsuarioModel_ConstrutorComSenhaESalt_InicializaPropriedades()
        {
            var salt = new byte[] { 1, 2, 3 };

            var usuario = new UsuarioModel("senhaCriptografada", salt);

            Assert.Equal("senhaCriptografada", usuario.Senha);
            Assert.Equal(salt, usuario.Salt);
            Assert.Null(usuario.Usuario);
        }

        [Fact]
        public void UsuarioModel_ConstrutorComSenhaSaltEUsuario_InicializaPropriedades()
        {
            var salt = new byte[] { 4, 5, 6 };

            var usuario = new UsuarioModel("senhaCriptografada", salt, "usuarioTeste");

            Assert.Equal("senhaCriptografada", usuario.Senha);
            Assert.Equal(salt, usuario.Salt);
            Assert.Equal("usuarioTeste", usuario.Usuario);
        }

        [Fact]
        public void UsuarioModel_PermiteDefinirELerIdUsuario()
        {
            var usuario = new UsuarioModel("senha", new byte[] { 1 }, "user")
            {
                idUsuario = 42
            };

            Assert.Equal(42, usuario.idUsuario);
        }
    }
}
