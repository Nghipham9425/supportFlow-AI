using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SupportFlow.Domain.Entities;

namespace SupportFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
    public DbSet<KnowledgeChunk> KnowledgeChunks => Set<KnowledgeChunk>();

    public DbSet<TicketReply> TicketReplies => Set<TicketReply>();

    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Name)
            .HasMaxLength(150)
            .IsRequired();

            entity.Property(user => user.Email)
            .HasMaxLength(255)
            .IsRequired();

            entity.HasIndex(user => user.Email)
            .IsUnique();

            entity.Property(user => user.PasswordHash)
            .IsRequired();

            entity.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(ticket => ticket.Id);

            entity.Property(ticket => ticket.CustomerName)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(ticket => ticket.CustomerEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(ticket => ticket.Subject)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(ticket => ticket.Description)
                .IsRequired();

            entity.Property(ticket => ticket.AiSummary)
                .HasMaxLength(1000);

            entity.Property(ticket => ticket.Channel)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(ticket => ticket.Category)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(ticket => ticket.Priority)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(ticket => ticket.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(ticket => ticket.Sentiment)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(ticket => ticket.AssignedToUser)
            .WithMany()
            .HasForeignKey(ticket => ticket.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(ticket => ticket.AssignedToUserId);

        });

        modelBuilder.Entity<TicketReply>(entity =>
        {
            entity.HasKey(reply => reply.Id);

            entity.Property(reply => reply.RecipientEmail)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(reply => reply.Subject)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(reply => reply.Content)
                .IsRequired();

            entity.Property(reply => reply.ProviderMessageId)
                .HasMaxLength(255);

            entity.HasOne(reply => reply.Ticket)
                .WithMany()
                .HasForeignKey(reply => reply.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(reply => reply.SentByUser)
                .WithMany()
                .HasForeignKey(reply => reply.SentByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(reply => reply.TicketId);
        });

        modelBuilder.Entity<KnowledgeArticle>(entity =>
        {
            entity.HasKey(article => article.Id);

            entity.Property(article => article.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(article => article.Content)
                .IsRequired();

            entity.Property(article => article.Category)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });
        modelBuilder.Entity<KnowledgeChunk>(entity =>
        {
            entity.HasKey(chunk => chunk.Id);
            entity.Property(chunk => chunk.Content).IsRequired();
            entity.Property(chunk => chunk.ChunkIndex).IsRequired();
            entity.Property(chunk => chunk.Embedding)
                .HasColumnType("vector(1536)");

            entity.HasOne(chunk => chunk.KnowledgeArticle)
                .WithMany()
                .HasForeignKey(chunk => chunk.KnowledgeArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
