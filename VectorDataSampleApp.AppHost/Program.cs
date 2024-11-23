var builder = DistributedApplication.CreateBuilder(args);

// ollama ��ǉ����ăf�[�^�̉i�����ƁA�R���e�i���f�o�b�O�I�����ɗ����Ȃ��悤�ɂ���
var ollama = builder.AddOllama("ollama")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);
// �x�N�g���p�̃��f����ǉ�
var embeddingModel = ollama.AddModel("bge-large");

// qdrant ��ǉ����ăf�[�^�̉i�����ƁA�R���e�i���f�o�b�O�I�����ɗ����Ȃ��悤�ɂ���
var qdrant = builder.AddQdrant("qdrant")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();
