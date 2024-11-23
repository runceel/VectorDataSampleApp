using DataInitializerApp;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
// qdrant �̃N���C�A���g��ǉ�
builder.AddQdrantClient("qdrant");
// ollama �� IEmbeddingGenerator ��ǉ�
builder.AddOllamaSharpEmbeddingGenerator("ollama-bge-large");
// IVectorStore �� Qdrant �p������ǉ�
builder.Services.AddSingleton<IVectorStore>(sp => 
    new QdrantVectorStore(sp.GetRequiredService<QdrantClient>()));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
