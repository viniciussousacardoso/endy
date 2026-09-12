using endy.Model;
using Microsoft.EntityFrameworkCore;

namespace endy.Services
{
    public class DatabaseContextService : DbContext
    {
        public DbSet<ClienteModel> ClienteModels { get; set; }
        public DbSet<UsuarioModel> usuarioModels { get; set; }

        private IConfiguration _configuration;
        public DatabaseContextService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Permite injetar opções já configuradas (ex.: provider InMemory/SQLite em testes),
        // sem alterar o comportamento padrão em produção (que continua usando MySQL).
        public DatabaseContextService(DbContextOptions<DatabaseContextService> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}
