using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SupportFlow.Application.Tickets.Interfaces;
using SupportFlow.Infrastructure.Persistence;
using SupportFlow.Infrastructure.Tickets;
using SupportFlow.Infrastructure.Knowledge;
using SupportFlow.Application.Knowledge.Interfaces;
using SupportFlow.Application.AI.Interfaces;
using SupportFlow.Application.Dashboard.Interfaces;
using SupportFlow.Infrastructure.AI;
using SupportFlow.Infrastructure.AI.OpenAI;
using SupportFlow.Infrastructure.Dashboard;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenAIOptions>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector());
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
builder.Services.AddScoped<IDashboardService, DashboardService>();

var embeddingProvider = builder.Configuration["AI:EmbeddingProvider"] ?? "Fake";

if (embeddingProvider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IEmbeddingProvider, FakeEmbeddingProvider>();
}
else if (embeddingProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<IEmbeddingProvider, OpenAIEmbeddingProvider>();
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
