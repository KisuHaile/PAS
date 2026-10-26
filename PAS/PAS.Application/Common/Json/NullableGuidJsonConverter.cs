using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Common.Json;

public sealed class NullableGuidJsonConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value) ||
                string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (Guid.TryParse(value, out var guid))
                return guid;

            throw new JsonException($"Invalid Guid value: '{value}'.");
        }

        throw new JsonException($"Unexpected token parsing Guid?: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
