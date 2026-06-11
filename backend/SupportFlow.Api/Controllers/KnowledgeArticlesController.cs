using Microsoft.AspNetCore.Mvc;
using SupportFlow.Application.Knowledge.DTOs;
using SupportFlow.Application.Knowledge.Interfaces;


namespace SupportFlow.Api.Controllers;



[ApiController]
[Route("api/knowledge-articles")]
public class KnowledgeArticlesController : ControllerBase
{
    private readonly IKnowledgeArticleService _knowledgeArticleService;

    public KnowledgeArticlesController(IKnowledgeArticleService knowledgeArticleService)
    {
        this._knowledgeArticleService = knowledgeArticleService;
    }
    [HttpGet]
    public async Task<ActionResult<List<KnowledgeArticleDto>>> GetArticles()
    {
        var articles = await _knowledgeArticleService.GetArticlesAsync();
        return Ok(articles);
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<KnowledgeArticleDto>> GetArticle(Guid id)
    {
        var article = await _knowledgeArticleService.GetArticleByIdAsync(id);
        if (article is null) return NotFound();
        return Ok(article);
    }

    [HttpPost]
    public async Task<ActionResult<KnowledgeArticleDto>> CreateArticle(CreateKnowledgeArticleDto request)
    {
        var article = await _knowledgeArticleService.CreateArticleAsync(request);

        return CreatedAtAction(
            nameof(GetArticle),
            new { id = article.Id }, article
        );
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<KnowledgeArticleDto>> UpdateArticle(Guid id, UpdateKnowledgeArticleDto request)
    {
        var article = await _knowledgeArticleService.UpdateArticleAsync(id, request);

        if (article is null) return NotFound();
        return Ok(article);

    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteArticle(Guid id)
    {
        var deleted = await _knowledgeArticleService.DeleteArticleAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

}