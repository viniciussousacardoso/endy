using endy.Model;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace endy.Services
{
    public class CriptografiaService
    {
        public Byte[] GeraSalt(byte[] salt)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string CriptografarSenha(string senha, byte[] salt)
        {
            string senhaCriptografada = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: senha,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return senhaCriptografada;
        }

        public UsuarioModel CriptografaUsuario(string usuario, string pass)
        {
            byte[] salt = new byte[128 / 8];
            byte[] saltFinal = GeraSalt(salt);
            string senha = CriptografarSenha(pass, salt);

            return new UsuarioModel(senha, saltFinal, usuario);
        }


        //string salt = Convert.ToBase64String(usuarioBanco.Salt);
        //string senha = CriptografarSenha(usuarioInput.Senha, usuarioBanco.Salt);
        //return _context.Usuario.Where(x => x.Login == user.Login && x.Senha == senha && x.Ativo == true).FirstOrDefault();
    }
}
