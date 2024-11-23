using DataInitializerApp;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
// qdrant のクライアントを追加
builder.AddQdrantClient("qdrant");
// ollama の IEmbeddingGenerator を追加
builder.AddOllamaSharpEmbeddingGenerator("ollama-bge-large");
// IVectorStore の Qdrant 用実装を追加
builder.Services.AddSingleton<IVectorStore>(sp => 
    new QdrantVectorStore(sp.GetRequiredService<QdrantClient>()));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
