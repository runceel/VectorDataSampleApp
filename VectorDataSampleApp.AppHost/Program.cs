var builder = DistributedApplication.CreateBuilder(args);

// ollama を追加してデータの永続化と、コンテナがデバッグ終了時に落ちないようにする
var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
// ベクトル用のモデルを追加
var embeddingModel = ollama.AddModel("bge-large");

// qdrant を追加してデータの永続化と、コンテナがデバッグ終了時に落ちないようにする
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();
