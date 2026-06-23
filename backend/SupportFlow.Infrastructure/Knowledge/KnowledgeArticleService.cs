using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Knowledge.DTOs;
using SupportFlow.Application.Knowledge.Interfaces;
using SupportFlow.Domain.Entities;
using SupportFlow.Infrastructure.Persistence;

namespace SupportFlow.Infrastructure.Knowledge;

public class KnowledgeArticleService : IKnowledgeArticleService
{
    private readonly AppDbContext _dbContext;

    public KnowledgeArticleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<KnowledgeArticleDto> CreateArticleAsync(CreateKnowledgeArticleDto request)
    {
        var article = new KnowledgeArticle
        {
            Title = request.Title,
            Content = request.Content,
            Category = request.Category
        };
        _dbContext.KnowledgeArticles.Add(article);
        await _dbContext.SaveChangesAsync();

        return ToDto(article);
    }

    public async Task<bool> DeleteArticleAsync(Guid id)
    {
        var article = await _dbContext.KnowledgeArticles.
                        FirstOrDefaultAsync(article => article.Id == id);

        if (article is null) return false;

        _dbContext.KnowledgeArticles.Remove(article);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<KnowledgeArticleDto?> GetArticleByIdAsync(Guid id)
    {
        var article =  await _dbContext.KnowledgeArticles
                    .FirstOrDefaultAsync(article => article.Id == id);

        if (article is null) return null;

        var chunkCount = await _dbContext.KnowledgeChunks
        .CountAsync(chunk => chunk.KnowledgeArticleId == article.Id);

        return ToDto(article, chunkCount);
    }

    public async Task<List<KnowledgeArticleDto>> GetArticlesAsync()
    {
        var articles = await _dbContext.KnowledgeArticles
                        .OrderByDescending(article => article.CreatedAt)
                        .ToListAsync();

        var articleIds = articles.Select(article => article.Id).ToList();
        var chunkCounts = await _dbContext.KnowledgeChunks
                        .Where(chunk => articleIds.Contains(chunk.KnowledgeArticleId))
                        .GroupBy(chunk => chunk.KnowledgeArticleId)
                        .Select(group => new
                        {
                            ArticleId = group.Key,
                            Count = group.Count()
                        })
                        .ToDictionaryAsync(item => item.ArticleId, item => item.Count);

        return articles
            .Select(article =>
            {
                chunkCounts.TryGetValue(article.Id, out var chunkCount);
                return ToDto(article, chunkCount);
            })
            .ToList();
    
    }

   public async Task<KnowledgeArticleDto?> UpdateArticleAsync(
        Guid id,
        UpdateKnowledgeArticleDto request)
    {
        var article = await _dbContext.KnowledgeArticles
            .FirstOrDefaultAsync(article => article.Id == id);

        if (article is null)
        {
            return null;
        }

        if (request.Title is not null)
        {
            article.Title = request.Title;
        }

        if (request.Content is not null)
        {
            article.Content = request.Content;
        }

        if (request.Category is not null)
        {
            article.Category = request.Category.Value;
        }

        article.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ToDto(article);
    }

    private static KnowledgeArticleDto ToDto(KnowledgeArticle article, int chunkCount = 0)
        {
        return new KnowledgeArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Category = article.Category,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            ChunkCount = chunkCount,
            IsAiReady = chunkCount > 0
        };
        }

}
