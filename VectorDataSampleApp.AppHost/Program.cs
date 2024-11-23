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

// DataInitializerApp を追加
builder.AddProject<Projects.DataInitializerApp>("datainitializerapp")
    // bge-large を使うための設定
    .WithReference(embeddingModel)
    // bge-large の起動が完了するまで起動を待つ
    .WaitFor(embeddingModel)
    // qdrant を使うための設定
    .WithReference(qdrant)
    // qdrant の起動が完了するまで起動を待つ
    .WaitFor(qdrant);

builder.AddProject<Projects.DataReadApp>("datareadapp")
    // bge-large を使うための設定
    .WithReference(embeddingModel)
    // bge-large の起動が完了するまで起動を待つ
    .WaitFor(embeddingModel)
    // qdrant を使うための設定
    .WithReference(qdrant)
    // qdrant の起動が完了するまで起動を待つ
    .WaitFor(qdrant);


builder.Build().Run();
