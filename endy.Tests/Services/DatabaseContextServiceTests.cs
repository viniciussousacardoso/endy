using endy.Model;
using endy.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace endy.Tests.Services
{
    public class DatabaseContextServiceTests
    {
        private static IConfiguration BuildConfiguration(string connectionString)
        {
            var dict = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            };

            return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        }

        [Fact]
        public void OnConfiguring_SemOptionsPreConfiguradas_UsaConnectionStringDaConfiguracao()
        {
            var configuration = BuildConfiguration("server=localhost;database=teste;user=root;password=senha;");

            using var context = new DatabaseContextService(configuration);

            Assert.Contains("MySql", context.Database.ProviderName);
        }

        [Fact]
        public void OnConfiguring_ComOptionsJaConfiguradas_NaoSobrescreveProvider()
        {
            var options = new DbContextOptionsBuilder<DatabaseContextService>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new DatabaseContextService(options);

            Assert.Contains("InMemory", context.Database.ProviderName);
        }

        [Fact]
        public void DbSets_PermitemAdicionarEConsultarRegistros()
        {
            var options = new DbContextOptionsBuilder<DatabaseContextService>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new DatabaseContextService(options))
            {
                context.ClienteModels.Add(new ClienteModel { Email = "a@a.com", Nome = "A" });
                context.usuarioModels.Add(new UsuarioModel("senha", new byte[] { 1 }, "user"));
                context.SaveChanges();
            }

            using (var context = new DatabaseContextService(options))
            {
                Assert.Single(context.ClienteModels);
                Assert.Single(context.usuarioModels);
            }
        }
    }
}
