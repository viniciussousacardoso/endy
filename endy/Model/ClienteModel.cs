using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace endy.Model
{
    [Table("cliente")]
    public class ClienteModel
    {
        [Key, Column("id_cliente")]
        public int IdCliente { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("nome")]
        public string Nome { get; set; }
        [Column("visualizado")]
        public bool Visualizado { get; set; }
    }
}
