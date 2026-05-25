using System.Text.Json.Serialization;

namespace SharedKernel
{
    public class GlobalResponse
    {
        public int Status { get; set; } = 1;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OpporNum { get; set; }

        public string Message { get; set; } = "Operación exitosa";
    }

    public class GlobalResponse<T>
    {
        public int Status { get; set; } = 1;
        public string Message { get; set; } = "Operación exitosa";
        public T? Data { get; set; }
    }
}
