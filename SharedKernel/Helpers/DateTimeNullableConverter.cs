using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.Helpers
{
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        private static readonly string[] Formats =
        [
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ",
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss"
        ];

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value)
                    || value.Equals("null", StringComparison.OrdinalIgnoreCase)
                    || value.Equals("undefined", StringComparison.OrdinalIgnoreCase)
                    || value.Equals("Invalid Date", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (DateTime.TryParseExact(
                    value,
                    Formats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out var exactResult))
                {
                    return exactResult;
                }

                if (DateTime.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                    out var result))
                {
                    return result;
                }

                if (DateTimeOffset.TryParse(
                    value,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AllowWhiteSpaces,
                    out var offsetResult))
                {
                    return offsetResult.LocalDateTime;
                }

                return null;
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to DateTime?");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
