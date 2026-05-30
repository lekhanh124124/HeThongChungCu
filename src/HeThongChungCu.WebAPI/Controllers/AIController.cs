using Asp.Versioning;
using HeThongChungCu.Application.Features.AI.Commands.TestBatchSearch;
using HeThongChungCu.Application.Features.AI.Commands.TestChunking;
using HeThongChungCu.Application.Features.AI.Commands.TestEmbedding;
using HeThongChungCu.Application.Features.AI.Commands.TestLLM;
using HeThongChungCu.Application.Features.AI.Commands.TestVectorStore;
using HeThongChungCu.Application.Features.AI.Queries.GetAIChatResponse;
using HeThongChungCu.WebAPI.Common.Models;
using HeThongChungCu.WebAPI.Controllers.AI;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeThongChungCu.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/ai")]
public class AIController : ApiControllerBase
{
    private readonly ISender _sender;

    public AIController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Kiểm tra chunking văn bản từ file Markdown
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Dùng trong giai đoạn phát triển để xác minh cấu hình chunking (chunk size, overlap) trên tài liệu tri thức thực tế.
    /// - **Hệ thống xử lý**:
    ///     - Đọc nội dung file Markdown (.md) được tải lên.
    ///     - Phân chia văn bản thành các đoạn nhỏ (chunks) theo cấu hình kích thước và độ chồng lấp.
    ///     - Trả về danh sách chunks kèm metadata (H1, H2, H3, token count).
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `File` (định dạng .md).
    ///     - **Tùy chọn**: `ChunkSize` (mặc định: 400), `ChunkOverlap` (mặc định: 60).
    /// </remarks>
    [HttpPost("test-chunking")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TestChunkingResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestChunking(
        [FromForm] RequestTestChunking request,
        CancellationToken cancellationToken)
    {
        var command = new TestChunkingCommand(
            fileStream: request.File.OpenReadStream(),
            fileName: request.File.FileName,
            fileSize: request.File.Length,
            chunkSize: request.ChunkSize,
            chunkOverlap: request.ChunkOverlap);

        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kiểm tra kết nối và phản hồi của Gemini LLM
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Dùng trong giai đoạn phát triển để xác minh tích hợp Gemini API hoạt động đúng.
    /// - **Hệ thống xử lý**:
    ///     - Gửi prompt tới Gemini LLM với system instruction cơ bản.
    ///     - Trả về phản hồi văn bản từ mô hình.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Prompt`.
    /// </remarks>
    [HttpPost("test-gemini")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TestLLMResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestGemini(
        [FromBody] TestLLMCommand command,
        CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kiểm tra sinh embedding vector từ Gemini Embedding API
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Dùng trong giai đoạn phát triển để xác minh Gemini Embedding API trả về vector đúng chiều.
    /// - **Hệ thống xử lý**:
    ///     - Gửi chuỗi văn bản tới Gemini Embedding API.
    ///     - Trả về kích thước vector và 10 giá trị đầu tiên để kiểm tra.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Text`.
    /// </remarks>
    [HttpPost("test-embedding")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TestEmbeddingResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestEmbedding(
        [FromBody] TestEmbeddingCommand command,
        CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Kiểm tra kết nối tới Qdrant Cloud qua IVectorStore
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Dùng trong giai đoạn phát triển để xác minh kết nối tới Qdrant Cloud hoạt động đúng.
    /// - **Hệ thống xử lý**:
    ///     - Thử tạo collection kiểm tra (nếu chưa tồn tại) với kích thước vector 3072.
    ///     - Trả về kết quả kết nối thành công hoặc thất bại.
    /// - **Yêu cầu dữ liệu**: Không có.
    /// </remarks>
    [HttpPost("test-qdrant")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TestVectorStoreResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestQdrant(CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(new TestVectorStoreCommand(), cancellationToken));
    }

    /// <summary>
    /// Kiểm tra nạp hàng loạt (Batch Upsert) và tìm kiếm tương đồng có bộ lọc (Filtered Search)
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Dùng trong giai đoạn phát triển để xác minh toàn bộ pipeline RAG: embedding → upsert → search.
    /// - **Hệ thống xử lý**:
    ///     - Sinh embedding cho từng đoạn văn bản và upsert vào Qdrant.
    ///     - Thực hiện tìm kiếm tương đồng không lọc và có lọc (theo trạng thái chẵn/lẻ).
    ///     - Trả về kết quả cả hai lần tìm kiếm.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Texts` (danh sách văn bản, tối đa 50 mục).
    ///     - **Tùy chọn**: `CollectionName` (mặc định: `test_batch_collection`).
    /// </remarks>
    [HttpPost("test-qdrant-batch-search")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<TestBatchSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestQdrantBatchAndSearch(
        [FromBody] TestBatchSearchCommand command,
        CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(command, cancellationToken));
    }

    /// <summary>
    /// Trò chuyện với trợ lý ảo cư dân thông minh sử dụng kỹ thuật RAG
    /// </summary>
    /// <remarks>
    /// - **Hoàn cảnh sử dụng**: Cư dân đặt câu hỏi liên quan đến quy định chung cư, dịch vụ, hoặc thông tin tòa nhà.
    /// - **Hệ thống xử lý**:
    ///     - Chuyển đổi câu hỏi thành vector embedding.
    ///     - Tìm kiếm các đoạn tài liệu tương đồng nhất trong Qdrant (hỗ trợ lọc theo loại tài liệu).
    ///     - Tổng hợp ngữ cảnh và sinh câu trả lời qua Gemini LLM.
    ///     - Trả về câu trả lời kèm danh sách nguồn trích dẫn.
    /// - **Yêu cầu dữ liệu**:
    ///     - **Bắt buộc**: `Prompt`.
    ///     - **Tùy chọn**: `DocumentType` (lọc theo loại tài liệu), `Limit` (số kết quả tìm kiếm, mặc định: 5).
    /// </remarks>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ApiResponse<AIChatResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Chat(
        [FromBody] GetAIChatResponseQuery query,
        CancellationToken cancellationToken)
    {
        return HandleResult(await _sender.Send(query, cancellationToken));
    }
}
