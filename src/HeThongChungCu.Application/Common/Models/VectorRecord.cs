using System;
using System.Collections.Generic;

namespace HeThongChungCu.Application.Common.Models;

public class VectorRecord
{
    public string Id { get; set; } = string.Empty;
    public float[] Vector { get; set; } = Array.Empty<float>();
    public Dictionary<string, object>? Payload { get; set; }
}
