using Microsoft.EntityFrameworkCore;
using Models;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class PubContext : DbContext
    {
        private readonly IConfiguration AppConfig;
        public PubContext(IConfiguration Config)
        {
            AppConfig = Config;
        }

        public DbSet<Publications> Publications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = AppConfig.GetConnectionString(AppConfig["DbEngine"]);
            optionsBuilder.UseSqlServer(conn);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ConfigurarPublications());
            base.OnModelCreating(modelBuilder);
        }
    }


    public class ConfigurarPublications : IEntityTypeConfiguration<Publications>
    {
        public void Configure(EntityTypeBuilder<Publications> builder)
        {
            builder.HasKey(aut => aut.Id);
            builder.ToTable("Publications");
            builder.Property(aut => aut.Id).HasColumnName("idPublication");
        }
    }

}
