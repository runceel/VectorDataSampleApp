using DataReadApp.Components;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// qdrant のクライアントを追加
builder.AddQdrantClient("qdrant");
// ollama の IEmbeddingGenerator を追加
builder.AddOllamaSharpEmbeddingGenerator("ollama-bge-large");
// IVectorStore の Qdrant 用実装を追加
builder.Services.AddSingleton<IVectorStore>(sp =>
    new QdrantVectorStore(sp.GetRequiredService<QdrantClient>()));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
