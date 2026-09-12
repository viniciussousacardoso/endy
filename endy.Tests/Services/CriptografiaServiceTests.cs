using endy.Services;

namespace endy.Tests.Services
{
    public class CriptografiaServiceTests
    {
        [Fact]
        public void GeraSalt_PreenchendoArrayComTamanhoInformado_RetornaArrayPreenchido()
        {
            var service = new CriptografiaService();
            var salt = new byte[16];

            var resultado = service.GeraSalt(salt);

            Assert.Same(salt, resultado);
            Assert.Equal(16, resultado.Length);
            Assert.Contains(resultado, b => b != 0);
        }

        [Fact]
        public void CriptografarSenha_MesmaSenhaEMesmoSalt_GeraSempreOMesmoHash()
        {
            var service = new CriptografiaService();
            var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            var hash1 = service.CriptografarSenha("minhaSenha", salt);
            var hash2 = service.CriptografarSenha("minhaSenha", salt);

            Assert.Equal(hash1, hash2);
            Assert.False(string.IsNullOrWhiteSpace(hash1));
        }

        [Fact]
        public void CriptografarSenha_SenhasDiferentes_GeramHashesDiferentes()
        {
            var service = new CriptografiaService();
            var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            var hash1 = service.CriptografarSenha("senhaUm", salt);
            var hash2 = service.CriptografarSenha("senhaDois", salt);

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void CriptografaUsuario_RetornaUsuarioModelComSaltESenhaCriptografados()
        {
            var service = new CriptografiaService();

            var usuario = service.CriptografaUsuario("usuarioTeste", "senhaTeste");

            Assert.Equal("usuarioTeste", usuario.Usuario);
            Assert.Equal(16, usuario.Salt.Length);
            Assert.False(string.IsNullOrWhiteSpace(usuario.Senha));

            var senhaEsperada = service.CriptografarSenha("senhaTeste", usuario.Salt);
            Assert.Equal(senhaEsperada, usuario.Senha);
        }
    }
}
