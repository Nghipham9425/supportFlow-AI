using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}