using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace DataInitializerApp;

public class Worker(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStore vectorStore,
    ILogger<Worker> logger) : BackgroundService
{
    // �o�^����f�[�^
    private static readonly string[] _sourceData = [
        "My name is Kazuki Ota.",
        "My favorite programming language is C#.",
        "My favorite game is Genshin.",
        "My birthday is 1981/01/30.",
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // �f�[�^��o�^����R���N�V�������擾
        var c = vectorStore.GetCollection<ulong, MyStoreData>("my-collection");
        // �f�[�^���N���A���邽�߂ɁA�R���N�V���������݂��Ă���΍폜
        if (await c.CollectionExistsAsync(stoppingToken))
        {
            await c.DeleteCollectionAsync(stoppingToken);
        }
        // �R���N�V�������쐬
        await c.CreateCollectionAsync(stoppingToken);

        // �f�[�^��o�^
        ulong id = 0;
        foreach (var data in _sourceData)
        {
            // �f�[�^���疄�ߍ��݃x�N�g���𐶐�
            var embedding = await embeddingGenerator.GenerateEmbeddingAsync(data, cancellationToken: stoppingToken);
            // �f�[�^��o�^
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

// �f�[�^�̃��f��
class MyStoreData
{
    // Id ��
    [VectorStoreRecordKey]
    public required ulong Id { get; init; }

    // Data ��
    [VectorStoreRecordData]
    public required string Data { get; init; }

    // Vector �� (Ollama �� bge-large �� 1024 ����)
    [VectorStoreRecordVector(1024)]
    public required ReadOnlyMemory<float> Vector { get; init; }
}
