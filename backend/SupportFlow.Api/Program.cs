using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
using SupportFlow.Application.Auth.Interfaces;
using SupportFlow.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using SupportFlow.Domain.Entities;
using SupportFlow.Application.Email.Interfaces;
using SupportFlow.Infrastructure.Email.Brevo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenAIOptions>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
{
    throw new InvalidOperationException("JWT secret key is missing.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });

    builder.Services.Configure<BrevoOptions>(
    builder.Configuration.GetSection(BrevoOptions.SectionName));

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.UseVector());
});

builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketAnalysisService, TicketAnalysisService>();
builder.Services.AddScoped<IKnowledgeArticleService, KnowledgeArticleService>();
builder.Services.AddScoped<IKnowledgeChunkService, KnowledgeChunkService>();
builder.Services.AddScoped<IKnowledgeEmbeddingService, KnowledgeEmbeddingService>();
builder.Services.AddScoped<ITicketDraftReplyService, TicketDraftReplyService>();
builder.Services.AddScoped<IRelatedKnowledgeService, RelatedKnowledgeService>();
builder.Services.AddScoped<ITicketActivityService, TicketActivityService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddHttpClient<IEmailSender, BrevoEmailSender>();
builder.Services.AddScoped<ITicketReplyService, TicketReplyService>();
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

var draftReplyProvider = builder.Configuration["AI:DraftReplyProvider"] ?? "Fake";

if (draftReplyProvider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ITicketDraftReplyGenerator, FakeTicketDraftReplyGenerator>();
}
else if (draftReplyProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<ITicketDraftReplyGenerator, OpenAITicketDraftReplyGenerator>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported draft reply provider '{draftReplyProvider}'. Supported values: Fake, OpenAI.");
}

var analysisProvider = builder.Configuration["AI:AnalysisProvider"] ?? "Fake";

if (analysisProvider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<ITicketAnalyzer, FakeTicketAnalyzer>();
}
else if (analysisProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<ITicketAnalyzer, OpenAITicketAnalyzer>();
}
else
{
    throw new InvalidOperationException(
        $"Unsupported analysis provider '{analysisProvider}'. Supported values: Fake, OpenAI.");
}

builder.Services.AddControllers();

var frontendUrl = builder.Configuration["FrontendUrl"];
var allowedOrigins = new[]
{
    "http://localhost:3000",
    frontendUrl
}
.OfType<string>()
.Where(url => !string.IsNullOrWhiteSpace(url)).ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

// app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapMethods(
    "/health",
    new[] { "GET", "HEAD" },
    () => Results.Ok(new { status = "healthy" }));
app.MapControllers();

app.Run();
