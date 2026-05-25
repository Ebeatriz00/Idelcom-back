using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedKernel.Helpers
{
    public class FlexibleBooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;

            if (reader.TokenType == JsonTokenType.False)
                return false;

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var number))
                return number == 1;

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString()?.Trim();

                if (string.IsNullOrEmpty(value))
                    return false;

                if (bool.TryParse(value, out var result))
                    return result;

                return value.ToLowerInvariant() switch
                {
                    "1" or "si" or "s" or "yes" or "y" => true,
                    "0" or "no" or "n" => false,
                    _ => throw new JsonException($"Cannot convert value '{value}' to Boolean.")
                };
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to Boolean.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
