﻿using endy.Model;
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
