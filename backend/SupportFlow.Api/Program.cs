using Microsoft.EntityFrameworkCore;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Infrastructure.Persistence;
using SupportFlow.Infrastructure.Tickets;
using SupportFlow.Infrastructure.Knowledge;
using SupportFlow.Application.Knowledge.Interfaces;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketAnalysisService, TicketAnalysisService>();
builder.Services.AddScoped<ITicketAnalyzer, FakeTicketAnalyzer>();
builder.Services.AddScoped<IKnowledgeArticleService, KnowledgeArticleService>();
builder.Services.AddScoped<IKnowledgeChunkService, KnowledgeChunkService>();
builder.Services.AddScoped<IKnowledgeEmbeddingService, KnowledgeEmbeddingService>();
builder.Services.AddScoped<ITicketDraftReplyService, TicketDraftReplyService>();
builder.Services.AddScoped<ITicketDraftReplyGenerator, FakeTicketDraftReplyGenerator>();
builder.Services.AddScoped<IRelatedKnowledgeService, RelatedKnowledgeService>();

var embeddingProvider = builder.Configuration["AI:EmbeddingProvider"] ?? "Fake";

if (embeddingProvider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IEmbeddingProvider, FakeEmbeddingProvider>();
}
else if (embeddingProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "OpenAI embedding provider is not implemented yet. Set AI:EmbeddingProvider to Fake.");
}
else
{
    throw new InvalidOperationException(
        $"Unsupported embedding provider '{embeddingProvider}'. Supported values: Fake, OpenAI.");
}

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("Frontend");

app.MapControllers();

app.Run();
