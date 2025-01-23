using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace endy.Model
{
    [Table("cliente")]
    public class ClienteModel
    {
        [Key, Column("id_cliente")]
        public int IdCliente { get; set; }
        [Column("email"), MaxLength(60)]
        public string Email { get; set; }
        [Column("nome"), MaxLength(60)]
        public string Nome { get; set; }
        [Column("visualizado")]
        public bool Visualizado { get; set; }
        [Column("telefone"), MaxLength(20)]
        public string telefone { get; set; }
        [Column("motivo_contato"), MaxLength(500)]
        public string motivo_contato { get; set; }
    }
}
