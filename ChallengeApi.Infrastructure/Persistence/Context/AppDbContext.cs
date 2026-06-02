using ChallengeApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengeApi.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<ConsultaHistorial> ConsultasHistorial => Set<ConsultaHistorial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(u => u.PasswordHash)
                      .IsRequired();
                entity.Property(u => u.BirthDate)
                      .IsRequired();
                entity.HasIndex(u => u.Email)
                      .IsUnique();
                entity.HasIndex(u => u.Username)
                      .IsUnique();
            });

            modelBuilder.Entity<ConsultaHistorial>(entity =>
            {
                entity.ToTable("ConsultasHistorial");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Signo)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(c => c.FechaConsulta)
                      .IsRequired();
                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
