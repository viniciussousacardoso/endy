using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace endy.Model
{
    [Table("usuario")]
    public class UsuarioModel
    {
        public UsuarioModel(string senha, byte[] salt)
        {
            Senha = senha;
            Salt = salt;
        }

        public UsuarioModel(string senha, byte[] salt, string usuario)
        {
            Senha = senha;
            Salt = salt;
            Usuario = usuario;
        }
        [Key, Column("id_usuario")]
        public int idUsuario { get; set; }

        [Column("pass"), MaxLength(200)]
        public string Senha { get; set; }

        [Column("salt"), MaxLength(200)]
        public byte[] Salt { get; set; }

        [Column("user"), MaxLength(200)]
        public string Usuario {  get; set; }
    }
}
