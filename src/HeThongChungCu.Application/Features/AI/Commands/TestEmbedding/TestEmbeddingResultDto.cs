namespace HeThongChungCu.Application.Features.AI.Commands.TestEmbedding;

public class TestEmbeddingResultDto
{
    public string Message { get; set; } = string.Empty;
    public int VectorSize { get; set; }
    public float[] Preview { get; set; } = [];
}
