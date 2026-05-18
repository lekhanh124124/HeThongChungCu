using HeThongChungCu.Application.Common.Models;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface ITextChunker
{
    /// <summary>
    /// Chia nhỏ văn bản thành các đoạn (chunks) cấu trúc để embedding và lưu trữ Qdrant.
    /// </summary>
    /// <param name="text">Văn bản gốc</param>
    /// <param name="source">Nguồn tài liệu (ví dụ: tên file)</param>
    /// <param name="chunkSize">Kích thước tối đa mỗi đoạn (tokens)</param>
    /// <param name="chunkOverlap">Số lượng tokens gối đầu giữa các đoạn</param>
    /// <returns>Danh sách các đoạn văn bản có cấu trúc</returns>
    List<TextChunk> SplitText(string text, string source = "", int chunkSize = 400, int chunkOverlap = 60);
}
