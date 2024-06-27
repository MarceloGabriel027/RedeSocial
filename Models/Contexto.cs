using Microsoft.EntityFrameworkCore;

namespace RedeSocial.Models
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
            usuario = Set<Usuario>();
            post = Set<Post>();
            comentarios = Set<Comentarios>();
            bloqueados = Set<Bloqueados>();
        }

        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Post> post { get; set; }
        public DbSet<Comentarios> comentarios { get; set; }
        public DbSet<Bloqueados> bloqueados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir chaves primárias
            modelBuilder.Entity<Usuario>().HasKey(u => u.usuarioId);
            modelBuilder.Entity<Post>().HasKey(p => p.postId);
            modelBuilder.Entity<Comentarios>().HasKey(c => c.comentarioId);
            modelBuilder.Entity<Bloqueados>().HasKey(b => b.idBloqueio);

            // Definir relacionamentos
            modelBuilder.Entity<Post>()
                .HasOne(e => e.usuarioPost)
                .WithMany()
                .HasForeignKey(e => e.usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentarios>()
                .HasOne(e => e.usuarioComentario)
                .WithMany()
                .HasForeignKey(e => e.usuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comentarios>()
                .HasOne(e => e.postComentario)
                .WithMany()
                .HasForeignKey(e => e.postId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bloqueados>()
                .HasOne(e => e.bloqueioUsuario)
                .WithMany()
                .HasForeignKey(e => e.idUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bloqueados>()
                .HasOne(e => e.bloqueadoUsuario)
                .WithMany()
                .HasForeignKey(e => e.idUsuarioBloqueado)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
