using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace DataInitializerApp;

public class Worker(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStore vectorStore,
    ILogger<Worker> logger) : BackgroundService
{
    // 登録するデータ
    private static readonly string[] _sourceData = [
        "My name is Kazuki Ota.",
        "My favorite programming language is C#.",
        "My favorite game is Genshin.",
        "My birthday is 1981/01/30.",
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // データを登録するコレクションを取得
        var c = vectorStore.GetCollection<ulong, MyStoreData>("my-collection");
        // データをクリアするために、コレクションが存在していれば削除
        if (await c.CollectionExistsAsync(stoppingToken))
        {
            await c.DeleteCollectionAsync(stoppingToken);
        }
        // コレクションを作成
        await c.CreateCollectionAsync(stoppingToken);

        // データを登録
        ulong id = 0;
        foreach (var data in _sourceData)
        {
            // データから埋め込みベクトルを生成
            var embedding = await embeddingGenerator.GenerateEmbeddingAsync(data, cancellationToken: stoppingToken);
            // データを登録
            var r = new MyStoreData
            {
                Id = id++,
                Data = data,
                Vector = embedding.Vector,
            };
            await c.UpsertAsync(r, cancellationToken: stoppingToken);
            logger.LogInformation("{data} was added.", data);
        }
    }
}

// データのモデル
class MyStoreData
{
    // Id 列
    [VectorStoreRecordKey]
    public required ulong Id { get; init; }

    // Data 列
    [VectorStoreRecordData]
    public required string Data { get; init; }

    // Vector 列 (Ollama の bge-large は 1024 次元)
    [VectorStoreRecordVector(1024)]
    public required ReadOnlyMemory<float> Vector { get; init; }
}
