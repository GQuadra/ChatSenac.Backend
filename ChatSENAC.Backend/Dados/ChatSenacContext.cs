using ChatSENAC.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSENAC.Backend.Dados;

public class ChatSenacContext(DbContextOptions<ChatSenacContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Mensagem> Mensagens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Users_Email");
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Mensagem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Conteudo).HasMaxLength(140).IsRequired();
            entity.Property(e => e.EnviadoEm).IsRequired();
            entity.HasOne(e => e.Usuario)
                .WithMany()
                .IsRequired();
        });
    }
}