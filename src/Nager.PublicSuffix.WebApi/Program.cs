using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using Nager.PublicSuffix.RuleProviders.CacheProviders;
using Nager.PublicSuffix.WebApi.GitHub;
using System.Text.Json.Serialization;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICacheProvider, LocalFileSystemCacheProvider>();
builder.Services.AddSingleton<IRuleProvider, CachedHttpRuleProvider>();
builder.Services.AddSingleton<IDomainParser, DomainParser>();
builder.Services.AddScoped<GitHubClient>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

var ruleProvider = app.Services.GetService<IRuleProvider>();
if (ruleProvider != null)
{
    await ruleProvider.BuildAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/DomainInfo/{domain}", (string domain, IDomainParser domainParser) =>
{
    domain = HttpUtility.UrlEncode(domain);

    var domainInfo = domainParser.Parse(domain);
    return domainInfo;
})
.WithName("DomainInfo")
.WithOpenApi();

app.MapPost("/CheckLastCommit", async (GitHubClient gitHubClient, CancellationToken cancellationToken) =>
{
    var lastGitHubCommit = await gitHubClient.GetCommitAsync("publicsuffix", "list", "master", cancellationToken);

    return lastGitHubCommit?.Commit?.Committer?.Date;
})
.WithName("CheckLastCommit")
.WithOpenApi();

app.MapPost("/UpdateRules", async (IRuleProvider ruleProvider) =>
{
    await ruleProvider.BuildAsync(ignoreCache: true);
    return Results.NoContent();
})
.WithName("UpdateRules")
.WithOpenApi();

app.Run();

